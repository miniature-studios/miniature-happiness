using UnityEngine;

public class LevelExecuter : MonoBehaviour
{
    public TileBuilderController TileBuilderController;
    public EmployeeFactory EmployeeFactory;
    [HideInInspector] public Tariffs Tariffs;
    public void SetTarrifs(Tariffs tariffs)
    {
        Tariffs = tariffs;
    }
}

