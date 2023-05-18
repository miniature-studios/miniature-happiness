using System;
using TMPro;
using UnityEngine;

public class FinancesCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text countText;
    [SerializeField] private float LerpSpeed = 1;
    [SerializeField] private RectTransform rectTransform;
    public UIHider UIHider;
    public int MoneyCount { get; set; } = 0;

    private float lerpCount = 0;

    private void Update()
    {
        lerpCount = Mathf.LerpUnclamped(lerpCount, MoneyCount, LerpSpeed * Time.deltaTime);
        countText.text = Convert.ToString((int)lerpCount);
    }
}
