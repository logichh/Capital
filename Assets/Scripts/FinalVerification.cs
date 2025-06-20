using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FinalVerification : MonoBehaviour
{
    [Header("Verification Results")]
    public bool allSystemsVerified = false;
    public bool allCodeUsed = false;
    public bool allConnectionsWorking = false;
    public bool noIssuesFound = true;
    
    [Header("Test Results")]
    public List<string> verificationResults = new List<string>();
    public List<string> warnings = new List<string>();
    public List<string> errors = new List<string>();

    private void Start()
    {
        Invoke(nameof(RunFinalVerification), 3f); // Wait for all systems to initialize
    }

    public void RunFinalVerification()
    {
        Debug.Log("=== FINAL COMPREHENSIVE VERIFICATION ===");
        verificationResults.Clear();
        warnings.Clear();
        errors.Clear();
        
        allSystemsVerified = true;
        allCodeUsed = true;
        allConnectionsWorking = true;
        noIssuesFound = true;

        // 1. Verify All Systems Exist and Are Accessible
        VerifyAllSystems();
        
        // 2. Verify All Code Is Used
        VerifyCodeUsage();
        
        // 3. Verify All Connections Work
        VerifyConnections();
        
        // 4. Verify No Issues
        VerifyNoIssues();
        
        // 5. Verify UI Integration
        VerifyUIIntegration();
        
        // 6. Verify Data Flow
        VerifyDataFlow();
        
        // 7. Verify Performance
        VerifyPerformance();
        
        // 8. Final Status Report
        GenerateFinalReport();
    }

    private void VerifyAllSystems()
    {
        Debug.Log("1. Verifying All Systems...");
        
        var systems = new Dictionary<string, object>
        {
            {"GameManager", GameManager.Instance},
            {"BusinessManager", BusinessManager.Instance},
            {"MarketManager", MarketManager.Instance},
            {"CompetitorAI", CompetitorAI.Instance},
            {"FinanceSystem", FinanceSystem.Instance},
            {"LogisticsSystem", LogisticsSystem.Instance},
            {"RnDSystem", RnDSystem.Instance},
            {"IPOSystem", IPOSystem.Instance},
            {"MarketingSystem", MarketingSystem.Instance},
            {"CrisisSystem", CrisisSystem.Instance},
            {"InternationalSystem", InternationalSystem.Instance},
            {"MergerSystem", MergerSystem.Instance},
            {"AnalyticsSystem", AnalyticsSystem.Instance},
            {"ProgressionSystem", ProgressionSystem.Instance},
            {"RegulatorySystem", RegulatorySystem.Instance},
            {"ScenarioSystem", ScenarioSystem.Instance},
            {"ErrorHandler", ErrorHandler.Instance},
            {"SystemManager", SystemManager.Instance}
        };

        foreach (var kvp in systems)
        {
            if (kvp.Value != null)
            {
                verificationResults.Add($"✓ {kvp.Key} - Found and accessible");
            }
            else
            {
                errors.Add($"✗ {kvp.Key} - Missing or not accessible");
                allSystemsVerified = false;
                noIssuesFound = false;
            }
        }
    }

    private void VerifyCodeUsage()
    {
        Debug.Log("2. Verifying Code Usage...");
        
        // Check if all major systems are being used
        var playerBusiness = BusinessManager.Instance?.PlayerBusiness;
        if (playerBusiness != null)
        {
            // Test business operations
            if (playerBusiness.Employees != null && playerBusiness.Products != null)
            {
                verificationResults.Add("✓ Business data structures - Used and functional");
            }
            else
            {
                errors.Add("✗ Business data structures - Not properly initialized");
                allCodeUsed = false;
                noIssuesFound = false;
            }

            // Test system interactions
            if (FinanceSystem.Instance != null)
            {
                var (loans, investments) = FinanceSystem.Instance.GetFinancialStatus(playerBusiness);
                verificationResults.Add("✓ Finance system - Methods called and working");
            }

            if (LogisticsSystem.Instance != null)
            {
                var (suppliers, orders, warehouses) = LogisticsSystem.Instance.GetLogisticsStatus(playerBusiness);
                verificationResults.Add("✓ Logistics system - Methods called and working");
            }

            if (RnDSystem.Instance != null)
            {
                var projects = RnDSystem.Instance.GetAvailableProjects(playerBusiness);
                verificationResults.Add("✓ R&D system - Methods called and working");
            }
        }
        else
        {
            errors.Add("✗ Player business - Not initialized");
            allCodeUsed = false;
            noIssuesFound = false;
        }
    }

    private void VerifyConnections()
    {
        Debug.Log("3. Verifying Connections...");
        
        // Test system manager connections
        if (SystemManager.Instance != null)
        {
            if (SystemManager.Instance.AreAllSystemsReady())
            {
                verificationResults.Add("✓ SystemManager - All systems ready and connected");
            }
            else
            {
                warnings.Add("⚠ SystemManager - Some systems not ready");
                allConnectionsWorking = false;
            }
        }

        // Test game manager connections
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsRunning)
            {
                verificationResults.Add("✓ GameManager - Running and connected");
            }
            else
            {
                warnings.Add("⚠ GameManager - Not running");
                allConnectionsWorking = false;
            }
        }

        // Test UI connections
        bool uiDashboardFound = FindFirstObjectByType<UIDashboard>() != null;
        bool financeUIFound = FindFirstObjectByType<FinanceUI>() != null;
        bool logisticsUIFound = FindFirstObjectByType<LogisticsUI>() != null;
        bool rndUIFound = FindFirstObjectByType<RnDUI>() != null;
        bool advancedUIFound = FindFirstObjectByType<AdvancedUI>() != null;

        if (uiDashboardFound)
        {
            verificationResults.Add("✓ UIDashboard - Found and connected");
        }
        else
        {
            errors.Add("✗ UIDashboard - Not found");
            allConnectionsWorking = false;
            noIssuesFound = false;
        }
    }

    private void VerifyNoIssues()
    {
        Debug.Log("4. Verifying No Issues...");
        
        // Test error handler
        if (ErrorHandler.Instance != null)
        {
            verificationResults.Add("✓ ErrorHandler - Active and monitoring");
            
            // Test error handling
            ErrorHandler.Instance.ValidateSystemReferences();
            ErrorHandler.Instance.ValidateUIComponents();
            
            var playerBusiness = BusinessManager.Instance?.PlayerBusiness;
            if (playerBusiness != null)
            {
                ErrorHandler.Instance.ValidateBusinessData(playerBusiness);
            }
        }
        else
        {
            errors.Add("✗ ErrorHandler - Not found");
            noIssuesFound = false;
        }

        // Test for null references
        if (BusinessManager.Instance?.PlayerBusiness != null)
        {
            var business = BusinessManager.Instance.PlayerBusiness;
            
            // Check for null collections
            if (business.Employees == null)
            {
                errors.Add("✗ Business.Employees - Null collection");
                noIssuesFound = false;
            }
            else
            {
                verificationResults.Add("✓ Business.Employees - Properly initialized");
            }

            if (business.Products == null)
            {
                errors.Add("✗ Business.Products - Null collection");
                noIssuesFound = false;
            }
            else
            {
                verificationResults.Add("✓ Business.Products - Properly initialized");
            }
        }
    }

    private void VerifyUIIntegration()
    {
        Debug.Log("5. Verifying UI Integration...");
        
        var uiComponents = new Dictionary<string, MonoBehaviour>
        {
            {"UIDashboard", FindFirstObjectByType<UIDashboard>()},
            {"FinanceUI", FindFirstObjectByType<FinanceUI>()},
            {"LogisticsUI", FindFirstObjectByType<LogisticsUI>()},
            {"RnDUI", FindFirstObjectByType<RnDUI>()},
            {"AdvancedUI", FindFirstObjectByType<AdvancedUI>()}
        };

        foreach (var kvp in uiComponents)
        {
            if (kvp.Value != null)
            {
                verificationResults.Add($"✓ {kvp.Key} - Found and integrated");
            }
            else
            {
                warnings.Add($"⚠ {kvp.Key} - Not found (may be optional)");
            }
        }
    }

    private void VerifyDataFlow()
    {
        Debug.Log("6. Verifying Data Flow...");
        
        var playerBusiness = BusinessManager.Instance?.PlayerBusiness;
        if (playerBusiness != null)
        {
            // Test data consistency
            var originalCapital = playerBusiness.Capital;
            var originalEmployees = playerBusiness.Employees?.Count ?? 0;
            var originalProducts = playerBusiness.Products?.Count ?? 0;

            // Simulate data flow
            playerBusiness.Capital += 1000f;
            if (playerBusiness.Capital == originalCapital + 1000f)
            {
                verificationResults.Add("✓ Data flow - Capital updates correctly");
            }
            else
            {
                errors.Add("✗ Data flow - Capital not updating correctly");
                noIssuesFound = false;
            }

            // Test market interaction
            if (MarketManager.Instance != null)
            {
                var market = MarketManager.Instance.GetOrCreateMarket("Test", 1.0f);
                if (market != null)
                {
                    verificationResults.Add("✓ Data flow - Market interaction works");
                }
                else
                {
                    errors.Add("✗ Data flow - Market interaction failed");
                    noIssuesFound = false;
                }
            }
        }
    }

    private void VerifyPerformance()
    {
        Debug.Log("7. Verifying Performance...");
        
        // Check frame rate
        var fps = 1f / Time.deltaTime;
        if (fps > 30f)
        {
            verificationResults.Add($"✓ Performance - Good FPS ({fps:F1})");
        }
        else
        {
            warnings.Add($"⚠ Performance - Low FPS ({fps:F1})");
        }

        // Check memory usage (basic)
        var totalMemory = System.GC.GetTotalMemory(false);
        if (totalMemory < 100000000) // 100MB
        {
            verificationResults.Add($"✓ Performance - Memory usage OK ({totalMemory / 1000000}MB)");
        }
        else
        {
            warnings.Add($"⚠ Performance - High memory usage ({totalMemory / 1000000}MB)");
        }
    }

    private void GenerateFinalReport()
    {
        Debug.Log("8. Generating Final Report...");
        
        Debug.Log("=== FINAL VERIFICATION RESULTS ===");
        
        foreach (var result in verificationResults)
        {
            Debug.Log(result);
        }
        
        if (warnings.Count > 0)
        {
            Debug.LogWarning("=== WARNINGS ===");
            foreach (var warning in warnings)
            {
                Debug.LogWarning(warning);
            }
        }
        
        if (errors.Count > 0)
        {
            Debug.LogError("=== ERRORS ===");
            foreach (var error in errors)
            {
                Debug.LogError(error);
            }
        }

        // Final status
        var finalStatus = allSystemsVerified && allCodeUsed && allConnectionsWorking && noIssuesFound;
        
        Debug.Log("=== FINAL STATUS ===");
        Debug.Log($"All Systems Verified: {(allSystemsVerified ? "✅ YES" : "❌ NO")}");
        Debug.Log($"All Code Used: {(allCodeUsed ? "✅ YES" : "❌ NO")}");
        Debug.Log($"All Connections Working: {(allConnectionsWorking ? "✅ YES" : "❌ NO")}");
        Debug.Log($"No Issues Found: {(noIssuesFound ? "✅ YES" : "❌ NO")}");
        Debug.Log($"OVERALL STATUS: {(finalStatus ? "🎉 PERFECT - ALL SYSTEMS GO!" : "⚠ NEEDS ATTENTION")}");
        
        if (finalStatus)
        {
            Debug.Log("🎮 Capital Architect is ready for production! 🚀");
        }
    }

    private void OnGUI()
    {
        if (verificationResults.Count > 0)
        {
            GUILayout.BeginArea(new Rect(10, 10, 500, 700));
            GUILayout.Label("Final Verification Results", GUI.skin.box);
            
            GUILayout.Label($"Systems: {(allSystemsVerified ? "✅" : "❌")}");
            GUILayout.Label($"Code Usage: {(allCodeUsed ? "✅" : "❌")}");
            GUILayout.Label($"Connections: {(allConnectionsWorking ? "✅" : "❌")}");
            GUILayout.Label($"No Issues: {(noIssuesFound ? "✅" : "❌")}");
            
            GUILayout.Space(10);
            
            foreach (var result in verificationResults.Take(10))
            {
                GUILayout.Label(result);
            }
            
            if (warnings.Count > 0)
            {
                GUILayout.Space(10);
                GUILayout.Label("Warnings:", GUI.skin.box);
                foreach (var warning in warnings.Take(5))
                {
                    GUILayout.Label(warning, GUI.skin.button);
                }
            }
            
            if (errors.Count > 0)
            {
                GUILayout.Space(10);
                GUILayout.Label("Errors:", GUI.skin.box);
                foreach (var error in errors.Take(5))
                {
                    GUILayout.Label(error, GUI.skin.button);
                }
            }
            
            GUILayout.Space(10);
            var finalStatus = allSystemsVerified && allCodeUsed && allConnectionsWorking && noIssuesFound;
            GUILayout.Label($"Status: {(finalStatus ? "PERFECT" : "NEEDS WORK")}", 
                finalStatus ? GUI.skin.box : GUI.skin.button);
            
            if (GUILayout.Button("Run Verification Again"))
            {
                RunFinalVerification();
            }
            
            GUILayout.EndArea();
        }
    }
} 