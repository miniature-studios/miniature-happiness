using Overlay;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Employee
{
    [RequireComponent(typeof(EmployeeImpl))]
    [AddComponentMenu("Scripts/Employee/Employee.View")]
    public partial class View : MonoBehaviour
    {
        private EmployeeImpl employee;

        [Required]
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private float animationSpeedMultiplier = 1.5f;

        [Required]
        [SerializeField]
        private NavMeshAgent agent;

        private void Start()
        {
            employee = GetComponent<EmployeeImpl>();
        }

        private void Update()
        {
            UpdateStressOverlay();
            animator.SetFloat(
                "WalkSpeed",
                agent.velocity.magnitude / agent.speed * animationSpeedMultiplier
            );
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

            float normalized_stress = employee.Stress.Stress;
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
        [SerializeField]
        [Required]
        [ChildGameObjectsOnly]
        private ExtendedInfo.View extendedInfoView;

        public void ApplyOverlay(ExtendedEmployeeInfo overlay)
        {
            extendedInfoView.gameObject.SetActive(true);
        }

        public void RevertExtendedInfoOverlay()
        {
            extendedInfoView.gameObject.SetActive(false);
        }
    }
}
