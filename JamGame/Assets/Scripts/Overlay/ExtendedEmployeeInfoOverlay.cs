using UnityEngine;

public class ExtendedEmployeeInfoOverlay : MonoBehaviour, IOverlay
{
    // TODO: template of UI for employee?

    public void Activate(Location location)
    {
        location.ApplyOverlay(this);
    }
}
