using UnityEngine;

[System.Serializable]
public class Employee
{
    public string Name;
    public string Role;
    public float Wage;
    public float Morale;
    public float Skill;
    public int LastTrainedTick = -1000;
    // Add more fields as needed

    public Employee(string name, string role, float wage, float skill)
    {
        Name = name;
        Role = role;
        Wage = wage;
        Skill = skill;
        Morale = 1.0f;
    }

    public static float AverageSkill(System.Collections.Generic.List<Employee> employees)
    {
        if (employees == null || employees.Count == 0) return 1.0f;
        float total = 0f;
        foreach (var e in employees) total += e.Skill;
        return total / employees.Count;
    }

    public void Train(float amount, int currentTick = 0)
    {
        Skill += amount;
        Skill = Mathf.Clamp(Skill, 0.5f, 2.0f);
        LastTrainedTick = currentTick;
    }
} 