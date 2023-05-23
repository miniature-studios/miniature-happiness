using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private Animator buttonCompleteMeetingAnimator;
    [SerializeField] private Animator buttonOpenShopAnimator;
    [SerializeField] private Animator shopPanelAnimator;
    [SerializeField] private Animator financesAnimator;
    [SerializeField] private Animator tilesInventoryAnimator;

    private UIState uIState = UIState.AllHidden;

    public enum UIState
    {
        AllHidden,
        ForMeeting,
        ForWorking
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

    private void HideAllUI()
    {
        buttonCompleteMeetingAnimator.SetBool("Showed", false);
        buttonOpenShopAnimator.SetBool("Showed", false);
        shopPanelAnimator.SetBool("Showed", false);
        financesAnimator.SetBool("Showed", false);
        tilesInventoryAnimator.SetBool("Interactble", false);
    }

    private void ShowForMeeting()
    {
        buttonCompleteMeetingAnimator.SetBool("Showed", true);
        buttonOpenShopAnimator.SetBool("Showed", true);
        shopPanelAnimator.SetBool("Showed", true);
        financesAnimator.SetBool("Showed", true);
        tilesInventoryAnimator.SetBool("Interactble", true);
    }

    private void ShowForWorking()
    {
        buttonCompleteMeetingAnimator.SetBool("Showed", false);
        buttonOpenShopAnimator.SetBool("Showed", false);
        shopPanelAnimator.SetBool("Showed", false);
        financesAnimator.SetBool("Showed", true);
        tilesInventoryAnimator.SetBool("Interactble", false);
    }
}

