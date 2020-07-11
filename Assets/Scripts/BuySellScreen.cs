using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class BuySellScreen : MonoBehaviour
{
    public List<Company> Companies { get; set; }

    private List<Text> headerElements = new List<Text>();
    private List<Dropdown> dropdownElements = new List<Dropdown>();
    private List<Text> subtotalElements = new List<Text>();
    private Text screenHeader = null;
    private Text totalElement = null;

    public delegate void Confirm();
    public delegate void Cancel();

    private Confirm currentConfirm;
    private Cancel currentCancel;

    private float totalCost = 0.0f;
    private string prefix = "";

    private List<int> limits = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        LoadElements();
        if(Companies != null)
        {
            int index = 0;
            foreach(Company company in Companies)
            {
                if (company == null) continue;
                if (index >= headerElements.Count) break;
                headerElements[index++].text = company.companyName;
            }
        }
        InitialiseDropdowns();
        UpdateValues(0);
    }

    private void LoadElements()
    {
        Text[] textElements = GetComponentsInChildren<Text>();
        if(textElements != null)
        {
            foreach(Text textElement in textElements)
            {
                if (textElement == null) continue;
                if (textElement.name.Contains("Header_")) headerElements.Add(textElement);
                else if (textElement.name.Contains("Subtotal_")) subtotalElements.Add(textElement);
                else if (textElement.name.Equals("Screen_Header")) screenHeader = textElement;
                else if (textElement.name.Equals("Total_Value")) totalElement = textElement;
            }
        }

        Dropdown[] dropdownFields = GetComponentsInChildren<Dropdown>();
        if(dropdownFields != null)
        {
            foreach(Dropdown dropdown in dropdownFields)
            {
                if (dropdown == null) continue;
                if (dropdown.name.Contains("Dropdown_")) dropdownElements.Add(dropdown);
                dropdown.onValueChanged.AddListener(UpdateValues);
            }
        }

        Button[] buttons = GetComponentsInChildren<Button>();
        if(buttons != null)
        {
            foreach(Button button in buttons)
            {
                if (button == null) continue;
                if (button.name.Equals("ConfirmButton")) button.onClick.AddListener(ConfirmClick);
                else if (button.name.Equals("CancelButton")) button.onClick.AddListener(CancelClick);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateValues(int triggerValue)
    {
        totalCost = 0.0f;
        for(int i=0; i< dropdownElements.Count && i <subtotalElements.Count; i++)
        {
            string value = dropdownElements[i].options[dropdownElements[i].value].text;
            int numericValue = 0;
            int.TryParse(value, out numericValue);
            float subtotal = Companies[i].Price * numericValue;
            subtotalElements[i].text = prefix + "$" + string.Format("{0:0.00}", subtotal);
            totalCost += subtotal;
        }
        if(totalElement != null) totalElement.text = prefix + "$" + string.Format("{0:0.00}", totalCost);
    }

    void InitialiseDropdowns()
    {
        for (int i = 0; i < dropdownElements.Count && i < limits.Count; i++)
        {
            List<string> options = new List<string>();
            for (int option = 0; option <= limits[i]; option++)
            {
                options.Add(option + "");
            }
            dropdownElements[i].options.Clear();
            dropdownElements[i].AddOptions(options);
            dropdownElements[i].value = 0;
        }
    }

    void ConfirmClick()
    {
        currentConfirm?.Invoke();
    }

    void CancelClick()
    {
        currentCancel?.Invoke();
    }

    public void Show(string header, string prefix, List<int> limits, Confirm confirm, Cancel cancel)
    {
        this.prefix = prefix;
        this.limits = limits;
        gameObject.SetActive(true);
        currentConfirm = confirm;
        currentCancel = cancel;
        InitialiseDropdowns();
        foreach (Text subtotal in subtotalElements)
        {
            if (subtotal == null) continue;
            subtotal.text = "";
        }
        if (screenHeader != null) screenHeader.text = header;
        if (totalElement != null) totalElement.text = "";
        UpdateValues(0);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public Dictionary<Company, int> GetSelectedValues()
    {
        Dictionary<Company, int> selectedValues = new Dictionary<Company, int>();

        if(Companies != null && dropdownElements != null)
        {
            for(int i=0; i< Companies.Count && i < dropdownElements.Count; i++)
            {
                string value = dropdownElements[i].options[dropdownElements[i].value].text;
                int numericValue = 0;
                int.TryParse(value, out numericValue);
                selectedValues.Add(Companies[i], numericValue);
            }
        }

        return selectedValues;
    }
}
