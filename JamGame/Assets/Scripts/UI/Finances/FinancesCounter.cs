using System;
using TMPro;
using UnityEngine;

public class FinancesCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text countText;
    [SerializeField] private float lerpSpeed = 1;

    public UIHider UIHider;
    public int MoneyCount = 0;

    private float lerpCount = 0;

    private void Update()
    {
        lerpCount = Mathf.Lerp(lerpCount, MoneyCount, lerpSpeed * Time.deltaTime);
        countText.text = Convert.ToString(Mathf.RoundToInt(lerpCount));
    }
}
