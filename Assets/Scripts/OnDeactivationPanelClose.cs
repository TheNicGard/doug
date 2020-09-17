using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeactivationPanelClose : MonoBehaviour
{
    [SerializeField]
    GameObject manager = null;

    void OnDisable()
    {
        if (manager != null)
            manager.GetComponent<HomeScreenManager>().StartRemoveDougCoroutine();
    }
}
