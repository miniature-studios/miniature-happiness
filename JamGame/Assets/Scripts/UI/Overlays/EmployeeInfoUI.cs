using System.Linq;
using TMPro;
using UnityEngine;

public class EmployeeInfoUI : MonoBehaviour
{
    private Camera cam;

    private Personality personality;
    private Employee employee;

    private TMP_Text text;

    void Start()
    {
        cam = Camera.main;

        personality = GetComponentInParent<Personality>();
        employee = GetComponentInParent<Employee>();

        text = GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        transform.LookAt(cam.transform.position);

        var buffs = employee.Buffs
            .Select(buff => buff.Name)
            .Aggregate("", (x, y) => x + (x.Length == 0 ? "" : ", ") + y);
        var quirks = personality.Quirks
            .Select(quirk => quirk.Name)
            .Aggregate("", (x, y) => x + (x.Length == 0 ? "" : ", ") + y);

        text.text = $"{personality.Name}\n{buffs}\n{quirks}";
    }
}
