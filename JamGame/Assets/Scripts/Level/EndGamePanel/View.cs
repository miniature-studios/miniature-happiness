using TMPro;
using UnityEngine;

namespace Level.EndGamePanel
{
    [AddComponentMenu("Scripts/Level.EndGamePanel.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text textOnPanel;

        [SerializeField]
        private Animator animator;

        public void UpdateText(string text)
        {
            textOnPanel.text = text;
        }

        public void ShownTrigger(bool shown)
        {
            animator.SetBool("Shown", shown);
        }
    }
}
