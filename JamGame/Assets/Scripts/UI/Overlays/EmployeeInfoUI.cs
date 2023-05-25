using TMPro;
using UnityEngine;

public class EmployeeInfoUI : MonoBehaviour
{
    private Camera cam;
    private Personality personality;
    private TMP_Text text;

    void Start()
    {
        cam = Camera.main;
        personality = GetComponentInParent<Personality>();
        text = GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        transform.LookAt(cam.transform.position);

        text.text = personality.Name + "\n[buffs]\n[quirks]";
    }
}
