using UnityEngine;
using System;
using System.Collections.Generic;

public class ErrorHandler : MonoBehaviour
{
    public static ErrorHandler Instance { get; private set; }

    [Header("Error Handling Settings")]
    public bool enableErrorLogging = true;
    public bool enableExceptionHandling = true;
    public bool showErrorUI = true;
    
    [Header("Error UI")]
    public GameObject errorPanel;
    public TMPro.TextMeshProUGUI errorText;
    public float errorDisplayDuration = 10f;
    
    private Queue<string> errorLog = new Queue<string>();
    private const int maxErrorLogSize = 100;
    private float lastErrorTime = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (enableExceptionHandling)
        {
            Application.logMessageReceived += HandleLog;
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            LogError($"Error: {logString}\nStackTrace: {stackTrace}");
        }
    }

    public void LogError(string errorMessage)
    {
        if (!enableErrorLogging) return;

        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        string fullError = $"[{timestamp}] {errorMessage}";
        
        errorLog.Enqueue(fullError);
        while (errorLog.Count > maxErrorLogSize)
        {
            errorLog.Dequeue();
        }

        Debug.LogError(fullError);

        if (showErrorUI && Time.time - lastErrorTime > 1f) // Prevent spam
        {
            ShowErrorUI(errorMessage);
            lastErrorTime = Time.time;
        }
    }

    public void LogWarning(string warningMessage)
    {
        if (!enableErrorLogging) return;

        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        string fullWarning = $"[{timestamp}] WARNING: {warningMessage}";
        
        errorLog.Enqueue(fullWarning);
        while (errorLog.Count > maxErrorLogSize)
        {
            errorLog.Dequeue();
        }

        Debug.LogWarning(fullWarning);
    }

    public void ValidateSystemReferences()
    {
        var missingSystems = new List<string>();
        
        if (GameManager.Instance == null) missingSystems.Add("GameManager");
        if (BusinessManager.Instance == null) missingSystems.Add("BusinessManager");
        if (MarketManager.Instance == null) missingSystems.Add("MarketManager");
        if (CompetitorAI.Instance == null) missingSystems.Add("CompetitorAI");
        if (FinanceSystem.Instance == null) missingSystems.Add("FinanceSystem");
        if (LogisticsSystem.Instance == null) missingSystems.Add("LogisticsSystem");
        if (RnDSystem.Instance == null) missingSystems.Add("RnDSystem");
        if (IPOSystem.Instance == null) missingSystems.Add("IPOSystem");
        if (MarketingSystem.Instance == null) missingSystems.Add("MarketingSystem");
        if (CrisisSystem.Instance == null) missingSystems.Add("CrisisSystem");
        if (InternationalSystem.Instance == null) missingSystems.Add("InternationalSystem");
        if (MergerSystem.Instance == null) missingSystems.Add("MergerSystem");
        if (AnalyticsSystem.Instance == null) missingSystems.Add("AnalyticsSystem");
        if (ProgressionSystem.Instance == null) missingSystems.Add("ProgressionSystem");
        if (RegulatorySystem.Instance == null) missingSystems.Add("RegulatorySystem");
        if (ScenarioSystem.Instance == null) missingSystems.Add("ScenarioSystem");

        if (missingSystems.Count > 0)
        {
            LogWarning($"Missing systems: {string.Join(", ", missingSystems)}");
        }
    }

    public void ValidateBusinessData(Business business)
    {
        if (business == null)
        {
            LogError("Business validation failed: Business is null");
            return;
        }

        // Validate financial data
        if (business.Capital < 0 && business.Capital > -1000000f) // Allow some debt
        {
            LogWarning($"Business {business.Name} has negative capital: ${business.Capital:N0}");
        }

        if (business.Capital > 10000000f) // Unrealistic high capital
        {
            LogWarning($"Business {business.Name} has unusually high capital: ${business.Capital:N0}");
        }

        // Validate employee data
        if (business.Employees != null)
        {
            foreach (var employee in business.Employees)
            {
                if (employee == null)
                {
                    LogError($"Business {business.Name} has null employee in list");
                }
                else if (employee.Wage < 0)
                {
                    LogWarning($"Employee {employee.Name} has negative wage: ${employee.Wage:N0}");
                }
            }
        }

        // Validate product data
        if (business.Products != null)
        {
            foreach (var product in business.Products)
            {
                if (product == null)
                {
                    LogError($"Business {business.Name} has null product in list");
                }
                else if (product.Inventory < 0)
                {
                    LogWarning($"Product {product.Name} has negative inventory: {product.Inventory}");
                }
                else if (product.Price < 0)
                {
                    LogWarning($"Product {product.Name} has negative price: ${product.Price:N2}");
                }
            }
        }
    }

    public void ValidateUIComponents()
    {
        var dashboard = FindFirstObjectByType<UIDashboard>();
        if (dashboard != null)
        {
            var missingComponents = new List<string>();
            
            if (dashboard.revenueText == null) missingComponents.Add("revenueText");
            if (dashboard.expensesText == null) missingComponents.Add("expensesText");
            if (dashboard.marketShareText == null) missingComponents.Add("marketShareText");
            if (dashboard.cashText == null) missingComponents.Add("cashText");
            if (dashboard.tickText == null) missingComponents.Add("tickText");
            
            if (missingComponents.Count > 0)
            {
                LogWarning($"UIDashboard missing components: {string.Join(", ", missingComponents)}");
            }
        }
    }

    private void ShowErrorUI(string errorMessage)
    {
        if (errorPanel != null && errorText != null)
        {
            errorText.text = $"Error: {errorMessage}";
            errorPanel.SetActive(true);
            
            // Auto-hide after duration
            Invoke(nameof(HideErrorUI), errorDisplayDuration);
        }
    }

    private void HideErrorUI()
    {
        if (errorPanel != null)
        {
            errorPanel.SetActive(false);
        }
    }

    public string GetErrorLog()
    {
        return string.Join("\n", errorLog);
    }

    public void ClearErrorLog()
    {
        errorLog.Clear();
    }

    public void TestErrorHandling()
    {
        LogError("This is a test error message");
        LogWarning("This is a test warning message");
        ValidateSystemReferences();
    }

    private void OnDestroy()
    {
        if (enableExceptionHandling)
        {
            Application.logMessageReceived -= HandleLog;
        }
    }

    private void LogSystemState()
    {
        var systems = new Dictionary<string, object>
        {
            { "GameManager", FindFirstObjectByType<GameManager>() },
            { "BusinessManager", FindFirstObjectByType<BusinessManager>() },
            { "MarketManager", FindFirstObjectByType<MarketManager>() },
            { "FinanceSystem", FindFirstObjectByType<FinanceSystem>() },
            { "LogisticsSystem", FindFirstObjectByType<LogisticsSystem>() },
            { "RnDSystem", FindFirstObjectByType<RnDSystem>() },
            { "IPOSystem", FindFirstObjectByType<IPOSystem>() },
            { "MarketingSystem", FindFirstObjectByType<MarketingSystem>() },
            { "CrisisSystem", FindFirstObjectByType<CrisisSystem>() },
            { "InternationalSystem", FindFirstObjectByType<InternationalSystem>() },
            { "MergerSystem", FindFirstObjectByType<MergerSystem>() },
            { "AnalyticsSystem", FindFirstObjectByType<AnalyticsSystem>() },
            { "ProgressionSystem", FindFirstObjectByType<ProgressionSystem>() },
            { "RegulatorySystem", FindFirstObjectByType<RegulatorySystem>() },
            { "ScenarioSystem", FindFirstObjectByType<ScenarioSystem>() }
        };

        var business = FindFirstObjectByType<BusinessManager>()?.PlayerBusiness;
        
        string systemState = "System State:\n";
        foreach (var system in systems)
        {
            systemState += $"{system.Key}: {system.Value}\n";
        }

        if (business != null)
        {
            systemState += $"\nBusiness: {business.Name} - Capital: ${business.Capital:N0}";
        }

        LogWarning(systemState);
    }
} 