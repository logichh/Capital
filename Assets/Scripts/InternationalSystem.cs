using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class InternationalMarket
{
    public string Country;
    public string Currency;
    public float ExchangeRate; // To USD
    public float MarketSize;
    public float MarketShare;
    public float RegulatoryCost;
    public float CulturalBarrier;
    public float InfrastructureQuality;
    public List<string> Regulations;
    public Dictionary<string, float> LocalPreferences;

    public InternationalMarket(string country, string currency, float exchangeRate, float marketSize)
    {
        Country = country;
        Currency = currency;
        ExchangeRate = exchangeRate;
        MarketSize = marketSize;
        MarketShare = 0f;
        RegulatoryCost = Random.Range(0.1f, 0.3f);
        CulturalBarrier = Random.Range(0.2f, 0.6f);
        InfrastructureQuality = Random.Range(0.5f, 0.9f);
        Regulations = new List<string>();
        LocalPreferences = new Dictionary<string, float>();
    }
}

[System.Serializable]
public class Subsidiary
{
    public string Name;
    public string Country;
    public float Capital;
    public List<Employee> Employees;
    public List<Product> Products;
    public float Revenue;
    public float Expenses;
    public float TaxRate;
    public bool IsProfitable;

    public Subsidiary(string name, string country, float capital)
    {
        Name = name;
        Country = country;
        Capital = capital;
        Employees = new List<Employee>();
        Products = new List<Product>();
        Revenue = 0f;
        Expenses = 0f;
        TaxRate = Random.Range(0.15f, 0.35f);
        IsProfitable = false;
    }
}

[System.Serializable]
public class TradeAgreement
{
    public string Country1;
    public string Country2;
    public float TariffReduction;
    public float TradeVolume;
    public int DurationTicks;
    public int RemainingTicks;

    public TradeAgreement(string country1, string country2, float tariffReduction, int duration)
    {
        Country1 = country1;
        Country2 = country2;
        TariffReduction = tariffReduction;
        TradeVolume = 0f;
        DurationTicks = duration;
        RemainingTicks = duration;
    }
}

public class InternationalSystem : MonoBehaviour
{
    public static InternationalSystem Instance { get; private set; }

    private Dictionary<Business, List<InternationalMarket>> businessMarkets = new Dictionary<Business, List<InternationalMarket>>();
    private Dictionary<Business, List<Subsidiary>> subsidiaries = new Dictionary<Business, List<Subsidiary>>();
    private Dictionary<Business, List<TradeAgreement>> tradeAgreements = new Dictionary<Business, List<TradeAgreement>>();
    private Dictionary<string, float> globalExchangeRates = new Dictionary<string, float>();

    [SerializeField] private float expansionCost = 500000f;
    [SerializeField] private float subsidiarySetupCost = 1000000f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeGlobalMarkets();
    }

    private void InitializeGlobalMarkets()
    {
        // Initialize exchange rates
        globalExchangeRates["USD"] = 1.0f;
        globalExchangeRates["EUR"] = 0.85f;
        globalExchangeRates["GBP"] = 0.73f;
        globalExchangeRates["JPY"] = 110.0f;
        globalExchangeRates["CNY"] = 6.45f;
        globalExchangeRates["INR"] = 74.5f;
        globalExchangeRates["BRL"] = 5.2f;
        globalExchangeRates["RUB"] = 73.8f;
    }

    public bool ExpandToMarket(Business business, string country)
    {
        if (business.Capital < expansionCost)
            return false;

        if (!businessMarkets.ContainsKey(business))
            businessMarkets[business] = new List<InternationalMarket>();

        // Check if already in market
        if (businessMarkets[business].Any(m => m.Country == country))
            return false;

        // Create new market entry
        string currency = GetCurrencyForCountry(country);
        float exchangeRate = globalExchangeRates.ContainsKey(currency) ? globalExchangeRates[currency] : 1.0f;
        float marketSize = Random.Range(1000000f, 10000000f);
        
        var market = new InternationalMarket(country, currency, exchangeRate, marketSize);
        
        // Add local regulations
        AddLocalRegulations(market);
        
        // Add local preferences
        AddLocalPreferences(market);

        businessMarkets[business].Add(market);
        business.Capital -= expansionCost;
        business.TotalExpenses += expansionCost;

        Debug.Log($"{business.Name} expanded to {country}");
        return true;
    }

    public bool CreateSubsidiary(Business business, string country, string name, float capital)
    {
        if (business.Capital < subsidiarySetupCost + capital)
            return false;

        if (!subsidiaries.ContainsKey(business))
            subsidiaries[business] = new List<Subsidiary>();

        var subsidiary = new Subsidiary(name, country, capital);
        subsidiaries[business].Add(subsidiary);

        business.Capital -= (subsidiarySetupCost + capital);
        business.TotalExpenses += subsidiarySetupCost;

        Debug.Log($"Created subsidiary {name} in {country}");
        return true;
    }

    public void ProcessInternationalOperations()
    {
        // Update exchange rates (simulate currency fluctuations)
        foreach (var currency in globalExchangeRates.Keys.ToList())
        {
            if (currency != "USD")
            {
                float fluctuation = Random.Range(-0.05f, 0.05f);
                globalExchangeRates[currency] *= (1 + fluctuation);
            }
        }

        // Process subsidiaries
        foreach (var kvp in subsidiaries)
        {
            var business = kvp.Key;
            foreach (var subsidiary in kvp.Value)
            {
                ProcessSubsidiary(business, subsidiary);
            }
        }

        // Process trade agreements
        foreach (var kvp in tradeAgreements)
        {
            var business = kvp.Key;
            var agreements = kvp.Value;

            for (int i = agreements.Count - 1; i >= 0; i--)
            {
                var agreement = agreements[i];
                agreement.RemainingTicks--;

                if (agreement.RemainingTicks <= 0)
                {
                    agreements.RemoveAt(i);
                    Debug.Log($"Trade agreement expired between {agreement.Country1} and {agreement.Country2}");
                }
            }
        }
    }

    private void ProcessSubsidiary(Business business, Subsidiary subsidiary)
    {
        // Simulate subsidiary operations
        float localRevenue = subsidiary.Products.Sum(p => p.Price * p.Inventory * 0.1f); // 10% of inventory sells
        float localExpenses = subsidiary.Employees.Sum(e => e.Wage) + subsidiary.Products.Sum(p => p.Cost * p.Inventory * 0.05f);
        
        subsidiary.Revenue = localRevenue;
        subsidiary.Expenses = localExpenses;
        subsidiary.IsProfitable = localRevenue > localExpenses;

        // Apply local taxes
        float taxes = (localRevenue - localExpenses) * subsidiary.TaxRate;
        subsidiary.Capital += (localRevenue - localExpenses - taxes);

        // Convert to USD and transfer profits to parent
        float profitUSD = (localRevenue - localExpenses - taxes) * GetExchangeRate(subsidiary.Country);
        if (profitUSD > 0)
        {
            business.Capital += profitUSD;
            business.Revenue += profitUSD;
        }
    }

    public bool NegotiateTradeAgreement(Business business, string country1, string country2, float cost)
    {
        if (business.Capital < cost)
            return false;

        if (!tradeAgreements.ContainsKey(business))
            tradeAgreements[business] = new List<TradeAgreement>();

        float tariffReduction = Random.Range(0.1f, 0.3f);
        int duration = Random.Range(50, 200);
        
        var agreement = new TradeAgreement(country1, country2, tariffReduction, duration);
        tradeAgreements[business].Add(agreement);

        business.Capital -= cost;
        business.TotalExpenses += cost;

        Debug.Log($"Trade agreement negotiated between {country1} and {country2}");
        return true;
    }

    private string GetCurrencyForCountry(string country)
    {
        switch (country.ToLower())
        {
            case "germany":
            case "france":
            case "italy":
            case "spain":
                return "EUR";
            case "united kingdom":
            case "uk":
                return "GBP";
            case "japan":
                return "JPY";
            case "china":
                return "CNY";
            case "india":
                return "INR";
            case "brazil":
                return "BRL";
            case "russia":
                return "RUB";
            default:
                return "USD";
        }
    }

    private void AddLocalRegulations(InternationalMarket market)
    {
        switch (market.Country.ToLower())
        {
            case "germany":
                market.Regulations.Add("Environmental Standards");
                market.Regulations.Add("Labor Laws");
                market.RegulatoryCost = 0.25f;
                break;
            case "japan":
                market.Regulations.Add("Quality Standards");
                market.Regulations.Add("Import Restrictions");
                market.RegulatoryCost = 0.3f;
                break;
            case "china":
                market.Regulations.Add("Government Approval");
                market.Regulations.Add("Local Partnership");
                market.RegulatoryCost = 0.4f;
                break;
            case "india":
                market.Regulations.Add("Bureaucratic Procedures");
                market.Regulations.Add("Local Content");
                market.RegulatoryCost = 0.35f;
                break;
        }
    }

    private void AddLocalPreferences(InternationalMarket market)
    {
        switch (market.Country.ToLower())
        {
            case "germany":
                market.LocalPreferences["Quality"] = 0.9f;
                market.LocalPreferences["Eco-Friendly"] = 0.8f;
                market.LocalPreferences["Price"] = 0.6f;
                break;
            case "japan":
                market.LocalPreferences["Quality"] = 0.95f;
                market.LocalPreferences["Innovation"] = 0.9f;
                market.LocalPreferences["Service"] = 0.8f;
                break;
            case "china":
                market.LocalPreferences["Price"] = 0.8f;
                market.LocalPreferences["Technology"] = 0.7f;
                market.LocalPreferences["Brand"] = 0.6f;
                break;
            case "india":
                market.LocalPreferences["Price"] = 0.9f;
                market.LocalPreferences["Value"] = 0.8f;
                market.LocalPreferences["Local"] = 0.7f;
                break;
        }
    }

    public float GetExchangeRate(string country)
    {
        string currency = GetCurrencyForCountry(country);
        return globalExchangeRates.ContainsKey(currency) ? globalExchangeRates[currency] : 1.0f;
    }

    public (List<InternationalMarket>, List<Subsidiary>, List<TradeAgreement>) GetInternationalStatus(Business business)
    {
        List<InternationalMarket> markets = businessMarkets.ContainsKey(business) ? businessMarkets[business] : new List<InternationalMarket>();
        List<Subsidiary> businessSubsidiaries = subsidiaries.ContainsKey(business) ? subsidiaries[business] : new List<Subsidiary>();
        List<TradeAgreement> agreements = tradeAgreements.ContainsKey(business) ? tradeAgreements[business] : new List<TradeAgreement>();

        return (markets, businessSubsidiaries, agreements);
    }

    public List<string> GetAvailableCountries()
    {
        return new List<string> { "Germany", "France", "United Kingdom", "Japan", "China", "India", "Brazil", "Russia" };
    }

    public float CalculateMarketPotential(string country)
    {
        switch (country.ToLower())
        {
            case "germany": return 5000000f;
            case "france": return 4000000f;
            case "united kingdom": return 4500000f;
            case "japan": return 6000000f;
            case "china": return 15000000f;
            case "india": return 8000000f;
            case "brazil": return 3000000f;
            case "russia": return 2500000f;
            default: return 1000000f;
        }
    }
} 