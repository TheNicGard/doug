using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject versionNumberText = null;
    [SerializeField] GameObject versionNoticeText = null;

    // Start is called before the first frame update
    void Start()
    {
        versionNumberText.GetComponent<TMPro.TextMeshProUGUI>().text = "v. " + GlobalConfig.major_version + "." + GlobalConfig.minor_version;
    }

    bool instantiated = false;
    // Update is called once per frame
    void Update()
    {
        if (!instantiated && PersistentGameManager.instance != null)
        {
            if (GlobalConfig.major_version != PersistentGameManager.instance.playerData.playerData.savefileVersionMajor &&
            GlobalConfig.minor_version != PersistentGameManager.instance.playerData.playerData.savefileVersionMinor)
            versionNoticeText.SetActive(true);
            instantiated = true;
        }
    }
}
