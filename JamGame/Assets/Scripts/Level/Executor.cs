using Level.Config;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    [AddComponentMenu("Level.Executor")]
    public class Executor : MonoBehaviour
    {
        [SerializeField]
        private TileBuilder.Controller tileBuilderController;

        [SerializeField]
        private Finances.Model financesModel;

        [SerializeField]
        private Shop.Controller shopController;

        [SerializeField]
        private AnimatorsSwitcher.AnimatorsSwitcher animatorSwitcher;

        [SerializeField]
        private TarrifsCounter tarrifsCounter;

        [SerializeField]
        private TransitionPanel.Model transitionPanel;

        [SerializeField]
        private Boss.Model boss;

        public UnityEvent ActionEndNotify;

        public void Execute(DayEnd day_end)
        {
            tarrifsCounter.UpdateCheck();
            if (financesModel.TryTakeMoney(tarrifsCounter.Check.Sum).Failure)
            {
                // TODO lose game
            }
            animatorSwitcher.SetAnimatorStates(typeof(DayEnd));
        }

        // Calls by button continue on daily bill panel
        public void CompleteDayEnd()
        {
            ActionEndNotify?.Invoke();
        }

        public void Execute(DayStart day_start)
        {
            financesModel.AddMoney(day_start.MorningMoney);
            animatorSwitcher.SetAnimatorStates(typeof(DayStart));
            _ = StartCoroutine(DayStartRoutine(day_start.Duration));
        }

        private IEnumerator DayStartRoutine(float time)
        {
            yield return new WaitForSeconds(time);
            ActionEndNotify?.Invoke();
        }

        public void Execute(Meeting meeting)
        {
            tileBuilderController.ChangeGameMode(TileBuilder.GameMode.Build);
            shopController.SetShopRooms(meeting.ShopRooms);
            shopController.SetShopEmployees(meeting.ShopEmployees);
            animatorSwitcher.SetAnimatorStates(typeof(Meeting));
            boss.ActivateNextTaskBunch();
        }

        // Calls by button complete meeting
        public void CompleteMeeting()
        {
            ActionEndNotify?.Invoke();
        }

        public void Execute(Working working)
        {
            animatorSwitcher.SetAnimatorStates(typeof(Working));
            _ = StartCoroutine(WorkingTime(working.WorkingTime));
        }

        private IEnumerator WorkingTime(float time)
        {
            yield return new WaitForSeconds(time);
            ActionEndNotify?.Invoke();
        }

        public void Execute(Cutscene cutscene)
        {
            transitionPanel.PanelText = cutscene.Text;
            animatorSwitcher.SetAnimatorStates(typeof(Cutscene));
            _ = StartCoroutine(CutsceneRoutine(cutscene.Duration));
        }

        public IEnumerator CutsceneRoutine(float time)
        {
            yield return new WaitForSeconds(time);
            ActionEndNotify?.Invoke();
        }
    }
}
