using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Shop.View
{
    [AddComponentMenu("Scripts/Level/Shop/Level.Shop.View")]
    public partial class ViewImpl : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Animator animator;

        public UnityEvent OnShopOpened;
        public UnityEvent OnShopClosed;

        // Called by controller event.
        public void Open()
        {
            animator.SetBool("Showed", true);
            OnShopOpened?.Invoke();
        }

        // Called by controller event.
        public void Close()
        {
            animator.SetBool("Showed", false);
            OnShopClosed?.Invoke();
        }
    }
}
