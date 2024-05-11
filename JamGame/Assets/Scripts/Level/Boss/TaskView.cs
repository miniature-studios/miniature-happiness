using System;
using Level.Boss.Task;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Level.Boss
{
    [AddComponentMenu("Scripts/Level/Boss/Level.Boss.TaskView")]
    [RequireComponent(typeof(RectTransform))]
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
        [Required]
        private TMP_Text description;

        [SerializeField]
        private RectTransform progressBar;

        [SerializeField]
        [Required]
        private TMP_Text progressLabel;

        [SerializeField]
        [Required]
        private GameObject unfoldedIcon;

        private RectTransform rectTransform;

        private bool unfolded = false;

        private float descriptionPadding;
        private float foldedDescriptionHeight;

        private void Start()
        {
            rectTransform = transform.GetComponent<RectTransform>();
            foldedDescriptionHeight = description.rectTransform.sizeDelta.y;
            descriptionPadding = rectTransform.sizeDelta.y - foldedDescriptionHeight;
        }

        // Called by toggle.
        public void FoldStateChanged(bool state)
        {
            unfoldedIcon.SetActive(state);
            unfolded = state;
        }

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
                    => $"Have at least {task.EmployeeCountTarget} with quirk [TODO: #170]",
                MaxWaitingLineLength task
                    => $"Keep waiting lines shorter than {task.LengthTarget} for {progress.Overall} days",
                DontSatisfyNeed task
                    => $"Don't let employees satisfy need {task.TargetNeed} for {progress.Overall} days",
                MinEarnPerWorkingSession _
                    => $"Earn at least {progress.Overall} money in a single working session",
                _ => throw new NotImplementedException("This task type is not supported")
            };
        }

        private void FixedUpdate()
        {
            Progress progress = task.Progress;
            _ = Mathf.Clamp01(progress.Completion / progress.Overall);
            //progress_bar.localScale = new Vector3(bar_pos, 1.0f);
            progressLabel.text = $"{progress.Completion:0.#}/{progress.Overall:0.#}";

            Vector2 desired_size = rectTransform.sizeDelta;
            float description_height = unfolded
                ? description.preferredHeight
                : foldedDescriptionHeight;
            desired_size.y = descriptionPadding + description_height;
            rectTransform.sizeDelta = desired_size;
        }
    }
}
