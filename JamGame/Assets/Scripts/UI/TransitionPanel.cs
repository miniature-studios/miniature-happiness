using TMPro;
using UnityEngine;

public class TransitionPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textOnPanel;

    public void SetText(string text)
    {
        textOnPanel.text = text;
    }
}
