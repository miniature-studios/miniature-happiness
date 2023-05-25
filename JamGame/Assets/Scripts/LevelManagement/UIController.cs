using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private Animator buttonCompleteMeetingAnimator;
    [SerializeField] private Animator buttonOpenShopAnimator;
    [SerializeField] private Animator shopPanelAnimator;
    [SerializeField] private Animator financesAnimator;
    [SerializeField] private Animator tilesInventoryAnimator;
    [SerializeField] private Animator transitionPanelAnimator;
    [SerializeField] private Animator inventoryButtonAnimator;
    [SerializeField] private TMP_Text transitionPanelText;
    [SerializeField] private DailyBillPanel dailyBillPanel;

    private UIState uIState = UIState.AllHidden;

    public enum UIState
    {
        AllHidden,
        ForMeeting,
        ForWorking
    }

    public void ShowTransitionPanel(string text, float exit_time, Action animation_end)
    {
        transitionPanelAnimator.SetBool("Showed", true);
        transitionPanelText.text = text;
        _ = StartCoroutine(ShowTransitionPanelRoutine(exit_time, animation_end));
    }
    private IEnumerator ShowTransitionPanelRoutine(float exit_time, Action animation_end)
    {
        yield return new WaitForSeconds(1 + exit_time);
        animation_end?.Invoke();
    }

    public void HideTransitionPanel(string text, float exit_time, Action animation_end)
    {
        transitionPanelText.text = text;
        _ = StartCoroutine(HideTransitionPanelRoutine(exit_time, animation_end));
    }

    private IEnumerator HideTransitionPanelRoutine(float exit_time, Action animation_end)
    {
        yield return new WaitForSeconds(exit_time);
        transitionPanelAnimator.SetBool("Showed", false);
        yield return new WaitForSeconds(1);
        animation_end?.Invoke();
    }

    public void SetUIState(UIState new_ui_state)
    {
        uIState = new_ui_state;
        switch (uIState)
        {
            default:
            case UIState.AllHidden: HideAllUI(); break;
            case UIState.ForMeeting: ShowForMeeting(); break;
            case UIState.ForWorking: ShowForWorking(); break;
        }
    }

    public void ShowDailyBillPanel(Check check)
    {
        dailyBillPanel.gameObject.SetActive(true);
        dailyBillPanel.ShowDailyBill(check);
    }

    public void HideDailyBillPanel()
    {
        dailyBillPanel.gameObject.SetActive(false);
    }

    private void HideAllUI()
    {
        buttonCompleteMeetingAnimator.SetBool("Showed", false);
        buttonOpenShopAnimator.SetBool("Showed", false);
        shopPanelAnimator.SetBool("Showed", false);
        financesAnimator.SetBool("Showed", false);
        inventoryButtonAnimator.SetBool("Showed", false);
        tilesInventoryAnimator.SetBool("Showed", false);
    }

    private void ShowForMeeting()
    {
        buttonCompleteMeetingAnimator.SetBool("Showed", true);
        buttonOpenShopAnimator.SetBool("Showed", true);
        shopPanelAnimator.SetBool("Showed", false);
        financesAnimator.SetBool("Showed", true);
        inventoryButtonAnimator.SetBool("Showed", true);
        tilesInventoryAnimator.SetBool("Showed", false);
    }

    private void ShowForWorking()
    {
        buttonCompleteMeetingAnimator.SetBool("Showed", false);
        buttonOpenShopAnimator.SetBool("Showed", false);
        shopPanelAnimator.SetBool("Showed", false);
        inventoryButtonAnimator.SetBool("Showed", false);
        financesAnimator.SetBool("Showed", true);
        tilesInventoryAnimator.SetBool("Showed", false);
    }
}

