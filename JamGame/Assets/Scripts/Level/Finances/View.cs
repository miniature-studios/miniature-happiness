using System;
using TMPro;
using UnityEngine;

namespace Level.Finances
{
    [AddComponentMenu("Scripts/Level.Finances.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text countText;

        [SerializeField]
        private float lerpSpeed;

        private int buffer_count;
        private float lerpCount = 0;

        private void Update()
        {
            lerpCount = Mathf.Lerp(lerpCount, buffer_count, lerpSpeed * Time.deltaTime);
            countText.text = Convert.ToString(Mathf.RoundToInt(lerpCount));
        }

        public void OnChanged(int money)
        {
            buffer_count = money;
        }
    }
}
