using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIBuilder
{
    [MenuItem("Game/Build Game UI")]
    private static void BuildUI()
    {
        // Find existing UI or create new
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("GameUICanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Create EventSystem if it doesn't exist
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        // --- Create Dashboard Panel ---
        GameObject dashboardPanelGO = CreatePanel(canvas.transform, "DashboardPanel");
        UIDashboard dashboard = dashboardPanelGO.AddComponent<UIDashboard>();

        // --- Create KPI Text Objects ---
        dashboard.revenueText = CreateText(dashboardPanelGO.transform, "RevenueText", "Revenue: $0", 200);
        dashboard.expensesText = CreateText(dashboardPanelGO.transform, "ExpensesText", "Expenses: $0", 160);
        dashboard.marketShareText = CreateText(dashboardPanelGO.transform, "MarketShareText", "Market Share: 0%", 120);
        dashboard.cashText = CreateText(dashboardPanelGO.transform, "CashText", "Cash: $0", 80);
        dashboard.tickText = CreateText(dashboardPanelGO.transform, "TickText", "Day: 0", 40);

        // --- Create Financial Metrics Text Objects ---
        dashboard.netIncomeText = CreateText(dashboardPanelGO.transform, "NetIncomeText", "Net Income: $0", 0);
        dashboard.totalAssetsText = CreateText(dashboardPanelGO.transform, "TotalAssetsText", "Total Assets: $0", -40);
        dashboard.totalLiabilitiesText = CreateText(dashboardPanelGO.transform, "TotalLiabilitiesText", "Total Liabilities: $0", -80);
        dashboard.netWorthText = CreateText(dashboardPanelGO.transform, "NetWorthText", "Net Worth: $0", -120);
        dashboard.creditScoreText = CreateText(dashboardPanelGO.transform, "CreditScoreText", "Credit Score: 0", -160);
        
        // --- Create Notification Panel ---
        GameObject notificationPanel = CreatePanel(canvas.transform, "NotificationPanel");
        dashboard.notificationPanel = notificationPanel;
        dashboard.notificationText = CreateText(notificationPanel.transform, "NotificationText", "Notification", 0);
        notificationPanel.SetActive(false);


        Debug.Log("Game UI has been built successfully!");
    }

    private static GameObject CreatePanel(Transform parent, string name)
    {
        GameObject panelGO = new GameObject(name);
        panelGO.transform.SetParent(parent, false);
        panelGO.layer = LayerMask.NameToLayer("UI");
        RectTransform rect = panelGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image image = panelGO.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.4f);
        return panelGO;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string name, string defaultText, float yPos)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);
        textGO.layer = LayerMask.NameToLayer("UI");
        textGO.AddComponent<CanvasRenderer>();
        
        RectTransform rect = textGO.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 50);
        rect.anchoredPosition = new Vector2(0, yPos);

        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = defaultText;
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        return text;
    }
} 