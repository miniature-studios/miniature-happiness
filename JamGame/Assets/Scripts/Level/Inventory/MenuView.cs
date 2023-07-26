using System;
using TileUnion;
using TMPro;
using UnityEngine;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Level.Inventory.Room.View")]
    public class MenuView : MonoBehaviour
    {
        public TileUnionImpl TileUnion;

        [SerializeField]
        private TMP_Text counterText;
        private int counter = 1;
        public int Counter
        {
            get => counter;
            set
            {
                counterText.text = Convert.ToString(value);
                counter = value;
                if (counter <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
