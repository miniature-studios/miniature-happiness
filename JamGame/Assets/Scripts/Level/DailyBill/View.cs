using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Level.DailyBill
{
    [AddComponentMenu("Scripts/Level/DailyBill/Level.DailyBill.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text dailyBillText;

        [SerializeField]
        private Model model;

        public UnityEvent ContinueButtonPressEvent;

        // Called by animator.
        public void Shown()
        {
            Check data = model.ComputeCheck();
            dailyBillText.text = $"Rent: {data.Rent} coins.\r\n\r\nSumma: {data.Sum}";
        }

        // Called by button continue.
        public void ContinueButtonPress()
        {
            ContinueButtonPressEvent?.Invoke();
        }
    }
}
