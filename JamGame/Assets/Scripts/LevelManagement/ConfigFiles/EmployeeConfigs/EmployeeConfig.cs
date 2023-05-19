using System.Collections.Generic;

public class EmployeeConfig
{
    public string EmployeeName;
    public Profession Profession;
    public List<Peculiarity> Peculiarities;
    public EmployeeConfig(string employee_name, Profession profession, List<Peculiarity> peculiarities)
    {
        EmployeeName = employee_name;
        Profession = profession;
        Peculiarities = peculiarities;
    }
}

