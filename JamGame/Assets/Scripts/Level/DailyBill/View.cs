using Common;
using Pickle;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Level.DailyBill
{
    [AddComponentMenu("Scripts/Level/DailyBill/Level.DailyBill.View")]
    public class View : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private TMP_Text sumLabel;

        [Required]
        [SerializeField]
        private Model model;

        [Pickle(typeof(LineView), LookupType = ObjectProviderType.Assets)]
        [SerializeField]
        private LineView lineViewPrototype;

        [SerializeField]
        private RectTransform contentParent;

        public UnityEvent ContinueButtonPressEvent;

        // Called by animator.
        public void Shown()
        {
            Check data = model.ComputeCheck();

            contentParent.DestroyChildren();
            foreach (RoomCheck roomCheck in data.Checks)
            {
                LineView lineView = Instantiate(lineViewPrototype, contentParent);
                lineView.FillWithData(roomCheck);
            }

            sumLabel.text = data.Sum.ToString();
        }

        // Called by button continue.
        public void ContinueButtonPress()
        {
            ContinueButtonPressEvent?.Invoke();
        }
    }
}
