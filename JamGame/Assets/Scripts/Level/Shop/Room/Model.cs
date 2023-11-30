using Sirenix.OdinInspector;
using System;
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

    [AddComponentMenu("Scripts/Level.Shop.Room.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private Cost cost;
        public Cost Cost => cost;
    }
}
