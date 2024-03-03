using System.Collections.Generic;
using Employee.Needs;
using UnityEngine;

namespace Employee.Personality
{
    [RequireComponent(typeof(EmployeeImpl))]
    [AddComponentMenu("Scripts/Employee/Personality/Employee.Personality")]
    public class PersonalityImpl : MonoBehaviour
    {
        [SerializeField]
        private string name_;
        public string Name => name_;

        [SerializeField]
        private List<Quirk> quirks;
        public IEnumerable<Quirk> Quirks => quirks;

        private void Start()
        {
            EmployeeImpl employee = GetComponent<EmployeeImpl>();

            foreach (Quirk quirk in quirks)
            {
                foreach (Need.NeedProperties additional_need in quirk.AdditionalNeeds)
                {
                    employee.AddNeed(additional_need);
                }
            }
        }
    }
}
