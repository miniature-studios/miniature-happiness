using System;
using UnityEngine;

public partial class LevelExecutor : MonoBehaviour
{
    [SerializeField]
    private LevelPropertiesConfig levelProportiesConfig;

    [SerializeField]
    private TileBuilderController tileBuilderController;

    [SerializeField]
    private Finances financesController;

    [SerializeField]
    private ShopController shopController;

    [SerializeField]
    private UIController uIController;

    [SerializeField]
    private TarrifsCounter tarrifsCounter;

    [SerializeField]
    private DailyBillPanel dailyBillPanel;

    [SerializeField]
    private LevelTemperaryData levelTemperaryData;

    [SerializeField]
    private TransitionPanel transitionPanel;

    private Action bufferAction;
}
