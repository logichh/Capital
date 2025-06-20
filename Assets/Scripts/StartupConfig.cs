using UnityEngine;

[CreateAssetMenu(fileName = "StartupConfig", menuName = "CapitalArchitect/StartupConfig", order = 3)]
public class StartupConfig : ScriptableObject
{
    public float StartingCapital;
    public string[] Strategies; // e.g., cost-leader, innovator, premium
    // Add more fields as needed
} 