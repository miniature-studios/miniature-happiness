using System.Collections.Generic;
using UnityEngine;

namespace Employee
{
    // TODO: Can we place Quirk here or even make it to be not SO.

    [RequireComponent(typeof(Employee))]
    public class Personality : MonoBehaviour
    {
        [SerializeField]
        private string name_;
        public string Name => name_;

        [SerializeField]
        private List<Quirk> quirks;

        // TODO: Will change when QuirkView will be implemented.
        public IEnumerable<Quirk> Quirks => quirks;

        private void Start()
        {
            Employee employee = GetComponent<Employee>();

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
