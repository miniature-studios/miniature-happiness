using System.Collections.Generic;
using UnityEngine;

namespace Employee
{
    [RequireComponent(typeof(SphereCollider))]
    public class PersonalSpace : MonoBehaviour
    {
        [SerializeField]
        private float actorRadius;

        [SerializeField]
        private float movelessVelocityThreshold;

        private SphereCollider personalSpaceTrigger;
        private EmployeeController controller;
        private float radius;

        private void Start()
        {
            personalSpaceTrigger = GetComponent<SphereCollider>();
            radius = personalSpaceTrigger.radius;

            controller = GetComponentInParent<EmployeeController>();
        }

        private readonly HashSet<EmployeeController> employeesInPersonalSpace = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out EmployeeController employee))
            {
                _ = employeesInPersonalSpace.Add(employee);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out EmployeeController employee))
            {
                _ = employeesInPersonalSpace.Remove(employee);
            }
        }

        private float SlowDownFactorByDistance(EmployeeController employee)
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
            foreach (EmployeeController employee in employeesInPersonalSpace)
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
            EmployeeController closest_employee = null;
            foreach (EmployeeController employee in employeesInPersonalSpace)
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
