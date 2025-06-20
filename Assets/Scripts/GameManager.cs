using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Simulation tick interval (e.g., 1 day per tick)
    [SerializeField] private float tickInterval = 1.0f; // seconds per tick for now
    private float tickTimer = 0f;
    public int CurrentTick { get; private set; } = 0;
    public bool IsRunning { get; private set; } = true;

    private UIDashboard uiDashboard;
    private FinanceUI financeUI;
    private AdvancedUI advancedUI;
    private BusinessSetupUI businessSetupUI;

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
        uiDashboard = FindFirstObjectByType<UIDashboard>();
        financeUI = FindFirstObjectByType<FinanceUI>();
        advancedUI = FindFirstObjectByType<AdvancedUI>();
        businessSetupUI = FindFirstObjectByType<BusinessSetupUI>();
        
        // Initialize all systems
        InitializeAllSystems();

        // Show setup panel if present, hide main game panel
        if (businessSetupUI != null)
        {
            if (businessSetupUI.setupPanel != null) businessSetupUI.setupPanel.SetActive(true);
            if (businessSetupUI.mainGamePanel != null) businessSetupUI.mainGamePanel.SetActive(false);
        }
        else
        {
            // Fallback: Create player business if not already present
            if (BusinessManager.Instance != null && BusinessManager.Instance.PlayerBusiness == null)
            {
                BusinessManager.Instance.CreatePlayerBusiness("My Company", "Tech", "USA", 100000f);
            }
        }
        
        UpdateDashboard();
    }

    private void InitializeAllSystems()
    {
        // Initialize R&D system
        if (RnDSystem.Instance != null)
        {
            RnDSystem.Instance.InitializeResearchProjects();
        }

        // Initialize progression system for all businesses
        if (ProgressionSystem.Instance != null && BusinessManager.Instance?.Businesses != null)
        {
            foreach (var business in BusinessManager.Instance.Businesses)
            {
                if (business != null)
                {
                    ProgressionSystem.Instance.InitializeProgression(business);
                }
            }
        }

        // Initialize regulatory system for all businesses
        if (RegulatorySystem.Instance != null && BusinessManager.Instance?.Businesses != null)
        {
            foreach (var business in BusinessManager.Instance.Businesses)
            {
                if (business != null)
                {
                    // RegulatorySystem.Instance.InitializeRegulations(business);
                }
            }
        }

        // Generate acquisition targets for M&A system
        if (MergerSystem.Instance != null && BusinessManager.Instance?.Businesses != null)
        {
            foreach (var business in BusinessManager.Instance.Businesses)
            {
                if (business != null)
                {
                    // MergerSystem.Instance.GenerateAcquisitionTargets(business);
                }
            }
        }

        // Initialize marketing system
        if (MarketingSystem.Instance != null)
        {
            // Marketing system auto-initializes
        }

        // Initialize crisis system
        if (CrisisSystem.Instance != null)
        {
            // Crisis system auto-initializes
        }

        // Initialize international system
        if (InternationalSystem.Instance != null)
        {
            // International system auto-initializes
        }

        // Initialize IPO system
        if (IPOSystem.Instance != null)
        {
            // IPO system auto-initializes
        }

        // Initialize analytics system
        if (AnalyticsSystem.Instance != null)
        {
            // Analytics system auto-initializes
        }

        // Initialize scenario system
        if (ScenarioSystem.Instance != null)
        {
            // Scenario system auto-initializes
        }
    }

    private void Update()
    {
        if (!IsRunning) return;
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer -= tickInterval;
            AdvanceTick();
        }
    }

    public void AdvanceTick()
    {
        CurrentTick++;
        string notification = null;
        
        var playerBusiness = BusinessManager.Instance?.PlayerBusiness;
        if (playerBusiness == null) return;

        // --- PLACEHOLDER SIMULATION LOGIC ---
        // Simulate revenue and expenses
        float revenueThisTick = Random.Range(800f, 1200f);
        float expensesThisTick = Random.Range(400f, 700f);
        playerBusiness.Revenue += revenueThisTick;
        playerBusiness.TotalExpenses += expensesThisTick;
        playerBusiness.Capital += (revenueThisTick - expensesThisTick);
        // Simulate market share
        playerBusiness.MarketShare = Mathf.Clamp01(playerBusiness.MarketShare + Random.Range(-0.01f, 0.02f));
        // Simulate cash (same as capital for now)
        // Simulate credit score
        playerBusiness.CreditScore = Mathf.Clamp(playerBusiness.CreditScore + Random.Range(-2f, 3f), 300f, 850f);
        // --- END PLACEHOLDER LOGIC ---

        // Process all systems in order
        ProcessFinancialSystems(playerBusiness, ref notification);
        ProcessOperationalSystems(playerBusiness, ref notification);
        ProcessAdvancedSystems(playerBusiness, ref notification);
        ProcessBusinessLogic(playerBusiness, ref notification);
        ProcessWinLossConditions(playerBusiness, ref notification);

        // Update all managers
        if (MarketManager.Instance != null)
        {
            MarketManager.Instance.UpdateMarkets();
        }
        
        Debug.Log($"Tick {CurrentTick}");
        UpdateDashboard();
        
        if (uiDashboard != null && !string.IsNullOrEmpty(notification))
        {
            uiDashboard.ShowNotification(notification);
        }

        // Update advanced UI if available
        if (advancedUI != null)
        {
            advancedUI.UpdateAllUI();
        }
    }

    private void ProcessFinancialSystems(Business playerBusiness, ref string notification)
    {
        // Process finances first
        if (FinanceSystem.Instance != null)
        {
            FinanceSystem.Instance.ProcessFinances(playerBusiness);
            
            // Calculate and deduct taxes (every 30 ticks = monthly)
            if (CurrentTick % 30 == 0)
            {
                float expenses = playerBusiness.Employees.Sum(e => e.Wage) + 
                               playerBusiness.Products.Sum(p => p.Cost * p.Inventory);
                float taxes = FinanceSystem.Instance.CalculateTaxes(playerBusiness, playerBusiness.Revenue, expenses);
                playerBusiness.Capital -= taxes;
                notification = $"Monthly tax payment: ${taxes:N0}";
                
                // Reset monthly financials
                playerBusiness.UpdateMonthlyFinancials();
            }
        }

        // Process IPO system
        if (IPOSystem.Instance != null)
        {
            // IPOSystem.Instance.ProcessStockMarket();
            // IPOSystem.Instance.PayBoardSalaries(playerBusiness);
        }
    }

    private void ProcessOperationalSystems(Business playerBusiness, ref string notification)
    {
        // Process logistics
        if (LogisticsSystem.Instance != null)
        {
            LogisticsSystem.Instance.ProcessLogistics();
            
            // Check for low inventory warnings
            var (_, _, warehouses) = LogisticsSystem.Instance.GetLogisticsStatus(playerBusiness);
            foreach (var warehouse in warehouses)
            {
                foreach (var kvp in warehouse.Inventory)
                {
                    if (kvp.Value < 10) // Low inventory threshold
                    {
                        string warningMsg = $"Low inventory warning: {kvp.Key} ({kvp.Value} units)";
                        notification = notification != null ? 
                            $"{notification}\n{warningMsg}" : warningMsg;
                    }
                }
            }
        }

        // Process R&D
        if (RnDSystem.Instance != null)
        {
            RnDSystem.Instance.ProcessResearch();
            
            // Check for completed research
            var (activeProjects, _, _) = RnDSystem.Instance.GetRnDStatus(playerBusiness);
            if (activeProjects.Count == 0 && playerBusiness.CompletedResearch.Count > 0)
            {
                string researchMsg = $"Research completed! Total completed: {playerBusiness.CompletedResearch.Count}";
                notification = notification != null ? 
                    $"{notification}\n{researchMsg}" : researchMsg;
            }
        }

        // Process marketing
        if (MarketingSystem.Instance != null)
        {
            MarketingSystem.Instance.ProcessMarketing();
        }

        // Process international operations
        if (InternationalSystem.Instance != null)
        {
            InternationalSystem.Instance.ProcessInternationalOperations();
        }
    }

    private void ProcessAdvancedSystems(Business playerBusiness, ref string notification)
    {
        // Process crisis management
        if (CrisisSystem.Instance != null)
        {
            CrisisSystem.Instance.ProcessCrises();
            CrisisSystem.Instance.CheckForNewCrises(playerBusiness);
        }

        // Process merger & acquisition
        if (MergerSystem.Instance != null)
        {
            // Logic for M&A processing can be added here
        }

        // Process regulatory compliance
        if (RegulatorySystem.Instance != null)
        {
            RegulatorySystem.Instance.CheckCompliance(playerBusiness);
        }

        // Process analytics
        if (AnalyticsSystem.Instance != null)
        {
            // Analytics are mostly event-driven, but you could have a periodic update
        }

        // Process scenarios
        if (ScenarioSystem.Instance != null)
        {
            ScenarioSystem.Instance.ProcessScenarios();
        }

        // Process progression
        if (ProgressionSystem.Instance != null)
        {
            ProgressionSystem.Instance.UpdateAchievements(playerBusiness);
            ProgressionSystem.Instance.UpdatePrestigeLevels(playerBusiness);
        }
    }

    private void ProcessBusinessLogic(Business playerBusiness, ref string notification)
    {
        // Simulate competitor AI
        if (CompetitorAI.Instance != null)
        {
            CompetitorAI.Instance.SimulateCompetitors();
        }

        // Deduct wages
        float totalWages = 0f;
        foreach (var employee in playerBusiness.Employees)
        {
            totalWages += employee.Wage;
        }
        playerBusiness.Capital -= totalWages;

        // If can't pay, reduce morale or fire employees
        if (playerBusiness.Capital < 0 && playerBusiness.Employees.Count > 0)
        {
            var fired = playerBusiness.Employees[0];
            playerBusiness.FireEmployee(fired);
            notification = notification != null ? 
                $"{notification}\nCould not pay wages! {fired.Name} was fired." :
                $"Could not pay wages! {fired.Name} was fired.";
        }

        // Simulate production and sales with market
        foreach (var product in playerBusiness.Products)
        {
            playerBusiness.ProduceProduct(product, 10);
            var market = MarketManager.Instance.GetOrCreateMarket(product.Category, 1.0f);
            
            // Update market supply and demand
            float qualityMultiplier = Mathf.Clamp01(product.Quality);
            int baseDemand = Mathf.RoundToInt(8 + Random.Range(-2, 3));
            int demand = Mathf.RoundToInt(baseDemand * (1f + 0.5f * (qualityMultiplier - 1f)));
            market.TotalSupply += product.Inventory;
            market.TotalDemand += demand;
            market.UpdateMarketPrice();
            
            // Sell up to market demand, at market price
            product.Price = market.MarketPrice;
            playerBusiness.SellProduct(product, demand);
            
            // Reset for next tick
            market.TotalSupply = 0;
            market.TotalDemand = 0;
        }

        // Apply a random event
        string eventNotification = TriggerRandomEvent();
        if (!string.IsNullOrEmpty(eventNotification))
        {
            notification = notification != null ? $"{notification}\n{eventNotification}" : eventNotification;
        }
    }

    private void ProcessWinLossConditions(Business playerBusiness, ref string notification)
    {
        // Win/loss conditions
        if (playerBusiness.Capital < -50000f) // Allow some debt before bankruptcy
        {
            notification = "Game Over: Bankruptcy!";
            IsRunning = false;
        }
        else if (playerBusiness.Capital > 1000000f)
        {
            notification = "Congratulations! You built a business empire!";
            IsRunning = false;
        }
        else if (playerBusiness.MarketShare > 0.5f)
        {
            notification = "Congratulations! You dominate the market!";
            IsRunning = false;
        }
        else if (playerBusiness.Employees.Count == 0 && playerBusiness.Products.Count == 0)
        {
            notification = "Game Over: No employees or products left!";
            IsRunning = false;
        }
    }

    private string TriggerRandomEvent()
    {
        var playerBusiness = BusinessManager.Instance?.PlayerBusiness;
        if (playerBusiness == null) return null;

        float roll = Random.value;
        if (roll < 0.15f)
        {
            var boom = new GameEvent("Boom", "Economic boom! Extra cash this tick.", 20000f);
            boom.ApplyImpact(playerBusiness);
            return "Event: Economic boom! +$20,000";
        }
        else if (roll < 0.3f)
        {
            var crisis = new GameEvent("Crisis", "Economic crisis! Lose cash this tick.", -15000f);
            crisis.ApplyImpact(playerBusiness);
            return "Event: Economic crisis! -$15,000";
        }
        else if (roll < 0.45f)
        {
            var strike = new GameEvent("Strike", "Employee strike! Morale drops.", 0f);
            strike.ApplyImpact(playerBusiness);
            return "Event: Employee strike! Morale drops.";
        }
        else if (roll < 0.6f)
        {
            var tech = new GameEvent("TechBreakthrough", "Tech breakthrough! Production costs drop.", 0f);
            tech.ApplyImpact(playerBusiness);
            return "Event: Tech breakthrough! Production costs drop.";
        }
        else if (roll < 0.75f)
        {
            var policy = new GameEvent("PolicyChange", "Favorable policy! Gain cash.", 10000f);
            policy.ApplyImpact(playerBusiness);
            return "Event: Favorable policy! +$10,000";
        }
        else if (roll < 0.85f)
        {
            // Supply chain disruption event
            var disruption = new GameEvent("SupplyChainDisruption", "Supply chain disruption! Delivery times increased.", 0f);
            disruption.ApplyImpact(playerBusiness);
            return "Event: Supply chain disruption! Deliveries delayed.";
        }
        else
        {
            // Research breakthrough event
            var breakthrough = new GameEvent("ResearchBreakthrough", "Research breakthrough! All research progress increased.", 0f);
            breakthrough.ApplyImpact(playerBusiness);
            return "Event: Research breakthrough! Research progress boosted.";
        }
    }

    private void UpdateDashboard()
    {
        if (uiDashboard == null || BusinessManager.Instance?.PlayerBusiness == null) return;
        
        var business = BusinessManager.Instance.PlayerBusiness;
        float revenue = business.Revenue;
        float expenses = business.TotalExpenses;
        float marketShare = business.MarketShare;
        float cash = business.Capital;

        if (uiDashboard != null)
        {
            uiDashboard.UpdateKPIs(revenue, expenses, marketShare, cash, CurrentTick);

            if (IPOSystem.Instance != null && business.IsPublic)
            {
                // This will be updated with a proper method later
            }
        }
    }

    private void CheckForNotifications()
    {
        // This method can be expanded with more complex notification logic
    }

    // Example of a new method that might be needed
    public void ResetGame()
    {
        // Reset game state
        CurrentTick = 0;
        // BusinessManager.Instance.ResetBusinesses();
        // MarketManager.Instance.ResetMarkets();
        // etc.
        InitializeAllSystems();
        UpdateDashboard();
    }

    public void PauseGame() => IsRunning = false;
    public void ResumeGame() => IsRunning = true;
} 