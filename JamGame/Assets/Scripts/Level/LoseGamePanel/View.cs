using UnityEngine;

namespace Level.LoseGamePanel
{
    [AddComponentMenu("Scripts/Level.LoseGamePanel.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        public void ShownTrigger(bool shown)
        {
            animator.SetBool("Shown", shown);
        }
    }
}
