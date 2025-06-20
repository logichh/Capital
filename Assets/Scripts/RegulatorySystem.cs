using UnityEngine;

public class RegulatorySystem : MonoBehaviour
{
    public static RegulatorySystem Instance { get; private set; }

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

    public bool IsCompliant(Business business)
    {
        // A business is compliant if its score is above a certain threshold (e.g., 50)
        return business.ComplianceScore > 50;
    }

    public void CheckCompliance(Business business)
    {
        if (!IsCompliant(business))
        {
            Debug.LogWarning($"{business.Name} is not compliant with regulations! Compliance Score: {business.ComplianceScore}");
            // Apply a penalty for non-compliance
            business.Capital -= 10000; // Example penalty
            business.Reputation -= 5;
        }
        else
        {
            Debug.Log($"{business.Name} passed regulatory compliance check.");
        }
    }
} 