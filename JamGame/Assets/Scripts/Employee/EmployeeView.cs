using UnityEngine;

[RequireComponent(typeof(Employee))]
public partial class EmployeeView : MonoBehaviour
{
    private Employee employee;

    private void Start()
    {
        employee = GetComponent<Employee>();
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
public partial class EmployeeView : IOverlayRenderer<StressOverlay>
{
    private MeshRenderer meshRenderer;
    private StressOverlay appliedStressOverlay;

    public void ApplyOverlay(StressOverlay overlay)
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
            / (appliedStressOverlay.MaximalStressBound - appliedStressOverlay.MinimalStressBound);
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

public partial class EmployeeView : IOverlayRenderer<ExtendedEmployeeInfoOverlay>
{
    private GameObject overlayUI;

    public void ApplyOverlay(ExtendedEmployeeInfoOverlay overlay)
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
