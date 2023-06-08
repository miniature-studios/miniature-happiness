public class EmployeeConfig
{
    public string Name { get; }
    public Employee Employee { get; }

    public EmployeeConfig(Employee employee, string name)
    {
        Name = name;
        Employee = employee;
    }
}
