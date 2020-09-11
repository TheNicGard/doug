using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuessingManager : MonoBehaviour
{
    [SerializeField]
    GameObject bisco = null;
    [SerializeField]
    GameObject cup1 = null;
    [SerializeField]
    GameObject cup2 = null;
    [SerializeField]
    GameObject cup3 = null;
    [SerializeField]
    int timesToSwap = 5;
    [SerializeField]
    float swapSpeed = .4f;

    private Vector3 cup1Position;
    private Vector3 cup2Position;
    private Vector3 cup3Position;
    private Queue<KeyValuePair<int, int>> swaps = new Queue<KeyValuePair<int, int>>();
    private float baseY;

    // Start is called before the first frame update
    void Start()
    {
        baseY = cup1.transform.localPosition.y;
        cup1Position = cup1.transform.localPosition;
        cup2Position = cup2.transform.localPosition;
        cup3Position = cup3.transform.localPosition;
        cup2.transform.LeanSetLocalPosY(baseY);
        cup3.transform.LeanSetLocalPosY(baseY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToHomeScreen()
    {
        SceneManager.LoadScene("HomeScreen");
    }

    public void StartSwapping()
    {
        for (int i = 0; i < timesToSwap; i++)
        {
            int numA = Random.Range(0, 3);
            int numB = Random.Range(0, 3);
            while (numB == numA)
                numB = Random.Range(0, 3);
            swaps.Enqueue(new KeyValuePair<int, int>(numA, numB));
        }
        NextAnimation();
    }
    
    public void ResetPositions()
    {
        cup1.transform.localPosition = cup1Position;
        cup2.transform.localPosition = cup2Position;
        cup3.transform.localPosition = cup3Position;
    }

    public void Swap(int cupA, int cupB, float speed)
    {
        if (cupA == cupB || cupA < 0 || cupB < 0 || cupA > 2 || cupB > 2)
        {
            Debug.LogError("Error swapping cups!");
            return;
        }

        GameObject cupAObject = null, cupBObject = null;

        switch (cupA)
        {
            case 0: cupAObject = cup1; break;
            case 1: cupAObject = cup2; break;
            case 2: cupAObject = cup3; break;
        }

        switch (cupB)
        {
            case 0: cupBObject = cup1; break;
            case 1: cupBObject = cup2; break;
            case 2: cupBObject = cup3; break;
        }

        Vector3 cupAPosition = cupAObject.transform.localPosition;
        Vector3 cupBPosition = cupBObject.transform.localPosition;

        cupAObject.LeanMoveLocalY(cupBPosition.y + 200, speed / 2f).setEaseOutQuad().setLoopPingPong(1).trans.LeanMoveLocalX(cupBPosition.x, speed);
        cupBObject.LeanMoveLocalY(cupAPosition.y - 200, speed / 2f).setEaseOutQuad().setLoopPingPong(1).trans.LeanMoveLocalX(cupAPosition.x, speed).setOnComplete(NextAnimation);
        //cupAObject.;
        //cupBObject.;
    }

    void NextAnimation()
    {
        if (swaps.Count == 0)
        {
            cup1.LeanMoveLocalY(baseY, 2f);
            cup2.LeanMoveLocalY(baseY, 2f);
            cup3.LeanMoveLocalY(baseY, 2f);
            return;
        }
        KeyValuePair<int, int> temp = swaps.Dequeue();
        Swap(temp.Key, temp.Value, swapSpeed);
    }
}
