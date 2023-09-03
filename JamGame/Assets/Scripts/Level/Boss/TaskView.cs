using Level.Boss.Task;
using TMPro;
using UnityEngine;

namespace Level.Boss
{
    [AddComponentMenu("Scripts/Level.Boss.TaskView")]
    public class TaskView : MonoBehaviour
    {
        private ITask task;
        public ITask Task
        {
            get => task;
            set
            {
                task = value;
                UpdateFromTask();
            }
        }

        [SerializeField]
        private TMP_Text description;

        [SerializeField]
        private RectTransform progress_bar;

        [SerializeField]
        private TMP_Text progress_label;

        private void UpdateFromTask()
        {
            Progress progress = task.Progress;

            switch (task)
            {
                case TargetEmployeeAmount:
                    description.text = $"Hire at least {progress.Overall} employees";
                    break;
                case MaxStressBound task:
                    description.text =
                    $"Hold maximum stress of employees less than {task.MaxStressTarget} for {progress.Overall} seconds";
                    break;
                case TargetRoomCount task:
                    description.text = $"Build at least {progress.Overall} [{task.RoomTitle}]s";
                    break;
                case RoomCountUpperBound task:
                    description.text =
                    $"Have at most {task.UpperBoundInclusive} [{task.RoomTitle}]s for {progress.Overall} days";
                    break;
                default:
                    Debug.LogError("This task type cannot be displayed: missing implementation");
                    break;
            }
        }

        private void Update()
        {
            Progress progress = task.Progress;
            float bar_pos = Mathf.Clamp01(progress.Completion / progress.Overall);
            progress_bar.localScale = new Vector3(bar_pos, 1.0f);
            progress_label.text = $"{progress.Completion:0.#}/{progress.Overall:0.#}";
        }
    }
}
