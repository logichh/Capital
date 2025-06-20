using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class AdvancedUI : MonoBehaviour
{
    public static AdvancedUI Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject ipoPanel;
    public GameObject marketingPanel;
    public GameObject crisisPanel;
    public GameObject internationalPanel;
    public GameObject mergerPanel;
    public GameObject analyticsPanel;
    public GameObject progressionPanel;
    public GameObject regulatoryPanel;
    public GameObject scenarioPanel;

    [Header("Navigation")]
    public Button[] systemButtons;
    public TextMeshProUGUI currentSystemText;

    [Header("IPO UI")]
    public TextMeshProUGUI ipoStatusText;
    public TextMeshProUGUI stockPriceText;
    public TextMeshProUGUI marketCapText;
    public Button goPublicButton;
    public Button buybackButton;
    public InputField sharePriceInput;
    public InputField sharesInput;

    [Header("Marketing UI")]
    public TextMeshProUGUI brandReputationText;
    public TextMeshProUGUI customerSatisfactionText;
    public Button launchCampaignButton;
    public Button improveServiceButton;
    public Button socialResponsibilityButton;
    public InputField campaignBudgetInput;
    public Dropdown campaignTypeDropdown;

    [Header("Crisis UI")]
    public TextMeshProUGUI crisisStatusText;
    public TextMeshProUGUI resistanceText;
    public Button respondButton;
    public Button preventionButton;
    public InputField responseBudgetInput;
    public Dropdown responseTypeDropdown;

    [Header("International UI")]
    public TextMeshProUGUI internationalStatusText;
    public TextMeshProUGUI exchangeRatesText;
    public Button expandButton;
    public Button createSubsidiaryButton;
    public InputField expansionCostInput;
    public Dropdown countryDropdown;

    [Header("Merger UI")]
    public TextMeshProUGUI mergerStatusText;
    public TextMeshProUGUI targetsText;
    public Button initiateAcquisitionButton;
    public Button conductDiligenceButton;
    public InputField offerPriceInput;
    public Dropdown targetDropdown;

    [Header("Analytics UI")]
    public TextMeshProUGUI performanceText;
    public TextMeshProUGUI forecastText;
    public TextMeshProUGUI healthScoreText;
    public Button generateReportButton;
    public Button updateMetricsButton;

    [Header("Progression UI")]
    public TextMeshProUGUI achievementText;
    public TextMeshProUGUI prestigeText;
    public TextMeshProUGUI progressText;
    public Button unlockFeatureButton;
    public Dropdown featureDropdown;

    [Header("Regulatory UI")]
    public TextMeshProUGUI complianceText;
    public TextMeshProUGUI auditText;
    public Button improveComplianceButton;
    public Button requestWaiverButton;
    public InputField complianceInvestmentInput;

    [Header("Scenario UI")]
    public TextMeshProUGUI scenarioText;
    public TextMeshProUGUI challengeText;
    public Button startScenarioButton;
    public Button acceptChallengeButton;
    public Dropdown scenarioDropdown;

    private Business currentBusiness;
    private string currentSystem = "Main";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        currentBusiness = BusinessManager.Instance?.PlayerBusiness;
        SetupUI();
        UpdateAllUI();
    }

    private void SetupUI()
    {
        // Setup navigation buttons
        for (int i = 0; i < systemButtons.Length; i++)
        {
            int index = i; // Capture for lambda
            systemButtons[i].onClick.AddListener(() => SwitchSystem(index));
        }

        // Setup IPO UI
        goPublicButton.onClick.AddListener(GoPublic);
        buybackButton.onClick.AddListener(BuybackShares);

        // Setup Marketing UI
        launchCampaignButton.onClick.AddListener(LaunchCampaign);
        improveServiceButton.onClick.AddListener(ImproveCustomerService);
        socialResponsibilityButton.onClick.AddListener(InvestInSocialResponsibility);

        // Setup Crisis UI
        respondButton.onClick.AddListener(RespondToCrisis);
        preventionButton.onClick.AddListener(ImproveCrisisPrevention);

        // Setup International UI
        expandButton.onClick.AddListener(ExpandToMarket);
        createSubsidiaryButton.onClick.AddListener(CreateSubsidiary);

        // Setup Merger UI
        initiateAcquisitionButton.onClick.AddListener(InitiateAcquisition);
        conductDiligenceButton.onClick.AddListener(ConductDueDiligence);

        // Setup Analytics UI
        generateReportButton.onClick.AddListener(GenerateAnalyticsReport);
        updateMetricsButton.onClick.AddListener(UpdateAnalyticsMetrics);

        // Setup Progression UI
        unlockFeatureButton.onClick.AddListener(UnlockFeature);

        // Setup Regulatory UI
        improveComplianceButton.onClick.AddListener(ImproveCompliance);
        requestWaiverButton.onClick.AddListener(RequestWaiver);

        // Setup Scenario UI
        startScenarioButton.onClick.AddListener(StartScenario);
        acceptChallengeButton.onClick.AddListener(AcceptChallenge);

        // Setup dropdowns
        SetupDropdowns();
    }

    private void SetupDropdowns()
    {
        // Campaign types
        campaignTypeDropdown.ClearOptions();
        campaignTypeDropdown.AddOptions(new List<string> { "TV", "Digital", "Print", "Social", "Influencer" });

        // Response types
        responseTypeDropdown.ClearOptions();
        responseTypeDropdown.AddOptions(new List<string> { "Apology", "Compensation", "Investigation", "LegalDefense", "Rebranding" });

        // Countries
        countryDropdown.ClearOptions();
        if (InternationalSystem.Instance != null)
        {
            countryDropdown.AddOptions(InternationalSystem.Instance.GetAvailableCountries());
        }

        // Scenarios
        scenarioDropdown.ClearOptions();
        if (ScenarioSystem.Instance != null)
        {
            scenarioDropdown.AddOptions(ScenarioSystem.Instance.GetAvailableScenarios());
        }
    }

    public void SwitchSystem(int systemIndex)
    {
        string[] systems = { "Main", "IPO", "Marketing", "Crisis", "International", "Merger", "Analytics", "Progression", "Regulatory", "Scenario" };
        currentSystem = systems[systemIndex];
        currentSystemText.text = currentSystem;

        // Hide all panels
        mainPanel.SetActive(false);
        ipoPanel.SetActive(false);
        marketingPanel.SetActive(false);
        crisisPanel.SetActive(false);
        internationalPanel.SetActive(false);
        mergerPanel.SetActive(false);
        analyticsPanel.SetActive(false);
        progressionPanel.SetActive(false);
        regulatoryPanel.SetActive(false);
        scenarioPanel.SetActive(false);

        // Show current panel
        switch (currentSystem)
        {
            case "Main": mainPanel.SetActive(true); break;
            case "IPO": ipoPanel.SetActive(true); break;
            case "Marketing": marketingPanel.SetActive(true); break;
            case "Crisis": crisisPanel.SetActive(true); break;
            case "International": internationalPanel.SetActive(true); break;
            case "Merger": mergerPanel.SetActive(true); break;
            case "Analytics": analyticsPanel.SetActive(true); break;
            case "Progression": progressionPanel.SetActive(true); break;
            case "Regulatory": regulatoryPanel.SetActive(true); break;
            case "Scenario": scenarioPanel.SetActive(true); break;
        }

        UpdateCurrentSystemUI();
    }

    public void UpdateAllUI()
    {
        if (currentBusiness == null) return;

        UpdateIPOUI();
        UpdateMarketingUI();
        UpdateCrisisUI();
        UpdateInternationalUI();
        UpdateMergerUI();
        UpdateAnalyticsUI();
        UpdateProgressionUI();
        UpdateRegulatoryUI();
        UpdateScenarioUI();
    }

    private void UpdateCurrentSystemUI()
    {
        switch (currentSystem)
        {
            case "IPO": UpdateIPOUI(); break;
            case "Marketing": UpdateMarketingUI(); break;
            case "Crisis": UpdateCrisisUI(); break;
            case "International": UpdateInternationalUI(); break;
            case "Merger": UpdateMergerUI(); break;
            case "Analytics": UpdateAnalyticsUI(); break;
            case "Progression": UpdateProgressionUI(); break;
            case "Regulatory": UpdateRegulatoryUI(); break;
            case "Scenario": UpdateScenarioUI(); break;
        }
    }

    private void UpdateIPOUI()
    {
        if (currentBusiness == null || IPOSystem.Instance == null) return;

        bool isPublic = currentBusiness.IsPublic;
        ipoStatusText.text = $"Status: {(isPublic ? "Public" : "Private")}";
        stockPriceText.text = isPublic ? $"Stock Price: ${currentBusiness.StockPrice:F2}" : "Stock Price: N/A";
        marketCapText.text = isPublic ? $"Market Cap: ${currentBusiness.StockPrice * 1000000:N0}" : "Market Cap: N/A"; // Assuming 1M shares
        goPublicButton.interactable = !isPublic && IPOSystem.Instance.CanGoPublic(currentBusiness);
        buybackButton.interactable = isPublic;
    }

    private void UpdateMarketingUI()
    {
        if (currentBusiness == null || MarketingSystem.Instance == null) return;
        brandReputationText.text = $"Brand Reputation: {currentBusiness.Reputation:F1}/100";
        customerSatisfactionText.text = $"Customer Satisfaction: {currentBusiness.CustomerSatisfaction:F1}/100";
    }

    private void UpdateCrisisUI()
    {
        if (currentBusiness == null || CrisisSystem.Instance == null) return;
        var (active, resolved, resistance) = CrisisSystem.Instance.GetCrisisStatus(currentBusiness);
        crisisStatusText.text = $"Active Crises: {active.Count}";
        resistanceText.text = $"Crisis Resistance: {resistance:F1}%";
    }

    private void UpdateInternationalUI()
    {
        if (currentBusiness == null || InternationalSystem.Instance == null) return;
        var (markets, subsidiaries, operations) = InternationalSystem.Instance.GetInternationalStatus(currentBusiness);
        internationalStatusText.text = $"International Markets: {markets.Count} | Subsidiaries: {subsidiaries.Count}";
        exchangeRatesText.text = "Exchange Rates: N/A"; // Placeholder
    }

    private void UpdateMergerUI()
    {
        if (currentBusiness == null || MergerSystem.Instance == null) return;
        
        int potentialTargets = BusinessManager.Instance.Businesses.Count(b => b != currentBusiness && MergerSystem.Instance.CanAcquire(currentBusiness, b));
        mergerStatusText.text = $"Completed Mergers: {currentBusiness.MergerExperience}";
        targetsText.text = $"Potential Targets: {potentialTargets}";
    }

    private void UpdateAnalyticsUI()
    {
        if (currentBusiness == null || AnalyticsSystem.Instance == null) return;
        var (reports, analyses, metrics, forecasts) = AnalyticsSystem.Instance.GetAnalyticsStatus(currentBusiness);
        performanceText.text = $"Reports: {reports.Count} | Analyses: {analyses.Count}";
        forecastText.text = forecasts.Count > 0 ? $"Forecast: {forecasts[0]}" : "Forecast: N/A";
        healthScoreText.text = $"Business Health Score: {AnalyticsSystem.Instance.GetBusinessHealthScore(currentBusiness):F2}";
    }

    private void UpdateProgressionUI()
    {
        if (currentBusiness == null || ProgressionSystem.Instance == null) return;
        var (achievements, prestige, unlockables, bonuses) = ProgressionSystem.Instance.GetProgressionStatus(currentBusiness);
        achievementText.text = $"Achievements: {achievements.Count(a => a.IsCompleted)} / {achievements.Count}";
        prestigeText.text = $"Prestige Level: {currentBusiness.PrestigeLevel}";
        progressText.text = $"Progress: {ProgressionSystem.Instance.GetProgressPercentage(currentBusiness):P1}";
    }

    private void UpdateRegulatoryUI()
    {
        if (currentBusiness == null || RegulatorySystem.Instance == null) return;
        bool isCompliant = RegulatorySystem.Instance.IsCompliant(currentBusiness);
        complianceText.text = $"Compliance Status: {(isCompliant ? "Compliant" : "Non-Compliant")}";
        auditText.text = $"Compliance Score: {currentBusiness.ComplianceScore:F1}";
    }

    private void UpdateScenarioUI()
    {
        if (currentBusiness == null || ScenarioSystem.Instance == null) return;

        var (scenario, challenges, events, completed) = ScenarioSystem.Instance.GetScenarioStatus(currentBusiness);

        scenarioText.text = scenario != null ? $"Active: {scenario.Name}" : "No active scenario";
        challengeText.text = $"Active Challenges: {challenges.Count}";
    }

    // UI Action Methods
    private void GoPublic()
    {
        if (currentBusiness != null && IPOSystem.Instance != null)
        {
            IPOSystem.Instance.GoPublic(currentBusiness);
            UpdateIPOUI();
        }
    }

    private void BuybackShares()
    {
        if (currentBusiness != null && IPOSystem.Instance != null && float.TryParse(sharesInput.text, out float shares))
        {
            // IPOSystem.Instance.BuyBackShares(currentBusiness, (int)shares); // This method does not exist
            Debug.Log("Buyback shares functionality not implemented.");
            UpdateIPOUI();
        }
    }

    private void LaunchCampaign()
    {
        if (currentBusiness != null && MarketingSystem.Instance != null && float.TryParse(campaignBudgetInput.text, out float budget))
        {
            string type = campaignTypeDropdown.options[campaignTypeDropdown.value].text;
            MarketingSystem.Instance.LaunchCampaign(currentBusiness, $"Campaign-{type}", type, budget, 30, "General");
            UpdateMarketingUI();
        }
    }

    private void ImproveCustomerService()
    {
        if (currentBusiness != null && MarketingSystem.Instance != null)
        {
            MarketingSystem.Instance.ImproveCustomerService(currentBusiness, 10000f); // Example cost
            UpdateMarketingUI();
        }
    }

    private void InvestInSocialResponsibility()
    {
        if (currentBusiness != null && MarketingSystem.Instance != null)
        {
            MarketingSystem.Instance.InvestInSocialResponsibility(currentBusiness, 20000f); // Example cost
            UpdateMarketingUI();
        }
    }

    private void RespondToCrisis()
    {
        if (currentBusiness != null && CrisisSystem.Instance != null && float.TryParse(responseBudgetInput.text, out float budget))
        {
            string type = responseTypeDropdown.options[responseTypeDropdown.value].text;
            CrisisSystem.Instance.RespondToCrisis(currentBusiness, type, budget);
            UpdateCrisisUI();
        }
    }

    private void ImproveCrisisPrevention()
    {
        if (currentBusiness != null && CrisisSystem.Instance != null)
        {
            CrisisSystem.Instance.ImproveCrisisPrevention(currentBusiness, 50000f); // Example cost
            UpdateCrisisUI();
        }
    }

    private void ExpandToMarket()
    {
        if (currentBusiness != null && InternationalSystem.Instance != null)
        {
            string country = countryDropdown.options[countryDropdown.value].text;
            InternationalSystem.Instance.ExpandToMarket(currentBusiness, country);
            UpdateInternationalUI();
        }
    }

    private void CreateSubsidiary()
    {
        if (currentBusiness != null && InternationalSystem.Instance != null)
        {
            string country = countryDropdown.options[countryDropdown.value].text;
            InternationalSystem.Instance.CreateSubsidiary(currentBusiness, country, "New Subsidiary", 500000f);
            UpdateInternationalUI();
        }
    }

    private void InitiateAcquisition()
    {
        if (currentBusiness != null && MergerSystem.Instance != null)
        {
            string targetName = targetDropdown.options[targetDropdown.value].text;
            var target = BusinessManager.Instance.Businesses.Find(b => b.Name == targetName);
            if(target != null)
            {
                MergerSystem.Instance.Acquire(currentBusiness, target);
                UpdateMergerUI();
            }
        }
    }

    private void ConductDueDiligence()
    {
        if (currentBusiness != null && MergerSystem.Instance != null)
        {
            // This functionality does not exist in MergerSystem
            Debug.Log("Due diligence functionality not implemented.");
            UpdateMergerUI();
        }
    }

    private void GenerateAnalyticsReport()
    {
        if (currentBusiness != null && AnalyticsSystem.Instance != null)
        {
            AnalyticsSystem.Instance.GetAnalyticsStatus(currentBusiness); // Doesn't generate, just gets status
            Debug.Log("Analytics status refreshed.");
            UpdateAnalyticsUI();
        }
    }

    private void UpdateAnalyticsMetrics()
    {
        if (currentBusiness != null && AnalyticsSystem.Instance != null)
        {
            // This functionality does not exist in AnalyticsSystem
            Debug.Log("Update metrics functionality not implemented.");
            UpdateAnalyticsUI();
        }
    }

    private void UnlockFeature()
    {
        if (currentBusiness != null && ProgressionSystem.Instance != null)
        {
            string feature = featureDropdown.options[featureDropdown.value].text;
            ProgressionSystem.Instance.UnlockFeature(currentBusiness, feature);
            UpdateProgressionUI();
        }
    }

    private void ImproveCompliance()
    {
        if (currentBusiness != null && float.TryParse(complianceInvestmentInput.text, out float investment))
        {
            currentBusiness.ImproveCompliance(investment);
            UpdateRegulatoryUI();
        }
    }

    private void RequestWaiver()
    {
        if (currentBusiness != null && RegulatorySystem.Instance != null)
        {
            // This functionality does not exist in RegulatorySystem
            Debug.Log("Request waiver functionality not implemented.");
            UpdateRegulatoryUI();
        }
    }

    private void StartScenario()
    {
        string scenarioName = scenarioDropdown.options[scenarioDropdown.value].text;
        ScenarioSystem.Instance.StartScenario(currentBusiness, scenarioName);
        UpdateScenarioUI();
    }

    private void AcceptChallenge()
    {
        // Challenges are automatically generated and accepted
        UpdateScenarioUI();
    }
} 