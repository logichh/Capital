using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class LogisticsUI : MonoBehaviour
{
    [Header("Supplier Management")]
    public Transform supplierListContent;
    public GameObject supplierEntryPrefab;
    public TMP_Dropdown supplierDropdown;
    public TMP_InputField orderQuantityInput;
    public Button placeOrderButton;

    [Header("Warehouse Management")]
    public Transform warehouseListContent;
    public GameObject warehouseEntryPrefab;
    public TMP_InputField warehouseLocationInput;
    public TMP_InputField warehouseCapacityInput;
    public Button addWarehouseButton;

    [Header("Order Status")]
    public Transform orderListContent;
    public GameObject orderEntryPrefab;
    public TextMeshProUGUI orderStatusText;

    [Header("Inventory Status")]
    public Transform inventoryListContent;
    public GameObject inventoryEntryPrefab;
    public TextMeshProUGUI totalStorageCostText;

    private void Start()
    {
        // Setup button listeners
        placeOrderButton.onClick.AddListener(OnPlaceOrder);
        addWarehouseButton.onClick.AddListener(OnAddWarehouse);

        // Initial UI update
        UpdateLogisticsUI();
    }

    public void OnPlaceOrder()
    {
        if (int.TryParse(orderQuantityInput.text, out int quantity))
        {
            var business = BusinessManager.Instance.PlayerBusiness;
            if (business != null && supplierDropdown.options.Count > 0)
            {
                var (suppliers, _, _) = LogisticsSystem.Instance.GetLogisticsStatus(business);
                if (suppliers.Count > supplierDropdown.value)
                {
                    var supplier = suppliers[supplierDropdown.value];
                    if (LogisticsSystem.Instance.PlaceOrder(business, supplier, quantity))
                    {
                        Debug.Log($"Order placed with {supplier.Name} for {quantity} units");
                        UpdateLogisticsUI();
                    }
                    else
                    {
                        Debug.Log("Could not place order - check minimum quantity and funds");
                    }
                }
            }
        }
    }

    public void OnAddWarehouse()
    {
        if (int.TryParse(warehouseCapacityInput.text, out int capacity))
        {
            string location = warehouseLocationInput.text;
            if (string.IsNullOrEmpty(location))
                location = $"Warehouse {Random.Range(1000, 9999)}";

            var business = BusinessManager.Instance.PlayerBusiness;
            if (business != null)
            {
                var warehouse = LogisticsSystem.Instance.AddWarehouse(business, location, capacity);
                Debug.Log($"Added warehouse at {location} with capacity {capacity}");
                UpdateLogisticsUI();
            }
        }
    }

    public void UpdateLogisticsUI()
    {
        var business = BusinessManager.Instance.PlayerBusiness;
        if (business == null) return;

        var (suppliers, orders, warehouses) = LogisticsSystem.Instance.GetLogisticsStatus(business);

        // Update supplier list and dropdown
        foreach (Transform child in supplierListContent)
            Destroy(child.gameObject);

        supplierDropdown.ClearOptions();
        foreach (var supplier in suppliers)
        {
            // Add to detailed list
            var entry = Instantiate(supplierEntryPrefab, supplierListContent).GetComponent<TextMeshProUGUI>();
            entry.text = $"Supplier: {supplier.Name}\n" +
                        $"Material: {supplier.MaterialType}\n" +
                        $"Base Price: ${supplier.BasePrice:N2}\n" +
                        $"Reliability: {supplier.Reliability:P0}\n" +
                        $"Quality: {supplier.Quality:P0}\n" +
                        $"Min Order: {supplier.MinOrderQuantity}\n" +
                        $"Bulk Discount: {supplier.BulkDiscountPercent:P0} (over {supplier.BulkDiscountThreshold} units)";

            // Add to dropdown
            supplierDropdown.options.Add(new TMP_Dropdown.OptionData($"{supplier.Name} ({supplier.MaterialType})"));
        }
        supplierDropdown.RefreshShownValue();

        // Update warehouse list
        foreach (Transform child in warehouseListContent)
            Destroy(child.gameObject);

        float totalStorageCost = 0f;
        foreach (var warehouse in warehouses)
        {
            var entry = Instantiate(warehouseEntryPrefab, warehouseListContent).GetComponent<TextMeshProUGUI>();
            string inventoryText = string.Join("\n", warehouse.Inventory
                .Select(kvp => $"  {kvp.Key}: {kvp.Value} units"));
            
            entry.text = $"Location: {warehouse.Location}\n" +
                        $"Capacity: {warehouse.Inventory.Values.Sum()}/{warehouse.Capacity}\n" +
                        $"Storage Cost: ${warehouse.StorageCostPerUnit:N2}/unit\n" +
                        $"Operating Cost: ${warehouse.OperatingCost:N0}/tick\n" +
                        $"Inventory:\n{inventoryText}";

            totalStorageCost += warehouse.GetStorageCost();
        }
        totalStorageCostText.text = $"Total Storage Cost: ${totalStorageCost:N2}/tick";

        // Update order status
        foreach (Transform child in orderListContent)
            Destroy(child.gameObject);

        foreach (var order in orders)
        {
            var entry = Instantiate(orderEntryPrefab, orderListContent).GetComponent<TextMeshProUGUI>();
            entry.text = $"Order from: {order.Supplier.Name}\n" +
                        $"Material: {order.Supplier.MaterialType}\n" +
                        $"Quantity: {order.Quantity}\n" +
                        $"Total Cost: ${order.TotalCost:N2}\n" +
                        $"Delivery in: {order.RemainingTicks} ticks";
        }
        orderStatusText.text = $"Active Orders: {orders.Count}";

        // Update inventory status
        foreach (Transform child in inventoryListContent)
            Destroy(child.gameObject);

        var totalInventory = new Dictionary<string, int>();
        foreach (var warehouse in warehouses)
        {
            foreach (var kvp in warehouse.Inventory)
            {
                if (!totalInventory.ContainsKey(kvp.Key))
                    totalInventory[kvp.Key] = 0;
                totalInventory[kvp.Key] += kvp.Value;
            }
        }

        foreach (var kvp in totalInventory)
        {
            var entry = Instantiate(inventoryEntryPrefab, inventoryListContent).GetComponent<TextMeshProUGUI>();
            entry.text = $"{kvp.Key}: {kvp.Value} units";
        }
    }
} 