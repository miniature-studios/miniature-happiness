using UnityEngine;

namespace Utils
{
    [AddComponentMenu("Scripts/Utils/Utils.SphereGizmo")]
    public class SphereGizmo : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    }
}
