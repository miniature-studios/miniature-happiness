using System.Collections.Generic;

public class EmployeeConfig
{
    public string EmployeeName;
    public Profession Profession;
    public List<Peculiarity> Peculiarities;
    public EmployeeConfig(string employeeName, Profession profession, List<Peculiarity> peculiarities)
    {
        EmployeeName = employeeName;
        Profession = profession;
        Peculiarities = peculiarities;
    }
}

