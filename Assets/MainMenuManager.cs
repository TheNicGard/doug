using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject versionNumberText;

    // Start is called before the first frame update
    void Start()
    {
        versionNumberText.GetComponent<TMPro.TextMeshProUGUI>().text = "v. " + GlobalConfig.major_version + "." + GlobalConfig.minor_version;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
