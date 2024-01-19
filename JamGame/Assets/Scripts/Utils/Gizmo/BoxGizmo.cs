using UnityEngine;

namespace Utils.Gizmo
{
    [AddComponentMenu("Scripts/Utils/Gizmo/Utils.Gizmo.BoxGizmo")]
    internal class BoxGizmo : MonoBehaviour
    {
        [SerializeField]
        private Vector3 size = Vector3.one;

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
                Gizmos.DrawWireCube(transform.position + shift, size);
            }
            else
            {
                Gizmos.DrawCube(transform.position + shift, size);
            }
            Gizmos.color = temp;
        }
    }
}
