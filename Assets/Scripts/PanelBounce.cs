using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBounce : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.1f);
    }

    public void Close()
    {
        LeanTween.scale(gameObject, new Vector3(0f, 0f, 0f), 0.1f).setOnComplete(ActuallyClose);
    }

    private void ActuallyClose() //TODO: i'm defintely doing something wrong here
    {
        gameObject.SetActive(false);
    }
}
