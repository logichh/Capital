using UnityEngine;
using System.Collections.Generic;

public class SystemManager : MonoBehaviour
{
    public static SystemManager Instance { get; private set; }

    [Header("System Status")]
    public bool allSystemsInitialized = false;
    
    [Header("System References")]
    public GameManager gameManager;
    public BusinessManager businessManager;
    public MarketManager marketManager;
    public CompetitorAI competitorAI;
    public FinanceSystem financeSystem;
    public LogisticsSystem logisticsSystem;
    public RnDSystem rndSystem;
    public IPOSystem ipoSystem;
    public MarketingSystem marketingSystem;
    public CrisisSystem crisisSystem;
    public InternationalSystem internationalSystem;
    public MergerSystem mergerSystem;
    public AnalyticsSystem analyticsSystem;
    public ProgressionSystem progressionSystem;
    public RegulatorySystem regulatorySystem;
    public ScenarioSystem scenarioSystem;

    [Header("UI References")]
    public UIDashboard uiDashboard;
    public FinanceUI financeUI;
    public LogisticsUI logisticsUI;
    public RnDUI rndUI;
    public AdvancedUI advancedUI;

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

    private void Start()
    {
        InitializeAllSystems();
        VerifySystemConnections();
    }

    public void InitializeAllSystems()
    {
        Debug.Log("Initializing all systems...");

        // Find all system instances
        gameManager = FindFirstObjectByType<GameManager>();
        businessManager = FindFirstObjectByType<BusinessManager>();
        marketManager = FindFirstObjectByType<MarketManager>();
        competitorAI = FindFirstObjectByType<CompetitorAI>();
        financeSystem = FindFirstObjectByType<FinanceSystem>();
        logisticsSystem = FindFirstObjectByType<LogisticsSystem>();
        rndSystem = FindFirstObjectByType<RnDSystem>();
        ipoSystem = FindFirstObjectByType<IPOSystem>();
        marketingSystem = FindFirstObjectByType<MarketingSystem>();
        crisisSystem = FindFirstObjectByType<CrisisSystem>();
        internationalSystem = FindFirstObjectByType<InternationalSystem>();
        mergerSystem = FindFirstObjectByType<MergerSystem>();
        analyticsSystem = FindFirstObjectByType<AnalyticsSystem>();
        progressionSystem = FindFirstObjectByType<ProgressionSystem>();
        regulatorySystem = FindFirstObjectByType<RegulatorySystem>();
        scenarioSystem = FindFirstObjectByType<ScenarioSystem>();

        // Find UI instances
        uiDashboard = FindFirstObjectByType<UIDashboard>();
        financeUI = FindFirstObjectByType<FinanceUI>();
        logisticsUI = FindFirstObjectByType<LogisticsUI>();
        rndUI = FindFirstObjectByType<RnDUI>();
        advancedUI = FindFirstObjectByType<AdvancedUI>();

        // Initialize systems that need initialization
        if (rndSystem != null)
        {
            rndSystem.InitializeResearchProjects();
        }

        if (progressionSystem != null && businessManager != null)
        {
            foreach (var business in businessManager.Businesses)
            {
                progressionSystem.InitializeProgression(business);
            }
        }

        if (regulatorySystem != null && businessManager != null)
        {
            foreach (var business in businessManager.Businesses)
            {
                
            }
        }

        if (mergerSystem != null && businessManager != null)
        {
            foreach (var business in businessManager.Businesses)
            {
                
            }
        }

        allSystemsInitialized = true;
        Debug.Log("All systems initialized successfully!");
    }

    public void VerifySystemConnections()
    {
        Debug.Log("Verifying system connections...");

        var missingSystems = new List<string>();
        var missingUI = new List<string>();

        // Check core systems
        if (gameManager == null) missingSystems.Add("GameManager");
        if (businessManager == null) missingSystems.Add("BusinessManager");
        if (marketManager == null) missingSystems.Add("MarketManager");
        if (competitorAI == null) missingSystems.Add("CompetitorAI");

        // Check financial systems
        if (financeSystem == null) missingSystems.Add("FinanceSystem");
        if (logisticsSystem == null) missingSystems.Add("LogisticsSystem");

        // Check advanced systems
        if (rndSystem == null) missingSystems.Add("RnDSystem");
        if (ipoSystem == null) missingSystems.Add("IPOSystem");
        if (marketingSystem == null) missingSystems.Add("MarketingSystem");
        if (crisisSystem == null) missingSystems.Add("CrisisSystem");
        if (internationalSystem == null) missingSystems.Add("InternationalSystem");
        if (mergerSystem == null) missingSystems.Add("MergerSystem");
        if (analyticsSystem == null) missingSystems.Add("AnalyticsSystem");
        if (progressionSystem == null) missingSystems.Add("ProgressionSystem");
        if (regulatorySystem == null) missingSystems.Add("RegulatorySystem");
        if (scenarioSystem == null) missingSystems.Add("ScenarioSystem");

        // Check UI systems
        if (uiDashboard == null) missingUI.Add("UIDashboard");
        if (financeUI == null) missingUI.Add("FinanceUI");
        if (logisticsUI == null) missingUI.Add("LogisticsUI");
        if (rndUI == null) missingUI.Add("RnDUI");
        if (advancedUI == null) missingUI.Add("AdvancedUI");

        // Report results
        if (missingSystems.Count > 0)
        {
            Debug.LogWarning($"Missing systems: {string.Join(", ", missingSystems)}");
        }
        else
        {
            Debug.Log("All systems found and connected!");
        }

        if (missingUI.Count > 0)
        {
            Debug.LogWarning($"Missing UI components: {string.Join(", ", missingUI)}");
        }
        else
        {
            Debug.Log("All UI components found!");
        }
    }

    public bool AreAllSystemsReady()
    {
        return allSystemsInitialized && 
               gameManager != null && 
               businessManager != null && 
               marketManager != null;
    }

    public string GetSystemStatus()
    {
        var status = new List<string>();
        
        status.Add($"GameManager: {(gameManager != null ? "✓" : "✗")}");
        status.Add($"BusinessManager: {(businessManager != null ? "✓" : "✗")}");
        status.Add($"MarketManager: {(marketManager != null ? "✓" : "✗")}");
        status.Add($"CompetitorAI: {(competitorAI != null ? "✓" : "✗")}");
        status.Add($"FinanceSystem: {(financeSystem != null ? "✓" : "✗")}");
        status.Add($"LogisticsSystem: {(logisticsSystem != null ? "✓" : "✗")}");
        status.Add($"RnDSystem: {(rndSystem != null ? "✓" : "✗")}");
        status.Add($"IPOSystem: {(ipoSystem != null ? "✓" : "✗")}");
        status.Add($"MarketingSystem: {(marketingSystem != null ? "✓" : "✗")}");
        status.Add($"CrisisSystem: {(crisisSystem != null ? "✓" : "✗")}");
        status.Add($"InternationalSystem: {(internationalSystem != null ? "✓" : "✗")}");
        status.Add($"MergerSystem: {(mergerSystem != null ? "✓" : "✗")}");
        status.Add($"AnalyticsSystem: {(analyticsSystem != null ? "✓" : "✗")}");
        status.Add($"ProgressionSystem: {(progressionSystem != null ? "✓" : "✗")}");
        status.Add($"RegulatorySystem: {(regulatorySystem != null ? "✓" : "✗")}");
        status.Add($"ScenarioSystem: {(scenarioSystem != null ? "✓" : "✗")}");
        
        return string.Join("\n", status);
    }

    public void TestSystemIntegration()
    {
        Debug.Log("Testing system integration...");
        
        if (businessManager?.PlayerBusiness != null)
        {
            var business = businessManager.PlayerBusiness;
            
            // Test each system
            TestFinanceSystem(business);
            TestLogisticsSystem(business);
            TestRnDSystem(business);
            TestIPOSystem(business);
            TestMarketingSystem(business);
            TestCrisisSystem(business);
            TestInternationalSystem(business);
            TestMergerSystem(business);
            TestAnalyticsSystem(business);
            TestProgressionSystem(business);
            TestRegulatorySystem(business);
            TestScenarioSystem(business);
            
            Debug.Log("System integration test completed!");
        }
        else
        {
            Debug.LogWarning("No player business found for integration testing");
        }
    }

    private void TestFinanceSystem(Business business)
    {
        if (financeSystem != null)
        {
            Debug.Log("Testing Finance System...");
            // Test loan application
            bool loanResult = financeSystem.TakeLoan(business, 100000f, 12);
            Debug.Log($"Loan application result: {loanResult}");
        }
    }

    private void TestLogisticsSystem(Business business)
    {
        if (logisticsSystem != null)
        {
            Debug.Log("Testing Logistics System...");
            // Test warehouse creation
            var warehouse = logisticsSystem.AddWarehouse(business, "Main Warehouse", 1000);
            Debug.Log($"Warehouse created: {warehouse.Location}");
        }
    }

    private void TestRnDSystem(Business business)
    {
        if (rndSystem != null)
        {
            Debug.Log("Testing R&D System...");
            var projects = rndSystem.GetAvailableProjects(business);
            Debug.Log($"Available R&D projects: {projects.Count}");
        }
    }

    private void TestIPOSystem(Business business)
    {
        if (ipoSystem != null)
        {
            Debug.Log("Testing IPO System...");
            bool canGoPublic = ipoSystem.CanGoPublic(business);
            Debug.Log($"Can go public: {canGoPublic}");
        }
    }

    private void TestMarketingSystem(Business business)
    {
        if (marketingSystem != null)
        {
            Debug.Log("Testing Marketing System...");
            var (campaigns, reputation, segments, research) = marketingSystem.GetMarketingStatus(business);
            Debug.Log($"Marketing campaigns: {campaigns.Count}, Reputation: {reputation.OverallScore}");
        }
    }

    private void TestCrisisSystem(Business business)
    {
        if (crisisSystem != null)
        {
            Debug.Log("Testing Crisis System...");
            var (crises, responses, resistance) = crisisSystem.GetCrisisStatus(business);
            Debug.Log($"Active crises: {crises.Count}, Resistance: {resistance * 100}%");
        }
    }

    private void TestInternationalSystem(Business business)
    {
        if (internationalSystem != null)
        {
            Debug.Log("Testing International System...");
            var (markets, subsidiaries, agreements) = internationalSystem.GetInternationalStatus(business);
            Debug.Log($"International markets: {markets.Count}, Subsidiaries: {subsidiaries.Count}");
        }
    }

    private void TestMergerSystem(Business business)
    {
        if (mergerSystem != null && businessManager != null)
        {
            Debug.Log("Testing Merger System...");
            var target = businessManager.Businesses.Find(b => b != business);
            if (target != null)
            {
                bool canAcquire = mergerSystem.CanAcquire(business, target);
                Debug.Log($"Can acquire {target.Name}: {canAcquire}");
                if (canAcquire)
                {
                    mergerSystem.Acquire(business, target);
                }
            }
            else
            {
                Debug.Log("No merger targets found to test.");
            }
        }
    }

    private void TestAnalyticsSystem(Business business)
    {
        if (analyticsSystem != null)
        {
            Debug.Log("Testing Analytics System...");
            // Test report generation
            var (reports, analyses, metrics, forecasts) = analyticsSystem.GetAnalyticsStatus(business);
            Debug.Log($"Generated reports: {string.Join(", ", reports)}");
            Debug.Log($"Generated analyses: {string.Join(", ", analyses)}");
            Debug.Log($"Generated metrics: {string.Join(", ", metrics)}");
            Debug.Log($"Generated forecasts: {string.Join(", ", forecasts)}");
        }
    }

    private void TestProgressionSystem(Business business)
    {
        if (progressionSystem != null)
        {
            Debug.Log("Testing Progression System...");
            var (achievements, levels, unlockables, bonuses) = progressionSystem.GetProgressionStatus(business);
            Debug.Log($"Achievements: {achievements.Count}, Prestige levels: {levels.Count}");
        }
    }

    private void TestRegulatorySystem(Business business)
    {
        if (regulatorySystem != null)
        {
            Debug.Log("Testing Regulatory System...");
            // Test compliance check
            regulatorySystem.CheckCompliance(business);
        }
    }

    private void TestScenarioSystem(Business business)
    {
        if (scenarioSystem != null)
        {
            Debug.Log("Testing Scenario System...");
            var (scenario, challenges, events, completed) = scenarioSystem.GetScenarioStatus(business);
            Debug.Log($"Active scenario: {(scenario != null ? scenario.Name : "None")}, Challenges: {challenges.Count}");
        }
    }
} 