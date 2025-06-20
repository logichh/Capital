using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Achievement
{
    public string Name;
    public string Description;
    public string Category; // "Financial", "Operational", "Market", "Innovation", "Social"
    public float TargetValue;
    public float CurrentValue;
    public bool IsCompleted;
    public float Reward;
    public string IconName;

    public Achievement(string name, string description, string category, float target, float reward)
    {
        Name = name;
        Description = description;
        Category = category;
        TargetValue = target;
        CurrentValue = 0f;
        IsCompleted = false;
        Reward = reward;
        IconName = "achievement_" + category.ToLower();
    }
}

[System.Serializable]
public class PrestigeLevel
{
    public int Level;
    public string Title;
    public float RequiredValue;
    public List<string> UnlockedFeatures;
    public Dictionary<string, float> Bonuses;
    public bool IsUnlocked;

    public PrestigeLevel(int level, string title, float requiredValue)
    {
        Level = level;
        Title = title;
        RequiredValue = requiredValue;
        UnlockedFeatures = new List<string>();
        Bonuses = new Dictionary<string, float>();
        IsUnlocked = false;
    }
}

[System.Serializable]
public class Unlockable
{
    public string Name;
    public string Description;
    public string Type; // "Feature", "Building", "Technology", "Employee"
    public float UnlockCost;
    public bool IsUnlocked;
    public List<string> Requirements;

    public Unlockable(string name, string description, string type, float cost)
    {
        Name = name;
        Description = description;
        Type = type;
        UnlockCost = cost;
        IsUnlocked = false;
        Requirements = new List<string>();
    }
}

[System.Serializable]
public class ProgressionBonus
{
    public string Name;
    public string Description;
    public float Value;
    public string Category;
    public bool IsActive;

    public ProgressionBonus(string name, string description, float value, string category)
    {
        Name = name;
        Description = description;
        Value = value;
        Category = category;
        IsActive = false;
    }
}

public class ProgressionSystem : MonoBehaviour
{
    public static ProgressionSystem Instance { get; private set; }

    private Dictionary<Business, List<Achievement>> achievements = new Dictionary<Business, List<Achievement>>();
    private Dictionary<Business, List<PrestigeLevel>> prestigeLevels = new Dictionary<Business, List<PrestigeLevel>>();
    private Dictionary<Business, List<Unlockable>> unlockables = new Dictionary<Business, List<Unlockable>>();
    private Dictionary<Business, List<ProgressionBonus>> activeBonuses = new Dictionary<Business, List<ProgressionBonus>>();

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

    public void InitializeProgression(Business business)
    {
        InitializeAchievements(business);
        InitializePrestigeLevels(business);
        InitializeUnlockables(business);
        InitializeBonuses(business);
    }

    private void InitializeAchievements(Business business)
    {
        if (!achievements.ContainsKey(business))
            achievements[business] = new List<Achievement>();

        // Financial achievements
        achievements[business].Add(new Achievement("First Million", "Reach $1M in capital", "Financial", 1000000f, 50000f));
        achievements[business].Add(new Achievement("Profit Master", "Achieve $100K monthly profit", "Financial", 100000f, 25000f));
        achievements[business].Add(new Achievement("Market Leader", "Reach 20% market share", "Market", 0.2f, 75000f));
        achievements[business].Add(new Achievement("Innovation Hub", "Complete 10 research projects", "Innovation", 10f, 100000f));
        achievements[business].Add(new Achievement("Team Builder", "Hire 50 employees", "Operational", 50f, 30000f));
        achievements[business].Add(new Achievement("Product Portfolio", "Launch 5 products", "Operational", 5f, 40000f));
        achievements[business].Add(new Achievement("Global Expansion", "Enter 3 international markets", "Market", 3f, 150000f));
        achievements[business].Add(new Achievement("Crisis Survivor", "Resolve 5 crises successfully", "Social", 5f, 50000f));
        achievements[business].Add(new Achievement("IPO Success", "Go public successfully", "Financial", 1f, 200000f));
        achievements[business].Add(new Achievement("Merger Master", "Complete 3 acquisitions", "Financial", 3f, 125000f));
    }

    private void InitializePrestigeLevels(Business business)
    {
        if (!prestigeLevels.ContainsKey(business))
            prestigeLevels[business] = new List<PrestigeLevel>();

        // Define prestige levels
        var startup = new PrestigeLevel(1, "Startup", 0f);
        startup.UnlockedFeatures.Add("Basic Operations");
        startup.UnlockedFeatures.Add("Employee Hiring");
        startup.Bonuses["Efficiency"] = 1.0f;
        prestigeLevels[business].Add(startup);

        var smallBusiness = new PrestigeLevel(2, "Small Business", 1000000f);
        smallBusiness.UnlockedFeatures.Add("Advanced Marketing");
        smallBusiness.UnlockedFeatures.Add("R&D Projects");
        smallBusiness.Bonuses["Efficiency"] = 1.1f;
        smallBusiness.Bonuses["ResearchSpeed"] = 1.1f;
        prestigeLevels[business].Add(smallBusiness);

        var mediumBusiness = new PrestigeLevel(3, "Medium Business", 5000000f);
        mediumBusiness.UnlockedFeatures.Add("International Expansion");
        mediumBusiness.UnlockedFeatures.Add("Mergers & Acquisitions");
        mediumBusiness.Bonuses["Efficiency"] = 1.2f;
        mediumBusiness.Bonuses["MarketAccess"] = 1.2f;
        prestigeLevels[business].Add(mediumBusiness);

        var largeBusiness = new PrestigeLevel(4, "Large Business", 20000000f);
        largeBusiness.UnlockedFeatures.Add("IPO Access");
        largeBusiness.UnlockedFeatures.Add("Advanced Analytics");
        largeBusiness.Bonuses["Efficiency"] = 1.3f;
        largeBusiness.Bonuses["FinancialAccess"] = 1.3f;
        prestigeLevels[business].Add(largeBusiness);

        var corporation = new PrestigeLevel(5, "Corporation", 50000000f);
        corporation.UnlockedFeatures.Add("Crisis Management");
        corporation.UnlockedFeatures.Add("Advanced Progression");
        corporation.Bonuses["Efficiency"] = 1.5f;
        corporation.Bonuses["AllBonuses"] = 1.2f;
        prestigeLevels[business].Add(corporation);
    }

    private void InitializeUnlockables(Business business)
    {
        if (!unlockables.ContainsKey(business))
            unlockables[business] = new List<Unlockable>();

        // Buildings
        unlockables[business].Add(new Unlockable("Research Lab", "Advanced research facility", "Building", 500000f));
        unlockables[business].Add(new Unlockable("Marketing HQ", "Centralized marketing operations", "Building", 300000f));
        unlockables[business].Add(new Unlockable("Training Center", "Employee development facility", "Building", 200000f));

        // Technologies
        unlockables[business].Add(new Unlockable("AI Integration", "Artificial intelligence systems", "Technology", 1000000f));
        unlockables[business].Add(new Unlockable("Cloud Infrastructure", "Scalable cloud computing", "Technology", 750000f));
        unlockables[business].Add(new Unlockable("Automation Systems", "Production automation", "Technology", 600000f));

        // Features
        unlockables[business].Add(new Unlockable("Advanced Analytics", "Detailed business insights", "Feature", 400000f));
        unlockables[business].Add(new Unlockable("Crisis Management", "Risk mitigation systems", "Feature", 350000f));
        unlockables[business].Add(new Unlockable("International Trade", "Global market access", "Feature", 800000f));
    }

    private void InitializeBonuses(Business business)
    {
        if (!activeBonuses.ContainsKey(business))
            activeBonuses[business] = new List<ProgressionBonus>();

        activeBonuses[business].Add(new ProgressionBonus("Efficiency Boost", "Increased production efficiency", 0.1f, "Operational"));
        activeBonuses[business].Add(new ProgressionBonus("Research Speed", "Faster research completion", 0.15f, "Innovation"));
        activeBonuses[business].Add(new ProgressionBonus("Market Access", "Better market penetration", 0.1f, "Market"));
        activeBonuses[business].Add(new ProgressionBonus("Financial Access", "Better loan terms", 0.1f, "Financial"));
        activeBonuses[business].Add(new ProgressionBonus("Employee Retention", "Lower employee turnover", 0.1f, "Operational"));
    }

    public void UpdateAchievements(Business business)
    {
        if (!achievements.ContainsKey(business))
            return;

        foreach (var achievement in achievements[business])
        {
            if (achievement.IsCompleted)
                continue;

            // Update achievement progress
            switch (achievement.Name)
            {
                case "First Million":
                    achievement.CurrentValue = business.Capital;
                    break;
                case "Profit Master":
                    achievement.CurrentValue = business.NetIncome;
                    break;
                case "Market Leader":
                    achievement.CurrentValue = business.MarketShare;
                    break;
                case "Innovation Hub":
                    achievement.CurrentValue = business.CompletedResearch.Count;
                    break;
                case "Team Builder":
                    achievement.CurrentValue = business.Employees.Count;
                    break;
                case "Product Portfolio":
                    achievement.CurrentValue = business.Products.Count;
                    break;
                case "Global Expansion":
                    if (InternationalSystem.Instance != null)
                    {
                        var (markets, _, _) = InternationalSystem.Instance.GetInternationalStatus(business);
                        achievement.CurrentValue = markets.Count;
                    }
                    break;
                case "Crisis Survivor":
                    if (CrisisSystem.Instance != null)
                    {
                        var (active, resolved, resistance) = CrisisSystem.Instance.GetCrisisStatus(business);
                        achievement.CurrentValue = resolved.Count;
                    }
                    break;
                case "IPO Success":
                    if (IPOSystem.Instance != null)
                    {
                        achievement.CurrentValue = business.IsPublic ? 1f : 0f;
                    }
                    break;
                case "Merger Master":
                    if (MergerSystem.Instance != null)
                    {
                        achievement.CurrentValue = business.MergerExperience;
                    }
                    break;
            }

            // Check if achievement is completed
            if (achievement.CurrentValue >= achievement.TargetValue && !achievement.IsCompleted)
            {
                achievement.IsCompleted = true;
                business.Capital += achievement.Reward;
                Debug.Log($"Achievement unlocked: {achievement.Name}! Reward: ${achievement.Reward:N0}");
            }
        }
    }

    public void UpdatePrestigeLevels(Business business)
    {
        if (!prestigeLevels.ContainsKey(business))
            return;

        float businessValue = business.Capital + business.TotalAssets;

        foreach (var level in prestigeLevels[business])
        {
            if (!level.IsUnlocked && businessValue >= level.RequiredValue)
            {
                level.IsUnlocked = true;
                ApplyPrestigeBonuses(business, level);
                Debug.Log($"Prestige level unlocked: {level.Title}!");
            }
        }
    }

    private void ApplyPrestigeBonuses(Business business, PrestigeLevel level)
    {
        if (!activeBonuses.ContainsKey(business))
            return;

        foreach (var bonus in activeBonuses[business])
        {
            if (level.Bonuses.ContainsKey(bonus.Category) || level.Bonuses.ContainsKey("AllBonuses"))
            {
                bonus.IsActive = true;
                float bonusValue = level.Bonuses.ContainsKey(bonus.Category) ? 
                    level.Bonuses[bonus.Category] : level.Bonuses["AllBonuses"];
                
                // Apply bonus effects
                ApplyBonusEffect(business, bonus, bonusValue);
            }
        }
    }

    private void ApplyBonusEffect(Business business, ProgressionBonus bonus, float value)
    {
        switch (bonus.Category)
        {
            case "Operational":
                if (bonus.Name.Contains("Efficiency"))
                    business.ProductionEfficiency *= (1 + value);
                break;
            case "Innovation":
                if (bonus.Name.Contains("Research"))
                    business.ResearchEfficiency *= (1 + value);
                break;
            case "Market":
                if (bonus.Name.Contains("Access"))
                    business.MarketShare *= (1 + value * 0.1f);
                break;
            case "Financial":
                if (bonus.Name.Contains("Access"))
                    business.CreditScore *= (1 + value);
                break;
        }
    }

    public bool UnlockFeature(Business business, string unlockableName)
    {
        if (!unlockables.ContainsKey(business))
            return false;

        var unlockable = unlockables[business].FirstOrDefault(u => u.Name == unlockableName);
        if (unlockable == null || unlockable.IsUnlocked)
            return false;

        if (business.Capital < unlockable.UnlockCost)
            return false;

        unlockable.IsUnlocked = true;
        business.Capital -= unlockable.UnlockCost;
        business.TotalExpenses += unlockable.UnlockCost;

        Debug.Log($"Unlocked: {unlockable.Name}");
        return true;
    }

    public (List<Achievement>, List<PrestigeLevel>, List<Unlockable>, List<ProgressionBonus>) GetProgressionStatus(Business business)
    {
        List<Achievement> businessAchievements = achievements.ContainsKey(business) ? achievements[business] : new List<Achievement>();
        List<PrestigeLevel> levels = prestigeLevels.ContainsKey(business) ? prestigeLevels[business] : new List<PrestigeLevel>();
        List<Unlockable> businessUnlockables = unlockables.ContainsKey(business) ? unlockables[business] : new List<Unlockable>();
        List<ProgressionBonus> bonuses = activeBonuses.ContainsKey(business) ? activeBonuses[business] : new List<ProgressionBonus>();

        return (businessAchievements, levels, businessUnlockables, bonuses);
    }

    public float GetProgressPercentage(Business business)
    {
        if (!achievements.ContainsKey(business))
            return 0f;

        int completed = achievements[business].Count(a => a.IsCompleted);
        int total = achievements[business].Count;
        
        return total > 0 ? (float)completed / total : 0f;
    }

    public int GetCurrentPrestigeLevel(Business business)
    {
        if (!prestigeLevels.ContainsKey(business))
            return 1;

        var currentLevel = prestigeLevels[business].Where(l => l.IsUnlocked).Max(l => l.Level);
        return currentLevel;
    }
} 