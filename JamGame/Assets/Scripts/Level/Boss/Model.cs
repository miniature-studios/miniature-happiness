using Common;
using Level.Boss.Task;
using Level.GlobalTime;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Boss
{
    internal enum TaskState
    {
        Scheduled,
        Active,
        Complete
    }

    [AddComponentMenu("Scripts/Level.Boss.Model")]
    public class Model : SerializedMonoBehaviour
    {
        [Serializable]
        private class MeetingTasks
        {
            public List<TaskWithCost> Tasks = new();
        }

        [Serializable]
        private class TaskWithCost
        {
            [HideLabel]
            [InlineProperty]
            [SerializeReference]
            [FoldoutGroup("Task With Cost")]
            private ITask task;
            public ITask Task => task;

            [SerializeField]
            [FoldoutGroup("Task With Cost")]
            public float Cost;
        }

        [SerializeField]
        private Days maxStressGatherTime;

        // Normalized (0..1)
        [SerializeField]
        [InspectorReadOnly]
        private float stress;
        public float Stress => stress;

        [SerializeField]
        private List<MeetingTasks> meetingTasks = new();
        private List<List<TaskWithCost>> scheduledTasks = new();

        private Dictionary<ITask, TaskState> taskState = new();
        private ObservableCollection<ITask> activeTasks = new();
        public UnityEvent<object, NotifyCollectionChangedEventArgs> ActiveTasksChanged = new();

        private int taskBunchToActivateNext = 0;

        private void Awake()
        {
            activeTasks.CollectionChanged += ActiveTasksChanged.Invoke;
        }

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
                activeTasks.Add(task.Task);
            }

            taskBunchToActivateNext++;
        }

        private void Update()
        {
            stress += Time.deltaTime / maxStressGatherTime.RealTimeSeconds;

            for (int i = 0; i < taskBunchToActivateNext; i++)
            {
                foreach (TaskWithCost task in scheduledTasks[i])
                {
                    if (taskState[task.Task] == TaskState.Active)
                    {
                        if (task.Task.Progress.Complete)
                        {
                            taskState[task.Task] = TaskState.Complete;
                            _ = activeTasks.Remove(task.Task);
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
