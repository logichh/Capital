using UnityEngine;
using System.Collections.Generic;

public class AnalyticsSystem : MonoBehaviour
{
    public static AnalyticsSystem Instance { get; private set; }

    // Dictionary to hold metrics for analytics
    private Dictionary<string, object> analyticsData = new Dictionary<string, object>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to satisfy AdvancedUI.cs
    public (List<string> reports, List<string> analyses, List<string> metrics, List<string> forecasts) GetAnalyticsStatus(Business business)
    {
        if (business == null) 
            return (new List<string>(), new List<string>(), new List<string>(), new List<string>());

        var reports = new List<string> { $"Q{Time.frameCount / 1000} Report" };
        var analyses = new List<string> { "Market Trend: Stable" };
        var metrics = new List<string> { $"Performance: {business.GetOverallPerformance():F2}" };
        var forecasts = new List<string> { "Next Quarter: +2% Growth" };
        return (reports, analyses, metrics, forecasts);
    }

    // Method to satisfy AdvancedUI.cs
    public float GetBusinessHealthScore(Business business)
    {
        if (business == null) return 0f;
        // A simple calculation based on various business properties
        return (business.NetWorth / 100000f) + business.Reputation + business.InnovationScore - (business.TotalLiabilities / 10000f);
    }

    // A more robust method to track various types of events
    public void TrackEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                analyticsData[param.Key] = param.Value;
            }
        }
        Debug.Log($"Analytics Event: '{eventName}' tracked with {parameters?.Count ?? 0} parameters.");
    }

    // Example of tracking a specific business event
    public void TrackBusinessMilestone(Business business, string milestone)
    {
        var parameters = new Dictionary<string, object>
        {
            { "BusinessName", business.Name },
            { "Milestone", milestone },
            { "NetWorth", business.NetWorth },
            { "DaysInBusiness", business.DaysInBusiness }
        };
        TrackEvent("BusinessMilestone", parameters);
    }
} 