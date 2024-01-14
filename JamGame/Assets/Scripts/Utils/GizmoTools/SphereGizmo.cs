using UnityEngine;

namespace Utils.GizmoTools
{
    [AddComponentMenu("Scripts/Utils/GizmoTools/Utils.GizmoTools.SphereGizmo")]
    public class SphereGizmo : MonoBehaviour
    {
        [SerializeField]
        private float size = 0.1f;

        [SerializeField]
        private Vector3 shift = Vector3.zero;

        [SerializeField]
        private Color color = Color.white;

        [SerializeField]
        private bool isWireframe = false;

        private void OnDrawGizmos()
        {
            Color temp = Gizmos.color;
            Gizmos.color = color;
            if (isWireframe)
            {
                Gizmos.DrawWireSphere(transform.position + shift, size);
            }
            else
            {
                Gizmos.DrawSphere(transform.position + shift, size);
            }
            Gizmos.color = temp;
        }
    }
}
