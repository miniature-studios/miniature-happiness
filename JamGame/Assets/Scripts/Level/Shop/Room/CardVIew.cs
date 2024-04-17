using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Shop.Room
{
    [AddComponentMenu("Scripts/Level/Shop/Room/Level.Shop.Room.CardVIew")]
    internal class CardVIew : MonoBehaviour
    {
        [ReadOnly]
        [SerializeField]
        private View view;
    }
}
