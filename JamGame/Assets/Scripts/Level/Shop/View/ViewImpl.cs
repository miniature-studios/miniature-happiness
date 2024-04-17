using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Shop.View
{
    [AddComponentMenu("Scripts/Level/Shop/Level.Shop.View")]
    public partial class ViewImpl : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Animator animator;

        [Required]
        [SerializeField]
        private RectTransform cardParent;
        public RectTransform CardParent => cardParent;

        [SerializeField]
        private List<GameObject> contentsGameObjects = new();
        private IEnumerable<IShopContent> Contents =>
            contentsGameObjects.Select(x => x.GetComponent<IShopContent>());

        private void Awake()
        {
            foreach (IShopContent content in Contents)
            {
                content.OnSwitchedTo += () => OnSwitchedToContent(content);
            }
        }

        private void OnSwitchedToContent(IShopContent source)
        {
            foreach (IShopContent content in Contents.Where(x => x != source))
            {
                content.Hide();
            }
        }

        [Button]
        public void Open()
        {
            animator.SetBool("Showed", true);
        }

        [Button]
        public void Close()
        {
            animator.SetBool("Showed", false);
        }
    }
}
