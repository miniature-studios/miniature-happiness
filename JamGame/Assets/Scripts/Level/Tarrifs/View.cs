using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Tarrifs
{
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text dailyBillText;
        public UnityEvent ContinueButtonPressEvent;

        public void OnChanged(Check data)
        {
            dailyBillText.text =
                $"Rent: {data.Data.Rent} coins\r\n"
                + $"Water {data.Data.Water} coins\r\n"
                + $"Electricity: {data.Data.Electricity} coins\r\n"
                + "\r\n"
                + $"Summ: {data.Data.Sum} coins";
        }

        public void ContinueButtonPress()
        {
            ContinueButtonPressEvent?.Invoke();
        }
    }
}
