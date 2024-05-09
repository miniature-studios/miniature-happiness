using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Scripts/Level/Inventory/Room/Level.Inventory.Room.Model")]
    public class Model : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Sprite miniature;
        public Sprite Miniature => miniature;
    }
}
