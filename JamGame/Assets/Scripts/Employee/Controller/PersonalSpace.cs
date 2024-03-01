using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Employee.Controller
{
    [RequireComponent(typeof(SphereCollider))]
    [AddComponentMenu("Scripts/Employee/Controller/Employee.Controller.PersonalSpace")]
    public class PersonalSpace : MonoBehaviour
    {
        [SerializeField]
        private float actorRadius;

        [SerializeField]
        private float movelessVelocityThreshold;

        [Required]
        [SerializeField]
        private ControllerImpl controller;

        private SphereCollider personalSpaceTrigger;
        private float radius;

        private void Start()
        {
            personalSpaceTrigger = GetComponent<SphereCollider>();
            radius = personalSpaceTrigger.radius;
        }

        private readonly HashSet<ControllerImpl> employeesInPersonalSpace = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ControllerImpl employee))
            {
                _ = employeesInPersonalSpace.Add(employee);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out ControllerImpl employee))
            {
                _ = employeesInPersonalSpace.Remove(employee);
            }
        }

        private float SlowDownFactorByDistance(ControllerImpl employee)
        {
            float distance = (transform.position - employee.transform.position).magnitude;
            return (radius - distance) / (radius - (2.0f * actorRadius));
        }

        public float GetCrowdMetrics()
        {
            if (employeesInPersonalSpace.Count == 0)
            {
                return 0.0f;
            }

            float max_metrics = 0.0f;
            foreach (ControllerImpl employee in employeesInPersonalSpace)
            {
                float metrics;
                bool employee_standing =
                    employee.AverageVelocity.magnitude < movelessVelocityThreshold;
                if (employee_standing)
                {
                    metrics = SlowDownFactorByDistance(employee);
                }
                else
                {
                    float relative_velocity = Vector3.Dot(
                        controller.AverageVelocity.normalized,
                        employee.AverageVelocity.normalized
                    );
                    metrics =
                        relative_velocity < 0.0f
                            ? 0.0f
                            : SlowDownFactorByDistance(employee) * relative_velocity;
                }

                if (metrics > max_metrics)
                {
                    max_metrics = metrics;
                }
            }

            return max_metrics;
        }

        public Vector3 GetPreferredSteeringNormalized()
        {
            if (controller.AverageVelocity.magnitude > movelessVelocityThreshold)
            {
                return Vector3.zero;
            }

            float min_distance = float.PositiveInfinity;
            ControllerImpl closest_employee = null;
            foreach (ControllerImpl employee in employeesInPersonalSpace)
            {
                Vector3 employee_to_this = transform.position - employee.transform.position;

                Vector3 employee_velocity = employee.AverageVelocity.normalized;
                float relative_velocity = Vector3.Dot(
                    employee_to_this.normalized,
                    employee_velocity
                );
                if (relative_velocity < 0.0)
                {
                    continue;
                }

                float distance_to_employee = employee_to_this.magnitude;
                if (distance_to_employee < min_distance)
                {
                    min_distance = distance_to_employee;
                    closest_employee = employee;
                }
            }

            if (closest_employee == null)
            {
                return Vector3.zero;
            }

            Vector3 closest_employee_velocity = closest_employee.AverageVelocity.normalized;
            Vector3 steering_direction =
                new(-closest_employee_velocity.z, 0.0f, closest_employee_velocity.x);
            float steering_strength = SlowDownFactorByDistance(closest_employee);
            return steering_direction * steering_strength;
        }
    }
}
