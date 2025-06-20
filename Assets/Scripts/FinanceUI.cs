using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class FinanceUI : MonoBehaviour
{
    [Header("Loan UI")]
    public TMP_InputField loanAmountInput;
    public TMP_InputField loanDurationInput;
    public Button takeLoanButton;
    public TextMeshProUGUI loansInfoText;

    [Header("Investment UI")]
    public TMP_Dropdown investmentTypeDropdown;
    public TMP_InputField investmentAmountInput;
    public TMP_InputField investmentDurationInput;
    public Button makeInvestmentButton;
    public TextMeshProUGUI investmentsInfoText;

    [Header("Tax Info")]
    public TextMeshProUGUI taxInfoText;

    private void Start()
    {
        // Setup investment types
        investmentTypeDropdown.ClearOptions();
        investmentTypeDropdown.AddOptions(new System.Collections.Generic.List<string> { "Stocks", "Bonds", "Real Estate" });

        // Setup button listeners
        takeLoanButton.onClick.AddListener(OnTakeLoan);
        makeInvestmentButton.onClick.AddListener(OnMakeInvestment);

        // Initial UI update
        UpdateFinancialInfo();
    }

    public void OnTakeLoan()
    {
        if (float.TryParse(loanAmountInput.text, out float amount) &&
            int.TryParse(loanDurationInput.text, out int duration))
        {
            var business = BusinessManager.Instance.PlayerBusiness;
            if (business != null && FinanceSystem.Instance.TakeLoan(business, amount, duration))
            {
                Debug.Log($"Loan taken: ${amount:N0} for {duration} ticks");
                UpdateFinancialInfo();
            }
            else
            {
                Debug.Log("Could not take loan - check credit worthiness");
            }
        }
    }

    public void OnMakeInvestment()
    {
        if (float.TryParse(investmentAmountInput.text, out float amount) &&
            int.TryParse(investmentDurationInput.text, out int duration))
        {
            string type = investmentTypeDropdown.options[investmentTypeDropdown.value].text;
            var business = BusinessManager.Instance.PlayerBusiness;
            if (business != null && FinanceSystem.Instance.MakeInvestment(business, type, amount, duration))
            {
                Debug.Log($"Investment made: ${amount:N0} in {type} for {duration} ticks");
                UpdateFinancialInfo();
            }
            else
            {
                Debug.Log("Could not make investment - insufficient funds");
            }
        }
    }

    public void UpdateFinancialInfo()
    {
        var business = BusinessManager.Instance.PlayerBusiness;
        if (business == null) return;

        var (loans, investments) = FinanceSystem.Instance.GetFinancialStatus(business);

        // Update loans info
        string loansText = "Active Loans:\n";
        foreach (var loan in loans)
        {
            loansText += $"Amount: ${loan.Amount:N0}\n";
            loansText += $"Interest Rate: {loan.InterestRate:P1}\n";
            loansText += $"Monthly Payment: ${loan.MonthlyPayment:N0}\n";
            loansText += $"Remaining Ticks: {loan.RemainingTicks}\n\n";
        }
        loansInfoText.text = loansText;

        // Update investments info
        string investmentsText = "Active Investments:\n";
        foreach (var investment in investments)
        {
            investmentsText += $"Type: {investment.Type}\n";
            investmentsText += $"Amount: ${investment.Amount:N0}\n";
            investmentsText += $"Return Rate: {investment.ReturnRate:P1}\n";
            investmentsText += $"Remaining Ticks: {investment.RemainingTicks}\n\n";
        }
        investmentsInfoText.text = investmentsText;

        // Update tax info
        float revenue = business.Revenue;
        float expenses = business.Employees.Sum(e => e.Wage) + 
                        business.Products.Sum(p => p.Cost * p.Inventory);
        float taxes = FinanceSystem.Instance.CalculateTaxes(business, revenue, expenses);
        
        taxInfoText.text = $"Financial Summary:\n" +
                          $"Revenue: ${revenue:N0}\n" +
                          $"Expenses: ${expenses:N0}\n" +
                          $"Taxable Income: ${(revenue - expenses):N0}\n" +
                          $"Estimated Tax: ${taxes:N0}";
    }
} 