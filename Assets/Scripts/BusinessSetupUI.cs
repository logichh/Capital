using UnityEngine;
using UnityEngine.UI;

public class BusinessSetupUI : MonoBehaviour
{
    public InputField nameInput;
    public Dropdown industryDropdown;
    public Dropdown regionDropdown;
    public InputField capitalInput;
    public GameObject setupPanel;
    public GameObject mainGamePanel;

    public void OnStartGame()
    {
        string name = nameInput.text;
        string industry = industryDropdown.options[industryDropdown.value].text;
        string region = regionDropdown.options[regionDropdown.value].text;
        float capital = float.TryParse(capitalInput.text, out float c) ? c : 100000f;
        BusinessManager.Instance.CreatePlayerBusiness(name, industry, region, capital);
        setupPanel.SetActive(false);
        mainGamePanel.SetActive(true);
    }
} 