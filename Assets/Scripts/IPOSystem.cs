using UnityEngine;

public class IPOSystem : MonoBehaviour
{
    public static IPOSystem Instance { get; private set; }

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

    public bool CanGoPublic(Business business)
    {
        // A business can go public if its Net Worth is over $5M and it's been in business for over 2 years (730 days)
        return business.NetWorth > 5000000f && business.DaysInBusiness > 730;
    }

    public void GoPublic(Business business)
    {
        if (CanGoPublic(business))
        {
            // Calculate IPO share price based on NetWorth and a P/E ratio of 15 (example)
            float sharePrice = (business.NetWorth / 1000000f) * 15f;
            business.GoPublic(sharePrice); // This method already exists on the business object
            Debug.Log($"{business.Name} has gone public! Initial Share Price: ${sharePrice:F2}");
        }
        else
        {
            Debug.LogWarning($"{business.Name} does not meet the requirements to go public.");
        }
    }
} 