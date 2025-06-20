using System.Collections.Generic;
using UnityEngine;

public class CompetitorAI : MonoBehaviour
{
    public static CompetitorAI Instance { get; private set; }
    public List<Business> Competitors { get; private set; } = new List<Business>();
    private string[] sampleIndustries = { "General", "Tech", "Food" };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateCompetitors(3);
    }

    public void CreateCompetitors(int count)
    {
        for (int i = 0; i < count; i++)
        {
            string name = $"AI Corp {i + 1}";
            string industry = sampleIndustries[Random.Range(0, sampleIndustries.Length)];
            string region = "Global";
            float capital = 150000f + Random.Range(-20000f, 20000f);
            var aiBusiness = new Business(name, industry, region, capital);
            // Add a product
            var product = new Product($"AI Product {i + 1}", industry, 100f, 50f, 0);
            aiBusiness.AddProduct(product);
            // Add employees
            for (int e = 0; e < 5; e++)
            {
                aiBusiness.HireEmployee(new Employee($"AI Employee {e + 1}", "Worker", 2000f, 1.0f));
            }
            Competitors.Add(aiBusiness);
        }
    }

    public void SimulateCompetitors()
    {
        foreach (var aiBusiness in Competitors)
        {
            // Deduct wages
            float totalWages = 0f;
            foreach (var employee in aiBusiness.Employees)
                totalWages += employee.Wage;
            aiBusiness.Capital -= totalWages;
            // Fire if can't pay
            if (aiBusiness.Capital < 0 && aiBusiness.Employees.Count > 0)
                aiBusiness.FireEmployee(aiBusiness.Employees[0]);
            // Produce and sell products
            foreach (var product in aiBusiness.Products)
            {
                aiBusiness.ProduceProduct(product, 10);
                var market = MarketManager.Instance.GetOrCreateMarket(product.Category, 1.0f);
                market.TotalSupply += product.Inventory;
                market.TotalDemand += Mathf.RoundToInt(8 + Random.Range(-2, 3));
                market.UpdateMarketPrice();
                int demand = Mathf.RoundToInt(market.TotalDemand / (market.TotalSupply + 1) * 10f);
                product.Price = market.MarketPrice;
                aiBusiness.SellProduct(product, demand);
                market.TotalSupply = 0;
                market.TotalDemand = 0;
            }
        }
    }
} 