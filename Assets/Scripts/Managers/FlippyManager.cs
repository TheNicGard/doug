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
    private Transform originalTransform;
    

    // Start is called before the first frame update
    void Start()
    {
        respawnTimeAdjustmentAdjusted = respawnAdjustment / 2f;
        StartCoroutine(SpawnCycle());
        Time.timeScale = 0f;
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        startButton.SetActive(false);
        infoButton.SetActive(false);
        Destroy(doug);
        doug = (GameObject) Instantiate(dougPrefab) as GameObject;
        doug.transform.parent = background.transform;
        doug.transform.localScale = dougPrefab.transform.localScale;
        doug.transform.localPosition = dougPrefab.transform.localPosition;
        doug.GetComponent<FlippyInput>().manager = gameObject;
        
        playerIsDead = false;
        Time.timeScale = 1f;
        playerIsPlaying = true;
        
        score = 0;
        UpdateText();

        GameObject[] pipes = GameObject.FindGameObjectsWithTag("Pipe");
        GameObject[] pipeColliders = GameObject.FindGameObjectsWithTag("Pipe Collider");
        foreach (GameObject p in pipes) Destroy(p);
        foreach (GameObject pc in pipeColliders) Destroy(pc);
    }

    public void PlayerScored()
    {
        score++;
        PersistentGameManager.instance.audioManager.PlaySound("arf");
        UpdateText();
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
            SpawnPipes();
        }
    }
}
