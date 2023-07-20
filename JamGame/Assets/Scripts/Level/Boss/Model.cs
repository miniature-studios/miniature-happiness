using Common;
using Level.Boss.Task;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private class MeetingTasks
        {
            public List<TaskWithCost> Tasks;
        }

        [Serializable]
        private class TaskWithCost
        {
            [SerializeField]
            private SerializedTask task;
            public ITask Task => task.ToTask();

            public float Cost;
        }

        // TODO: Implement some global time manager and store all the time constants e.g. how much seconds there is in day
        // and compute this according to it.
        [SerializeField]
        private float stressGatherSpeed;

        [SerializeField]
        [InspectorReadOnly]
        private float stress;

        [SerializeField]
        private List<MeetingTasks> meetingTasks;
        private readonly List<TaskWithCost> scheduledTasks = new();

        private readonly Dictionary<ITask, TaskState> taskState = new();
        private int taskToActivateNext = 0;

        private void Start()
        {
            foreach (MeetingTasks meeting_tasks in meetingTasks)
            {
                scheduledTasks.AddRange(meeting_tasks.Tasks.Select(x => x));
            }

            foreach (TaskWithCost task in scheduledTasks)
            {
                task.Task.ValidateProviders();
                taskState.Add(task.Task, TaskState.Scheduled);
            }
        }

        public void ActivateNextTask()
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
                if (taskState[task] == TaskState.Active)
                {
                    if (task.IsComplete())
                    {
                        taskState[task] = TaskState.Complete;
                        stress -= scheduledTasks[i].Cost;
                    }
                    else
                    {
                        task.Update(Time.deltaTime);
                    }
                }
            }
        }
    }
}