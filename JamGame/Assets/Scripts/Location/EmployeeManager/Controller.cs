using System.Collections.Generic;
using System.Linq;
using Employee;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils.Raycast;

namespace Location.EmployeeManager
{
    [AddComponentMenu("Scripts/Location/EmployeeManager/Location.EmployeeManager.Controller")]
    internal class Controller : MonoBehaviour
    {
        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        [SerializeField]
        private Model model;

        [SerializeField]
        [Required]
        private LayerMask uiMask;

        [SerializeField]
        [Required]
        private GameObject uiBlocker;

        private bool firingMode = false;
        public bool FiringMode => firingMode;

        private InputActions inputActions;

        private void Awake()
        {
            inputActions = new();
            inputActions.UI.LeftClick.performed += (e) =>
            {
                if (firingMode && e.ReadValueAsButton())
                {
                    TryFireEmployee();
                }
            };
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
            uiBlocker.SetActive(firingMode);
        }

        private void TryFireEmployee()
        {
            Vector3 position = inputActions.UI.Point.ReadValue<Vector2>();

            IEnumerable<GameObject> ui_hits = Raycaster.MaskedUIRaycast(position, uiMask);
            if (ui_hits.Any())
            {
                return;
            }

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
