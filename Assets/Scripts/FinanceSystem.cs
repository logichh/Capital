using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Loan
{
    public float Amount { get; private set; }
    public float InterestRate { get; private set; }
    public int DurationTicks { get; private set; }
    public int RemainingTicks { get; private set; }
    public float MonthlyPayment { get; private set; }

    public Loan(float amount, float interestRate, int durationTicks)
    {
        Amount = amount;
        InterestRate = interestRate;
        DurationTicks = durationTicks;
        RemainingTicks = durationTicks;
        // Simple monthly payment calculation
        MonthlyPayment = (amount * (1 + interestRate)) / durationTicks;
    }

    public float ProcessPayment()
    {
        if (RemainingTicks > 0)
        {
            RemainingTicks--;
            return MonthlyPayment;
        }
        return 0f;
    }
}

[System.Serializable]
public class Investment
{
    public string Type { get; private set; }
    public float Amount { get; private set; }
    public float ReturnRate { get; private set; }
    public int MaturityTicks { get; private set; }
    public int RemainingTicks { get; set; }

    public Investment(string type, float amount, float returnRate, int maturityTicks)
    {
        Type = type;
        Amount = amount;
        ReturnRate = returnRate;
        MaturityTicks = maturityTicks;
        RemainingTicks = maturityTicks;
    }

    public float GetMaturityValue()
    {
        return Amount * (1 + ReturnRate);
    }
}

public class FinanceSystem : MonoBehaviour
{
    public static FinanceSystem Instance { get; private set; }
    
    [SerializeField] private float baseTaxRate = 0.2f;
    [SerializeField] private float baseInterestRate = 0.05f;
    
    private Dictionary<Business, List<Loan>> businessLoans = new Dictionary<Business, List<Loan>>();
    private Dictionary<Business, List<Investment>> businessInvestments = new Dictionary<Business, List<Investment>>();

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

    public bool TakeLoan(Business business, float amount, int durationTicks)
    {
        if (!businessLoans.ContainsKey(business))
            businessLoans[business] = new List<Loan>();

        // Check credit worthiness (simple implementation)
        float totalLoans = businessLoans[business].Count > 0 ? 
            businessLoans[business].Sum(loan => loan.Amount) : 0f;
        
        if (totalLoans + amount > business.Capital * 2) // Can't borrow more than 2x capital
            return false;

        float interestRate = baseInterestRate * (1 + totalLoans / business.Capital);
        var loan = new Loan(amount, interestRate, durationTicks);
        businessLoans[business].Add(loan);
        business.Capital += amount;
        return true;
    }

    public bool MakeInvestment(Business business, string type, float amount, int maturityTicks)
    {
        if (business.Capital < amount)
            return false;

        if (!businessInvestments.ContainsKey(business))
            businessInvestments[business] = new List<Investment>();

        float returnRate = CalculateInvestmentReturn(type, amount, maturityTicks);
        var investment = new Investment(type, amount, returnRate, maturityTicks);
        businessInvestments[business].Add(investment);
        business.Capital -= amount;
        return true;
    }

    private float CalculateInvestmentReturn(string type, float amount, int maturityTicks)
    {
        // Simple investment return calculation
        switch (type.ToLower())
        {
            case "stocks":
                return 0.15f + Random.Range(-0.05f, 0.05f);
            case "bonds":
                return 0.05f + Random.Range(-0.01f, 0.01f);
            case "realestate":
                return 0.10f + Random.Range(-0.03f, 0.03f);
            default:
                return 0.08f + Random.Range(-0.02f, 0.02f);
        }
    }

    public float CalculateTaxes(Business business, float revenue, float expenses)
    {
        float taxableIncome = revenue - expenses;
        float taxRate = baseTaxRate;
        
        // Progressive tax rate based on income
        if (taxableIncome > 1000000f)
            taxRate += 0.1f;
        else if (taxableIncome > 500000f)
            taxRate += 0.05f;
        
        return Mathf.Max(0, taxableIncome * taxRate);
    }

    public void ProcessFinances(Business business)
    {
        if (businessLoans.ContainsKey(business))
        {
            foreach (var loan in businessLoans[business].ToList())
            {
                float payment = loan.ProcessPayment();
                business.Capital -= payment;
                
                if (loan.RemainingTicks <= 0)
                    businessLoans[business].Remove(loan);
            }
        }

        if (businessInvestments.ContainsKey(business))
        {
            foreach (var investment in businessInvestments[business].ToList())
            {
                if (--investment.RemainingTicks <= 0)
                {
                    business.Capital += investment.GetMaturityValue();
                    businessInvestments[business].Remove(investment);
                }
            }
        }
    }

    public (List<Loan>, List<Investment>) GetFinancialStatus(Business business)
    {
        List<Loan> loans = businessLoans.ContainsKey(business) ? businessLoans[business] : new List<Loan>();
        List<Investment> investments = businessInvestments.ContainsKey(business) ? businessInvestments[business] : new List<Investment>();
        return (loans, investments);
    }
} 