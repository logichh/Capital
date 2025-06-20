using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class MarketingCampaign
{
    public string Name;
    public string Type; // "TV", "Digital", "Print", "Social", "Influencer"
    public float Budget;
    public int DurationTicks;
    public int RemainingTicks;
    public float Effectiveness;
    public float Reach;
    public string TargetAudience;
    public Dictionary<string, float> Effects;

    public MarketingCampaign(string name, string type, float budget, int duration, string targetAudience)
    {
        Name = name;
        Type = type;
        Budget = budget;
        DurationTicks = duration;
        RemainingTicks = duration;
        TargetAudience = targetAudience;
        Effectiveness = CalculateEffectiveness(type, budget);
        Reach = CalculateReach(type, budget);
        Effects = new Dictionary<string, float>();
    }

    private float CalculateEffectiveness(string type, float budget)
    {
        float baseEffectiveness = 0.5f;
        switch (type.ToLower())
        {
            case "tv": return baseEffectiveness * (budget / 100000f);
            case "digital": return baseEffectiveness * (budget / 50000f);
            case "print": return baseEffectiveness * (budget / 30000f);
            case "social": return baseEffectiveness * (budget / 20000f);
            case "influencer": return baseEffectiveness * (budget / 40000f);
            default: return baseEffectiveness;
        }
    }

    private float CalculateReach(string type, float budget)
    {
        float baseReach = 1000f;
        switch (type.ToLower())
        {
            case "tv": return baseReach * (budget / 50000f);
            case "digital": return baseReach * (budget / 10000f);
            case "print": return baseReach * (budget / 15000f);
            case "social": return baseReach * (budget / 5000f);
            case "influencer": return baseReach * (budget / 20000f);
            default: return baseReach;
        }
    }
}

[System.Serializable]
public class BrandReputation
{
    public float OverallScore; // 0-100
    public float CustomerSatisfaction;
    public float ProductQuality;
    public float CustomerService;
    public float Innovation;
    public float SocialResponsibility;
    public float EnvironmentalImpact;
    public Dictionary<string, float> AudienceScores;

    public BrandReputation()
    {
        OverallScore = 50f;
        CustomerSatisfaction = 50f;
        ProductQuality = 50f;
        CustomerService = 50f;
        Innovation = 50f;
        SocialResponsibility = 50f;
        EnvironmentalImpact = 50f;
        AudienceScores = new Dictionary<string, float>();
    }

    public void UpdateOverallScore()
    {
        OverallScore = (CustomerSatisfaction + ProductQuality + CustomerService + 
                       Innovation + SocialResponsibility + EnvironmentalImpact) / 6f;
    }
}

[System.Serializable]
public class CustomerSegment
{
    public string Name;
    public float Size; // Market size
    public float Penetration; // Current market share
    public float Satisfaction;
    public float Loyalty;
    public float PriceSensitivity;
    public List<string> PreferredFeatures;

    public CustomerSegment(string name, float size, float priceSensitivity = 0.5f)
    {
        Name = name;
        Size = size;
        Penetration = 0f;
        Satisfaction = 50f;
        Loyalty = 0.5f;
        PriceSensitivity = priceSensitivity;
        PreferredFeatures = new List<string>();
    }
}

[System.Serializable]
public class MarketResearch
{
    public string ResearchType; // "Customer Survey", "Competitor Analysis", "Market Trends"
    public float Cost;
    public int DurationTicks;
    public int RemainingTicks;
    public Dictionary<string, float> Results;

    public MarketResearch(string type, float cost, int duration)
    {
        ResearchType = type;
        Cost = cost;
        DurationTicks = duration;
        RemainingTicks = duration;
        Results = new Dictionary<string, float>();
    }
}

public class MarketingSystem : MonoBehaviour
{
    public static MarketingSystem Instance { get; private set; }

    private Dictionary<Business, List<MarketingCampaign>> activeCampaigns = new Dictionary<Business, List<MarketingCampaign>>();
    private Dictionary<Business, BrandReputation> brandReputations = new Dictionary<Business, BrandReputation>();
    private Dictionary<Business, List<CustomerSegment>> customerSegments = new Dictionary<Business, List<CustomerSegment>>();
    private Dictionary<Business, List<MarketResearch>> marketResearch = new Dictionary<Business, List<MarketResearch>>();

    [SerializeField] private float brandDecayRate = 0.1f;
    [SerializeField] private float satisfactionDecayRate = 0.05f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool LaunchCampaign(Business business, string name, string type, float budget, int duration, string targetAudience)
    {
        if (business.Capital < budget)
            return false;

        if (!activeCampaigns.ContainsKey(business))
            activeCampaigns[business] = new List<MarketingCampaign>();

        var campaign = new MarketingCampaign(name, type, budget, duration, targetAudience);
        activeCampaigns[business].Add(campaign);
        business.Capital -= budget;
        business.TotalExpenses += budget;

        return true;
    }

    public void ProcessMarketing()
    {
        foreach (var kvp in activeCampaigns)
        {
            var business = kvp.Key;
            var campaigns = kvp.Value;

            for (int i = campaigns.Count - 1; i >= 0; i--)
            {
                var campaign = campaigns[i];
                campaign.RemainingTicks--;

                // Apply campaign effects
                ApplyCampaignEffects(business, campaign);

                if (campaign.RemainingTicks <= 0)
                {
                    campaigns.RemoveAt(i);
                }
            }
        }

        // Process brand reputation decay
        foreach (var kvp in brandReputations)
        {
            var business = kvp.Key;
            var reputation = kvp.Value;

            // Natural decay
            reputation.CustomerSatisfaction -= satisfactionDecayRate;
            reputation.OverallScore -= brandDecayRate;

            // Clamp values
            reputation.CustomerSatisfaction = Mathf.Clamp(reputation.CustomerSatisfaction, 0f, 100f);
            reputation.OverallScore = Mathf.Clamp(reputation.OverallScore, 0f, 100f);
            reputation.UpdateOverallScore();
        }

        // Process market research
        foreach (var kvp in marketResearch)
        {
            var business = kvp.Key;
            var research = kvp.Value;

            for (int i = research.Count - 1; i >= 0; i--)
            {
                var study = research[i];
                study.RemainingTicks--;

                if (study.RemainingTicks <= 0)
                {
                    CompleteMarketResearch(business, study);
                    research.RemoveAt(i);
                }
            }
        }
    }

    private void ApplyCampaignEffects(Business business, MarketingCampaign campaign)
    {
        var reputation = GetOrCreateBrandReputation(business);
        var segments = GetOrCreateCustomerSegments(business);

        // Apply effects based on campaign type
        switch (campaign.Type.ToLower())
        {
            case "tv":
                reputation.CustomerSatisfaction += campaign.Effectiveness * 0.1f;
                reputation.OverallScore += campaign.Effectiveness * 0.05f;
                break;
            case "digital":
                reputation.CustomerSatisfaction += campaign.Effectiveness * 0.15f;
                reputation.Innovation += campaign.Effectiveness * 0.1f;
                break;
            case "social":
                reputation.CustomerSatisfaction += campaign.Effectiveness * 0.2f;
                reputation.SocialResponsibility += campaign.Effectiveness * 0.1f;
                break;
            case "influencer":
                reputation.CustomerSatisfaction += campaign.Effectiveness * 0.25f;
                reputation.ProductQuality += campaign.Effectiveness * 0.1f;
                break;
        }

        // Update customer segments
        foreach (var segment in segments)
        {
            if (segment.Name == campaign.TargetAudience || campaign.TargetAudience == "General")
            {
                segment.Satisfaction += campaign.Effectiveness * 0.1f;
                segment.Loyalty += campaign.Effectiveness * 0.05f;
                segment.Satisfaction = Mathf.Clamp(segment.Satisfaction, 0f, 100f);
                segment.Loyalty = Mathf.Clamp(segment.Loyalty, 0f, 1f);
            }
        }

        reputation.UpdateOverallScore();
    }

    public bool ConductMarketResearch(Business business, string type, float cost, int duration)
    {
        if (business.Capital < cost)
            return false;

        if (!marketResearch.ContainsKey(business))
            marketResearch[business] = new List<MarketResearch>();

        var research = new MarketResearch(type, cost, duration);
        marketResearch[business].Add(research);
        business.Capital -= cost;
        business.TotalExpenses += cost;

        return true;
    }

    private void CompleteMarketResearch(Business business, MarketResearch research)
    {
        switch (research.ResearchType)
        {
            case "Customer Survey":
                research.Results["Satisfaction"] = Random.Range(40f, 80f);
                research.Results["PriceSensitivity"] = Random.Range(0.3f, 0.8f);
                research.Results["FeaturePreference"] = Random.Range(0.2f, 0.9f);
                break;
            case "Competitor Analysis":
                research.Results["CompetitorStrength"] = Random.Range(30f, 90f);
                research.Results["MarketShare"] = Random.Range(0.1f, 0.4f);
                research.Results["PriceCompetitiveness"] = Random.Range(0.5f, 1.2f);
                break;
            case "Market Trends":
                research.Results["GrowthRate"] = Random.Range(-0.1f, 0.3f);
                research.Results["DemandTrend"] = Random.Range(0.8f, 1.5f);
                research.Results["TechnologyAdoption"] = Random.Range(0.1f, 0.8f);
                break;
        }

        Debug.Log($"Market research completed: {research.ResearchType}");
    }

    public void ImproveCustomerService(Business business, float investment)
    {
        if (business.Capital < investment)
            return;

        var reputation = GetOrCreateBrandReputation(business);
        reputation.CustomerService += investment / 10000f; // $10k = 1 point
        reputation.CustomerService = Mathf.Clamp(reputation.CustomerService, 0f, 100f);
        reputation.UpdateOverallScore();

        business.Capital -= investment;
        business.TotalExpenses += investment;
    }

    public void InvestInSocialResponsibility(Business business, float investment)
    {
        if (business.Capital < investment)
            return;

        var reputation = GetOrCreateBrandReputation(business);
        reputation.SocialResponsibility += investment / 50000f; // $50k = 1 point
        reputation.EnvironmentalImpact += investment / 75000f; // $75k = 1 point
        reputation.SocialResponsibility = Mathf.Clamp(reputation.SocialResponsibility, 0f, 100f);
        reputation.EnvironmentalImpact = Mathf.Clamp(reputation.EnvironmentalImpact, 0f, 100f);
        reputation.UpdateOverallScore();

        business.Capital -= investment;
        business.TotalExpenses += investment;
    }

    private BrandReputation GetOrCreateBrandReputation(Business business)
    {
        if (!brandReputations.ContainsKey(business))
            brandReputations[business] = new BrandReputation();
        return brandReputations[business];
    }

    private List<CustomerSegment> GetOrCreateCustomerSegments(Business business)
    {
        if (!customerSegments.ContainsKey(business))
        {
            customerSegments[business] = new List<CustomerSegment>
            {
                new CustomerSegment("Young Professionals", 1000000f, 0.7f),
                new CustomerSegment("Families", 2000000f, 0.5f),
                new CustomerSegment("Seniors", 800000f, 0.3f),
                new CustomerSegment("Students", 500000f, 0.9f)
            };
        }
        return customerSegments[business];
    }

    public (List<MarketingCampaign>, BrandReputation, List<CustomerSegment>, List<MarketResearch>) GetMarketingStatus(Business business)
    {
        List<MarketingCampaign> campaigns = activeCampaigns.ContainsKey(business) ? activeCampaigns[business] : new List<MarketingCampaign>();
        BrandReputation reputation = brandReputations.ContainsKey(business) ? brandReputations[business] : new BrandReputation();
        List<CustomerSegment> segments = customerSegments.ContainsKey(business) ? customerSegments[business] : new List<CustomerSegment>();
        List<MarketResearch> research = marketResearch.ContainsKey(business) ? marketResearch[business] : new List<MarketResearch>();

        return (campaigns, reputation, segments, research);
    }

    public float GetBrandMultiplier(Business business)
    {
        var reputation = GetOrCreateBrandReputation(business);
        return 0.5f + (reputation.OverallScore / 100f) * 0.5f; // 0.5x to 1.0x multiplier
    }
} 