using Common;
using TileUnion;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Level.Inventory.Room.Model")]
    public class Model : MonoBehaviour
    {
        public TileUnionImpl TileUnion;

        public UnityEvent<int> CountUpdated;

        [SerializeField, InspectorReadOnly]
        private int count = 1;
        public int Count
        {
            get => count;
            set
            {
                count = value;
                CountUpdated?.Invoke(count);
                if (count <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
