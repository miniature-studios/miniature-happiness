using Common;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EmployeeController))]
[RequireComponent(typeof(NeedCollectionModifier))]
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
    [SerializeField] private Vector2Int currentPosition = new(0, 0);
    private Vector2Int movingToPosition;
    private float satisfyingNeedRemaining = 0.0f;
    private EmployeeController controller;
    [SerializeField] private List<Need> needs = new();
    private Need currentNeed;
    private NeedSlot occupiedSlot;

    [SerializeField] private NeedCollectionModifier needCollectionModifier;
    private float stress;
    [SerializeField] private float stressStartThreshold;
    [SerializeField] private float stressIncreaseSpeed;
    [SerializeField] private float stressLimit;

    private void Start()
    {
        controller = GetComponent<EmployeeController>();
        UpdateNeedPriority();
    }

    private void Update()
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
                    currentNeed.Satisfy();
                    occupiedSlot.Free();
                }
                break;
        }

        UpdateStress();
    }

    private void UpdateStress()
    {
        int unsatisfied_count = 0;
        foreach (Need need in needs)
        {
            if (need.satisfied < stressStartThreshold)
            {
                unsatisfied_count++;
            }
        }

        if (unsatisfied_count * 2 > needs.Count)
        {
            stress += stressIncreaseSpeed * Time.deltaTime;
        }

        if (stress > stressLimit)
        {
            LeaveJob();
        }
    }

    //bool whantsToLeave = true;
    private void LeaveJob()
    {
        //whantsToLeave = true;
    }

    [SerializeField] private List<NeedDesatisfactionSpeedModifier> needDesatisfactionSpeedModifiers;
    public void DesatisfyNeed(NeedType ty, float delta)
    {
        foreach (Need need in needs)
        {
            if (need.Parameters.NeedType == ty)
            {
                float mul = 1.0f;
                foreach (NeedDesatisfactionSpeedModifier mod in needDesatisfactionSpeedModifiers)
                {
                    if (mod.ty == ty)
                    {
                        mul = mod.multiplier;
                        break;
                    }
                }

                need.Desatisfy(delta * mul);
            }
        }
    }

    public void AddNeed(NeedParameters need)
    {
        needs.Add(new Need(need));
    }

    private void UpdateNeedPriority()
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

            NeedSlot booked = location.TryBookSlotInNeedProvider(this, need.Parameters.NeedType);
            if (booked != null)
            {
                currentNeed = need;
                currentNeed.Parameters = booked.GetNeedParameters(currentNeed.Parameters.NeedType);
                currentNeed.Parameters = needCollectionModifier.Apply(new List<NeedParameters>() { currentNeed.Parameters })[0];

                occupiedSlot = booked;
                MoveToSlot(booked);
                break;
            }
        }
    }

    private void MoveToSlot(NeedSlot slot)
    {
        state = State.Walking;

        List<Vector2Int> path_points = location.PathfindingProvider.FindPath(currentPosition, slot.room.position);
        if (path_points.Count < 3)
        {
            FinishedMoving();
            return;
        }

        List<(Room, RoomInternalPath)> path = PathPointsToPath(path_points);

        movingToPosition = path[^1].Item1.position;

        controller.SetPath(path);
    }

    private List<(Room, RoomInternalPath)> PathPointsToPath(List<Vector2Int> path_points)
    {
        List<(Room, RoomInternalPath)> path = new();

        for (int i = 0; i < path_points.Count; i++)
        {
            Direction from_direction = Direction.Center;
            Direction to_direction = Direction.Center;

            if (i > 0)
            {
                from_direction = (path_points[i - 1] - path_points[i]).ToDirection();
            }

            if (i < path_points.Count - 1)
            {
                to_direction = (path_points[i + 1] - path_points[i]).ToDirection();
            }

            Room room = location.rooms[path_points[i]];
            RoomInternalPath int_path = room.GetInternalPath(from_direction, to_direction);
            if (int_path == null)
            {
                Debug.LogError($"Internal path in room {room.position} not found for directions {from_direction} {to_direction}");
            }

            path.Add((room, int_path));
        }

        return path;
    }

    private void FinishedMoving()
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
