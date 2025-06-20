using UnityEngine;

[System.Serializable]
public class Market
{
    public string ProductCategory;
    public float TotalSupply;
    public float TotalDemand;
    public float PriceElasticity;
    public float MarketPrice;
    // Add more fields as needed

    public Market(string category, float elasticity)
    {
        ProductCategory = category;
        PriceElasticity = elasticity;
        TotalSupply = 0f;
        TotalDemand = 0f;
        MarketPrice = 0f;
    }

    // Example: Update market price based on supply/demand
    public void UpdateMarketPrice()
    {
        if (TotalSupply > 0)
            MarketPrice = (TotalDemand / TotalSupply) * PriceElasticity;
        else
            MarketPrice = PriceElasticity;
    }
} 