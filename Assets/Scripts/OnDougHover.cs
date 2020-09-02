using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnDougHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 originalScale;
    public float animationSpeed = 2f;
    public float animationGrowth = 0.25f;

    void Start()
    {
        originalScale = transform.localScale;
        LeanTween.scale(gameObject, originalScale * (1 - animationGrowth), animationSpeed * 2).setFrom(originalScale).setLoopPingPong().setEaseInOutSine();
    }


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, originalScale * (1 - animationGrowth), animationSpeed).setFrom(originalScale * (1 + animationGrowth)).setLoopPingPong().setEaseInOutSine();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, originalScale * (1 - animationGrowth), animationSpeed * 2).setFrom(originalScale).setLoopPingPong().setEaseInOutSine();
    }

}
