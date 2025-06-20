using UnityEngine;

public class BusinessActionsUI : MonoBehaviour
{
    public void OnHireEmployee()
    {
        // Placeholder: create a sample employee
        Employee newEmployee = new Employee($"Employee {Random.Range(1, 1000)}", "Worker", 2000f, 1.0f);
        BusinessManager.Instance.HireEmployee(newEmployee);
        Debug.Log($"Hired {newEmployee.Name}");
    }

    public void OnLaunchProduct()
    {
        // Placeholder: create a sample product
        float quality = Random.Range(0.7f, 1.3f);
        Product newProduct = new Product($"Product {Random.Range(1, 1000)}", "General", 100f, 50f, 0, quality);
        // Add a random feature
        string[] features = { "Eco-Friendly", "High Performance", "Compact", "Luxury", "Budget" };
        newProduct.FeatureList.Add(features[Random.Range(0, features.Length)]);
        var business = BusinessManager.Instance.PlayerBusiness;
        if (business != null)
        {
            business.AddProduct(newProduct);
            Debug.Log($"Launched {newProduct.Name} (Quality: {quality:F2}, Feature: {newProduct.FeatureList[0]})");
        }
    }

    public void OnTrainEmployees()
    {
        bool success = BusinessManager.Instance.TrainEmployees(0.05f, 1000f, 5);
        if (success)
            Debug.Log("Trained eligible employees (+0.05 skill, -$1000 per employee, 5-tick cooldown)");
        else
            Debug.Log("No employees eligible for training or not enough cash!");
    }
} 