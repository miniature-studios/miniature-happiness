using System;
using TMPro;
using UnityEngine;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Level.Inventory.Room.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text counterText;

        public void CountUpdated(int count)
        {
            counterText.text = Convert.ToString(count);
        }
    }
}
