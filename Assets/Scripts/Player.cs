using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public string username;
    public float BankBalance { get; set; }

    private List<Company> companies = new List<Company>();
    private Dictionary<Company, int> shareValues = new Dictionary<Company, int>();
    private Dictionary<Company, Text> shareValueLabels = new Dictionary<Company, Text>();

    private Text balanceLabel;

    private BuySellScreen buySellScreen;

    // Start is called before the first frame update
    void Start()
    {
        FindElements();
        BankBalance = Random.Range(1000.0f, 1500.0f);
    }

    private void FindElements()
    {
        Company[] companies = GetComponentsInChildren<Company>();
        if (companies != null)
        {
            foreach (Company company in companies)
            {
                if (company == null) continue;
                this.companies.Add(company);
                shareValues.Add(company, 0);
                Text[] textElements = company.GetComponentsInChildren<Text>();
                if(textElements != null)
                {
                    foreach(Text textElement in textElements)
                    {
                        if (textElement == null) continue;
                        if (textElement.name.Equals("Owned")) shareValueLabels.Add(company, textElement);
                    }
                }
            }
        }

        Text[] allTextElements = GetComponentsInChildren<Text>();
        if(allTextElements != null)
        {
            foreach(Text textElement in allTextElements)
            {
                if (textElement == null) continue;
                if (textElement.name.Equals("PlayerName")) textElement.text = username;
                else if (textElement.name.Equals("BankBalance")) balanceLabel = textElement;
            }
        }

        Button[] buttons = GetComponentsInChildren<Button>();
        if (buttons != null)
        {
            foreach (Button button in buttons)
            {
                if (button == null) continue;
                Text buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    if (buttonText.text.Equals("Buy")) button.onClick.AddListener(BuyClick);
                    else if (buttonText.text.Equals("Sell")) button.onClick.AddListener(SellClick);
                    else if (buttonText.text.Equals("Invest")) button.onClick.AddListener(InvestClick);
                }
            }
        }

        buySellScreen = GetComponentInChildren<BuySellScreen>(true);
        if(buySellScreen != null)
        {
            buySellScreen.Companies = this.companies;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Company company in companies)
        {
            if (company == null) continue;
            Text label = shareValueLabels[company];
            if (label == null) continue;
            int owned = shareValues[company];
            label.text = string.Format("{0:n0}", owned);
        }

        if(balanceLabel != null) balanceLabel.text = "$" + string.Format("{0:0.00}", BankBalance);
    }

    void BuyClick()
    {
        if (buySellScreen != null)
        {
            List<int> availableShares = new List<int>();
            foreach(Company company in companies)
            {
                if (company == null) continue;
                availableShares.Add(company.AvailableShares);
            }

            buySellScreen.Show("Buy Shares", "-", availableShares, () => // Confirm Click
            {
                Dictionary<Company, int> selectedValues = buySellScreen.GetSelectedValues();
                if(selectedValues != null)
                {
                    foreach (Company company in selectedValues.Keys)
                    {
                        BuyFromCompany(company, selectedValues[company]);
                    }
                }
                buySellScreen.Hide();
            }, () => // Cancel Click
            {
                buySellScreen.Hide();
            });
        }
    }

    void SellClick()
    {
        if (buySellScreen != null)
        {
            List<int> ownedShares = new List<int>();
            foreach(Company company in companies)
            {
                if (company == null) continue;
                ownedShares.Add(shareValues[company]);
            }

            buySellScreen.Show("Sell Shares", "+", ownedShares, () => // Confirm Click
            {
                Dictionary<Company, int> selectedValues = buySellScreen.GetSelectedValues();
                if (selectedValues != null)
                {
                    foreach (Company company in selectedValues.Keys)
                    {
                        SellFromCompany(company, selectedValues[company]);
                    }
                }
                buySellScreen.Hide();
            }, () => // Cancel Click
            {
                buySellScreen.Hide();
            });
        }
    }

    void InvestClick()
    {
        Debug.Log("Invest in: " + GetCompanyNamesForDisplay());
    }

    private string GetCompanyNamesForDisplay()
    {
        string companyNames = "[";
        foreach (Company company in companies)
        {
            companyNames += company.companyName + ", ";
        }
        companyNames = companyNames.Substring(0, companyNames.Length - 2) + "]";
        return companyNames;
    }

    private void BuyFromCompany(Company company, int amount)
    {
        int existingAmount = shareValues[company];
        existingAmount += amount;
        shareValues[company] = existingAmount;
        BankBalance -= amount * company.Price;
        company.SellShares(amount);
    }

    private void SellFromCompany(Company company, int amount)
    {
        int existingAmount = shareValues[company];
        existingAmount -= amount;
        shareValues[company] = existingAmount;
        BankBalance += amount * company.Price;
        company.BuyShares(amount);
    }

}
