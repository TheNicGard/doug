using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Button : MonoBehaviour
{
    public string buttonName;
    public int cost;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.Find("Name Text").gameObject.GetComponent<TextMeshProUGUI>().text = buttonName;
        gameObject.transform.Find("Cost Text").gameObject.GetComponent<TextMeshProUGUI>().text = cost.ToString() + "subs needed";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
