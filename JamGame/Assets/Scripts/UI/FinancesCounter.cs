using Common;
using System;
using TMPro;
using UnityEngine;

public class FinancesCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text countText;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private Finances finances;
    private int buffer_count;
    private float lerpCount = 0;

    private void Update()
    {
        lerpCount = Mathf.Lerp(lerpCount, buffer_count, lerpSpeed * Time.deltaTime);
        countText.text = Convert.ToString(Mathf.RoundToInt(lerpCount));
    }

    public void OnChanged(IReadonlyData<Money> data)
    {
        buffer_count = data.Data.Count;
    }
}
