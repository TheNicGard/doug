using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBounce : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.1f);
    }

    public void Close()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.1f).setOnComplete(DisablePanel);
    }

    private void DisablePanel() //TODO: i'm defintely doing something wrong here
    {
        gameObject.SetActive(false);
    }
}
