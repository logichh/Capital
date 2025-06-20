using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Scenario
{
    public string Name;
    public string Description;
    public string Difficulty; // "Easy", "Medium", "Hard", "Expert"
    public float Duration; // In game ticks
    public float RemainingTime;
    public List<string> Objectives;
    public List<string> Restrictions;
    public Dictionary<string, float> Modifiers;
    public bool IsCompleted;
    public float Reward;

    public Scenario(string name, string description, string difficulty, float duration)
    {
        Name = name;
        Description = description;
        Difficulty = difficulty;
        Duration = duration;
        RemainingTime = duration;
        Objectives = new List<string>();
        Restrictions = new List<string>();
        Modifiers = new Dictionary<string, float>();
        IsCompleted = false;
        Reward = CalculateReward(difficulty, duration);
    }

    private float CalculateReward(string difficulty, float duration)
    {
        float baseReward = 100000f;
        float difficultyMultiplier = 1f;
        float durationMultiplier = 1f;

        switch (difficulty)
        {
            case "Easy": difficultyMultiplier = 0.5f; break;
            case "Medium": difficultyMultiplier = 1f; break;
            case "Hard": difficultyMultiplier = 2f; break;
            case "Expert": difficultyMultiplier = 4f; break;
        }

        durationMultiplier = duration / 100f; // Longer scenarios = more reward

        return baseReward * difficultyMultiplier * durationMultiplier;
    }
}

[System.Serializable]
public class Challenge
{
    public string Name;
    public string Description;
    public string Type; // "Speed", "Efficiency", "Innovation", "Survival"
    public float TargetValue;
    public float CurrentValue;
    public bool IsCompleted;
    public float TimeLimit;
    public float RemainingTime;
    public List<string> Requirements;

    public Challenge(string name, string description, string type, float target, float timeLimit)
    {
        Name = name;
        Description = description;
        Type = type;
        TargetValue = target;
        CurrentValue = 0f;
        IsCompleted = false;
        TimeLimit = timeLimit;
        RemainingTime = timeLimit;
        Requirements = new List<string>();
    }
}

[System.Serializable]
public class SpecialEvent
{
    public string Name;
    public string Description;
    public string Type; // "Economic", "Technological", "Social", "Environmental"
    public float Duration;
    public float RemainingTime;
    public Dictionary<string, float> Effects;
    public bool IsActive;
    public List<string> Responses;

    public SpecialEvent(string name, string description, string type, float duration)
    {
        Name = name;
        Description = description;
        Type = type;
        Duration = duration;
        RemainingTime = duration;
        Effects = new Dictionary<string, float>();
        IsActive = false;
        Responses = new List<string>();
    }
}

public class ScenarioSystem : MonoBehaviour
{
    public static ScenarioSystem Instance { get; private set; }

    private Dictionary<Business, Scenario> activeScenarios = new Dictionary<Business, Scenario>();
    private Dictionary<Business, List<Challenge>> activeChallenges = new Dictionary<Business, List<Challenge>>();
    private Dictionary<Business, List<SpecialEvent>> specialEvents = new Dictionary<Business, List<SpecialEvent>>();
    private Dictionary<Business, List<Scenario>> completedScenarios = new Dictionary<Business, List<Scenario>>();

    [SerializeField] private float specialEventChance = 0.05f; // 5% chance per tick
    [SerializeField] private float challengeChance = 0.1f; // 10% chance per tick

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

    public void StartScenario(Business business, string scenarioName)
    {
        var scenario = CreateScenario(scenarioName);
        if (scenario != null)
        {
            activeScenarios[business] = scenario;
            ApplyScenarioModifiers(business, scenario);
            Debug.Log($"Started scenario: {scenario.Name}");
        }
    }

    private Scenario CreateScenario(string scenarioName)
    {
        switch (scenarioName.ToLower())
        {
            case "startup challenge":
                var startup = new Scenario("Startup Challenge", "Build a successful startup from scratch", "Medium", 200f);
                startup.Objectives.Add("Reach $1M in capital");
                startup.Objectives.Add("Hire 10 employees");
                startup.Objectives.Add("Launch 2 products");
                startup.Restrictions.Add("No external funding");
                startup.Restrictions.Add("Limited marketing budget");
                startup.Modifiers["CapitalGrowth"] = 0.8f;
                startup.Modifiers["EmployeeCost"] = 1.2f;
                return startup;

            case "market domination":
                var domination = new Scenario("Market Domination", "Achieve market leadership", "Hard", 300f);
                domination.Objectives.Add("Reach 30% market share");
                domination.Objectives.Add("Complete 5 acquisitions");
                domination.Objectives.Add("Go public");
                domination.Modifiers["MarketShare"] = 1.3f;
                domination.Modifiers["AcquisitionCost"] = 0.8f;
                return domination;

            case "innovation race":
                var innovation = new Scenario("Innovation Race", "Lead in research and development", "Hard", 250f);
                innovation.Objectives.Add("Complete 15 research projects");
                innovation.Objectives.Add("File 5 patents");
                innovation.Objectives.Add("Achieve 50% R&D efficiency");
                innovation.Modifiers["ResearchSpeed"] = 1.5f;
                innovation.Modifiers["PatentValue"] = 1.4f;
                return innovation;

            case "global expansion":
                var global = new Scenario("Global Expansion", "Expand to international markets", "Expert", 400f);
                global.Objectives.Add("Enter 5 countries");
                global.Objectives.Add("Create 3 subsidiaries");
                global.Objectives.Add("Achieve $10M international revenue");
                global.Modifiers["InternationalCost"] = 0.7f;
                global.Modifiers["ExchangeRate"] = 1.1f;
                return global;

            case "crisis management":
                var crisis = new Scenario("Crisis Management", "Navigate through multiple crises", "Expert", 350f);
                crisis.Objectives.Add("Resolve 10 crises");
                crisis.Objectives.Add("Maintain 80% reputation");
                crisis.Objectives.Add("Avoid bankruptcy");
                crisis.Modifiers["CrisisFrequency"] = 2.0f;
                crisis.Modifiers["CrisisSeverity"] = 1.5f;
                return crisis;

            default:
                return null;
        }
    }

    public void ProcessScenarios()
    {
        foreach (var kvp in activeScenarios)
        {
            var business = kvp.Key;
            var scenario = kvp.Value;

            scenario.RemainingTime--;

            // Check scenario completion
            if (CheckScenarioCompletion(business, scenario))
            {
                CompleteScenario(business, scenario);
            }
            else if (scenario.RemainingTime <= 0)
            {
                FailScenario(business, scenario);
            }
        }

        // Process challenges
        foreach (var kvp in activeChallenges)
        {
            var business = kvp.Key;
            var challenges = kvp.Value;

            for (int i = challenges.Count - 1; i >= 0; i--)
            {
                var challenge = challenges[i];
                challenge.RemainingTime--;

                UpdateChallengeProgress(business, challenge);

                if (challenge.IsCompleted)
                {
                    CompleteChallenge(business, challenge);
                    challenges.RemoveAt(i);
                }
                else if (challenge.RemainingTime <= 0)
                {
                    FailChallenge(business, challenge);
                    challenges.RemoveAt(i);
                }
            }
        }

        // Process special events
        foreach (var kvp in specialEvents)
        {
            var business = kvp.Key;
            var events = kvp.Value;

            for (int i = events.Count - 1; i >= 0; i--)
            {
                var specialEvent = events[i];
                if (specialEvent.IsActive)
                {
                    specialEvent.RemainingTime--;
                    ApplySpecialEventEffects(business, specialEvent);

                    if (specialEvent.RemainingTime <= 0)
                    {
                        EndSpecialEvent(business, specialEvent);
                        events.RemoveAt(i);
                    }
                }
            }
        }

        // Randomly trigger new events/challenges
        if (BusinessManager.Instance != null)
        {
            foreach (var business in BusinessManager.Instance.Businesses)
            {
                if (Random.value < specialEventChance)
                {
                    TriggerSpecialEvent(business);
                }

                if (Random.value < challengeChance)
                {
                    TriggerChallenge(business);
                }
            }
        }
    }

    private bool CheckScenarioCompletion(Business business, Scenario scenario)
    {
        foreach (var objective in scenario.Objectives)
        {
            if (!IsObjectiveCompleted(business, objective))
                return false;
        }
        return true;
    }

    private bool IsObjectiveCompleted(Business business, string objective)
    {
        switch (objective.ToLower())
        {
            case "reach $1m in capital":
                return business.Capital >= 1000000f;
            case "hire 10 employees":
                return business.Employees.Count >= 10;
            case "launch 2 products":
                return business.Products.Count >= 2;
            case "reach 30% market share":
                return business.MarketShare >= 0.3f;
            case "complete 5 acquisitions":
                if (BusinessManager.Instance != null)
                {
                    return business.MergerExperience >= 5;
                }
                return false;
            case "go public":
                return business.IsPublic;
            case "complete 15 research projects":
                return business.CompletedResearch.Count >= 15;
            case "file 5 patents":
                if (RnDSystem.Instance != null)
                {
                    var (_, patents, _) = RnDSystem.Instance.GetRnDStatus(business);
                    return patents.Count >= 5;
                }
                return false;
            case "achieve 50% r&d efficiency":
                return business.ResearchEfficiency >= 0.5f;
            case "enter 5 countries":
                 return business.InternationalPresence >= 5;
            case "create 3 subsidiaries":
                return business.InternationalPresence >= 3;
            case "achieve $10m international revenue":
                 return false; 
            case "resolve 10 crises":
                return false; 
            case "maintain 80% reputation":
                return business.Reputation >= 80f;
            case "avoid bankruptcy":
                return business.Capital > 0;
            default:
                return false;
        }
    }

    private void CompleteScenario(Business business, Scenario scenario)
    {
        scenario.IsCompleted = true;
        business.Capital += scenario.Reward;
        
        if (!completedScenarios.ContainsKey(business))
            completedScenarios[business] = new List<Scenario>();
        completedScenarios[business].Add(scenario);
        
        activeScenarios.Remove(business);
        RemoveScenarioModifiers(business, scenario);
        
        Debug.Log($"Scenario completed: {scenario.Name}! Reward: ${scenario.Reward:N0}");
    }

    private void FailScenario(Business business, Scenario scenario)
    {
        activeScenarios.Remove(business);
        RemoveScenarioModifiers(business, scenario);
        Debug.Log($"Scenario failed: {scenario.Name}");
    }

    private void ApplyScenarioModifiers(Business business, Scenario scenario)
    {
        foreach (var modifier in scenario.Modifiers)
        {
            switch (modifier.Key)
            {
                case "CapitalGrowth":
                    business.Capital *= modifier.Value;
                    break;
                case "EmployeeCost":
                    // Applied in employee hiring
                    break;
                case "MarketShare":
                    business.MarketShare *= modifier.Value;
                    break;
                case "ResearchSpeed":
                    business.ResearchEfficiency *= modifier.Value;
                    break;
            }
        }
    }

    private void RemoveScenarioModifiers(Business business, Scenario scenario)
    {
        foreach (var modifier in scenario.Modifiers)
        {
            switch (modifier.Key)
            {
                case "CapitalGrowth":
                    business.Capital /= modifier.Value;
                    break;
                case "MarketShare":
                    business.MarketShare /= modifier.Value;
                    break;
                case "ResearchSpeed":
                    business.ResearchEfficiency /= modifier.Value;
                    break;
            }
        }
    }

    private void TriggerChallenge(Business business)
    {
        if (!activeChallenges.ContainsKey(business))
            activeChallenges[business] = new List<Challenge>();

        string[] challengeTypes = { "Speed", "Efficiency", "Innovation", "Survival" };
        string type = challengeTypes[Random.Range(0, challengeTypes.Length)];

        var challenge = CreateChallenge(type);
        if (challenge != null)
        {
            activeChallenges[business].Add(challenge);
            Debug.Log($"New challenge: {challenge.Name}");
        }
    }

    private Challenge CreateChallenge(string type)
    {
        switch (type)
        {
            case "Speed":
                return new Challenge("Speed Run", "Achieve rapid growth", "Speed", 1000000f, 50f);
            case "Efficiency":
                return new Challenge("Efficiency Master", "Maximize operational efficiency", "Efficiency", 2.0f, 100f);
            case "Innovation":
                return new Challenge("Innovation Sprint", "Complete research quickly", "Innovation", 5f, 75f);
            case "Survival":
                return new Challenge("Survival Mode", "Survive with limited resources", "Survival", 100f, 200f);
            default:
                return null;
        }
    }

    private void UpdateChallengeProgress(Business business, Challenge challenge)
    {
        switch (challenge.Type)
        {
            case "Speed":
                challenge.CurrentValue = business.GetOverallPerformance();
                break;
            case "Efficiency":
                challenge.CurrentValue = business.ProductionEfficiency;
                break;
            case "Innovation":
                challenge.CurrentValue = business.InnovationScore;
                break;
            case "Survival":
                challenge.CurrentValue = business.Capital;
                break;
        }

        if (challenge.CurrentValue >= challenge.TargetValue)
        {
            challenge.IsCompleted = true;
        }
    }

    private void CompleteChallenge(Business business, Challenge challenge)
    {
        float reward = challenge.TargetValue * 0.1f;
        business.Capital += reward;
        Debug.Log($"Challenge completed: {challenge.Name}! Reward: ${reward:N0}");
    }

    private void FailChallenge(Business business, Challenge challenge)
    {
        Debug.Log($"Challenge failed: {challenge.Name}");
    }

    private void TriggerSpecialEvent(Business business)
    {
        if (!specialEvents.ContainsKey(business))
            specialEvents[business] = new List<SpecialEvent>();

        var specialEvent = CreateSpecialEvent();
        if (specialEvent != null)
        {
            specialEvent.IsActive = true;
            specialEvents[business].Add(specialEvent);
            Debug.Log($"Special event: {specialEvent.Name}");
        }
    }

    private SpecialEvent CreateSpecialEvent()
    {
        string[] eventTypes = { "Economic", "Technological", "Social", "Environmental" };
        string type = eventTypes[Random.Range(0, eventTypes.Length)];

        switch (type)
        {
            case "Economic":
                var economic = new SpecialEvent("Economic Boom", "Favorable economic conditions", "Economic", 30f);
                economic.Effects["Revenue"] = 1.3f;
                economic.Effects["MarketGrowth"] = 1.2f;
                return economic;
            case "Technological":
                var tech = new SpecialEvent("Tech Revolution", "Rapid technological advancement", "Technological", 25f);
                tech.Effects["ResearchSpeed"] = 1.5f;
                tech.Effects["Innovation"] = 1.4f;
                return tech;
            case "Social":
                var social = new SpecialEvent("Social Movement", "Changing social preferences", "Social", 20f);
                social.Effects["CustomerSatisfaction"] = 1.2f;
                social.Effects["BrandValue"] = 1.3f;
                return social;
            case "Environmental":
                var environmental = new SpecialEvent("Environmental Crisis", "Environmental challenges", "Environmental", 35f);
                environmental.Effects["ComplianceCost"] = 1.5f;
                environmental.Effects["Reputation"] = 0.8f;
                return environmental;
            default:
                return null;
        }
    }

    private void ApplySpecialEventEffects(Business business, SpecialEvent specialEvent)
    {
        foreach (var effect in specialEvent.Effects)
        {
            switch (effect.Key)
            {
                case "Revenue":
                    business.Revenue *= effect.Value;
                    break;
                case "ResearchSpeed":
                    business.ResearchEfficiency *= effect.Value;
                    break;
                case "ComplianceCost":
                    // Applied in regulatory system
                    break;
            }
        }
    }

    private void EndSpecialEvent(Business business, SpecialEvent specialEvent)
    {
        specialEvent.IsActive = false;
        Debug.Log($"Special event ended: {specialEvent.Name}");
    }

    public (Scenario, List<Challenge>, List<SpecialEvent>, List<Scenario>) GetScenarioStatus(Business business)
    {
        Scenario activeScenario = activeScenarios.ContainsKey(business) ? activeScenarios[business] : null;
        List<Challenge> challenges = activeChallenges.ContainsKey(business) ? activeChallenges[business] : new List<Challenge>();
        List<SpecialEvent> events = specialEvents.ContainsKey(business) ? specialEvents[business] : new List<SpecialEvent>();
        List<Scenario> completed = completedScenarios.ContainsKey(business) ? completedScenarios[business] : new List<Scenario>();

        return (activeScenario, challenges, events, completed);
    }

    public List<string> GetAvailableScenarios()
    {
        return new List<string> { "Startup Challenge", "Market Domination", "Innovation Race", "Global Expansion", "Crisis Management" };
    }
} 