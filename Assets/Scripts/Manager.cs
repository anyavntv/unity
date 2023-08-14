using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI; 



public class Manager : MonoBehaviourPunCallbacks
{
    public static Manager Instance;


    public GameObject playerPrefab;
    public Text pingText;
    private bool Off = false;
    public GameObject disconUI;

    public GameObject playerFeed;
    public GameObject feedGrid;

    public GameObject RespawnMenu;
    [HideInInspector]public GameObject LocalPlayer;

    internal void EnableRespawn()
    {
        RespawnMenu.SetActive(true);
    }
    public void StartRespawn()
    {
        LocalPlayer.GetComponent<PhotonView>().RPC("Respawn", RpcTarget.AllBuffered);
        LocalPlayer.GetComponent<PlayerHealth>().EnableInput();
        RespawnLocation();
        RespawnMenu.SetActive(false);


    }

    public void RespawnLocation()
    {
        float randomValue = Random.Range(-3f, 5f);
        LocalPlayer.transform.localPosition = new Vector2(randomValue, 3f);
    }
    

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {   
       
        SpawnPlayer();
    }
    
    private void CheckInput()
    {
        if(Off&&Input.GetKeyDown(KeyCode.Escape))
        {
            disconUI.SetActive(false);
            Off= false;
        }
        else if (!Off && Input.GetKeyDown(KeyCode.Escape))
        {
            disconUI.SetActive(true);
            Off = true;
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0); 
    }
    private void Update()
    {
        CheckInput();
        pingText.text = "Ping: " + PhotonNetwork.GetPing();


    }
    // Update is called once per frame
    void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, playerPrefab.transform.position, playerPrefab.transform.rotation);
    }

    public override void  OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject obj = Instantiate(playerFeed, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(feedGrid.transform, false);
        obj.GetComponent<Text>().text = newPlayer.NickName + " joined the game";
        obj.GetComponent<Text>().color = Color.green;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GameObject obj = Instantiate(playerFeed, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(feedGrid.transform, false);
        obj.GetComponent<Text>().text = otherPlayer.NickName + " left the game";
        obj.GetComponent<Text>().color = Color.red;
    }
}
 