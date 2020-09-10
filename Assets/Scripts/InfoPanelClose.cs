using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoPanelClose : MonoBehaviour
{
    void Awake()
    {
         EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
         gameObject.SetActive(false);
    }
}

// credit: http://answers.unity.com/answers/1649175/view.html