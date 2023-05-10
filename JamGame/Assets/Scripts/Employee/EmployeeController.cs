using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EmployeeController : MonoBehaviour
{
    [SerializeField] private float maxVelocity;

    private NavMeshAgent agent;
    private Vector3 averageVelocity = Vector3.zero;
    public Vector3 AverageVelocity => averageVelocity;

    private PersonalSpace personalSpace;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        personalSpace = GetComponentInChildren<PersonalSpace>();
        if (personalSpace == null)
        {
            Debug.LogError("Personal space not found");
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

    private Vector3 currentDestination;

    public void SetDestination(Vector3 target_position)
    {
        moving = true;
        currentDestination = target_position;
        _ = agent.SetDestination(currentDestination);
    }

    private bool moving = false;
    private Vector3 prevPosition;

    private void Update()
    {
        averageVelocity = (transform.position - prevPosition) / Time.deltaTime;
        prevPosition = transform.position;

        if (moving && agent.remainingDistance < 0.01f)
        {
            moving = false;
            OnFinishedMoving?.Invoke();
        }

        agent.speed = (1.0f - personalSpace.GetCrowdMetrics()) * maxVelocity;
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

    public delegate void FinishedMovingHandler();
    public event FinishedMovingHandler OnFinishedMoving;
}
