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
        versionNumberText.GetComponent<TMPro.TextMeshProUGUI>().text = "v. " + Application.version;
    }

    bool instantiated = false;
    // Update is called once per frame
    void Update()
    {
        if (!instantiated && PersistentGameManager.instance != null)
        {
            if (Application.version != PersistentGameManager.instance.playerData.playerData.savefileVersion)
            versionNoticeText.SetActive(true);
            instantiated = true;
        }
    }
}
