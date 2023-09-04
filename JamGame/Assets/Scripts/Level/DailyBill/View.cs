using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Level.DailyBill
{
    [AddComponentMenu("Scripts/Level.DailyBill.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text dailyBillText;
        public UnityEvent ContinueButtonPressEvent;

        // Called by event from model.
        public void OnChanged(Check data)
        {
            dailyBillText.text = $"Rent: {data.Rent} coins.\r\n\r\nSumma: {data.Sum}";
        }

        // Called by button continue.
        public void ContinueButtonPress()
        {
            ContinueButtonPressEvent?.Invoke();
        }
    }
}
