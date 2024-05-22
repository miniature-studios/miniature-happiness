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

            if (data.RentByRoom.Keys.Count > 0)
            {
                int longest = data.RentByRoom.Values.Select(x => x.ToString().Length).Max();
                foreach (KeyValuePair<Room.CoreModel, int> item in data.RentByRoom)
                {
                    _ = stringBuilder.AppendLine(
                        $"{item.Value.ToString().PadRight(longest)}   - {item.Key.RoomInfo.Title}"
                    );
                }
                _ = stringBuilder.AppendLine();
            }

            _ = stringBuilder.AppendLine($"You lost: {data.Sum} Coins");
            dailyBillText.text = stringBuilder.ToString();
        }

        // Called by button continue.
        public void ContinueButtonPress()
        {
            ContinueButtonPressEvent?.Invoke();
        }
    }
}
