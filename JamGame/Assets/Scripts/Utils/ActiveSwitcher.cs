using Sirenix.OdinInspector;
using UnityEngine;

namespace Utils
{
    [AddComponentMenu("Scripts/Utils/Utils.ActiveSwitcher")]
    internal class ActiveSwitcher : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private GameObject targetGameObject;

        public void SwitchActive()
        {
            targetGameObject.SetActive(!targetGameObject.activeSelf);
        }
    }
}
