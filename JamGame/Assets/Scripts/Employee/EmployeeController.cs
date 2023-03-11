using System.Collections.Generic;
using UnityEngine;

public class EmployeeController : MonoBehaviour
{
    [SerializeField] float speed;

    List<(Room, RoomInternalPath)> currentPath;

    public void SetPath(List<(Room, RoomInternalPath)> path)
    {
        currentPath = path;

        // FIXME: Employee can move from slot to slot in bounds of a single room in future.
        if (path[0].Item1 == path[^1].Item1)
        {
            OnFinishedMoving?.Invoke();
            return;
        }

        moving = true;
        current_internal_path = 0;
        current_internal_path_normalized_time = 0;
        current_internal_path_length = path[0].Item2.GetPathLength();
    }

    bool moving = false;
    public void Update()
    {
        if (moving)
            Move();
    }

    int current_internal_path;
    float current_internal_path_normalized_time;
    float current_internal_path_length;
    void Move()
    {
        float current_path_rem = 1.0f - current_internal_path_normalized_time;
        current_path_rem *= current_internal_path_length / speed;
        if (current_path_rem < Time.deltaTime)
        {
            float next_path_time_offset = Time.deltaTime - current_path_rem;

            if (current_internal_path == currentPath.Count - 1)
            {
                moving = false;
                OnFinishedMoving?.Invoke();
                return;
            }
            current_internal_path++;

            current_internal_path_length = currentPath[current_internal_path].Item2.GetPathLength();
            current_internal_path_normalized_time = next_path_time_offset * speed / current_internal_path_length;
        }

        current_internal_path_normalized_time += Time.deltaTime / current_internal_path_length * speed;

        // FIXME: Incapsulate fetching global position into RoomInternalPath
        var new_position = currentPath[current_internal_path].Item2.GetPathPoint(current_internal_path_normalized_time);
        transform.position = new_position + currentPath[current_internal_path].Item1.transform.position;
    }

    public delegate void FinishedMovingHandler();
    public event FinishedMovingHandler OnFinishedMoving;
}
