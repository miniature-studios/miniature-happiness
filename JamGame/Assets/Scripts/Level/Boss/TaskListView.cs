using Level.Boss.Task;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace Level.Boss
{
    [AddComponentMenu("Scripts/Level.Boss.TaskListView")]
    public class TaskListView : MonoBehaviour
    {
        [SerializeField] private GameObject tasks_parent;
        [SerializeField] private TaskView taskViewPrefab;

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
            TaskView task_view = Instantiate(taskViewPrefab, tasks_parent.transform);
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
