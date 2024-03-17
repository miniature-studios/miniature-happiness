using Employee;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Location.EmployeeManager
{
    [AddComponentMenu("Scripts/Location/EmployeeManager/Location.EmployeeManager.Controller")]
    internal class Controller : MonoBehaviour
    {
        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        [SerializeField]
        private Model model;

        [SerializeField]
        private LayerMask employeesLayer;

        private bool firingMode = false;
        public bool FiringMode => firingMode;

        private InputActions inputActions;

        private void Awake()
        {
            inputActions = new();
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void FiringButtonPressed()
        {
            firingMode ^= true;
        }

        public void Update()
        {
            if (firingMode && inputActions.UI.LeftClick.WasPressedThisFrame())
            {
                TryFireEmployee();
            }
        }

        private void TryFireEmployee()
        {
            Vector3 position = inputActions.UI.Point.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(position);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            EmployeeImpl closest_employee = null;
            float closest_employee_dist = float.PositiveInfinity;
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.TryGetComponent(out EmployeeImpl employee))
                {
                    if (hit.distance < closest_employee_dist)
                    {
                        closest_employee = employee;
                    }
                }
            }

            if (closest_employee != null)
            {
                model.FireEmployee(closest_employee);
            }
        }
    }
}
