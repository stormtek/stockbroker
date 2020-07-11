using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    private Dictionary<string, Company> companyRegistry = new Dictionary<string, Company>();
    private Dictionary<Company, GameObject> companyGraphs = new Dictionary<Company, GameObject>();
    private DD_DataDiagram diagram;
    public float secondsPerDay = 2.0f;
    private float gameTime = 0.0f;
    private int gameDays = 0;
    private int gameWeeks = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting the game");
        FindElements();
        // TODO: prepopulate "history" of the past couple of years so that the graph has sensible data to start with
    }

    private void FindElements()
    {
        diagram = GetComponentInChildren<DD_DataDiagram>();
        Company[] companies = GetComponentsInChildren<Company>();
        if(companies != null)
        {
            foreach(Company company in companies)
            {
                if (company == null) continue;
                companyRegistry.Add(company.companyName, company);
                if (diagram != null)
                {
                    companyGraphs.Add(company, diagram.AddLine(company.companyName, company.Color));
                }
            }
        }
        // Slightly nasty hack, but the label bar needs to be enabled when adding lines to the graph but I don't want to see it after that
        Transform labelBar = FindLabelBar(transform);
        if (labelBar != null) labelBar.gameObject.SetActive(false);
    }

    private Transform FindLabelBar(Transform current)
    {
        if (current == null || current.childCount == 0) return null;
        Transform result = null;
        for(int i=0; i<current.childCount; i++)
        {
            Transform child = current.GetChild(i);
            if (child.name.Equals("LabelBar")) return child;
            Transform grandchild = FindLabelBar(child);
            if (grandchild != null) result = grandchild;
        }
        return result;
    }

    private void FixedUpdate()
    {
        gameTime += Time.deltaTime;
        if(gameTime - (gameDays * secondsPerDay) > secondsPerDay)
        {
            gameDays++;
            // TODO: trigger AI behaviours for the day
            Debug.Log("Day " + gameDays);
        }
        if(gameDays - (gameWeeks * 7) > 7)
        {
            gameWeeks++;
            UpdateGraph();
            Debug.Log("Week " + gameWeeks);
        }
    }

    private void UpdateGraph()
    {
        foreach (KeyValuePair<Company, GameObject> entry in companyGraphs)
        {
            Company company = entry.Key;
            GameObject graph = entry.Value;
            diagram.InputPoint(graph, new Vector2(1.0f, Random.Range(company.MinPrice, company.MaxPrice)));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
