using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;   
//윤기 작업
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NickNameInput;
    public GameObject DisconnectPanel;
    public Transform spawnPoint;
    public GameObject choiceChracter;

    [SerializeField]
    private int colorIndex;

    [SerializeField] 
    private TMP_Dropdown dropdown;

    [SerializeField]
    private GameObject StartCanvans;

    public GameObject singlePlayerObj;
    

    private void Awake()
    {
        Screen.SetResolution(960,540,false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        dropdown.onValueChanged.AddListener(OnDropdownEvent);
        
        
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();   //커넥트하면 바로 서버에 접속  

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        DisconnectPanel.SetActive(false);
        spawn();
    }

    void Update()
    {
        //if(PhotonNetwork.IsConnected)
            //Debug.Log("Connect");
        if(Input.GetKeyDown(KeyCode.Escape)&&PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        
    }

   
    public void spawn()
    {
        StartCanvans.SetActive(true);
        choiceChracter.SetActive(false);
        GameObject playerObj = PhotonNetwork.Instantiate("Player", spawnPoint.position, spawnPoint.rotation);
        playerObj.name = "Player_" + PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        GhostMotionController playerScript = playerObj.GetComponent<GhostMotionController>();
        playerScript.i=colorIndex; //플레이어스크립트 색정하기
        
    }

    public void SinglePlaySpawn()
    {
        Debug.Log("미구현");
    }

    

    public void OnDropdownEvent(int index)
    {
        colorIndex = index;
    }
    
}
