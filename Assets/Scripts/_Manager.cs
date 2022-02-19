using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class _Manager : MonoBehaviour
{
    private bool gameStarted;
    // Camera
    private Camera mCamera;
    private cameraFollow mCameraScript;

    // Bots
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject botPrefab;
    public List<Transform> playerList;
    private List<Transform> spawnPoints;
    private List<Transform> destinationPoints;

    // Required Gameobjects and transforms
    [SerializeField] private Transform endPoint;
    [SerializeField] private Transform playerParent;
    
    // UI elements
    [Header("Playing UI")]
    [SerializeField] private GameObject playingUI;
    [SerializeField] private TextMeshProUGUI rankingNumbers;

    [Header("EndGame UI")] 
    [SerializeField] private GameObject endUI;
    
    private void Start()
    {
        // camera things
        mCamera = Camera.main;
        mCameraScript = mCamera.GetComponent<cameraFollow>();
        
        // parent transforms
        Transform spawnPointParent = GameObject.FindWithTag("SpawnPoint").transform;
        Transform destinationPointParent = GameObject.FindWithTag("DestinationPoint").transform;

        spawnPoints = new List<Transform>();
        for (int i = 0; i < spawnPointParent.childCount; i++) // prepare lists
        {
            spawnPoints.Add(spawnPointParent.GetChild(i));
        }

        destinationPoints = new List<Transform>();
        for (int i = 0; i < destinationPointParent.childCount; i++)
        {
            destinationPoints.Add(destinationPointParent.GetChild(i));
        }
        
        for (int i = 0; i < destinationPoints.Count; i++) { // quick shuffle for random destinations
            Transform temp = destinationPoints[i];
            int randomIndex = Random.Range(i, destinationPoints.Count);
            destinationPoints[i] = destinationPoints[randomIndex];
            destinationPoints[randomIndex] = temp;
        }

        int randomSpawnPointForPlayer = Random.Range(0, spawnPoints.Count); // index
        
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (i == randomSpawnPointForPlayer)
            {
                // Spawn / Add list / Set Script variables
                GameObject player = Instantiate(playerPrefab, spawnPoints[i].position, Quaternion.identity);
                playerList.Add(player.transform);
                characterController playerScript = player.GetComponent<characterController>();
                playerScript.spawnPoint = spawnPoints[i];
                
                player.transform.parent = playerParent;
                // dont forget to attach
                mCameraScript.player = player.transform;
                continue;
            }
            
            GameObject newBot = Instantiate(botPrefab, spawnPoints[i].position, Quaternion.identity); // 1 bot for each spawnpoint
            playerList.Add(newBot.transform);
            navigateBots botScript = newBot.GetComponent<navigateBots>();
            botScript.spawnPoint = spawnPoints[i]; // set spawnpoint
            botScript.finishLine = destinationPoints[i]; // set random destination point
            newBot.transform.parent = playerParent;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gameStarted) // only works onces start game
            StartGame();

        if(!gameStarted) // not started wait for starting it
            return;

        CheckRanking();
    }

    private void StartGame()
    {
        gameStarted = true;
        for (int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].gameObject.CompareTag("Player")) continue; // dont mess up with real-player
            playerList[i].GetComponent<navigateBots>().StartNavigation();
        }
        EnablePlayUI();
    }

    private void CheckRanking()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (i+1 < playerList.Count) // swap until last index
            {
                float distanceToFinish = Vector3.Distance(playerList[i].position, endPoint.position);
                float distanceToFinish2 = Vector3.Distance(playerList[i + 1].position, endPoint.position);
                
                // this method is easiest ones 
                if (distanceToFinish2 > distanceToFinish)
                {
                    (playerList[i], playerList[i + 1]) = (playerList[i + 1], playerList[i]);
                    // swap indexes foreach first indes will be furthest
                }
            }

            if (playerList[i].CompareTag("Player"))
            {
                UpdateUIForPlayer(playerList.Count-i);
            }
        }
    }

    private void UpdateUIForPlayer(int ranking)
    {
        rankingNumbers.text = $"{ranking}/{playerList.Count}";

        if (ranking == 1)
        {
            rankingNumbers.fontSize = 156;
            rankingNumbers.color = Color.green;
        }
        else if (ranking == 2)
        {
            rankingNumbers.fontSize = 136;
            rankingNumbers.color = Color.yellow;
        }
        else if (ranking == 3)
        {
            rankingNumbers.fontSize = 116;
            rankingNumbers.color = Color.red;
        }
        else
        {
            rankingNumbers.fontSize = 96;
            rankingNumbers.color = Color.white;
        }
    }

    public void EnablePlayUI()
    {
        playingUI.SetActive(true);
    }

    public void DisablePlayUI()
    {
        playingUI.SetActive(false);
        
        for (int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].gameObject.CompareTag("Player")) continue; // dont mess up with player
            playerList[i].gameObject.SetActive(false);
        }
        
        endUI.SetActive(true);
    }
}
