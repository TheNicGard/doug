using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorSpriteMovement : MonoBehaviour
{
    public float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        var originalPosition = transform.localPosition;
        LeanTween.moveLocalX(gameObject, originalPosition.x + 2f, speed / 3).setEaseInOutSine().setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
