using System;
using UnityEngine;

public partial class LevelExecutor : MonoBehaviour
{
    [SerializeField] private LevelPropertiesConfig levelProportiesConfig;
    [SerializeField] private TileBuilderController tileBuilderController;
    [SerializeField] private Finances financesController;
    [SerializeField] private ShopController shopController;
    [SerializeField] private UIController uIController;
    [SerializeField] private TarrifsCounter tarrifsCounter;
    [SerializeField] private DailyBillPanel dailyBillPanel;
    [SerializeField] private LevelTemperaryData levelTemperaryData;
    [SerializeField] private TransitionPanel transitionPanel;

    private Action bufferAction;

    public void ExecuteDayAction(IDayAction day_action, Action next_action)
    {
        switch (day_action)
        {
            case DayStart: Execute(day_action as DayStart, next_action); break;
            case DayEnd: Execute(day_action as DayEnd, next_action); break;
            case Meeting: Execute(day_action as Meeting, next_action); break;
            case Working: Execute(day_action as Working, next_action); break;
            default: throw new ArgumentException();
        }
    }
}

