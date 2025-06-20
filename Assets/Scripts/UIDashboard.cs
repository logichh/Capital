using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDashboard : MonoBehaviour
{
    [Header("KPI Display")]
    public TextMeshProUGUI revenueText;
    public TextMeshProUGUI expensesText;
    public TextMeshProUGUI marketShareText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI tickText;

    [Header("Financial Metrics")]
    public TextMeshProUGUI netIncomeText;
    public TextMeshProUGUI totalAssetsText;
    public TextMeshProUGUI totalLiabilitiesText;
    public TextMeshProUGUI netWorthText;
    public TextMeshProUGUI creditScoreText;

    [Header("Tabs")]
    public GameObject dashboardTab;
    public GameObject financeTab;
    public GameObject hrTab;
    public GameObject marketingTab;
    public GameObject operationsTab;
    public GameObject rndTab;

    [Header("Market Info")]
    public TextMeshProUGUI marketInfoText;
    public TextMeshProUGUI competitorInfoText;

    [Header("Notifications")]
    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;
    public float notificationDuration = 5f;
    private float notificationTimer = 0f;

    [Header("Tips")]
    public GameObject tipPanel;
    public TextMeshProUGUI tipText;
    public float tipDuration = 10f;
    private float tipTimer = 0f;

    [Header("Tooltips")]
    public TextMeshProUGUI revenueTooltip;
    public TextMeshProUGUI expensesTooltip;
    public TextMeshProUGUI marketShareTooltip;
    public TextMeshProUGUI cashTooltip;
    public TextMeshProUGUI tickTooltip;

    [Header("Event History")]
    public TextMeshProUGUI eventHistoryText;
    private readonly System.Collections.Generic.Queue<string> eventHistory = new System.Collections.Generic.Queue<string>();
    private const int maxEventHistory = 10;

    [Header("Contextual Tips")]
    public TextMeshProUGUI tipsText;

    public void ShowTab(string tabName)
    {
        dashboardTab.SetActive(tabName == "Dashboard");
        financeTab.SetActive(tabName == "Finance");
        hrTab.SetActive(tabName == "HR");
        marketingTab.SetActive(tabName == "Marketing");
        operationsTab.SetActive(tabName == "Operations");
        rndTab.SetActive(tabName == "R&D");
    }

    public void UpdateKPIs(float revenue, float expenses, float marketShare, float cash, int tick)
    {
        if (revenueText != null) revenueText.text = $"Revenue: ${revenue:N0}";
        if (expensesText != null) expensesText.text = $"Expenses: ${expenses:N0}";
        if (marketShareText != null) marketShareText.text = $"Market Share: {marketShare:P1}";
        if (cashText != null) cashText.text = $"Cash: ${cash:N0}";
        if (tickText != null) tickText.text = $"Day: {tick}";

        var business = BusinessManager.Instance?.PlayerBusiness;
        if (business != null)
        {
            if (netIncomeText != null) netIncomeText.text = $"Net Income: ${business.NetIncome:N0}";
            if (totalAssetsText != null) totalAssetsText.text = $"Total Assets: ${business.TotalAssets:N0}";
            if (totalLiabilitiesText != null) totalLiabilitiesText.text = $"Total Liabilities: ${business.TotalLiabilities:N0}";
            if (netWorthText != null) netWorthText.text = $"Net Worth: ${business.NetWorth:N0}";
            if (creditScoreText != null) creditScoreText.text = $"Credit Score: {business.CreditScore:N0}";
        }
    }

    public void UpdateMarketInfo(string info)
    {
        if (marketInfoText != null) marketInfoText.text = info;
    }

    public void UpdateCompetitorInfo(string info)
    {
        if (competitorInfoText != null) competitorInfoText.text = info;
    }

    public void ShowNotification(string message)
    {
        if (notificationText != null) notificationText.text = message;
        if (notificationPanel != null) notificationPanel.SetActive(true);
        notificationTimer = 0f;
    }

    public void ShowTooltip(string kpi, string message)
    {
        switch (kpi)
        {
            case "Revenue": if (revenueTooltip != null) revenueTooltip.text = message; break;
            case "Expenses": if (expensesTooltip != null) expensesTooltip.text = message; break;
            case "MarketShare": if (marketShareTooltip != null) marketShareTooltip.text = message; break;
            case "Cash": if (cashTooltip != null) cashTooltip.text = message; break;
            case "Tick": if (tickTooltip != null) tickTooltip.text = message; break;
        }
    }

    public void ClearTooltips()
    {
        if (revenueTooltip != null) revenueTooltip.text = "";
        if (expensesTooltip != null) expensesTooltip.text = "";
        if (marketShareTooltip != null) marketShareTooltip.text = "";
        if (cashTooltip != null) cashTooltip.text = "";
        if (tickTooltip != null) tickTooltip.text = "";
    }

    public void AddEventToHistory(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        eventHistory.Enqueue(message);
        while (eventHistory.Count > maxEventHistory)
            eventHistory.Dequeue();
        if (eventHistoryText != null)
            eventHistoryText.text = string.Join("\n", eventHistory);
    }

    public void ShowTip(string message)
    {
        if (tipText != null) tipText.text = message;
        if (tipPanel != null) tipPanel.SetActive(true);
        tipTimer = 0f;
    }

    private void Update()
    {
        // Auto-fade notifications
        if (notificationPanel != null && notificationPanel.activeSelf)
        {
            notificationTimer += Time.deltaTime;
            if (notificationTimer >= notificationDuration)
            {
                notificationPanel.SetActive(false);
                notificationTimer = 0f;
            }
        }

        // Auto-fade tips
        if (tipPanel != null && tipPanel.activeSelf)
        {
            tipTimer += Time.deltaTime;
            if (tipTimer >= tipDuration)
            {
                tipPanel.SetActive(false);
                tipTimer = 0f;
            }
        }
    }
} 