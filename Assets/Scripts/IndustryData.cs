using UnityEngine;

[CreateAssetMenu(fileName = "IndustryData", menuName = "CapitalArchitect/IndustryData", order = 1)]
public class IndustryData : ScriptableObject
{
    public string IndustryName;
    [TextArea]
    public string Description;
    public float BaseDemand;
    public float BaseSupply;
    public float PriceElasticity;
    // Add more fields as needed
} 