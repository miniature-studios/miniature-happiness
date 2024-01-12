using System;
using TMPro;
using UnityEngine;

namespace Level.Finances
{
    [AddComponentMenu("Scripts/Level/Finances/Level.Finances.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text countText;

        [SerializeField]
        private float lerpSpeed;

        private int bufferCount;
        private float lerpCount = 0;

        private void Update()
        {
            lerpCount = Mathf.Lerp(lerpCount, bufferCount, lerpSpeed * Time.unscaledDeltaTime);
            countText.text = Convert.ToString(Mathf.RoundToInt(lerpCount));
        }

        public void OnChanged(int money)
        {
            bufferCount = money;
        }
    }
}
