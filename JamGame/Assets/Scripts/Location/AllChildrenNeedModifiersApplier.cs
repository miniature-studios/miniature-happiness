using Employee;
using Employee.Needs;
using UnityEngine;

namespace Location
{
    [AddComponentMenu("Scripts/Location/Location.AllChildrenNeedModifiersApplier")]
    [RequireComponent(typeof(NeedModifiers))]
    public class AllChildrenNeedModifiersApplier : MonoBehaviour
    {
        [SerializeField]
        private GameObject root;

        private NeedModifiers modifiers;
        private EmployeeImpl[] registeredOn = new EmployeeImpl[0] { };

        private void Start()
        {
            modifiers = GetComponent<NeedModifiers>();
        }

        public void Register()
        {
            registeredOn = root.GetComponentsInChildren<EmployeeImpl>(true);
            foreach (EmployeeImpl employee in registeredOn)
            {
                employee.RegisterModifier(modifiers);
            }
        }

        public void Unregister()
        {
            foreach (EmployeeImpl employee in registeredOn)
            {
                employee.UnregisterModifier(modifiers);
            }
        }
    }
}
