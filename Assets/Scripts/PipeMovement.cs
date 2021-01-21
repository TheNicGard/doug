using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using code from https://answers.unity.com/questions/284068/pauseing-and-resuming-a-rigidbody.html

public class PipeMovement : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] Rigidbody2D rb = null;
    private FlippyManager manager = null;
    private bool moving = true;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<FlippyManager>();
        if (manager.playerIsPlaying)
        {
            speed += (manager.score / 10f);
            rb = this.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector3(-speed, 0f, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
