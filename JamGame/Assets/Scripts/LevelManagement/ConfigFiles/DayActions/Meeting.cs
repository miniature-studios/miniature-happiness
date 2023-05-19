using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Meeting", menuName = "Level/DayActions/Meeting", order = 1)]
public class Meeting : DayAction
{
    [SerializeField] private List<AbstractEmployeeConfig> shopEmployees;
    [SerializeField] private List<AbstractRoomConfig> shopRooms;
    [SerializeField] private List<DayEvent> dayEvents;
    public override void ReleaseAction(LevelExecutor Level_executor, Action show_end_handler)
    {
        Level_executor.TileBuilderController.ChangeGameMode(Gamemode.Building);
        Level_executor.TileBuilderController.СompleteMeetingEvent = show_end_handler;
        Level_executor.TileBuilderController.TilesPanelController.ScrollViewHider.UIElementState = UIElementState.Shown;
        Level_executor.TileBuilderController.ButtonCompleteMeetingUIHider.SetState(UIElementState.Shown);
        Level_executor.BossStressController.UIHider.SetState(UIElementState.Shown);
        Level_executor.BossStressController.StressState = BossStressState.Accumulate;
        Level_executor.FinancesController.FinancesCounterUIHider.SetState(UIElementState.Shown);
        Level_executor.ShopController.ButtonShopOpenUIHider.SetState(UIElementState.Shown);
        Level_executor.ShopController.SetShopState(UIElementState.Hidden);
        Level_executor.ShopController.SetShopRooms(shopRooms.Select(x => x.GetRoomConfig()));
        Level_executor.ShopController.SetShopEmployees(shopEmployees.Select(x => x.GetEmployeeConfig()));
    }
}
