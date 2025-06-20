using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class RnDUI : MonoBehaviour
{
    [Header("Research Projects")]
    public Transform projectListContent;
    public GameObject projectEntryPrefab;
    public TMP_Dropdown projectCategoryDropdown;
    public Button startProjectButton;

    [Header("Active Research")]
    public Transform activeResearchContent;
    public GameObject activeResearchEntryPrefab;
    public TextMeshProUGUI researchProgressText;

    [Header("Researchers")]
    public Transform researcherListContent;
    public GameObject researcherEntryPrefab;
    public TMP_InputField researcherNameInput;
    public TMP_Dropdown specializationDropdown;
    public TMP_InputField researcherWageInput;
    public TMP_InputField researcherSkillInput;
    public TMP_InputField researcherEfficiencyInput;
    public Button hireResearcherButton;

    [Header("Patents")]
    public Transform patentListContent;
    public GameObject patentEntryPrefab;
    public TextMeshProUGUI totalPatentValueText;

    [Header("Research Stats")]
    public TextMeshProUGUI completedResearchText;
    public TextMeshProUGUI researchBudgetText;
    public TextMeshProUGUI efficiencyStatsText;

    private List<ResearchProject> availableProjects = new List<ResearchProject>();

    private void Start()
    {
        // Setup dropdowns
        projectCategoryDropdown.ClearOptions();
        projectCategoryDropdown.AddOptions(new System.Collections.Generic.List<string> { "All", "Technology", "Product", "Process", "Patent" });

        specializationDropdown.ClearOptions();
        specializationDropdown.AddOptions(new System.Collections.Generic.List<string> { "Technology", "Product", "Process", "Patent" });

        // Setup button listeners
        startProjectButton.onClick.AddListener(OnStartProject);
        hireResearcherButton.onClick.AddListener(OnHireResearcher);

        RefreshAvailableProjects();

        // Initial UI update
        UpdateRnDUI();
    }

    private void RefreshAvailableProjects()
    {
        var business = BusinessManager.Instance.PlayerBusiness;
        if (business != null)
            availableProjects = RnDSystem.Instance.GetAvailableProjects(business);
        else
            availableProjects.Clear();
    }

    public void OnStartProject()
    {
        var business = BusinessManager.Instance.PlayerBusiness;
        if (business == null) return;

        // Get selected project from UI (simplified - would need proper selection mechanism)
        var selectedProject = availableProjects.FirstOrDefault();
        if (selectedProject != null && RnDSystem.Instance.StartResearchProject(business, selectedProject))
        {
            Debug.Log($"Started research project: {selectedProject.Name}");
            RefreshAvailableProjects();
            UpdateRnDUI();
        }
        else
        {
            Debug.Log("Could not start research project - check prerequisites and funds");
        }
    }

    public void OnHireResearcher()
    {
        if (float.TryParse(researcherWageInput.text, out float wage) &&
            float.TryParse(researcherSkillInput.text, out float skill) &&
            float.TryParse(researcherEfficiencyInput.text, out float efficiency))
        {
            string name = researcherNameInput.text;
            if (string.IsNullOrEmpty(name))
                name = $"Researcher {Random.Range(1000, 9999)}";

            string specialization = specializationDropdown.options[specializationDropdown.value].text;
            var business = BusinessManager.Instance.PlayerBusiness;
            
            if (business != null && RnDSystem.Instance.HireResearcher(business, name, specialization, wage, skill, efficiency))
            {
                Debug.Log($"Hired researcher: {name} ({specialization})");
                UpdateRnDUI();
            }
            else
            {
                Debug.Log("Could not hire researcher - insufficient funds");
            }
        }
    }

    public void UpdateRnDUI()
    {
        var business = BusinessManager.Instance.PlayerBusiness;
        if (business == null) return;

        RefreshAvailableProjects();

        var (activeProjects, patents, researchers) = RnDSystem.Instance.GetRnDStatus(business);

        // Update available projects list
        foreach (Transform child in projectListContent)
            Destroy(child.gameObject);

        string selectedCategory = projectCategoryDropdown.options[projectCategoryDropdown.value].text;
        var filteredProjects = selectedCategory == "All" ? 
            availableProjects : 
            availableProjects.Where(p => p.Category == selectedCategory);

        foreach (var project in filteredProjects)
        {
            var entry = Instantiate(projectEntryPrefab, projectListContent).GetComponent<TextMeshProUGUI>();
            bool canStart = project.CanStart(business) && business.Capital >= project.BaseCost;
            string status = canStart ? "Available" : "Locked";
            
            entry.text = $"Project: {project.Name}\n" +
                        $"Category: {project.Category}\n" +
                        $"Cost: ${project.BaseCost:N0}\n" +
                        $"Duration: {project.DurationTicks} ticks\n" +
                        $"Success Rate: {project.SuccessChance:P0}\n" +
                        $"Status: {status}";
        }

        // Update active research
        foreach (Transform child in activeResearchContent)
            Destroy(child.gameObject);

        foreach (var project in activeProjects)
        {
            var entry = Instantiate(activeResearchEntryPrefab, activeResearchContent).GetComponent<TextMeshProUGUI>();
            entry.text = $"Project: {project.Name}\n" +
                        $"Progress: {project.Progress:P1}\n" +
                        $"Remaining: {project.RemainingTicks} ticks\n" +
                        $"Success Rate: {project.SuccessChance:P0}";
        }
        researchProgressText.text = $"Active Projects: {activeProjects.Count}";

        // Update researchers
        foreach (Transform child in researcherListContent)
            Destroy(child.gameObject);

        foreach (var researcher in researchers)
        {
            var entry = Instantiate(researcherEntryPrefab, researcherListContent).GetComponent<TextMeshProUGUI>();
            entry.text = $"Name: {researcher.Name}\n" +
                        $"Specialization: {researcher.Specialization}\n" +
                        $"Skill: {researcher.Skill:F2}\n" +
                        $"Efficiency: {researcher.ResearchEfficiency:F2}\n" +
                        $"Wage: ${researcher.Wage:N0}";
        }

        // Update patents
        foreach (Transform child in patentListContent)
            Destroy(child.gameObject);

        float totalPatentValue = 0f;
        foreach (var patent in patents)
        {
            var entry = Instantiate(patentEntryPrefab, patentListContent).GetComponent<TextMeshProUGUI>();
            entry.text = $"Patent: {patent.Name}\n" +
                        $"Value: ${patent.Value:N0}\n" +
                        $"Licensing Revenue: ${patent.LicensingRevenue:N0}/tick\n" +
                        $"Remaining: {patent.RemainingTicks} ticks";
            totalPatentValue += patent.Value;
        }
        totalPatentValueText.text = $"Total Patent Value: ${totalPatentValue:N0}";

        // Update stats
        completedResearchText.text = $"Completed Research: {business.CompletedResearch.Count}";
        researchBudgetText.text = $"Research Budget: ${business.ResearchBudget:N0}";
        efficiencyStatsText.text = $"Production Efficiency: {business.ProductionEfficiency:F2}\n" +
                                  $"Process Efficiency: {business.ProcessEfficiency:F2}\n" +
                                  $"Logistics Efficiency: {business.LogisticsEfficiency:F2}";
    }
} 