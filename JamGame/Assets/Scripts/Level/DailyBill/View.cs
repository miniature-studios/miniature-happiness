using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            StringBuilder stringBuilder = new();
            int longest = data.RentByRoom.Keys.Select(x => x.RoomInfo.Title.Length).Max();
            foreach (KeyValuePair<Room.CoreModel, int> item in data.RentByRoom)
            {
                _ = stringBuilder.AppendLine(
                    $"{item.Key.RoomInfo.Title.PadRight(longest)} cost: {item.Value}"
                );
            }
            _ = stringBuilder.AppendLine();
            _ = stringBuilder.AppendLine($"Sum: {data.Sum}");
            dailyBillText.text = stringBuilder.ToString();
        }

        // Called by button continue.
        public void ContinueButtonPress()
        {
            ContinueButtonPressEvent?.Invoke();
        }
    }
}
