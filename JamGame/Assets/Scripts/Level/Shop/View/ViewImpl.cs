using UnityEngine;

namespace Level.Shop.View
{
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("Scripts/Level/Shop/Level.Shop.View")]
    public partial class ViewImpl : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        // Called by button open shop.
        public void Open()
        {
            animator.SetBool("Showed", true);
        }

        // Called by button close shop.
        public void Close()
        {
            animator.SetBool("Showed", false);
        }
    }
}
