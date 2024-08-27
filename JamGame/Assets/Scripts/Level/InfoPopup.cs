using Sirenix.OdinInspector;
using UnityEngine;

namespace Level
{
    [AddComponentMenu("Scripts/Level/Level.InfoPopup")]
    internal class InfoPopup : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private GameObject viewRoot;

        [Button]
        public void Hide()
        {
            viewRoot.SetActive(false);
        }

        [Button]
        public void Show()
        {
            viewRoot.SetActive(true);
        }
    }
}
