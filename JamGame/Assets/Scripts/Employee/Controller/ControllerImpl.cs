using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Employee.Controller
{
    [RequireComponent(typeof(NavMeshAgent))]
    [AddComponentMenu("Scripts/Employee/Controller/Employee.Controller")]
    public class ControllerImpl : MonoBehaviour, IEffectExecutor<ControllerEffect>
    {
        private enum State
        {
            Idle,
            Moving,
            BuildingPath,
        }

        public enum NavigationMode
        {
            Navmesh,
            FreeMove
        }

        [SerializeField]
        private float maxVelocity;

        private NavMeshAgent agent;
        private Vector3 averageVelocity = Vector3.zero;
        public Vector3 AverageVelocity => averageVelocity;

        [Required]
        [SerializeField]
        private PersonalSpace personalSpace;

        private Vector3 currentDestination;
        private Vector3 prevPosition;

        [SerializeField]
        [ReadOnly]
        private State state = State.Idle;

        private NavigationMode navigationMode = NavigationMode.Navmesh;

        public event Action OnReachedNeedProvider;

        private bool avoidancePriorityIncreased = false;
        private bool isFirstInWaitingLine = false;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            averageVelocity = (transform.position - prevPosition) / Time.deltaTime;
            prevPosition = transform.position;

            switch (state)
            {
                case State.BuildingPath:
                    if (!agent.pathPending)
                    {
                        state = State.Moving;
                    }
                    break;
                case State.Idle:
                    break;
                case State.Moving:
                    CorrectMovement();

                    if (
                        agent.remainingDistance < 2.0f
                        && !avoidancePriorityIncreased
                        && isFirstInWaitingLine
                    )
                    {
                        agent.avoidancePriority -= 1;
                        avoidancePriorityIncreased = true;
                    }

                    if (agent.remainingDistance < 0.01f)
                    {
                        state = State.Idle;
                        transform.position = currentDestination + (agent.baseOffset * Vector3.up);

                        if (avoidancePriorityIncreased)
                        {
                            agent.avoidancePriority += 1;
                            avoidancePriorityIncreased = false;
                        }
                        isFirstInWaitingLine = false;

                        OnReachedNeedProvider?.Invoke();
                    }
                    break;
            }
        }

        public void MoveToNeedProvider(Vector3 position)
        {
            SetNavigationMode(NavigationMode.Navmesh);
            SetDestination(position);
            isFirstInWaitingLine = true;
        }

        public void SetDestination(Vector3 target_position)
        {
            currentDestination = target_position;
            _ = agent.SetDestination(currentDestination);
            state = State.BuildingPath;
        }

        public void SetNavigationMode(NavigationMode mode)
        {
            if (navigationMode == mode)
            {
                return;
            }

            navigationMode = mode;
            agent.enabled ^= true;

            if (mode == NavigationMode.FreeMove)
            {
                state = State.Idle;
            }
            else
            {
                _ = agent.SetDestination(currentDestination);
                state = State.BuildingPath;
            }
        }

        private void CorrectMovement()
        {
            float max_speed =
                (1.0f - personalSpace.GetCrowdMetrics())
                * maxVelocity
                * maxVelocityMultiplierByEffects;
            max_speed = Mathf.Max(max_speed, 0.0f);

            agent.speed = max_speed;

            Vector3 steering = personalSpace.GetPreferredSteeringNormalized();
            if (steering.sqrMagnitude > 0.0001)
            {
                agent.velocity = steering;
            }
            else
            {
                _ = agent.SetDestination(currentDestination);
            }
        }

        public void Teleport(Vector3 destination)
        {
            transform.position = destination + (agent.baseOffset * Vector3.up);
            state = State.BuildingPath;
        }

        public float? ApproximatePathLength(Vector3 destination)
        {
            if (!agent.enabled)
            {
                Vector3 distance = destination - transform.position;
                distance.y = 0;
                return distance.magnitude;
            }

            NavMeshPath path = new();
            if (agent.CalculatePath(destination, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    float total_length = 0f;
                    for (int i = 0; i < path.corners.Length - 1; i++)
                    {
                        total_length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                    }

                    return total_length;
                }
            }

            return null;
        }

        private float maxVelocityMultiplierByEffects = 1.0f;
        private List<ControllerEffect> registeredEffects = new();

        public void RegisterEffect(ControllerEffect effect)
        {
            registeredEffects.Add(effect);
            maxVelocityMultiplierByEffects *= effect.SpeedMultiplier;
        }

        public void UnregisterEffect(ControllerEffect effect)
        {
            if (!registeredEffects.Remove(effect))
            {
                Debug.LogError("Failed to remove ControllerEffect: Not registered");
                return;
            }

            maxVelocityMultiplierByEffects = 1.0f;
            foreach (ControllerEffect eff in registeredEffects)
            {
                maxVelocityMultiplierByEffects *= eff.SpeedMultiplier;
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 steering = personalSpace.GetPreferredSteeringNormalized();

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + (steering * 10));

            UnityEditor.Handles.Label(transform.position, agent.avoidancePriority.ToString());
        }
    }
}
