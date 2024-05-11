using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Level.Boss.Task;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Boss
{
    [AddComponentMenu("Scripts/Level/Boss/Level.Boss.TaskListView")]
    public class TaskListView : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private GameObject tasksParent;

        [SerializeField]
        [Required]
        private TaskView taskViewPrefab;

        private List<TaskView> task_views = new();

        public void OnActiveTasksChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddTaskToList(e.NewItems[0] as ITask);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveTaskFromList(e.OldItems[0] as ITask);
                    break;
                default:
                    Debug.LogError(
                        $"Unexpected variant of NotifyCollectionChangedAction: {e.Action}"
                    );
                    throw new ArgumentException();
            }
        }

        private void AddTaskToList(ITask task)
        {
            TaskView task_view = Instantiate(taskViewPrefab, tasksParent.transform);
            task_view.Task = task;
            task_views.Add(task_view);
        }

        private void RemoveTaskFromList(ITask task)
        {
            foreach (TaskView task_view in task_views)
            {
                if (task_view.Task == task)
                {
                    Destroy(task_view.gameObject);
                    break;
                }
            }
        }
    }
}
