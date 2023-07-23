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
        private readonly List<List<TaskWithCost>> scheduledTasks = new();

        private readonly Dictionary<ITask, TaskState> taskState = new();
        private int taskBunchToActivateNext = 0;

        private void Start()
        {
            foreach (MeetingTasks meeting_tasks in meetingTasks)
            {
                scheduledTasks.Add(meeting_tasks.Tasks.Select(x => x).ToList());
            }

            foreach (List<TaskWithCost> task_bunch in scheduledTasks)
            {
                foreach (TaskWithCost task in task_bunch)
                {
                    task.Task.ValidateProviders();
                    taskState.Add(task.Task, TaskState.Scheduled);
                }
            }
        }

        public void ActivateNextTaskBunch()
        {
            if (taskBunchToActivateNext == scheduledTasks.Count)
            {
                return;
            }

            foreach (TaskWithCost task in scheduledTasks[taskBunchToActivateNext])
            {
                taskState[task.Task] = TaskState.Active;
            }

            taskBunchToActivateNext++;
        }

        private void Update()
        {
            stress += stressGatherSpeed;

            for (int i = 0; i < taskBunchToActivateNext; i++)
            {
                foreach (TaskWithCost task in scheduledTasks[i])
                {
                    if (taskState[task.Task] == TaskState.Active)
                    {
                        if (task.Task.IsComplete())
                        {
                            taskState[task.Task] = TaskState.Complete;
                            stress -= task.Cost;
                        }
                        else
                        {
                            task.Task.Update(Time.deltaTime);
                        }
                    }
                }
            }
        }
    }
}