using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameField;
    void Awake() 
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        
    }
    void Start()
    {
        ConnectToPUN();
    }

    void ConnectToPUN()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void CreateRoom()
    {
        const string glyphs= "abcdefghijklmnopqrstuvwxyz0123456789";

        int charAmount = 6;
        string roomName="";
        for(int i=0; i<charAmount; i++)
            roomName += glyphs[Random.Range(0, glyphs.Length)];

        Debug.Log(roomName);
        ExitGames.Client.Photon.Hashtable customPropreties = new ExitGames.Client.Photon.Hashtable();
        customPropreties.Add("Room",1);
        RoomOptions roomOptions = new RoomOptions() {CustomRoomProperties = customPropreties, IsVisible = true, IsOpen = true, MaxPlayers = 10, CleanupCacheOnLeave = true };
        roomOptions.CustomRoomPropertiesForLobby = new string[]
        {
            "Room"
        };
        PhotonNetwork.CreateRoom(roomName,roomOptions);
    }
    public void JoinRoomUsingRoomName()
    {
        PhotonNetwork.JoinRoom(roomNameField.text);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void CustomiseAvatar()
    {
        SceneManager.LoadScene(2);
    }

    public void ChooseRoom(int index)
    {

    }
}
