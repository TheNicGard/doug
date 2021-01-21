using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMovement : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] GameObject manager;
    [SerializeField] Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        speed += (GameObject.Find("Manager").GetComponent<FlippyManager>().score / 10f);
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector3(-speed, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
