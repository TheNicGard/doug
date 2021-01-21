using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlippyInput : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb = null;
    [SerializeField] float thrustVelocity = 180f;
    [SerializeField] float maxVelocity = 260f;
    [SerializeField] public GameObject manager;
    public Vector3 originalPosition;
    public Vector3 originalScale;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = gameObject.transform.position;
        originalScale = gameObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current;
        Touchscreen touchscreen = Touchscreen.current;

        if (manager.GetComponent<FlippyManager>().playerIsPlaying && !manager.GetComponent<FlippyManager>().playerIsDead)
        {
            // these are hardcoded based on the velocity used during testing; let's hope it doesn't change
            gameObject.transform.eulerAngles = new Vector3(0f, 0f, (3.4f * rb.velocity.y) + 1.2f);

            if ((mouse != null && mouse.leftButton.wasPressedThisFrame) || (touchscreen != null && touchscreen.press.wasPressedThisFrame))
            {
                PersistentGameManager.instance.audioManager.PlaySound("flap");
                rb.AddForce(new Vector2(0f, thrustVelocity));
                if (rb.velocity.y > maxVelocity)
                    rb.velocity = new Vector3(0, maxVelocity, 0);
            }
        }   
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!manager.GetComponent<FlippyManager>().playerIsDead)
        {
            if (other.gameObject.tag == "Pipe" || other.gameObject.tag == "Ground")
            {
                rb.angularVelocity = 400f;
                rb.velocity = new Vector2(0f, 0f);
                manager.GetComponent<FlippyManager>().PlayerDied();
            }
            else if (other.gameObject.tag == "Pipe Collider")
            {
                manager.GetComponent<FlippyManager>().PlayerScored();
            }
        }
        else
        {
            if (other.gameObject.tag == "Death Floor")
            {
                if (manager.GetComponent<FlippyManager>().playerIsPlaying)
                {
                    manager.GetComponent<FlippyManager>().playerIsPlaying = false;
                }     
            }
        }
    }
}
