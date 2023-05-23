public class EmployeeConfig
{
    public string EmployeeName { get; }
    public Employee Employee { get; }
    public EmployeeConfig(Employee employee, string employee_name)
    {
        EmployeeName = employee_name;
        Employee = employee;
    }
}

