using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Company : MonoBehaviour
{

    public string companyName;

    public float Price { get; set; }
    public float MinPrice { get; set; }
    public float MaxPrice { get; set; }

    public int AvailableShares { get; set; }

    private Text priceText;
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        FindElements();
        RandomiseStartState();
    }

    private void FindElements()
    {
        Text[] textElements = GetComponentsInChildren<Text>();
        if (textElements != null)
        {
            foreach (Text textElement in textElements)
            {
                if (textElement == null) continue;
                if (textElement.name.Equals("Price")) priceText = textElement;
            }
        }

        button = GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ButtonClick);
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = companyName;
        }
    }

    private void RandomiseStartState()
    {
        MinPrice = Random.Range(1.0f, 3.0f);
        MaxPrice = Random.Range(4.0f, 6.0f);
        Price = Random.Range(MinPrice, MaxPrice);
        AvailableShares = Random.Range(50, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (priceText != null) priceText.text = "$" + string.Format("{0:0.00}", Price) + " (" + AvailableShares + ")";
    }

    void ButtonClick()
    {
        Debug.Log("clicked on " + companyName);
    }

    public void SellShares(int amount)
    {
        // TODO: note amount so this can modify Price appropriately at next evaluation step
        AvailableShares -= amount;
    }

    public void BuyShares(int amount)
    {
        // TODO: note amount so this can modify Price appropriately at next evaluation step
        AvailableShares += amount;
    }
}
