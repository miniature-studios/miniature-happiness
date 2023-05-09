using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                    currentNeed.Satisfy();
                    targetNeedProvider.Release();
                    targetNeedProvider = null;
                }
                break;
        }
    }

    private void UpdateNeeds(float delta_time)
    {
        foreach (Need need in needs)
        {
            need.Desatisfy(delta_time);
        }
    }

    public void AddNeed(Need.NeedProperties properties)
    {
        Need need = new(properties);
        foreach (NeedModifiers modifer in registeredModifiers)
        {
            need.RegisterModifier(modifer);
        }
        needs.Add(need);
    }

    private NeedProvider GetTargetNeedProvider()
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

                if (distance.Value < min_distance)
                {
                    min_distance = distance.Value;
                    selected_provider = provider;
                }
            }

            if (selected_provider == null)
            {
                continue;
            }

            currentNeed = need;
            return selected_provider;
        }

        Debug.LogError("Failed to select target NeedProvider");
        return null;
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

    public void RegisterModifier(NeedModifiers modifiers)
    {
        if (registeredModifiers.Contains(modifiers))
        {
            Debug.LogWarning("Modifiers already registered");
            return;
        }

        registeredModifiers.Add(modifiers);
        foreach (Need need in needs)
        {
            need.RegisterModifier(modifiers);
        }
    }

    public void UnregisterModifier(NeedModifiers modifiers)
    {
        if (!registeredModifiers.Remove(modifiers))
        {
            Debug.LogWarning("Modifiers to unregister not found");
            return;
        }

        foreach (Need need in needs)
        {
            need.UnregisterModifier(modifiers);
        }
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
