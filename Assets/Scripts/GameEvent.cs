using UnityEngine;

[System.Serializable]
public class GameEvent
{
    public string EventType;
    public string Description;
    public float ImpactValue;
    // Add more fields as needed

    public GameEvent(string type, string description, float impact)
    {
        EventType = type;
        Description = description;
        ImpactValue = impact;
    }

    // Example: Apply event impact to a business
    public void ApplyImpact(Business business)
    {
        switch (EventType)
        {
            case "Boom":
            case "Crisis":
                business.Capital += ImpactValue;
                break;
            case "Strike":
                foreach (var employee in business.Employees)
                {
                    employee.Morale -= 0.3f;
                }
                break;
            case "TechBreakthrough":
                foreach (var product in business.Products)
                {
                    product.Cost *= 0.8f; // Reduce cost by 20%
                }
                break;
            case "PolicyChange":
                business.Capital += ImpactValue;
                break;
            case "SupplyChainDisruption":
                // Increase delivery times for logistics
                if (LogisticsSystem.Instance != null)
                {
                    // This would require adding a method to LogisticsSystem to handle disruptions
                    Debug.Log("Supply chain disruption applied");
                }
                break;
            case "ResearchBreakthrough":
                // Boost research progress
                if (RnDSystem.Instance != null)
                {
                    var (activeProjects, _, _) = RnDSystem.Instance.GetRnDStatus(business);
                    foreach (var project in activeProjects)
                    {
                        project.Progress += 0.2f; // Boost progress by 20%
                    }
                }
                break;
            default:
                business.Capital += ImpactValue;
                break;
        }
    }
} 