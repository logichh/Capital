using System.Collections.Generic;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    public static MarketManager Instance { get; private set; }
    private Dictionary<string, Market> markets = new Dictionary<string, Market>();

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

    public Market GetOrCreateMarket(string category, float elasticity = 1.0f)
    {
        if (!markets.ContainsKey(category))
        {
            markets[category] = new Market(category, elasticity);
        }
        return markets[category];
    }

    public void UpdateMarkets()
    {
        foreach (var market in markets.Values)
        {
            market.UpdateMarketPrice();
        }
    }
} 