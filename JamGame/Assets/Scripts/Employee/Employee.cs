using Common;
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
    [SerializeField] List<Need> needs = new List<Need>();
    Need currentNeed;
    NeedSlot occupiedSlot;

    void Start()
    {
        controller = GetComponent<EmployeeController>();
        UpdateNeedPriority();
    }

    void Update()
    {
        foreach (var need in needs)
            need.Update(Time.deltaTime);

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
    }

    public void AddNeed(NeedParameters need)
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

        foreach (var need in needs)
        {
            if (need == currentNeed)
                break;

            NeedSlot booked = location.TryBookSlotInNeedProvider(this, need.Parameters.NeedType);
            if (booked != null)
            {
                currentNeed = need;
                occupiedSlot = booked;
                MoveToSlot(booked);
                break;
            }
        }
    }

    void MoveToSlot(NeedSlot slot)
    {
        state = State.Walking;

        var path_points = location.PathfindingProvider.FindPath(currentPosition, slot.room.position);
        var path = PathPointsToPath(path_points);

        movingToPosition = path[path.Count - 1].Item1.position;

        controller.SetPath(path);
    }

    List<(Room, RoomInternalPath)> PathPointsToPath(List<Vector2Int> path_points)
    {
        List<(Room, RoomInternalPath)> path = new List<(Room, RoomInternalPath)>();

        for (int i = 0; i < path_points.Count; i++)
        {
            Direction from_direction = Direction.Center;
            Direction to_direction = Direction.Center;

            if (i > 0)
                from_direction = (path_points[i - 1] - path_points[i]).ToDirection();
            if (i < path_points.Count - 1)
                to_direction = (path_points[i + 1] - path_points[i]).ToDirection();

            var room = location.rooms[path_points[i]];
            var int_path = room.GetInternalPath(from_direction, to_direction);
            path.Add((room, int_path));
        }

        return path;
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
            controller = GetComponent<EmployeeController>();
        controller.OnFinishedMoving += FinishedMoving;
    }

    private void OnDisable()
    {
        controller.OnFinishedMoving -= FinishedMoving;
    }
}
