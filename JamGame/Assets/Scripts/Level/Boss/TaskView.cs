using System;
using Level.Boss.Task;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        private GameObject progressScaleParent;

        [SerializeField]
        [Required]
        private Color progressComplete;

        [SerializeField]
        [Required]
        private Color progressIncomplete;

        private Image[] progressScale;

        [SerializeField]
        [Required]
        private GameObject unfoldedIcon;

        private RectTransform rectTransform;

        private float initialHeight;

        private void Start()
        {
            rectTransform = transform.GetComponent<RectTransform>();
            initialHeight = rectTransform.sizeDelta.y;

            progressScale = progressScaleParent.GetComponentsInChildren<Image>();
        }

        // Called by toggle.
        public void FoldStateChanged(bool unfolded)
        {
            unfoldedIcon.SetActive(unfolded);

            Vector2 desired_size = rectTransform.sizeDelta;
            if (unfolded)
            {
                float current_size = description.renderedHeight;
                desired_size.y = description.preferredHeight + initialHeight - current_size;
            }
            else
            {
                desired_size.y = initialHeight;
            }

            rectTransform.sizeDelta = desired_size;
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
                    => $"Have at least {task.MinBalanceTarget} coins for {progress.Overall} days",
                MinEmployeesWithQuirk task
                    => $"Have at least {task.EmployeeCountTarget} with quirk [TODO: #170] {task.TargetQuirk.FullName}",
                MaxWaitingLineLength task
                    => $"Keep waiting lines shorter than {task.LengthTarget} for {progress.Overall} days",
                DontSatisfyNeed task
                    => $"Don't let employees satisfy need {task.TargetNeed} for {progress.Overall} days",
                MinEarnPerWorkingSession _
                    => $"Earn at least {progress.Overall} money in a single working session",
                _ => throw new NotImplementedException("This task type is not supported")
            };

            progressScale = progressScaleParent.GetComponentsInChildren<Image>();
            UpdateProgress();
        }

        private void FixedUpdate()
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            Progress progress = task.Progress;

            float progress_normalized = Mathf.Clamp01(progress.Completion / progress.Overall);
            int scale_divisions = Mathf.RoundToInt(progress_normalized * progressScale.Length);
            for (int i = 0; i < progressScale.Length; i++)
            {
                Color color = (i < scale_divisions) ? progressComplete : progressIncomplete;
                progressScale[i].color = color;
            }

            progressLabel.text = $"{progress.Completion:0.#}/{progress.Overall:0.#}";
        }
    }
}
