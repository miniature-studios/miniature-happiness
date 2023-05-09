[RequireComponent(typeof(EmployeeController))]
public class Employee : MonoBehaviour
{
    private enum State
    {
        Idle,
        Walking,
        SatisfyingNeed
    }

    private State state = State.Idle;

    [SerializeField] private Location location;

    [SerializeField] private List<Need> needs = new();
    private Need currentNeed;
    private float satisfyingNeedRemaining = 0.0f;
    private NeedProvider targetNeedProvider = null;

    private EmployeeController controller;

    private readonly List<NeedModifiers> registeredModifiers = new();

    private void Start()
    [SerializeField] NeedCollectionModifier needCollectionModifier;

    float stress;
    [SerializeField] float stressStartThreshold;
    [SerializeField] float stressIncreaseSpeed;
    [SerializeField] float stressLimit;

    void Start()
    {
        controller = GetComponent<EmployeeController>();
    }

    private void Update()
    {
        UpdateNeeds(Time.deltaTime);

        switch (state)
        {
            case State.Idle:
                targetNeedProvider = GetTargetNeedProvider();
                state = State.Walking;
                // TODO: Fetch target position from NeedProvider in future.
                controller.SetDestination(targetNeedProvider.transform.position);
                break;
            case State.Walking:
                break;
            case State.SatisfyingNeed:
                satisfyingNeedRemaining -= Time.deltaTime;
                if (satisfyingNeedRemaining < 0.0f)
                {
                    state = State.Idle;
                    private void UpdateNeeds(float delta_time)
                    targetNeedProvider.Release();
                    foreach (Need need in needs)
                    {
                        need.Desatisfy(delta_time);
                    }
                }
                LeaveJob();
                public void AddNeed(Need.NeedProperties properties)
    //bool whantsToLeave = true;
        Need need = new(properties);
                foreach (NeedModifiers modifer in registeredModifiers)
                {
                    need.RegisterModifier(modifer);
                }
                needs.Add(need);
        }
        {
            mul = mod.multiplier;
            break;

            private NeedProvider GetTargetNeedProvider()
            {
                needs.Add(new Need(need));
            }

            void UpdateNeedPriority()
            {
                needs.Sort((x, y) => x.satisfied.CompareTo(y.satisfied));

                if (state == State.Idle)
                {
                    // Force select top-priority need.
                    currentNeed = null;
                }

                foreach (Need need in needs)
                {
                    if (need == currentNeed)
                    {
                        break;
                    }

                    List<NeedProvider> available_providers = location.FindAllAvailableProviders(this, need.NeedType).ToList();

                    NeedProvider selected_provider = null;
                    float min_distance = float.PositiveInfinity;
                    foreach (NeedProvider provider in available_providers)
                    {
                        float? distance = controller.ComputePathLength(provider);
                        if (!distance.HasValue)
                        {
                            Debug.LogWarning($"NeedProvider {provider.name} is inaccessible");
                            continue;
                        }
                        if (selected_provider == null)
                        {
                            continue;
                        }
                        state = State.Walking;
                        currentNeed = need;
                        return selected_provider;
                    }
                    {
                        FinishedMoving();
                        return;
                    }

                    private void FinishedMoving()
                    {
                        if (targetNeedProvider.TryTake(this))
                        {
                            state = State.SatisfyingNeed;
                            satisfyingNeedRemaining = currentNeed.GetProperties().SatisfactionTime;
                        }
                        else
                        {
                            state = State.Idle;
                        }
                    }

                    Debug.LogError("Failed to select target NeedProvider");
                    return null;
                }

                Debug.LogWarning("Modifiers already registered");
                return;
            }
            from_direction = (path_points[i - 1] - path_points[i]).ToDirection();
            if (i < path_points.Count - 1)
                to_direction = (path_points[i + 1] - path_points[i]).ToDirection();

            var room = location.rooms[path_points[i]];
            var int_path = room.GetInternalPath(from_direction, to_direction);
            if (int_path == null)
                Debug.LogError($"Internal path in room {room.position} not found for directions {from_direction} {to_direction}");

            registeredModifiers.Add(modifiers);
            foreach (Need need in needs)
            {
                need.RegisterModifier(modifiers);
            }
        }

        foreach (Need need in needs)
        {
            need.UnregisterModifier(modifiers);
        }
        return;
    }

    void FinishedMoving()
    {
        currentPosition = movingToPosition;
        state = State.SatisfyingNeed;
        satisfyingNeedRemaining = currentNeed.Parameters.GetSatisfactionTime();
    }

    private void OnEnable()
    {
        if (controller == null)
        {
            controller = GetComponent<EmployeeController>();
        }

        controller.OnFinishedMoving += FinishedMoving;
    }

    private void OnDisable()
    {
        controller.OnFinishedMoving -= FinishedMoving;
    }
}
