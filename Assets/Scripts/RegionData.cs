using UnityEngine;

[CreateAssetMenu(fileName = "RegionData", menuName = "CapitalArchitect/RegionData", order = 2)]
public class RegionData : ScriptableObject
{
    public string RegionName;
    [TextArea]
    public string Description;
    public int Population;
    public float EconomicGrowthRate;
    public float TaxRate;
    // Add more fields as needed
} 