using UnityEngine;

public class EmployeeFactory : MonoBehaviour
{
    public void CreateEmployee(EmployeeConfig employeeConfig)
    {
        Debug.Log("Creating fixed employee");
    }
}
