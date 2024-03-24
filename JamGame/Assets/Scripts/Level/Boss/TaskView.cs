using System;
using Level.Boss.Task;
using TMPro;
using UnityEngine;

namespace Level.Boss
{
    [AddComponentMenu("Scripts/Level/Boss/Level.Boss.TaskView")]
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

            description.text = task switch
            {
                TargetEmployeeAmount => $"Hire at least {progress.Overall} employees",
                MaxStressBound task
                    => $"Hold maximum stress of employees less than {task.MaxStressTarget} for {progress.Overall} seconds",
                TargetRoomCount task => $"Build at least {progress.Overall} [{task.RoomTitle}]s",
                RoomCountUpperBound task
                    => $"Have at most {task.UpperBoundInclusive} [{task.RoomTitle}]s for {progress.Overall} days",
                MinBalance task
                    => $"Have at least {task.MinBalanceTarget} for {progress.Overall} days",
                MinEmployeesWithQuirk task
                    => $"Have at least {task.EmployeeCountTarget} with quirk [TODO: show quirk icon here]",
                MaxWaitingLineLength task
                    => $"Keep waiting lines shorter than {task.LengthTarget} for {progress.Overall} days",
                DontSatisfyNeed task
                    => $"Don't let employees satisfy need {task.TargetNeed} for {progress.Overall} days",
                MinEarnPerWorkingSession _
                    => $"Earn at least {progress.Overall} money in a single working session",
                _ => throw new NotImplementedException("This task type is not supported")
            };
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
