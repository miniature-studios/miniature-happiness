using UnityEngine;

public class LevelExecuter : MonoBehaviour
{
    public TileBuilderController TileBuilderController;
    public EmployeeFactory EmployeeFactory;
    public FinancesController FinancesController;
    public BossStressController BossStressController;
    public ShopController ShopController;
    [HideInInspector] public Tariffs Tariffs;
    public void SetTarrifs(Tariffs tariffs)
    {
        Tariffs = tariffs;
    }
}

