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
                Vector3 position = inputActions.UI.Point.ReadValue<Vector2>();
                Ray ray = Camera.main.ScreenPointToRay(position);
                RaycastHit[] hits = Physics.RaycastAll(ray);

                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.TryGetComponent(out EmployeeImpl employee))
                    {
                        model.FireEmployee(employee);
                        return;
                    }
                }
            }
        }
    }
}
