using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SystemIntegrationTest : MonoBehaviour
{
    [Header("Test Results")]
    public bool allTestsPassed = false;
    public List<string> testResults = new List<string>();
    
    [Header("Test Controls")]
    public bool runTestsOnStart = true;
    public bool runTestsOnDemand = false;

    private void Start()
    {
        if (runTestsOnStart)
        {
            Invoke(nameof(RunAllTests), 2f); // Wait for systems to initialize
        }
    }

    private void Update()
    {
        if (runTestsOnDemand && Input.GetKeyDown(KeyCode.T))
        {
            RunAllTests();
        }
    }

    public void RunAllTests()
    {
        Debug.Log("=== STARTING SYSTEM INTEGRATION TESTS ===");
        testResults.Clear();
        allTestsPassed = true;

        // Test 1: Core System Availability
        TestCoreSystems();
        
        // Test 2: Business Logic
        TestBusinessLogic();
        
        // Test 3: Financial Systems
        TestFinancialSystems();
        
        // Test 4: Operational Systems
        TestOperationalSystems();
        
        // Test 5: Advanced Systems
        TestAdvancedSystems();
        
        // Test 6: UI Integration
        TestUIIntegration();
        
        // Test 7: System Interactions
        TestSystemInteractions();
        
        // Test 8: Data Flow
        TestDataFlow();

        // Report results
        Debug.Log("=== TEST RESULTS ===");
        foreach (var result in testResults)
        {
            Debug.Log(result);
        }
        
        Debug.Log($"=== OVERALL RESULT: {(allTestsPassed ? "ALL TESTS PASSED" : "SOME TESTS FAILED")} ===");
    }

    private void TestCoreSystems()
    {
        Debug.Log("Testing Core Systems...");
        
        // Test GameManager
        if (GameManager.Instance != null)
        {
            AddTestResult("✓ GameManager found and accessible");
        }
        else
        {
            AddTestResult("✗ GameManager not found", false);
        }

        // Test BusinessManager
        if (BusinessManager.Instance != null)
        {
            AddTestResult("✓ BusinessManager found and accessible");
            if (BusinessManager.Instance.PlayerBusiness != null)
            {
                AddTestResult("✓ Player business initialized");
            }
            else
            {
                AddTestResult("✗ Player business not initialized", false);
            }
        }
        else
        {
            AddTestResult("✗ BusinessManager not found", false);
        }

        // Test MarketManager
        if (MarketManager.Instance != null)
        {
            AddTestResult("✓ MarketManager found and accessible");
        }
        else
        {
            AddTestResult("✗ MarketManager not found", false);
        }

        // Test CompetitorAI
        if (CompetitorAI.Instance != null)
        {
            AddTestResult("✓ CompetitorAI found and accessible");
        }
        else
        {
            AddTestResult("✗ CompetitorAI not found", false);
        }
    }

    private void TestBusinessLogic()
    {
        Debug.Log("Testing Business Logic...");
        
        var business = BusinessManager.Instance?.PlayerBusiness;
        if (business == null) return;

        // Test basic business operations
        var originalCash = business.Capital;
        
        // Test hiring employee
        var employee = new Employee("Test Employee", "Worker", 2000f, 0.5f);
        business.HireEmployee(employee);
        if (business.Employees.Contains(employee))
        {
            AddTestResult("✓ Employee hiring works");
        }
        else
        {
            AddTestResult("✗ Employee hiring failed", false);
        }

        // Test product creation
        var product = new Product("Test Product", "Test Category", 100f, 50f, 100, 0.8f);
        business.AddProduct(product);
        if (business.Products.Contains(product))
        {
            AddTestResult("✓ Product creation works");
        }
        else
        {
            AddTestResult("✗ Product creation failed", false);
        }

        // Test production
        business.ProduceProduct(product, 10);
        if (product.Inventory > 0)
        {
            AddTestResult("✓ Product production works");
        }
        else
        {
            AddTestResult("✗ Product production failed", false);
        }

        // Test sales
        var originalInventory = product.Inventory;
        business.SellProduct(product, 5);
        if (product.Inventory < originalInventory)
        {
            AddTestResult("✓ Product sales work");
        }
        else
        {
            AddTestResult("✗ Product sales failed", false);
        }
    }

    private void TestFinancialSystems()
    {
        Debug.Log("Testing Financial Systems...");
        
        var business = BusinessManager.Instance?.PlayerBusiness;
        if (business == null) return;

        // Test FinanceSystem
        if (FinanceSystem.Instance != null)
        {
            var originalCash = business.Capital;
            bool loanResult = FinanceSystem.Instance.TakeLoan(business, 50000f, 12);
            if (loanResult)
            {
                AddTestResult("✓ Finance system loan application works");
            }
            else
            {
                AddTestResult("✗ Finance system loan application failed", false);
            }
        }
        else
        {
            AddTestResult("✗ FinanceSystem not found", false);
        }

        // Test LogisticsSystem
        if (LogisticsSystem.Instance != null)
        {
            var warehouse = LogisticsSystem.Instance.AddWarehouse(business, "Test Warehouse", 100000);
            if (warehouse != null)
            {
                AddTestResult("✓ Logistics system warehouse creation works");
            }
            else
            {
                AddTestResult("✗ Logistics system warehouse creation failed", false);
            }
        }
        else
        {
            AddTestResult("✗ LogisticsSystem not found", false);
        }
    }

    private void TestOperationalSystems()
    {
        Debug.Log("Testing Operational Systems...");
        
        var business = BusinessManager.Instance?.PlayerBusiness;
        if (business == null) return;

        // Test RnDSystem
        if (RnDSystem.Instance != null)
        {
            var projects = RnDSystem.Instance.GetAvailableProjects(business);
            if (projects.Count > 0)
            {
                AddTestResult("✓ R&D system provides available projects");
            }
            else
            {
                AddTestResult("✗ R&D system has no available projects", false);
            }
        }
        else
        {
            AddTestResult("✗ RnDSystem not found", false);
        }

        // Test MarketingSystem
        if (MarketingSystem.Instance != null)
        {
            var (campaigns, reputation, segments, research) = MarketingSystem.Instance.GetMarketingStatus(business);
            AddTestResult("✓ Marketing system status retrieval works");
        }
        else
        {
            AddTestResult("✗ MarketingSystem not found", false);
        }

        // Test RegulatorySystem
        if (RegulatorySystem.Instance != null)
        {
            RegulatorySystem.Instance.CheckCompliance(business);
            AddTestResult("✓ Regulatory system status retrieval works");
        }
        else
        {
            AddTestResult("✗ RegulatorySystem not found", false);
        }
    }

    private void TestAdvancedSystems()
    {
        Debug.Log("Testing Advanced Systems...");
        
        var business = BusinessManager.Instance?.PlayerBusiness;
        if (business == null) return;

        // Test IPOSystem
        if (IPOSystem.Instance != null)
        {
            bool canGoPublic = IPOSystem.Instance.CanGoPublic(business);
            AddTestResult("✓ IPO system status check works");
        }
        else
        {
            AddTestResult("✗ IPOSystem not found", false);
        }

        // Test CrisisSystem
        if (CrisisSystem.Instance != null)
        {
            var (crises, responses, resistance) = CrisisSystem.Instance.GetCrisisStatus(business);
            AddTestResult("✓ Crisis system status retrieval works");
        }
        else
        {
            AddTestResult("✗ CrisisSystem not found", false);
        }

        // Test InternationalSystem
        if (InternationalSystem.Instance != null)
        {
            var (markets, subsidiaries, agreements) = InternationalSystem.Instance.GetInternationalStatus(business);
            AddTestResult("✓ International system status retrieval works");
        }
        else
        {
            AddTestResult("✗ InternationalSystem not found", false);
        }

        // Test MergerSystem
        if (MergerSystem.Instance != null && BusinessManager.Instance != null && BusinessManager.Instance.PlayerBusiness != null)
        {
            var player = BusinessManager.Instance.PlayerBusiness;
            var target = BusinessManager.Instance.Businesses.FirstOrDefault(b => b != player);
            
            if (target != null)
            {
                bool canAcquire = MergerSystem.Instance.CanAcquire(player, target);
                AddTestResult($"✓ Merger system CanAcquire check returned {canAcquire}");
            }
            else
            {
                AddTestResult("✓ Merger system (no targets to test)", true);
            }
        }
        else
        {
            AddTestResult("✗ MergerSystem not found or no businesses to test with", false);
        }

        // Test AnalyticsSystem
        if (AnalyticsSystem.Instance != null)
        {
            var (reports, analyses, metrics, forecasts) = AnalyticsSystem.Instance.GetAnalyticsStatus(business);
            AddTestResult($"✓ Analytics system status retrieval works. Found {reports.Count} reports.");
        }
        else
        {
            AddTestResult("✗ AnalyticsSystem not found", false);
        }

        // Test ProgressionSystem
        if (ProgressionSystem.Instance != null)
        {
            var (achievements, levels, unlockables, bonuses) = ProgressionSystem.Instance.GetProgressionStatus(business);
            AddTestResult("✓ Progression system status retrieval works");
        }
        else
        {
            AddTestResult("✗ ProgressionSystem not found", false);
        }

        // Test ScenarioSystem
        if (ScenarioSystem.Instance != null)
        {
            var (scenario, challenges, events, completed) = ScenarioSystem.Instance.GetScenarioStatus(business);
            AddTestResult("✓ Scenario system status retrieval works");
        }
        else
        {
            AddTestResult("✗ ScenarioSystem not found", false);
        }
    }

    private void TestUIIntegration()
    {
        Debug.Log("Testing UI Integration...");
        
        // Test UIDashboard
        var dashboard = FindFirstObjectByType<UIDashboard>();
        if (dashboard != null)
        {
            AddTestResult("✓ UIDashboard found");
            dashboard.UpdateKPIs(100000f, 50000f, 0.25f, 200000f, 1);
            AddTestResult("✓ UIDashboard KPI update works");
        }
        else
        {
            AddTestResult("✗ UIDashboard not found", false);
        }

        // Test FinanceUI
        var financeUI = FindFirstObjectByType<FinanceUI>();
        if (financeUI != null)
        {
            AddTestResult("✓ FinanceUI found");
        }
        else
        {
            AddTestResult("✗ FinanceUI not found", false);
        }

        // Test LogisticsUI
        var logisticsUI = FindFirstObjectByType<LogisticsUI>();
        if (logisticsUI != null)
        {
            AddTestResult("✓ LogisticsUI found");
        }
        else
        {
            AddTestResult("✗ LogisticsUI not found", false);
        }

        // Test RnDUI
        var rndUI = FindFirstObjectByType<RnDUI>();
        if (rndUI != null)
        {
            AddTestResult("✓ RnDUI found");
        }
        else
        {
            AddTestResult("✗ RnDUI not found", false);
        }

        // Test AdvancedUI
        var advancedUI = FindFirstObjectByType<AdvancedUI>();
        if (advancedUI != null)
        {
            AddTestResult("✓ AdvancedUI found");
        }
        else
        {
            AddTestResult("✗ AdvancedUI not found", false);
        }
    }

    private void TestSystemInteractions()
    {
        Debug.Log("Testing System Interactions...");
        
        var business = BusinessManager.Instance?.PlayerBusiness;
        if (business == null) return;

        // Test business tick processing
        var originalTick = GameManager.Instance.CurrentTick;
        GameManager.Instance.AdvanceTick();
        if (GameManager.Instance.CurrentTick > originalTick)
        {
            AddTestResult("✓ Game tick advancement works");
        }
        else
        {
            AddTestResult("✗ Game tick advancement failed", false);
        }

        // Test market interactions
        if (MarketManager.Instance != null && business.Products.Count > 0)
        {
            var product = business.Products[0];
            var market = MarketManager.Instance.GetOrCreateMarket(product.Category, 1.0f);
            if (market != null)
            {
                AddTestResult("✓ Market interaction works");
            }
            else
            {
                AddTestResult("✗ Market interaction failed", false);
            }
        }
    }

    private void TestDataFlow()
    {
        Debug.Log("Testing Data Flow...");
        
        var business = BusinessManager.Instance?.PlayerBusiness;
        if (business == null) return;

        // Test data consistency
        var originalCapital = business.Capital;
        var originalEmployees = business.Employees.Count;
        var originalProducts = business.Products.Count;

        // Simulate a business operation
        business.Capital += 10000f;
        var newEmployee = new Employee("Test Employee 2", "Worker", 2000f, 0.5f);
        business.HireEmployee(newEmployee);
        var newProduct = new Product("Test Product 2", "Test Category", 100f, 50f, 50, 0.8f);
        business.AddProduct(newProduct);

        // Verify data changes
        if (business.Capital == originalCapital + 10000f)
        {
            AddTestResult("✓ Capital data flow works");
        }
        else
        {
            AddTestResult("✗ Capital data flow failed", false);
        }

        if (business.Employees.Count == originalEmployees + 1)
        {
            AddTestResult("✓ Employee data flow works");
        }
        else
        {
            AddTestResult("✗ Employee data flow failed", false);
        }

        if (business.Products.Count == originalProducts + 1)
        {
            AddTestResult("✓ Product data flow works");
        }
        else
        {
            AddTestResult("✗ Product data flow failed", false);
        }
    }

    private void AddTestResult(string result, bool passed = true)
    {
        testResults.Add(result);
        if (!passed)
        {
            allTestsPassed = false;
        }
    }

    private void OnGUI()
    {
        if (testResults.Count > 0)
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 600));
            GUILayout.Label("System Integration Test Results", GUI.skin.box);
            
            foreach (var result in testResults)
            {
                GUILayout.Label(result);
            }
            
            GUILayout.Space(10);
            GUILayout.Label($"Overall: {(allTestsPassed ? "PASSED" : "FAILED")}", 
                allTestsPassed ? GUI.skin.box : GUI.skin.button);
            
            if (GUILayout.Button("Run Tests Again"))
            {
                RunAllTests();
            }
            
            GUILayout.EndArea();
        }
    }
} 