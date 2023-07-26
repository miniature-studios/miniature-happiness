using Location;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Employee
{
    [RequireComponent(typeof(NavMeshAgent))]
    [AddComponentMenu("Employee.Controller")]
    public class Controller : MonoBehaviour, IEffectExecutor<ControllerEffect>
    {
        private enum State
        {
            Idle,
            Moving,
            BuildingPath,
        }

        [SerializeField]
        private float maxVelocity;

        private NavMeshAgent agent;
        private Vector3 averageVelocity = Vector3.zero;
        public Vector3 AverageVelocity => averageVelocity;

        private PersonalSpace personalSpace;

        private Vector3 currentDestination;
        private Vector3 prevPosition;

        private State state = State.Idle;

        public delegate void FinishedMovingHandler();
        public event FinishedMovingHandler OnFinishedMoving;

        public void SetDestination(Vector3 target_position)
        {
            currentDestination = target_position;
            _ = agent.SetDestination(currentDestination);
            state = State.BuildingPath;
        }

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();

            personalSpace = GetComponentInChildren<PersonalSpace>();
            if (personalSpace == null)
            {
                Debug.LogError("Personal space not found");
            }
        }

        private void Update()
        {
            averageVelocity = (transform.position - prevPosition) / Time.deltaTime;
            prevPosition = transform.position;

            switch (state)
            {
                case State.Idle:
                    break;
                case State.BuildingPath:
                    if (!agent.pathPending)
                    {
                        state = State.Moving;
                    }
                    break;
                case State.Moving:
                    CorrectMovement();
                    if (agent.remainingDistance < 0.01f)
                    {
                        state = State.Idle;
                        OnFinishedMoving?.Invoke();
                    }
                    break;
            }
        }

        private void CorrectMovement()
        {
            agent.speed =
                (1.0f - personalSpace.GetCrowdMetrics())
                * maxVelocity
                * maxVelocityMultiplierByEffects;

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

        public float? ComputePathLength(NeedProvider need_provider)
        {
            NavMeshPath path = new();
            if (agent.CalculatePath(need_provider.transform.position, path))
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
    }
}
