using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeactivationPanelClose : MonoBehaviour
{
    [SerializeField]
    GameObject manager = null;

    void OnDisable()
    {
        manager.GetComponent<HomeScreenManager>().StartRemoveDougCoroutine();
    }
}
