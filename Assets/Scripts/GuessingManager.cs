using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    GameObject easyDifficultyButton = null;
    [SerializeField]
    GameObject normalDifficultyButton = null;
    [SerializeField]
    GameObject hardDifficultyButton = null;
    [SerializeField]
    GameObject canvas = null;
    [SerializeField]
    GameObject passDougPopup = null;
    [SerializeField]
    GameObject failDougPopup = null;
    [SerializeField]
    GameObject passDougPopupStartPositionObject = null;
    [SerializeField]
    GameObject failDougPopupStartPositionObject = null;
    [SerializeField]
    GameObject failDougPopupEndPositionObject = null;
    [SerializeField]
    GameObject scorePanel = null;

    private Queue<KeyValuePair<int, int>> swaps = new Queue<KeyValuePair<int, int>>();
    private int randomCup = 1;
    private bool swapping = false;
    private float cupHeight;
    private float biscoHeight;
    private float cup1X;
    private float cup2X;
    private float cup3X;
    private int timesToSwap = 1;
    private float swapSpeed = 1f;
    private int difficulty = -1;
    private Vector3 passPopupStartPosition;
    private Vector3 failPopupStartPosition;
    private Vector3 failPopupEndPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateStats", 60f, 60f * 1f);

        cupHeight = cupHeightLine.transform.localPosition.y;
        biscoHeight = biscoHeightLine.transform.localPosition.y;
        cup1X = cup1Line.transform.localPosition.x;
        cup2X = cup2Line.transform.localPosition.x;
        cup3X = cup3Line.transform.localPosition.x;
        passPopupStartPosition = passDougPopupStartPositionObject.transform.localPosition;
        failPopupStartPosition = failDougPopupStartPositionObject.transform.localPosition;
        failPopupEndPosition = failDougPopupEndPositionObject.transform.localPosition;

        cup1.transform.LeanSetLocalPosY(cupHeight);
        cup2.transform.LeanSetLocalPosY(cupHeight);
        cup3.transform.LeanSetLocalPosY(cupHeight);

        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStats()
    {
        HomeScreenManager.ModifyStat(HomeScreenManager.Stat.Weight, -1, PersistentGameManager.instance.playerData.playerData);
        PersistentGameManager.instance.SaveGame();
    }

    public void GoToHomeScreen()
    {
        PersistentGameManager.instance.SwitchScene((int) SceneIndexes.HOME_SCREEN);
    }


    public void StartSwapping(int _difficulty)
    {
        difficulty = _difficulty;
        switch (_difficulty)
        {
            case 0:
                timesToSwap = GlobalConfig.easyTimesToSwap;
                swapSpeed = GlobalConfig.easyswapSpeed;
                break;
            case 1:
                timesToSwap = GlobalConfig.normalTimesToSwap;
                swapSpeed = GlobalConfig.normalswapSpeed;
                break;
            case 2:
                timesToSwap = GlobalConfig.hardTimesToSwap;
                swapSpeed = GlobalConfig.hardswapSpeed;
                break;
        }

        easyDifficultyButton.SetActive(false);
        normalDifficultyButton.SetActive(false);
        hardDifficultyButton.SetActive(false);
        scorePanel.LeanMoveLocalX(scorePanel.transform.localPosition.x + 400f, 1f);
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
        yield return new WaitForSeconds(swapSpeed);
        NextAnimation();
    }

    void NextAnimation()
    {
        if (swaps.Count == 0)
        {
            cup1.LeanMoveLocalY(biscoHeight, 1f);
            cup2.LeanMoveLocalY(biscoHeight, 1f);
            cup3.LeanMoveLocalY(biscoHeight, 1f).setOnComplete(() => {SetCupButtonActive(true);});
            return;
        }
        KeyValuePair<int, int> temp = swaps.Dequeue();
        StartCoroutine(Swap(temp.Key, temp.Value, swapSpeed));
    }

    public void SetCupButtonActive(bool state)
    {
        cup1.GetComponent<UnityEngine.UI.Button>().interactable = state;
        cup2.GetComponent<UnityEngine.UI.Button>().interactable = state;
        cup3.GetComponent<UnityEngine.UI.Button>().interactable = state;
    }

    public void ModifyText(TMP_Text text, string str)
    {
        text.SetText(str);
    }

    public void UpdateText()
    {
        ModifyText(scorePanel.transform.Find("Easy Score Text").GetComponent<TextMeshProUGUI>(), "easy: " + PersistentGameManager.instance.playerData.playerData.guessingEasyHiScore.ToString());
        ModifyText(scorePanel.transform.Find("Normal Score Text").GetComponent<TextMeshProUGUI>(), "ok: " + PersistentGameManager.instance.playerData.playerData.guessingNormalHiScore.ToString());
        ModifyText(scorePanel.transform.Find("Hard Score Text").GetComponent<TextMeshProUGUI>(), "no: " + PersistentGameManager.instance.playerData.playerData.guessingHardHiScore.ToString());
    }

    public void IncrementScore()
    {
        switch (difficulty)
        {
            case 0:
                PersistentGameManager.instance.playerData.playerData.guessingEasyHiScore++;
                break;
            case 1:
                PersistentGameManager.instance.playerData.playerData.guessingNormalHiScore++;
                break;
            case 2:
                PersistentGameManager.instance.playerData.playerData.guessingHardHiScore++;
                break;
        }
        HomeScreenManager.ModifyStat(HomeScreenManager.Stat.Boredom, -2, PersistentGameManager.instance.playerData.playerData);
        PersistentGameManager.instance.SaveGame();
        UpdateText();
    }

    public void RevealBisco(int i)
    {
        if (cup1.transform.Find("Bisco") != null)
        {
            if (i == 0)
            {
                MakePopupDoug(true);
                GetComponent<AudioManager>().PlaySound("bork");
                IncrementScore();
            }
            else
            {
                MakePopupDoug(false);
                GetComponent<AudioManager>().PlaySound("boo");
            }
            bisco.transform.SetParent(cup1.transform.parent);
            bisco.transform.localPosition = new Vector3(cup1.transform.localPosition.x, biscoHeight, bisco.transform.localPosition.z);
        }
        else if (cup2.transform.Find("Bisco") != null)
        {
            if (i == 1)
            {
                MakePopupDoug(true);
                GetComponent<AudioManager>().PlaySound("bork");
                IncrementScore();
            }
            else
            {
                MakePopupDoug(false);
                GetComponent<AudioManager>().PlaySound("boo");
            }
            bisco.transform.SetParent(cup2.transform.parent);
            bisco.transform.localPosition = new Vector3(cup2.transform.localPosition.x, biscoHeight, bisco.transform.localPosition.z);
        }
        else if (cup3.transform.Find("Bisco") != null)
        {
            if (i == 2)
            {
                MakePopupDoug(true);
                GetComponent<AudioManager>().PlaySound("bork");
                IncrementScore();
            }
            else
            {
                MakePopupDoug(false);
                GetComponent<AudioManager>().PlaySound("boo");
            }
            bisco.transform.SetParent(cup3.transform.parent);
            bisco.transform.localPosition = new Vector3(cup3.transform.localPosition.x, biscoHeight, bisco.transform.localPosition.z);
        }

        bisco.SetActive(true);
        randomCup = ClosestPosition(bisco);
        cup1.LeanMoveLocalX(cup1X, .5f).trans.LeanMoveLocalY(cupHeight, .5f);
        cup2.LeanMoveLocalX(cup2X, .5f).trans.LeanMoveLocalY(cupHeight, .5f);
        cup3.LeanMoveLocalX(cup3X, .5f).trans.LeanMoveLocalY(cupHeight, .5f).setOnComplete(() => {SetCupButtonActive(false);});
        swapping = false;
        easyDifficultyButton.SetActive(true);
        normalDifficultyButton.SetActive(true);
        hardDifficultyButton.SetActive(true);
        scorePanel.LeanMoveLocalX(scorePanel.transform.localPosition.x - 400f, 1f);
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

    public void MakePopupDoug(bool pass)
    {
        GameObject popupInstance;
        float popupTime = 1.5f;
        if (pass)
        {
            popupInstance = Instantiate(passDougPopup, canvas.transform.Find("Background Image").transform, true);
            popupInstance.transform.localPosition = passPopupStartPosition;
            popupInstance.transform.localScale = passDougPopup.transform.localScale;
            popupInstance.LeanRotateZ(-1500f, popupTime).setOnComplete(() => {Destroy(popupInstance, 0f);});
        }
        else
        {
            popupInstance = Instantiate(failDougPopup, canvas.transform.Find("Background Image").transform, true);
            popupInstance.transform.localPosition = failPopupStartPosition;
            popupInstance.transform.localScale = failDougPopup.transform.localScale;
            popupInstance.LeanRotateZ(-1500f, popupTime);
            popupInstance.LeanMoveLocalY(cupHeight, popupTime / 2).setEaseOutQuart().setLoopPingPong(1);
            popupInstance.LeanMoveLocalX(failPopupEndPosition.x, popupTime).setOnComplete(() => {Destroy(popupInstance, 0f);});
        }
    }
}
