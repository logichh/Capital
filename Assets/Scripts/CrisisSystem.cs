using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Crisis
{
    public string Type; // "ProductRecall", "PRDisaster", "LegalIssue", "NaturalDisaster", "DataBreach"
    public string Description;
    public float Severity; // 0-1, affects impact
    public int DurationTicks;
    public int RemainingTicks;
    public float FinancialImpact;
    public float ReputationImpact;
    public bool IsResolved;
    public List<string> RequiredActions;
    public Dictionary<string, float> Effects;

    public Crisis(string type, string description, float severity, int duration)
    {
        Type = type;
        Description = description;
        Severity = severity;
        DurationTicks = duration;
        RemainingTicks = duration;
        FinancialImpact = 0f;
        ReputationImpact = 0f;
        IsResolved = false;
        RequiredActions = new List<string>();
        Effects = new Dictionary<string, float>();
    }
}

[System.Serializable]
public class CrisisResponse
{
    public string ActionType; // "Apology", "Compensation", "Investigation", "LegalDefense", "Rebranding"
    public float Cost;
    public float Effectiveness;
    public int DurationTicks;
    public int RemainingTicks;
    public bool IsCompleted;

    public CrisisResponse(string actionType, float cost, float effectiveness, int duration)
    {
        ActionType = actionType;
        Cost = cost;
        Effectiveness = effectiveness;
        DurationTicks = duration;
        RemainingTicks = duration;
        IsCompleted = false;
    }
}

public class CrisisSystem : MonoBehaviour
{
    public static CrisisSystem Instance { get; private set; }

    private Dictionary<Business, List<Crisis>> activeCrises = new Dictionary<Business, List<Crisis>>();
    private Dictionary<Business, List<CrisisResponse>> crisisResponses = new Dictionary<Business, List<CrisisResponse>>();
    private Dictionary<Business, float> crisisResistance = new Dictionary<Business, float>();

    [SerializeField] private float crisisChance = 0.05f; // 5% chance per tick
    [SerializeField] private float baseCrisisResistance = 0.3f;

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

    public void ProcessCrises()
    {
        foreach (var kvp in activeCrises)
        {
            var business = kvp.Key;
            var crises = kvp.Value;

            for (int i = crises.Count - 1; i >= 0; i--)
            {
                var crisis = crises[i];
                crisis.RemainingTicks--;

                // Apply ongoing crisis effects
                ApplyCrisisEffects(business, crisis);

                if (crisis.RemainingTicks <= 0)
                {
                    // Crisis ends
                    crisis.IsResolved = true;
                    crises.RemoveAt(i);
                    Debug.Log($"Crisis resolved: {crisis.Description}");
                }
            }
        }

        // Process crisis responses
        foreach (var kvp in crisisResponses)
        {
            var business = kvp.Key;
            var responses = kvp.Value;

            for (int i = responses.Count - 1; i >= 0; i--)
            {
                var response = responses[i];
                response.RemainingTicks--;

                if (response.RemainingTicks <= 0)
                {
                    // Response completed
                    response.IsCompleted = true;
                    ApplyResponseEffects(business, response);
                    responses.RemoveAt(i);
                }
            }
        }
    }

    public void CheckForNewCrises(Business business)
    {
        if (!activeCrises.ContainsKey(business))
            activeCrises[business] = new List<Crisis>();

        // Check if new crisis should occur
        float resistance = GetCrisisResistance(business);
        if (Random.value < crisisChance * (1 - resistance))
        {
            var crisis = GenerateRandomCrisis(business);
            activeCrises[business].Add(crisis);
            Debug.Log($"New crisis: {crisis.Description}");
        }
    }

    private Crisis GenerateRandomCrisis(Business business)
    {
        float roll = Random.value;
        
        if (roll < 0.25f)
        {
            // Product Recall
            var crisis = new Crisis("ProductRecall", "Product safety issue discovered", Random.Range(0.3f, 0.9f), Random.Range(10, 30));
            crisis.FinancialImpact = Random.Range(50000f, 200000f);
            crisis.ReputationImpact = Random.Range(0.2f, 0.6f);
            crisis.RequiredActions.Add("Investigation");
            crisis.RequiredActions.Add("Compensation");
            crisis.Effects["SalesReduction"] = 0.3f;
            return crisis;
        }
        else if (roll < 0.5f)
        {
            // PR Disaster
            var crisis = new Crisis("PRDisaster", "Negative media coverage", Random.Range(0.2f, 0.8f), Random.Range(5, 20));
            crisis.FinancialImpact = Random.Range(20000f, 100000f);
            crisis.ReputationImpact = Random.Range(0.1f, 0.5f);
            crisis.RequiredActions.Add("Apology");
            crisis.RequiredActions.Add("Rebranding");
            crisis.Effects["BrandDamage"] = 0.4f;
            return crisis;
        }
        else if (roll < 0.75f)
        {
            // Legal Issue
            var crisis = new Crisis("LegalIssue", "Legal dispute or lawsuit", Random.Range(0.4f, 1.0f), Random.Range(15, 40));
            crisis.FinancialImpact = Random.Range(100000f, 500000f);
            crisis.ReputationImpact = Random.Range(0.1f, 0.3f);
            crisis.RequiredActions.Add("LegalDefense");
            crisis.Effects["LegalCosts"] = 0.5f;
            return crisis;
        }
        else
        {
            // Natural Disaster
            var crisis = new Crisis("NaturalDisaster", "Natural disaster affecting operations", Random.Range(0.5f, 1.0f), Random.Range(20, 50));
            crisis.FinancialImpact = Random.Range(50000f, 300000f);
            crisis.ReputationImpact = Random.Range(0.05f, 0.2f);
            crisis.RequiredActions.Add("InfrastructureRepair");
            crisis.Effects["ProductionDisruption"] = 0.6f;
            return crisis;
        }
    }

    private void ApplyCrisisEffects(Business business, Crisis crisis)
    {
        // Apply financial impact
        float dailyFinancialImpact = crisis.FinancialImpact / crisis.DurationTicks;
        business.Capital -= dailyFinancialImpact;
        business.TotalExpenses += dailyFinancialImpact;

        // Apply reputation impact
        if (MarketingSystem.Instance != null)
        {
            var (_, reputation, _, _) = MarketingSystem.Instance.GetMarketingStatus(business);
            reputation.OverallScore -= crisis.ReputationImpact / crisis.DurationTicks;
            reputation.CustomerSatisfaction -= crisis.ReputationImpact / crisis.DurationTicks;
            reputation.UpdateOverallScore();
        }

        // Apply specific effects
        foreach (var effect in crisis.Effects)
        {
            switch (effect.Key)
            {
                case "SalesReduction":
                    // Reduce product sales
                    foreach (var product in business.Products)
                    {
                        product.Demand *= (1 - effect.Value / crisis.DurationTicks);
                    }
                    break;
                case "ProductionDisruption":
                    // Reduce production efficiency
                    business.ProductionEfficiency *= (1 - effect.Value / crisis.DurationTicks);
                    break;
                case "LegalCosts":
                    // Additional legal expenses
                    business.Capital -= effect.Value * 1000f;
                    business.TotalExpenses += effect.Value * 1000f;
                    break;
            }
        }
    }

    public bool RespondToCrisis(Business business, string actionType, float budget)
    {
        if (business.Capital < budget)
            return false;

        if (!crisisResponses.ContainsKey(business))
            crisisResponses[business] = new List<CrisisResponse>();

        float effectiveness = CalculateResponseEffectiveness(actionType, budget);
        int duration = CalculateResponseDuration(actionType);
        
        var response = new CrisisResponse(actionType, budget, effectiveness, duration);
        crisisResponses[business].Add(response);

        business.Capital -= budget;
        business.TotalExpenses += budget;

        return true;
    }

    private float CalculateResponseEffectiveness(string actionType, float budget)
    {
        float baseEffectiveness = 0.5f;
        switch (actionType.ToLower())
        {
            case "apology": return baseEffectiveness * (budget / 10000f);
            case "compensation": return baseEffectiveness * (budget / 50000f);
            case "investigation": return baseEffectiveness * (budget / 30000f);
            case "legaldefense": return baseEffectiveness * (budget / 100000f);
            case "rebranding": return baseEffectiveness * (budget / 200000f);
            default: return baseEffectiveness;
        }
    }

    private int CalculateResponseDuration(string actionType)
    {
        switch (actionType.ToLower())
        {
            case "apology": return 3;
            case "compensation": return 10;
            case "investigation": return 15;
            case "legaldefense": return 20;
            case "rebranding": return 30;
            default: return 10;
        }
    }

    private void ApplyResponseEffects(Business business, CrisisResponse response)
    {
        if (MarketingSystem.Instance != null)
        {
            var (_, reputation, _, _) = MarketingSystem.Instance.GetMarketingStatus(business);
            
            switch (response.ActionType.ToLower())
            {
                case "apology":
                    reputation.CustomerSatisfaction += response.Effectiveness * 0.2f;
                    break;
                case "compensation":
                    reputation.CustomerSatisfaction += response.Effectiveness * 0.3f;
                    break;
                case "investigation":
                    reputation.OverallScore += response.Effectiveness * 0.1f;
                    break;
                case "legaldefense":
                    reputation.OverallScore += response.Effectiveness * 0.05f;
                    break;
                case "rebranding":
                    reputation.OverallScore += response.Effectiveness * 0.2f;
                    reputation.CustomerSatisfaction += response.Effectiveness * 0.1f;
                    break;
            }
            
            reputation.UpdateOverallScore();
        }

        // Improve crisis resistance
        float currentResistance = GetCrisisResistance(business);
        crisisResistance[business] = Mathf.Min(0.8f, currentResistance + response.Effectiveness * 0.1f);
    }

    private float GetCrisisResistance(Business business)
    {
        if (!crisisResistance.ContainsKey(business))
            crisisResistance[business] = baseCrisisResistance;
        return crisisResistance[business];
    }

    public (List<Crisis>, List<CrisisResponse>, float) GetCrisisStatus(Business business)
    {
        List<Crisis> crises = activeCrises.ContainsKey(business) ? activeCrises[business] : new List<Crisis>();
        List<CrisisResponse> responses = crisisResponses.ContainsKey(business) ? crisisResponses[business] : new List<CrisisResponse>();
        float resistance = GetCrisisResistance(business);

        return (crises, responses, resistance);
    }

    public void ImproveCrisisPrevention(Business business, float investment)
    {
        if (business.Capital < investment)
            return;

        float currentResistance = GetCrisisResistance(business);
        crisisResistance[business] = Mathf.Min(0.9f, currentResistance + investment / 100000f); // $100k = 0.1 resistance

        business.Capital -= investment;
        business.TotalExpenses += investment;
    }
} 