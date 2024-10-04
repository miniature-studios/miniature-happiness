using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Employee
{
    [RequireComponent(typeof(EmployeeImpl))]
    [AddComponentMenu("Scripts/Employee/Employee.View")]
    public partial class View : MonoBehaviour
    {
        private EmployeeImpl employee;

        [Required]
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private float animationSpeedMultiplier = 1.5f;

        [Required]
        [SerializeField]
        private NavMeshAgent agent;

        private void Start()
        {
            employee = GetComponent<EmployeeImpl>();
        }

        private void Update()
        {
            animator.SetFloat(
                "WalkSpeed",
                agent.velocity.magnitude / agent.speed * animationSpeedMultiplier
            );
        }
    }
}
