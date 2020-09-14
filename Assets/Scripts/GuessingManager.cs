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
    GameObject cupHeightLine = null;
    [SerializeField]
    GameObject biscoHeightLine = null;
    [SerializeField]
    GameObject cup1Line = null;
    [SerializeField]
    GameObject cup2Line = null;
    [SerializeField]
    GameObject cup3Line = null;
    [SerializeField]
    int timesToSwap = 5;
    [SerializeField]
    float swapSpeed = .4f;
    [SerializeField]
    float swapDelay = .4f;

    private Queue<KeyValuePair<int, int>> swaps = new Queue<KeyValuePair<int, int>>();
    private int randomCup = 1;
    private bool swapping = false;
    private float cupHeight;
    private float biscoHeight;
    private float cup1X;
    private float cup2X;
    private float cup3X;

    // Start is called before the first frame update
    void Start()
    {
        cupHeight = cupHeightLine.transform.localPosition.y;
        biscoHeight = biscoHeightLine.transform.localPosition.y;
        cup1X = cup1Line.transform.localPosition.x;
        cup2X = cup2Line.transform.localPosition.x;
        cup3X = cup3Line.transform.localPosition.x;

        cup1.transform.LeanSetLocalPosY(cupHeight);
        cup2.transform.LeanSetLocalPosY(cupHeight);
        cup3.transform.LeanSetLocalPosY(cupHeight);
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
        if (!swapping)
        {
            swapping = true;
            cup1.LeanMoveLocalY(biscoHeight, 1f);
            cup2.LeanMoveLocalY(biscoHeight, 1f);
            cup3.LeanMoveLocalY(biscoHeight, 1f).setOnComplete(StartSwappingHelper);
        }
    }

    void StartSwappingHelper()
    {
        switch (randomCup)
        {
            case 0: bisco.transform.SetParent(cup1.transform); break;
            case 1: bisco.transform.SetParent(cup2.transform); break;
            case 2: bisco.transform.SetParent(cup3.transform); break;
        }
        
        bisco.SetActive(false);
        for (int i = 0; i < timesToSwap; i++)
        {
            Random.InitState(System.DateTime.Now.Millisecond + i);
            int numA = Random.Range(0, 3);
            int numB = Random.Range(0, 3);
            while (numB == numA)
                numB = Random.Range(0, 3);
            
            swaps.Enqueue(new KeyValuePair<int, int>(numA, numB));
        }
        NextAnimation();
    }

    IEnumerator Swap(int cupA, int cupB, float speed)
    {
        GameObject cupAObject = null, cupBObject = null;

        GameObject[] cups = new GameObject[]{cup1, cup2, cup3};
        cupAObject = cups[cupA];
        cupBObject = cups[cupB];

        Vector3 cupAPosition = cupAObject.transform.localPosition;
        Vector3 cupBPosition = cupBObject.transform.localPosition;

        cupAObject.LeanMoveLocalY(cupBPosition.y + 200, speed / 2f).setEaseOutQuad().setLoopPingPong(1).trans.LeanMoveLocalX(cupBPosition.x, speed);
        cupBObject.LeanMoveLocalY(cupAPosition.y - 200, speed / 2f).setEaseOutQuad().setLoopPingPong(1).trans.LeanMoveLocalX(cupAPosition.x, speed);
        yield return new WaitForSeconds(swapDelay);
        NextAnimation();
    }

    void NextAnimation()
    {
        if (swaps.Count == 0)
        {
            cup1.LeanMoveLocalY(biscoHeight, 1f);
            cup2.LeanMoveLocalY(biscoHeight, 1f);
            cup3.LeanMoveLocalY(biscoHeight, 1f);
            return;
        }
        KeyValuePair<int, int> temp = swaps.Dequeue();
        StartCoroutine(Swap(temp.Key, temp.Value, swapSpeed));
    }

    public void RevealBisco(int i)
    {
        if (cup1.transform.Find("Bisco") != null)
        {
            if (i == 0)
                Debug.Log("good!");
            else
                Debug.Log("bad!");
            bisco.transform.SetParent(cup1.transform.parent);
            bisco.transform.localPosition = cup1.transform.localPosition;
        }
        else if (cup2.transform.Find("Bisco") != null)
        {
            if (i == 1)
                Debug.Log("good!");
            else
                Debug.Log("bad!");
            bisco.transform.SetParent(cup2.transform.parent);
            bisco.transform.localPosition = cup2.transform.localPosition;
        }
        else if (cup3.transform.Find("Bisco") != null)
        {
            if (i == 2)
                Debug.Log("good!");
            else
                Debug.Log("bad!");
            bisco.transform.SetParent(cup3.transform.parent);
            bisco.transform.localPosition = cup3.transform.localPosition;
        }

        bisco.SetActive(true);
        randomCup = ClosestPosition(bisco);
        cup1.LeanMoveLocalX(cup1X, .5f).trans.LeanMoveLocalY(cupHeight, .5f);
        cup2.LeanMoveLocalX(cup2X, .5f).trans.LeanMoveLocalY(cupHeight, .5f);
        cup3.LeanMoveLocalX(cup3X, .5f).trans.LeanMoveLocalY(cupHeight, .5f);
        swapping = false;
    }

    // oh my god there has to be an easier way to do this
    int ClosestPosition(GameObject bisco)
    {
        float[] distance = new float[]
        {
            Vector2.Distance(bisco.transform.localPosition, new Vector2(cup1X, biscoHeight)),
            Vector2.Distance(bisco.transform.localPosition, new Vector2(cup2X, biscoHeight)),
            Vector2.Distance(bisco.transform.localPosition, new Vector2(cup3X, biscoHeight))
        };
        
        float smallest = float.MaxValue;
        int pos = 0;

        for (int i = 0; i < 3; i++)
            if (distance[i] < smallest)
            {
                smallest = distance[i];
                pos = i;
            }

        return pos;
    }
}
