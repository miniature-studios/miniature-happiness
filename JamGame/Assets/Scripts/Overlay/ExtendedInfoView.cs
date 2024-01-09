using System.Linq;
using Employee;
using Employee.Personality;
using TMPro;
using UnityEngine;

namespace Overlay
{
    [AddComponentMenu("Scripts/Overlay/ExtendedInfoView")]
    public class ExtendedInfoView : MonoBehaviour
    {
        private Camera cam;

        private Personality personality;
        private EmployeeImpl employee;

        private TMP_Text text;

        private void Start()
        {
            cam = Camera.main;

            personality = GetComponentInParent<Personality>();
            employee = GetComponentInParent<EmployeeImpl>();

            text = GetComponentInChildren<TMP_Text>();
        }

        private void Update()
        {
            transform.LookAt(cam.transform.position);

            string buffs = employee.Buffs
                .Select(buff => buff.Name)
                .Aggregate("", (x, y) => x + (x.Length == 0 ? "" : ", ") + y);
            string quirks = personality.Quirks
                .Select(quirk => quirk.Name)
                .Aggregate("", (x, y) => x + (x.Length == 0 ? "" : ", ") + y);

            text.text = $"{personality.Name}\n{buffs}\n{quirks}";
        }
    }
}
