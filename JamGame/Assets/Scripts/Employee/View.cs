using System;
using System.Collections.Generic;
using System.Linq;
using Overlay;
using UnityEngine;

namespace Employee
{
    [Serializable]
    internal struct AnimationByState
    {
        public string AnimationName;
        public State State;
    }

    [RequireComponent(typeof(EmployeeImpl))]
    [AddComponentMenu("Scripts/Employee/Employee.View")]
    public partial class View : MonoBehaviour
    {
        private EmployeeImpl employee;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private List<AnimationByState> rawAnimationsByState;
        private Dictionary<State, string> AnimationsByState =>
            new Lazy<Dictionary<State, string>>(
                rawAnimationsByState.ToDictionary(key => key.State, value => value.AnimationName)
            ).Value;

        private void Start()
        {
            employee = GetComponent<EmployeeImpl>();
            employee.OnStateChanged += EmployeeStateChanged;
        }

        private void EmployeeStateChanged(State state)
        {
            animator.Play(AnimationsByState[state]);
        }

        private void Update()
        {
            UpdateStressOverlay();
        }

        public void RevertOverlays()
        {
            RevertStressOverlay();
            RevertExtendedInfoOverlay();
        }
    }

    public partial class View : IOverlayRenderer<Stress>
    {
        [SerializeField]
        private SkinnedMeshRenderer meshRenderer;

        private Stress appliedStressOverlay;

        public void ApplyOverlay(Stress overlay)
        {
            appliedStressOverlay = overlay;
        }

        private void UpdateStressOverlay()
        {
            if (appliedStressOverlay == null)
            {
                return;
            }

            float normalized_stress = employee.Stress.Value;
            normalized_stress =
                (normalized_stress - appliedStressOverlay.MinimalStressBound)
                / (
                    appliedStressOverlay.MaximalStressBound
                    - appliedStressOverlay.MinimalStressBound
                );
            normalized_stress = Mathf.Clamp01(normalized_stress);

            Color tint = appliedStressOverlay.Gradient.Evaluate(normalized_stress);
            SetColorTint(tint);
        }

        public void RevertStressOverlay()
        {
            appliedStressOverlay = null;

            SetColorTint(Color.white);
        }

        private void SetColorTint(Color color)
        {
            foreach (Material material in meshRenderer.materials)
            {
                material.color = color;
            }
        }
    }

    public partial class View : IOverlayRenderer<ExtendedEmployeeInfo>
    {
        private GameObject overlayUI;

        public void ApplyOverlay(ExtendedEmployeeInfo overlay)
        {
            if (overlayUI == null)
            {
                overlayUI = Instantiate(overlay.UIPrefab, transform, false);
            }

            overlayUI.SetActive(true);
        }

        public void RevertExtendedInfoOverlay()
        {
            if (overlayUI == null)
            {
                return;
            }

            overlayUI.SetActive(false);
        }
    }
}
