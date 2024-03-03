using Employee;
using UnityEngine;

namespace Location
{
    [AddComponentMenu("Scripts/Scripts/Location/Location.MeetingRoomEmployeeBinder")]
    internal class MeetingRoomEmployeeBinder : MonoBehaviour
    {
        private Transform originalEmployeeParent;

        // Bound to `NeedProvider.taken`
        public void PlaceTaken(EmployeeImpl employee)
        {
            originalEmployeeParent = employee.transform.parent;
            employee.transform.parent = transform;
        }

        // Bound to `NeedProvider.released`
        public void PlaceReleased(EmployeeImpl employee)
        {
            if (originalEmployeeParent == null)
            {
                Debug.LogError("Employee left meeting room without entering it");
            }

            employee.transform.parent = originalEmployeeParent;
            originalEmployeeParent = null;
        }
    }
}
