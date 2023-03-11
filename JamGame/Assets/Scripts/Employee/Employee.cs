using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EmployeeController))]
public class Employee : MonoBehaviour
{
    enum State
    {
        Idle,
        Walking,
        SatisfyingNeed
    }

    State state = State.Idle;

    [SerializeField] Location location;
    [SerializeField] Vector2Int currentPosition = new Vector2Int(0, 0);
    Vector2Int movingToPosition;

    float satisfyingNeedRemaining = 0.0f;

    EmployeeController controller;
    List<Need> needs = new List<Need>();
    Need currentNeed;

    void Start()
    {
        controller = GetComponent<EmployeeController>();
        UpdateNeedPriority();
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                UpdateNeedPriority();
                break;
            case State.Walking:
                break;
            case State.SatisfyingNeed:
                satisfyingNeedRemaining -= Time.deltaTime;
                if (satisfyingNeedRemaining < 0.0f)
                {
                    state = State.Idle;
                    // TODO: Change satisfaction level.
                }
                break;
        }
    }

    void UpdateNeedPriority()
    {
        needs.Sort((x, y) => x.satisfied.CompareTo(y.satisfied));

        if (state == State.Idle)
        {
            // Force select top-priority need.
            currentNeed = null;
        }

        foreach (var need in needs)
        {
            if (need == currentNeed)
                break;

            NeedProvider.Slot booked = location.TryBookSlotInNeedProvider(this, need.NeedType);
            if (booked != null)
            {
                currentNeed = need;
                MoveToSlot(booked);
            }
        }
    }

    void MoveToSlot(NeedProvider.Slot slot)
    {
        state = State.Walking;

        List<(Room, RoomInternalPath)> path = new List<(Room, RoomInternalPath)>();
        location.PathfindingProvider.FindPath(currentPosition);
        movingToPosition = path[path.Count - 1].Item1.position;

        controller.SetPath(path);
    }

    void FinishedMoving()
    {
        currentPosition = movingToPosition;
        state = State.SatisfyingNeed;
        satisfyingNeedRemaining = currentNeed.GetSatisfactionTime();
    }

    private void OnEnable()
    {
        if (controller == null)
            controller = GetComponent<EmployeeController>();
        controller.OnFinishedMoving += FinishedMoving;
    }

    private void OnDisable()
    {
        controller.OnFinishedMoving -= FinishedMoving;
    }
}
