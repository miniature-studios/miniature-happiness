using Common;
using System;

public partial class LevelExecutor
{
    public void Execute(DayEnd day_end, Action next_action)
    {

        Check check = tarrifsCounter.GetCheck(levelProportiesConfig.Tariffs);
        Result result = financesController.TryTakeMoney(check.Sum);
        if (result.Success)
        {
            levelTemperaryData.CreateCheck(check);
            bufferAction = next_action;
            transitionPanel.SetText("Day end start.");
            uIController.PlayDayActionStart(day_end.GetType(), null);
        }
        else
        {
            // TODO lose game
        }
    }

    // Calls by button continue on daily bill panel
    public void CompleteDayEnd()
    {
        transitionPanel.SetText("Day end end.");
        uIController.PlayDayActionEnd(bufferAction);
    }
}

