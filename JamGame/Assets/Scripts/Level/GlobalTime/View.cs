using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Level.GlobalTime
{
    [AddComponentMenu("Scripts/Level/GlobalTime/Level.GlobalTime.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private Button buttonX05;

        [SerializeField]
        private Button buttonX1;

        [SerializeField]
        private Button buttonX2;

        [SerializeField]
        private Button buttonX5;

        private List<Button> buttons;

        private void Start()
        {
            buttons = new List<Button> { buttonX05, buttonX1, buttonX2, buttonX5 };

            foreach (Button button in buttons)
            {
                button.onClick.AddListener(new UnityAction(() => UpdateActiveButtons(button)));
            }

            UpdateActiveButtons(buttonX1);
        }

        private void UpdateActiveButtons(Button active_button)
        {
            foreach (Button button in buttons)
            {
                button.image.color = button != active_button ? Color.white : Color.black;
            }
        }
    }
}
