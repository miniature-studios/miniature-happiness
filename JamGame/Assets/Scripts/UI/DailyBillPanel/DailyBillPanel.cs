using System;
using TMPro;
using UnityEngine;


public class DailyBillPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text dailyBillText;
    public event Action continueButtonPress;

    public void ShowDailyBill(Check check)
    {
        dailyBillText.text =
            $"Rent: {check.Rent} coins\r\n" +
            $"Water {check.Water} coins\r\n" +
            $"Electricity: {check.Electricity} coins\r\n" +
            "\r\n" +
            $"Summ: {check.Summ} coins";
    }

    public void ContinueButtonPress()
    {
        continueButtonPress();
    }
}

