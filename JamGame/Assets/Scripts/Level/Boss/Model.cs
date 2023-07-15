using Common;
using Level.Boss.Task;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Boss
{
    internal enum TaskState
    {
        Scheduled,
        Active,
        Complete
    }

    [AddComponentMenu("Level.Boss.Model")]
    public class Model : MonoBehaviour
    {
        [Serializable]
        private class TaskWithCost
        {
            public ITask Task;
            public float Cost;
        }

        // TODO: Implement some global time manager and store all the time constants e.g. how much seconds there is in day
        // and compute this according to it.
        [SerializeField]
        private readonly float stressGatherSpeed;

        [SerializeField]
        [InspectorReadOnly]
        private float stress;

        [SerializeField]
        private List<TaskWithCost> scheduledTasks;

        private readonly Dictionary<ITask, TaskState> taskState = new();
        private int taskToActivateNext = 0;

        private void Start()
        {
            foreach (TaskWithCost task in scheduledTasks)
            {
                task.Task.ValidateProviders();
                taskState.Add(task.Task, TaskState.Scheduled);
            }
        }

        private void ActivateNextTask()
        {
            if (taskToActivateNext == scheduledTasks.Count)
            {
                return;
            }

            taskState[scheduledTasks[taskToActivateNext].Task] = TaskState.Active;

            taskToActivateNext++;
        }

        private void Update()
        {
            stress += stressGatherSpeed;

            for (int i = 0; i < scheduledTasks.Count; i++)
            {
                ITask task = scheduledTasks[i].Task;
                if (taskState[task] == TaskState.Active && task.IsComplete())
                {
                    taskState[task] = TaskState.Complete;
                    stress -= scheduledTasks[i].Cost;
                }
            }
        }
    }
}