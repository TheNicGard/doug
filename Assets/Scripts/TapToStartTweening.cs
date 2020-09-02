using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapToStartTweening : MonoBehaviour
{
    Vector3 originalScale;
    public float animationSpeed = 2f;
    public float animationGrowth = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        LeanTween.scale(gameObject, originalScale * (1 - animationGrowth), animationSpeed * 2).setFrom(originalScale).setLoopPingPong().setEaseInOutSine();
    }
}
