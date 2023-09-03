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
            dailyBillText.text = $"Rent: {data.Rent} coins\r\n";
        }

        public void ContinueButtonPress()
        {
            ContinueButtonPressEvent?.Invoke();
        }
    }
}
