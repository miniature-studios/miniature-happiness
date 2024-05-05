using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Level.Boss.Task;
using Level.GlobalTime;
using Sirenix.OdinInspector;
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

    [AddComponentMenu("Scripts/Level/Boss/Level.Boss.Model")]
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
            [HideLabel]
            [InlineProperty]
            [SerializeReference]
            [FoldoutGroup("Task")]
            private ITask task;
            public ITask Task => task;

            [SerializeField]
            [FoldoutGroup("Task")]
            [Unit(Units.Percent)]
            private float cost;

            public float CostNormalized => cost / 100.0f;
        }

        [SerializeField]
        private Days maxStressGatherTime;

        private float stressNormalized;
        public float StressNormalized => stressNormalized;

#if UNITY_EDITOR
        [SerializeField]
        [ReadOnly]
        [Unit(Units.Percent)]
        private float stress;
#endif

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

        public bool AllTasksAreComplete()
        {
            return taskState.Values.All(state => state == TaskState.Complete);
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
                task.Task.Init();
            }

            taskBunchToActivateNext++;
        }

        private void Update()
        {
            stressNormalized += Time.deltaTime / maxStressGatherTime.RealTimeSeconds.Value;

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
                            stressNormalized -= task.CostNormalized;
                            stressNormalized = Mathf.Max(0, stressNormalized);
                        }
                        else
                        {
                            task.Task.Update(RealTimeSeconds.FromDeltaTime());
                        }
                    }
                }
            }

            if (stressNormalized > 1)
            {
                // TODO: Lose game (#164)
            }

#if UNITY_EDITOR
            stress = stressNormalized * 100.0f;
#endif
        }
    }
}
