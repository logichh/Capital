using UnityEngine;

public class MergerSystem : MonoBehaviour
{
    public static MergerSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanAcquire(Business acquirer, Business target)
    {
        // Acquirer must have a higher Net Worth and enough Capital to cover the target's Net Worth
        return acquirer.NetWorth > target.NetWorth && acquirer.Capital > target.NetWorth;
    }

    public void Acquire(Business acquirer, Business target)
    {
        if (CanAcquire(acquirer, target))
        {
            float acquisitionCost = target.NetWorth; // Use NetWorth as acquisition cost
            
            acquirer.Capital -= acquisitionCost;
            acquirer.TotalExpenses += acquisitionCost; // Record as an expense
            
            // Absorb assets and liabilities (simplified)
            acquirer.AddLiability(target.TotalLiabilities);
            // In a real scenario, you'd merge employees, products, etc.
            
            acquirer.CompleteMerger(); // Use the existing method on the Business class
            
            Debug.Log($"{acquirer.Name} has acquired {target.Name} for ${acquisitionCost:N0}!");

            // Remove the acquired business (this would need to be handled by the GameManager)
            // For example: GameManager.Instance.RemoveBusiness(target);
        }
        else
        {
            Debug.LogWarning($"{acquirer.Name} cannot acquire {target.Name}. Conditions not met.");
        }
    }
} 