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
}

[RequireComponent(typeof(MeshRenderer))]
public partial class EmployeeView : IOverlayRenderer<StressOverlay>
{
    private MeshRenderer meshRenderer;

    public void ApplyOverlay(StressOverlay overlay)
    {
        float normalized_stress = employee.Stress.Value;

        normalized_stress = (normalized_stress - overlay.MinimalStressBound)
            / (overlay.MaximalStressBound - overlay.MinimalStressBound);
        normalized_stress = Mathf.Clamp01(normalized_stress);

        meshRenderer.materials[0].color = Color.Lerp(
            overlay.MinimalStressColor,
            overlay.MaximalStressColor,
            normalized_stress
        );
    }

    public void RevertOverlay(StressOverlay overlay)
    {
        meshRenderer.materials[0].color = Color.white;
    }
}

public partial class EmployeeView : IOverlayRenderer<ExtendedEmployeeInfoOverlay>
{
    public void ApplyOverlay(ExtendedEmployeeInfoOverlay overlay)
    {

    }

    public void RevertOverlay(ExtendedEmployeeInfoOverlay overlay)
    {

    }
}
