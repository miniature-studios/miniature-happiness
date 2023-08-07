using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Tariffs
{
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text dailyBillText;
        public UnityEvent ContinueButtonPressEvent;

        public void OnChanged(Check data)
        {
            dailyBillText.text =
                $"Rent: {data.Rent} coins\r\n"
                + $"Water {data.Water} coins\r\n"
                + $"Electricity: {data.Electricity} coins\r\n"
                + "\r\n"
                + $"Sum: {data.Sum} coins";
        }

        public void ContinueButtonPress()
        {
            ContinueButtonPressEvent?.Invoke();
        }
    }
}
