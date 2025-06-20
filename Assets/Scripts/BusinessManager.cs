using UnityEngine;
using System.Collections.Generic;

public class BusinessManager : MonoBehaviour
{
    public static BusinessManager Instance { get; private set; }
    public Business PlayerBusiness { get; private set; }
    
    // List to hold all businesses, including player and AI
    public List<Business> Businesses { get; private set; } = new List<Business>();

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

    public void CreatePlayerBusiness(string name, string industry, string region, float capital)
    {
        PlayerBusiness = new Business(name, industry, region, capital);
        Businesses.Add(PlayerBusiness); // Add player business to the list
        
        // Initialize finance system
        if (FinanceSystem.Instance != null)
        {
            // Initial credit score adjustment based on starting capital
            if (capital >= 200000f)
                PlayerBusiness.CreditScore += 100f;
            else if (capital >= 100000f)
                PlayerBusiness.CreditScore += 50f;
        }

        // Initialize logistics system
        if (LogisticsSystem.Instance != null)
        {
            // Generate initial suppliers
            LogisticsSystem.Instance.GenerateInitialSuppliers(PlayerBusiness);
            
            // Create initial warehouse
            LogisticsSystem.Instance.AddWarehouse(PlayerBusiness, region, 1000);
        }

        Debug.Log($"Created business: {name} in {industry} at {region} with ${capital}");
    }

    public void HireEmployee(Employee employee)
    {
        if (PlayerBusiness != null)
            PlayerBusiness.HireEmployee(employee);
    }

    public void FireEmployee(Employee employee)
    {
        if (PlayerBusiness != null)
            PlayerBusiness.FireEmployee(employee);
    }

    public void ProduceProduct(Product product, int quantity)
    {
        if (PlayerBusiness != null)
            PlayerBusiness.ProduceProduct(product, quantity);
    }

    public void SellProduct(Product product, int quantity)
    {
        if (PlayerBusiness != null)
            PlayerBusiness.SellProduct(product, quantity);
    }

    public bool TrainEmployees(float amount, float costPerEmployee, int cooldownTicks = 5)
    {
        if (PlayerBusiness != null)
        {
            int currentTick = GameManager.Instance != null ? GameManager.Instance.CurrentTick : 0;
            var eligible = new System.Collections.Generic.List<Employee>();
            foreach (var employee in PlayerBusiness.Employees)
            {
                if (currentTick - employee.LastTrainedTick >= cooldownTicks)
                    eligible.Add(employee);
            }
            float totalCost = eligible.Count * costPerEmployee;
            if (eligible.Count > 0 && PlayerBusiness.Capital >= totalCost)
            {
                foreach (var employee in eligible)
                {
                    employee.Train(amount, currentTick);
                }
                PlayerBusiness.Capital -= totalCost;
                return true;
            }
        }
        return false;
    }
} 