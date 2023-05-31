using TMPro;
using UnityEngine;

// TODO: Remove to replace with another transition screen
public class TransitionPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text textOnPanel;
    public void SetText(string text)
    {
        textOnPanel.text = text;
    }
}
