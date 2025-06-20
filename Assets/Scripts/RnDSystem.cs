using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ResearchProject
{
    public string Name;
    public string Description;
    public string Category; // "Technology", "Product", "Process", "Patent"
    public float BaseCost;
    public int DurationTicks;
    public int RemainingTicks;
    public float Progress; // 0-1
    public float SuccessChance;
    public List<string> Prerequisites;
    public Dictionary<string, float> Effects; // Effect type -> value

    public ResearchProject(string name, string description, string category, float baseCost, 
                          int durationTicks, float successChance, List<string> prerequisites = null)
    {
        Name = name;
        Description = description;
        Category = category;
        BaseCost = baseCost;
        DurationTicks = durationTicks;
        RemainingTicks = durationTicks;
        Progress = 0f;
        SuccessChance = successChance;
        Prerequisites = prerequisites ?? new List<string>();
        Effects = new Dictionary<string, float>();
    }

    public bool CanStart(Business business)
    {
        // Check if prerequisites are met
        foreach (var prereq in Prerequisites)
        {
            if (!business.CompletedResearch.Contains(prereq))
                return false;
        }
        return true;
    }

    public void AddEffect(string effectType, float value)
    {
        Effects[effectType] = value;
    }
}

[System.Serializable]
public class Patent
{
    public string Name;
    public string Description;
    public float Value;
    public int DurationTicks;
    public int RemainingTicks;
    public float LicensingRevenue;

    public Patent(string name, string description, float value, int durationTicks)
    {
        Name = name;
        Description = description;
        Value = value;
        DurationTicks = durationTicks;
        RemainingTicks = durationTicks;
        LicensingRevenue = value * 0.01f; // 1% of value per tick
    }
}

[System.Serializable]
public class Researcher : Employee
{
    public float ResearchEfficiency;
    public string Specialization;
    public List<string> CompletedProjects;

    public Researcher(string name, string specialization, float wage, float skill, float researchEfficiency) 
        : base(name, "Researcher", wage, skill)
    {
        Specialization = specialization;
        ResearchEfficiency = researchEfficiency;
        CompletedProjects = new List<string>();
    }

    public float GetResearchBonus(string category)
    {
        float bonus = ResearchEfficiency;
        if (Specialization.ToLower() == category.ToLower())
            bonus *= 1.5f;
        return bonus;
    }
}

public class RnDSystem : MonoBehaviour
{
    public static RnDSystem Instance { get; private set; }

    private Dictionary<Business, List<ResearchProject>> businessProjects = new Dictionary<Business, List<ResearchProject>>();
    private Dictionary<Business, List<Patent>> businessPatents = new Dictionary<Business, List<Patent>>();
    private Dictionary<Business, List<Researcher>> businessResearchers = new Dictionary<Business, List<Researcher>>();

    [SerializeField] private float baseResearchEfficiency = 1.0f;

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

    public void InitializeResearchProjects()
    {
        // Technology research projects
        var projects = new List<ResearchProject>
        {
            new ResearchProject("Basic Automation", "Improve production efficiency", "Technology", 50000f, 20, 0.9f),
            new ResearchProject("Advanced Materials", "Develop new material technologies", "Technology", 100000f, 30, 0.8f),
            new ResearchProject("AI Integration", "Implement AI in production", "Technology", 200000f, 40, 0.7f),
            
            // Product research projects
            new ResearchProject("Quality Improvement", "Enhance product quality", "Product", 30000f, 15, 0.95f),
            new ResearchProject("Feature Development", "Add new product features", "Product", 75000f, 25, 0.85f),
            new ResearchProject("Design Innovation", "Revolutionary product design", "Product", 150000f, 35, 0.75f),
            
            // Process research projects
            new ResearchProject("Lean Manufacturing", "Optimize production processes", "Process", 40000f, 18, 0.9f),
            new ResearchProject("Supply Chain Optimization", "Improve logistics efficiency", "Process", 80000f, 28, 0.8f),
            new ResearchProject("Green Technology", "Sustainable production methods", "Process", 120000f, 32, 0.85f),
            
            // Patent research projects
            new ResearchProject("Patent Filing", "File for product patent", "Patent", 25000f, 10, 0.95f),
            new ResearchProject("Technology Patent", "Patent new technology", "Patent", 100000f, 25, 0.8f),
            new ResearchProject("Process Patent", "Patent production process", "Patent", 75000f, 20, 0.85f)
        };

        // Add effects to projects
        projects[0].AddEffect("ProductionEfficiency", 0.2f); // Basic Automation
        projects[1].AddEffect("MaterialQuality", 0.3f); // Advanced Materials
        projects[2].AddEffect("AIEfficiency", 0.4f); // AI Integration
        
        projects[3].AddEffect("ProductQuality", 0.25f); // Quality Improvement
        projects[4].AddEffect("ProductFeatures", 0.3f); // Feature Development
        projects[5].AddEffect("DesignInnovation", 0.5f); // Design Innovation
        
        projects[6].AddEffect("ProcessEfficiency", 0.2f); // Lean Manufacturing
        projects[7].AddEffect("LogisticsEfficiency", 0.3f); // Supply Chain Optimization
        projects[8].AddEffect("Sustainability", 0.25f); // Green Technology
        
        projects[9].AddEffect("PatentValue", 50000f); // Patent Filing
        projects[10].AddEffect("PatentValue", 200000f); // Technology Patent
        projects[11].AddEffect("PatentValue", 150000f); // Process Patent

        // Store projects for access
        foreach (var project in projects)
        {
            // Projects will be available to all businesses
            // Implementation will be per-business
        }
    }

    public bool StartResearchProject(Business business, ResearchProject project)
    {
        if (!project.CanStart(business))
            return false;

        if (business.Capital < project.BaseCost)
            return false;

        if (!businessProjects.ContainsKey(business))
            businessProjects[business] = new List<ResearchProject>();

        // Check if project is already in progress
        if (businessProjects[business].Any(p => p.Name == project.Name))
            return false;

        var newProject = new ResearchProject(project.Name, project.Description, project.Category,
                                           project.BaseCost, project.DurationTicks, project.SuccessChance,
                                           new List<string>(project.Prerequisites));
        newProject.Effects = new Dictionary<string, float>(project.Effects);

        businessProjects[business].Add(newProject);
        business.Capital -= project.BaseCost;
        business.TotalExpenses += project.BaseCost;

        return true;
    }

    public void ProcessResearch()
    {
        foreach (var kvp in businessProjects)
        {
            var business = kvp.Key;
            var projects = kvp.Value;

            for (int i = projects.Count - 1; i >= 0; i--)
            {
                var project = projects[i];
                
                // Calculate research progress based on researchers
                float totalEfficiency = baseResearchEfficiency;
                if (businessResearchers.ContainsKey(business))
                {
                    foreach (var researcher in businessResearchers[business])
                    {
                        totalEfficiency += researcher.GetResearchBonus(project.Category);
                    }
                }

                project.Progress += totalEfficiency / project.DurationTicks;
                project.RemainingTicks--;

                if (project.Progress >= 1f || project.RemainingTicks <= 0)
                {
                    // Research completed
                    bool success = Random.value <= project.SuccessChance;
                    
                    if (success)
                    {
                        // Apply research effects
                        ApplyResearchEffects(business, project);
                        
                        // Add to completed research
                        business.CompletedResearch.Add(project.Name);
                        
                        // Create patent if applicable
                        if (project.Category == "Patent" && project.Effects.ContainsKey("PatentValue"))
                        {
                            CreatePatent(business, project);
                        }
                        
                        Debug.Log($"Research completed: {project.Name}");
                    }
                    else
                    {
                        Debug.Log($"Research failed: {project.Name}");
                    }

                    projects.RemoveAt(i);
                }
            }
        }

        // Process patent licensing revenue
        foreach (var kvp in businessPatents)
        {
            var business = kvp.Key;
            foreach (var patent in kvp.Value)
            {
                business.Capital += patent.LicensingRevenue;
                business.Revenue += patent.LicensingRevenue;
            }
        }
    }

    private void ApplyResearchEffects(Business business, ResearchProject project)
    {
        foreach (var effect in project.Effects)
        {
            switch (effect.Key)
            {
                case "ProductionEfficiency":
                    business.ProductionEfficiency += effect.Value;
                    break;
                case "ProductQuality":
                    foreach (var product in business.Products)
                    {
                        product.Quality += effect.Value;
                        product.Quality = Mathf.Clamp(product.Quality, 0.5f, 2.0f);
                    }
                    break;
                case "ProcessEfficiency":
                    business.ProcessEfficiency += effect.Value;
                    break;
                case "LogisticsEfficiency":
                    business.LogisticsEfficiency += effect.Value;
                    break;
                // Add more effects as needed
            }
        }
    }

    private void CreatePatent(Business business, ResearchProject project)
    {
        if (!businessPatents.ContainsKey(business))
            businessPatents[business] = new List<Patent>();

        float patentValue = project.Effects.ContainsKey("PatentValue") ? project.Effects["PatentValue"] : 50000f;
        var patent = new Patent(project.Name, project.Description, patentValue, 100); // 100 ticks duration
        businessPatents[business].Add(patent);
    }

    public bool HireResearcher(Business business, string name, string specialization, float wage, float skill, float researchEfficiency)
    {
        if (business.Capital < wage)
            return false;

        if (!businessResearchers.ContainsKey(business))
            businessResearchers[business] = new List<Researcher>();

        var researcher = new Researcher(name, specialization, wage, skill, researchEfficiency);
        businessResearchers[business].Add(researcher);
        business.HireEmployee(researcher);

        return true;
    }

    public (List<ResearchProject>, List<Patent>, List<Researcher>) GetRnDStatus(Business business)
    {
        List<ResearchProject> projects = businessProjects.ContainsKey(business) ? businessProjects[business] : new List<ResearchProject>();
        List<Patent> patents = businessPatents.ContainsKey(business) ? businessPatents[business] : new List<Patent>();
        List<Researcher> researchers = businessResearchers.ContainsKey(business) ? businessResearchers[business] : new List<Researcher>();
        return (projects, patents, researchers);
    }

    public List<ResearchProject> GetAvailableProjects(Business business)
    {
        // Return all projects that can be started by this business
        var availableProjects = new List<ResearchProject>();
        
        // This would be populated with the projects from InitializeResearchProjects
        // For now, return empty list - will be implemented in UI
        return availableProjects;
    }
} 