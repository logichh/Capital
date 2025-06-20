using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Business
{
    public string Name;
    public string Industry;
    public string Region;
    public float Capital;
    public List<Employee> Employees = new List<Employee>();
    public List<Product> Products = new List<Product>();
    public float MarketShare;
    public float Reputation;
    
    // Financial metrics
    public float Revenue { get; set; } = 0f;
    public float TotalExpenses { get; set; } = 0f;
    public float NetIncome => Revenue - TotalExpenses;
    public float TotalAssets => Capital + Products.Sum(p => p.Inventory * p.Cost);
    public float TotalLiabilities { get; private set; } = 0f;
    public float NetWorth => TotalAssets - TotalLiabilities;
    public float CreditScore { get; set; } = 500f; // Base credit score
    
    // Tax-related properties
    public float LastMonthRevenue { get; private set; } = 0f;
    public float LastMonthExpenses { get; private set; } = 0f;
    public float AccumulatedTaxes { get; private set; } = 0f;

    // R&D and Efficiency metrics
    public float ProductionEfficiency { get; set; } = 1.0f;
    public float ProcessEfficiency { get; set; } = 1.0f;
    public float LogisticsEfficiency { get; set; } = 1.0f;
    public float ResearchEfficiency { get; set; } = 1.0f; // New field for R&D speed
    public HashSet<string> CompletedResearch { get; private set; } = new HashSet<string>();
    public float ResearchBudget { get; set; } = 0f;

    // Advanced system fields
    public bool IsPublic { get; set; } = false;
    public float StockPrice { get; set; } = 0f;
    public float BrandValue { get; set; } = 0f;
    public float CustomerSatisfaction { get; set; } = 50f;
    public float InnovationScore { get; set; } = 0f;
    public float SocialResponsibility { get; set; } = 50f;
    public float EnvironmentalImpact { get; set; } = 50f;
    public float CrisisResistance { get; set; } = 30f;
    public float ComplianceScore { get; set; } = 70f;
    public float InternationalPresence { get; set; } = 0f;
    public float MergerExperience { get; set; } = 0f;
    public float PrestigeLevel { get; set; } = 1f;
    public float AchievementProgress { get; set; } = 0f;

    // Performance tracking
    public float PeakCapital { get; private set; } = 0f;
    public float PeakMarketShare { get; private set; } = 0f;
    public int DaysInBusiness { get; set; } = 0;
    public float TotalRevenueEver { get; private set; } = 0f;
    public float TotalProfitEver { get; private set; } = 0f;

    public Business(string name, string industry, string region, float capital)
    {
        Name = name;
        Industry = industry;
        Region = region;
        Capital = capital;
        MarketShare = 0f;
        Reputation = 0f;
        PeakCapital = capital;
    }

    public void AddProduct(Product product)
    {
        Products.Add(product);
    }

    public void HireEmployee(Employee employee)
    {
        Employees.Add(employee);
        Capital -= employee.Wage; // Pay initial wage
        TotalExpenses += employee.Wage;
        UpdateCreditScore();
    }

    public void FireEmployee(Employee employee)
    {
        if (Employees.Remove(employee))
        {
            // Severance pay = 1 month salary
            Capital -= employee.Wage;
            TotalExpenses += employee.Wage;
            UpdateCreditScore();
        }
    }

    public void ProduceProduct(Product product, int quantity)
    {
        float avgSkill = Employee.AverageSkill(Employees);
        int effectiveQuantity = Mathf.RoundToInt(quantity * avgSkill * ProductionEfficiency);
        float totalCost = product.Cost * effectiveQuantity;
        
        if (Capital >= totalCost)
        {
            product.Inventory += effectiveQuantity;
            Capital -= totalCost;
            TotalExpenses += totalCost;
            
            // Improve product quality based on skill and research
            product.Quality += 0.01f * (avgSkill - 1f);
            product.Quality = Mathf.Clamp(product.Quality, 0.5f, 2.0f);
            
            UpdateCreditScore();
        }
    }

    public void SellProduct(Product product, int quantity)
    {
        int sellQuantity = Mathf.Min(product.Inventory, quantity);
        float sales = sellQuantity * product.Price;
        product.Inventory -= sellQuantity;
        Capital += sales;
        Revenue += sales;
        TotalRevenueEver += sales;
        UpdateCreditScore();
        UpdatePeakMetrics();
    }

    public void UpdateMonthlyFinancials()
    {
        LastMonthRevenue = Revenue;
        LastMonthExpenses = TotalExpenses;
        TotalProfitEver += NetIncome;
        Revenue = 0f;
        TotalExpenses = 0f;
        DaysInBusiness += 30;
    }

    public void AddLiability(float amount)
    {
        TotalLiabilities += amount;
        UpdateCreditScore();
    }

    public void ReduceLiability(float amount)
    {
        TotalLiabilities = Mathf.Max(0, TotalLiabilities - amount);
        UpdateCreditScore();
    }

    private void UpdateCreditScore()
    {
        // Simple credit score calculation
        float debtToAssetRatio = TotalAssets > 0 ? TotalLiabilities / TotalAssets : 1f;
        float profitMargin = Revenue > 0 ? NetIncome / Revenue : 0f;
        
        // Base score adjustments
        CreditScore = 500f;
        
        // Asset-based adjustment (up to +200 points)
        CreditScore += Mathf.Min(200f, TotalAssets / 10000f);
        
        // Debt ratio penalty (up to -100 points)
        CreditScore -= Mathf.Min(100f, debtToAssetRatio * 100f);
        
        // Profit margin bonus (up to +100 points)
        CreditScore += Mathf.Min(100f, profitMargin * 100f);
        
        // Employee count bonus (up to +100 points)
        CreditScore += Mathf.Min(100f, Employees.Count * 10f);
        
        // Research bonus (up to +100 points)
        CreditScore += Mathf.Min(100f, CompletedResearch.Count * 10f);
        
        // Advanced system bonuses
        CreditScore += Mathf.Min(50f, BrandValue / 10000f);
        CreditScore += Mathf.Min(50f, InnovationScore / 10f);
        CreditScore += Mathf.Min(50f, ComplianceScore / 10f);
        
        // Clamp final score
        CreditScore = Mathf.Clamp(CreditScore, 300f, 850f);
    }

    private void UpdatePeakMetrics()
    {
        if (Capital > PeakCapital)
            PeakCapital = Capital;
        if (MarketShare > PeakMarketShare)
            PeakMarketShare = MarketShare;
    }

    public void AddTax(float amount)
    {
        AccumulatedTaxes += amount;
    }

    public void PayTaxes(float amount)
    {
        float payment = Mathf.Min(amount, AccumulatedTaxes);
        AccumulatedTaxes -= payment;
        Capital -= payment;
        TotalExpenses += payment;
    }

    public void CompleteResearch(string researchName)
    {
        CompletedResearch.Add(researchName);
        InnovationScore += 10f;
        UpdateCreditScore();
    }

    // New methods for advanced systems
    public void GoPublic(float sharePrice)
    {
        IsPublic = true;
        StockPrice = sharePrice;
        BrandValue += 100000f;
    }

    public void ImproveBrand(float amount)
    {
        BrandValue += amount;
        CustomerSatisfaction = Mathf.Min(100f, CustomerSatisfaction + amount / 10000f);
    }

    public void ImproveInnovation(float amount)
    {
        InnovationScore += amount;
        ResearchEfficiency = Mathf.Min(2.0f, ResearchEfficiency + amount / 100f);
    }

    public void ImproveSocialResponsibility(float amount)
    {
        SocialResponsibility = Mathf.Min(100f, SocialResponsibility + amount);
        Reputation = Mathf.Min(100f, Reputation + amount / 10f);
    }

    public void ImproveEnvironmentalImpact(float amount)
    {
        EnvironmentalImpact = Mathf.Min(100f, EnvironmentalImpact + amount);
        Reputation = Mathf.Min(100f, Reputation + amount / 10f);
    }

    public void ImproveCrisisResistance(float amount)
    {
        CrisisResistance = Mathf.Min(90f, CrisisResistance + amount);
    }

    public void ImproveCompliance(float amount)
    {
        ComplianceScore = Mathf.Min(100f, ComplianceScore + amount);
    }

    public void ExpandInternationally()
    {
        InternationalPresence += 1f;
        BrandValue += 50000f;
    }

    public void CompleteMerger()
    {
        MergerExperience += 1f;
        BrandValue += 75000f;
    }

    public void IncreasePrestige(float amount)
    {
        PrestigeLevel += amount;
        BrandValue += amount * 10000f;
    }

    public void UpdateAchievementProgress(float progress)
    {
        AchievementProgress = Mathf.Min(100f, AchievementProgress + progress);
    }

    public float GetOverallPerformance()
    {
        float performance = 0f;
        performance += Capital / 100000f * 25f; // 25% weight
        performance += MarketShare * 100f * 20f; // 20% weight
        performance += InnovationScore / 10f * 15f; // 15% weight
        performance += BrandValue / 100000f * 15f; // 15% weight
        performance += CustomerSatisfaction * 10f; // 10% weight
        performance += ComplianceScore * 10f; // 10% weight
        performance += PrestigeLevel * 5f; // 5% weight
        return performance;
    }
} 