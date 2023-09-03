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
            if (task is TargetEmployeeAmount)
            {
                description.text = $"Hire at least {progress.Overall} employees";
            }
            else if (task is MaxStressBound mtb)
            {
                description.text =
                    $"Hold maximum stress of employees less than {mtb.MaxStressTarget} for {progress.Overall} seconds";
            }
            else if (task is TargetRoomCount trc)
            {
                description.text =
                    $"Build at least {progress.Overall} [{trc.RoomTitle}]s";
            }
            else if (task is RoomCountUpperBound ub)
            {
                description.text =
                    $"Have at most {ub.UpperBoundInclusive} [{ub.RoomTitle}]s for {progress.Overall} days";
            }
            else
            {
                Debug.LogError("This task type cannot be displayed: missing implementation");
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
