using Employee;
using UnityEngine;

namespace Location
{
    [AddComponentMenu("Scripts/Location.AllChildrenNeedModifiersApplier")]
    [RequireComponent(typeof(NeedModifiers))]
    public class AllChildrenNeedModifiersApplier : MonoBehaviour
    {
        [SerializeField]
        private GameObject root;

        private NeedModifiers modifiers;
        private EmployeeImpl[] registeredOn;

        private void Start()
        {
            modifiers = GetComponent<NeedModifiers>();
        }

        public void Register()
        {
            registeredOn = root.GetComponentsInChildren<EmployeeImpl>();
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
