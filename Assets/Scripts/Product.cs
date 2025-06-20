using UnityEngine;

[System.Serializable]
public class Product
{
    public string Name;
    public string Category;
    public float Price;
    public float Cost;
    public int Inventory;
    public float Demand;
    public float Quality;
    public System.Collections.Generic.List<string> FeatureList = new System.Collections.Generic.List<string>();
    // Add more fields as needed

    public Product(string name, string category, float price, float cost, int inventory, float quality = 1.0f)
    {
        Name = name;
        Category = category;
        Price = price;
        Cost = cost;
        Inventory = inventory;
        Demand = 0f;
        Quality = quality;
    }
} 