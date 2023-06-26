using Overlay;
using UnityEngine;

namespace Employee
{
    [RequireComponent(typeof(EmployeeImpl))]
    public partial class EmployeeView : MonoBehaviour
    {
        private EmployeeImpl employee;

        private void Start()
        {
            employee = GetComponent<EmployeeImpl>();
            meshRenderer = GetComponent<MeshRenderer>();
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

    [RequireComponent(typeof(MeshRenderer))]
    public partial class EmployeeView : IOverlayRenderer<Overlay.Stress>
    {
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

            meshRenderer.materials[0].color = Color.Lerp(
                appliedStressOverlay.MinimalStressColor,
                appliedStressOverlay.MaximalStressColor,
                normalized_stress
            );
        }

        public void RevertStressOverlay()
        {
            appliedStressOverlay = null;
            meshRenderer.materials[0].color = Color.white;
        }
    }

    public partial class EmployeeView : IOverlayRenderer<Overlay.ExtendedEmployeeInfo>
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
