using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Shop.Room
{
    [Serializable]
    [InlineProperty]
    public struct Cost
    {
        [SerializeField]
        private int cost;
        public int Value => cost;
    }

    [AddComponentMenu("Scripts/Level/Shop/Room/Level.Shop.Room.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private Cost cost;
        public Cost Cost => cost;
    }
}
