using System.Collections.Generic;
using Common;
using Employee.Needs;
using Level.Boss.Task;
using Level.Config;
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

        private DataProvider<EmployeeQuirks> employeeQuirksDataProvider;

        private void InitPersonality()
        {
            employeeQuirksDataProvider = new DataProvider<EmployeeQuirks>(
                () => new EmployeeQuirks() { Quirks = quirks },
                DataProviderServiceLocator.ResolveType.MultipleSources
            );

            EmployeeImpl employee = GetComponent<EmployeeImpl>();

            foreach (Quirk quirk in quirks)
            {
                foreach (Need.NeedProperties additional_need in quirk.AdditionalNeeds)
                {
                    employee.AddNeed(additional_need);
                }

                foreach (IEffect effect in quirk.Effects)
                {
                    employee.RegisterEffect(effect);
                }
            }
        }

        public void SetEmployeeConfig(EmployeeConfig employeeConfig)
        {
            name_ = employeeConfig.Name;
            quirks = employeeConfig.Quirks;
            InitPersonality();
        }
    }
}
