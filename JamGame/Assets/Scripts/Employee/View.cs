using Overlay;
using UnityEngine;

namespace Employee
{
    [RequireComponent(typeof(EmployeeImpl))]
    [AddComponentMenu("Scripts/Employee.View")]
    public partial class View : MonoBehaviour
    {
        private EmployeeImpl employee;

        private void Start()
        {
            employee = GetComponent<EmployeeImpl>();
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

    public partial class View : IOverlayRenderer<Overlay.Stress>
    {
        [SerializeField]
        private MeshRenderer meshRenderer;

        private Overlay.Stress appliedStressOverlay;

        public void ApplyOverlay(Overlay.Stress overlay)
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

    public partial class View : IOverlayRenderer<Overlay.ExtendedEmployeeInfo>
    {
        private GameObject overlayUI;

        public void ApplyOverlay(Overlay.ExtendedEmployeeInfo overlay)
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
