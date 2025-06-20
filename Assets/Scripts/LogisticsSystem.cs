using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Supplier
{
    public string Name;
    public string MaterialType;
    public float BasePrice;
    public float Reliability; // 0-1, affects delivery success
    public float Quality; // 0-1, affects product quality
    public int MinOrderQuantity;
    public float BulkDiscountThreshold;
    public float BulkDiscountPercent;

    public Supplier(string name, string materialType, float basePrice, float reliability, float quality, 
                   int minOrderQuantity, float bulkDiscountThreshold, float bulkDiscountPercent)
    {
        Name = name;
        MaterialType = materialType;
        BasePrice = basePrice;
        Reliability = reliability;
        Quality = quality;
        MinOrderQuantity = minOrderQuantity;
        BulkDiscountThreshold = bulkDiscountThreshold;
        BulkDiscountPercent = bulkDiscountPercent;
    }

    public float GetPrice(int quantity)
    {
        if (quantity >= BulkDiscountThreshold)
            return BasePrice * (1 - BulkDiscountPercent);
        return BasePrice;
    }
}

[System.Serializable]
public class SupplyOrder
{
    public Supplier Supplier;
    public int Quantity;
    public float TotalCost;
    public int RemainingTicks;
    public bool IsDelivered;

    public SupplyOrder(Supplier supplier, int quantity, float totalCost, int deliveryTicks)
    {
        Supplier = supplier;
        Quantity = quantity;
        TotalCost = totalCost;
        RemainingTicks = deliveryTicks;
        IsDelivered = false;
    }
}

[System.Serializable]
public class Warehouse
{
    public string Location;
    public Dictionary<string, int> Inventory;
    public int Capacity;
    public float StorageCostPerUnit;
    public float OperatingCost;

    public Warehouse(string location, int capacity, float storageCostPerUnit, float operatingCost)
    {
        Location = location;
        Capacity = capacity;
        StorageCostPerUnit = storageCostPerUnit;
        OperatingCost = operatingCost;
        Inventory = new Dictionary<string, int>();
    }

    public bool CanStore(int amount)
    {
        int currentTotal = Inventory.Values.Sum();
        return currentTotal + amount <= Capacity;
    }

    public float GetStorageCost()
    {
        return Inventory.Values.Sum() * StorageCostPerUnit + OperatingCost;
    }
}

public class LogisticsSystem : MonoBehaviour
{
    public static LogisticsSystem Instance { get; private set; }

    private Dictionary<Business, List<Supplier>> businessSuppliers = new Dictionary<Business, List<Supplier>>();
    private Dictionary<Business, List<SupplyOrder>> businessOrders = new Dictionary<Business, List<SupplyOrder>>();
    private Dictionary<Business, List<Warehouse>> businessWarehouses = new Dictionary<Business, List<Warehouse>>();

    [SerializeField] private float baseDeliveryTime = 5f; // ticks
    [SerializeField] private float deliveryTimeVariance = 2f;
    [SerializeField] private float failureChanceMultiplier = 0.1f;

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

    public void RegisterSupplier(Business business, Supplier supplier)
    {
        if (!businessSuppliers.ContainsKey(business))
            businessSuppliers[business] = new List<Supplier>();
        
        businessSuppliers[business].Add(supplier);
    }

    public bool PlaceOrder(Business business, Supplier supplier, int quantity)
    {
        if (quantity < supplier.MinOrderQuantity)
            return false;

        float totalCost = supplier.GetPrice(quantity) * quantity;
        if (business.Capital < totalCost)
            return false;

        if (!businessOrders.ContainsKey(business))
            businessOrders[business] = new List<SupplyOrder>();

        int deliveryTicks = Mathf.RoundToInt(baseDeliveryTime + Random.Range(-deliveryTimeVariance, deliveryTimeVariance));
        var order = new SupplyOrder(supplier, quantity, totalCost, deliveryTicks);
        
        businessOrders[business].Add(order);
        business.Capital -= totalCost;
        business.AddLiability(totalCost); // Track as liability until delivered

        return true;
    }

    public Warehouse AddWarehouse(Business business, string location, int capacity)
    {
        if (!businessWarehouses.ContainsKey(business))
            businessWarehouses[business] = new List<Warehouse>();

        var warehouse = new Warehouse(location, capacity, 0.1f, 1000f);
        businessWarehouses[business].Add(warehouse);
        return warehouse;
    }

    public void ProcessLogistics()
    {
        foreach (var kvp in businessOrders)
        {
            var business = kvp.Key;
            var orders = kvp.Value;

            for (int i = orders.Count - 1; i >= 0; i--)
            {
                var order = orders[i];
                if (--order.RemainingTicks <= 0)
                {
                    // Check for delivery failure based on supplier reliability
                    float failureChance = (1 - order.Supplier.Reliability) * failureChanceMultiplier;
                    bool failed = Random.value < failureChance;

                    if (!failed)
                    {
                        // Find warehouse with space
                        var warehouse = FindAvailableWarehouse(business, order.Quantity);
                        if (warehouse != null)
                        {
                            // Successfully deliver to warehouse
                            if (!warehouse.Inventory.ContainsKey(order.Supplier.MaterialType))
                                warehouse.Inventory[order.Supplier.MaterialType] = 0;

                            warehouse.Inventory[order.Supplier.MaterialType] += order.Quantity;
                            business.ReduceLiability(order.TotalCost); // Remove from liabilities

                            // Affect product quality based on supplier quality
                            foreach (var product in business.Products)
                            {
                                product.Quality *= 1 + (order.Supplier.Quality - 0.5f) * 0.1f;
                                product.Quality = Mathf.Clamp(product.Quality, 0.5f, 2.0f);
                            }

                            orders.RemoveAt(i);
                        }
                        else
                        {
                            // Delay delivery until space is available
                            order.RemainingTicks = 1;
                            Debug.Log($"No warehouse space for order from {order.Supplier.Name}. Delivery delayed.");
                        }
                    }
                    else
                    {
                        // Order failed, refund business
                        business.Capital += order.TotalCost;
                        business.ReduceLiability(order.TotalCost);
                        Debug.Log($"Order from {order.Supplier.Name} failed delivery!");
                        orders.RemoveAt(i);
                    }
                }
            }
        }

        // Process warehouse costs
        foreach (var kvp in businessWarehouses)
        {
            var business = kvp.Key;
            foreach (var warehouse in kvp.Value)
            {
                float cost = warehouse.GetStorageCost();
                business.Capital -= cost;
                business.TotalExpenses += cost;
            }
        }
    }

    private Warehouse FindAvailableWarehouse(Business business, int amount)
    {
        if (!businessWarehouses.ContainsKey(business))
            return null;

        return businessWarehouses[business].FirstOrDefault(w => w.CanStore(amount));
    }

    public (List<Supplier>, List<SupplyOrder>, List<Warehouse>) GetLogisticsStatus(Business business)
    {
        List<Supplier> suppliers = businessSuppliers.ContainsKey(business) ? businessSuppliers[business] : new List<Supplier>();
        List<SupplyOrder> orders = businessOrders.ContainsKey(business) ? businessOrders[business] : new List<SupplyOrder>();
        List<Warehouse> warehouses = businessWarehouses.ContainsKey(business) ? businessWarehouses[business] : new List<Warehouse>();
        return (suppliers, orders, warehouses);
    }

    public void GenerateInitialSuppliers(Business business)
    {
        // Generate some sample suppliers
        var suppliers = new List<Supplier>
        {
            new Supplier("Quality Materials Co.", "Raw Materials", 100f, 0.9f, 0.9f, 10, 50, 0.15f),
            new Supplier("Budget Supplies Inc.", "Raw Materials", 60f, 0.7f, 0.6f, 5, 30, 0.1f),
            new Supplier("Premium Components", "Components", 200f, 0.95f, 0.95f, 5, 20, 0.2f),
            new Supplier("Global Trading LLC", "Components", 150f, 0.8f, 0.8f, 8, 40, 0.12f)
        };

        foreach (var supplier in suppliers)
        {
            RegisterSupplier(business, supplier);
        }
    }
} 