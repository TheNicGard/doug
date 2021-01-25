using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlippyManager : MonoBehaviour
{
    public int score = 0;
    public bool playerIsDead = false;
    public bool playerIsPlaying = false;
    [SerializeField] GameObject pipePrefab = null;
    [SerializeField] GameObject dougPrefab = null;
    [SerializeField] float initialWaitTime = 1f;
    [SerializeField] float respawnTime = 1.75f;
    [SerializeField] float respawnAdjustment = 0.25f;
    [SerializeField] float respawnTimeAdjustmentAdjusted;
    [SerializeField] GameObject background = null;
    [SerializeField] GameObject scoreText = null;
    [SerializeField] GameObject doug = null;
    [SerializeField] GameObject startButton = null;
    [SerializeField] GameObject startButtonText = null;
    [SerializeField] GameObject infoButton = null;
    [SerializeField] GameObject ground = null;
    private Transform originalTransform;
    public int pipeValue = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        respawnTimeAdjustmentAdjusted = respawnAdjustment / 2f;
        StartCoroutine(SpawnCycle());
        UpdateText();
        ground.GetComponent<UnityEngine.UI.Image>().sprite =
            Resources.Load<UnityEngine.Sprite>(GlobalConfig.wallpaperFileNames[(int) PersistentGameManager.instance.playerData.playerData.currentWallpaper]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GoToHomeScreen()
    {
        PersistentGameManager.instance.SwitchScene((int) SceneIndexes.HOME_SCREEN);
    }

    public void StartGame()
    {
        pipeValue = 0;

        GameObject[] pipes = GameObject.FindGameObjectsWithTag("Pipe");
        GameObject[] pipeColliders = GameObject.FindGameObjectsWithTag("Pipe Collider");
        foreach (GameObject p in pipes) Destroy(p);
        foreach (GameObject pc in pipeColliders) Destroy(pc);

        startButton.SetActive(false);
        infoButton.SetActive(false);
        Destroy(doug);
        doug = (GameObject) Instantiate(dougPrefab) as GameObject;
        doug.transform.parent = background.transform;
        doug.transform.localScale = dougPrefab.transform.localScale;
        doug.transform.localPosition = dougPrefab.transform.localPosition;
        doug.GetComponent<FlippyInput>().manager = gameObject;
        doug.GetComponent<Rigidbody2D>().gravityScale = 1f;
        
        playerIsDead = false;
        playerIsPlaying = true;
        
        score = 0;
        UpdateText();
    }

    public void PlayerScored(int pipeValue)
    {
        if (pipeValue > score)
        {
            score++;
            PersistentGameManager.instance.audioManager.PlaySound("arf");
            UpdateText();
            if (score % 5 == 0)
                PersistentGameManager.instance.ModifyStat(Stat.Boredom, -3);
        }
    }

    public void PlayerDied()
    {
        playerIsDead = true;
        PersistentGameManager.instance.audioManager.PlaySound("boo");
        if (score > PersistentGameManager.instance.playerData.playerData.flippyHiScore)
            PersistentGameManager.instance.playerData.playerData.flippyHiScore = score;
        startButtonText.GetComponent<TextMeshProUGUI>().text = "restart";
        startButton.SetActive(true);
        infoButton.SetActive(true);
        UpdateText();
    }
    

    public void UpdateText()
    {
        string t = "score: " + score.ToString() + "\n";
        t += "hi score: " + PersistentGameManager.instance.playerData.playerData.flippyHiScore;
        scoreText.GetComponent<TextMeshProUGUI>().text = t;
    }

    private void SpawnPipes()
    {
        var pipe = (GameObject) Instantiate(pipePrefab) as GameObject;
        pipe.transform.SetParent(background.transform);
        pipe.transform.localPosition = pipePrefab.transform.localPosition;
        pipe.transform.localScale = pipePrefab.transform.localScale;

        float heightAdjust = Random.Range(0f, 400f) * -1f;
        pipe.transform.localPosition += new Vector3(0f, heightAdjust, 0f);

        Destroy(pipe, 5f);
    }

    IEnumerator SpawnCycle()
    {
        yield return new WaitForSeconds(initialWaitTime + Random.Range(-respawnTimeAdjustmentAdjusted, respawnTimeAdjustmentAdjusted));
        while (true)
        {
            yield return new WaitForSeconds(respawnTime + Random.Range(-respawnTimeAdjustmentAdjusted, respawnTimeAdjustmentAdjusted));
            if (playerIsPlaying)  
                SpawnPipes();
        }
    }
}
