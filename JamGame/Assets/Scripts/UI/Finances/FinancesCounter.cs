using System;
using TMPro;
using UnityEngine;

public class FinancesCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text countText;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private Finances finances;

    private float lerpCount = 0;

    private void Update()
    {
        lerpCount = Mathf.Lerp(lerpCount, finances.MoneyCount, lerpSpeed * Time.deltaTime);
        countText.text = Convert.ToString(Mathf.RoundToInt(lerpCount));
    }
}
