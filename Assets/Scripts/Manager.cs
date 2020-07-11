using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    private Dictionary<string, Company> companyRegistry = new Dictionary<string, Company>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting the game");
        FindElements();
    }

    private void FindElements()
    {
        Company[] companies = GetComponentsInChildren<Company>();
        if(companies != null)
        {
            foreach(Company company in companies)
            {
                if (company == null) continue;
                companyRegistry.Add(company.companyName, company);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
