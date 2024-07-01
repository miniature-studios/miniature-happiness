using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Level
{
    [AddComponentMenu("Scripts/Level/Level.InfoPopup")]
    internal class InfoPopup : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private TMP_Text infoLabel;

        [Required]
        [SerializeField]
        private GameObject viewRoot;

        [Button]
        public void HidePopup()
        {
            infoLabel.text = "Hidden";
            viewRoot.SetActive(false);
        }

        [Button]
        public void ShowPopup(string info)
        {
            infoLabel.text = info;
            viewRoot.SetActive(true);
        }
    }
}
