using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using UnityEngine.UI;
using Photon.Voice.Unity;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour
{
    public CameraController cameraController;
    public PlayerMovementScript PMScript;
    public Transform playerSpawnPoint;
    public Transform moveTargetTransform;
    public InteractionMenuScript interactionMenuScript;
    public Text RoomNameText;
    public Recorder voiceRecorder;
    public bool muted;
    public bool deafened;
    public Button muteButton;
    public Button deafenButton;
    public Color inactiveVoiceControlColor;
    public Color activeVoiceControlColor;
    public Text interactionLogsText;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
        RoomNameText.text = "Room ID : "+ PhotonNetwork.CurrentRoom.Name;
    }

    public void MuteButtonClicked()
    {
        muted = !muted;
        voiceRecorder.TransmitEnabled = !muted;
        muteButton.image.color = (muted)?inactiveVoiceControlColor:activeVoiceControlColor;
        muteButton.transform.Find("#Cut").gameObject.SetActive(muted);
    }

    public void DeafenButtonClicked()
    {
        deafened = !deafened;
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            player.GetComponent<AudioSource>().mute = deafened;
        deafenButton.image.color = (deafened)?inactiveVoiceControlColor:activeVoiceControlColor;
        deafenButton.transform.Find("#Cut").gameObject.SetActive(deafened);
    }

    void SpawnPlayer()
    {
       PMScript =  PhotonNetwork.Instantiate("Player",playerSpawnPoint.position+new Vector3(Random.Range(-0.5f,0.5f),0,Random.Range(-1,1)),playerSpawnPoint.rotation).GetComponent<PlayerMovementScript>();
       cameraController.PMScript = PMScript;
       cameraController.ResetPlayer(PMScript);
       PMScript.moveTargetTransform = moveTargetTransform;
       PMScript.IntMenuScript = interactionMenuScript;
       interactionMenuScript.PMScript = PMScript;
    }
    public void LeaveRoom()
    {
        // interactionMenuScript.interactionHandler.ResetInteractionProperty();
        interactionMenuScript.ClearOccupancy();
        PhotonNetwork.SendAllOutgoingCommands();
        // PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    void OnApplicationQuit()
    {
        LeaveRoom();
    }
    private void Update() 
    {
        interactionLogsText.text = "";
        // foreach(Player p in PhotonNetwork.PlayerList)
        //     interactionLogsText.text += p.ActorNumber+" interacting " + (bool)p.CustomProperties["Interacting"]+" Interactable "+ (int)p.CustomProperties["InteractableIndex"]+" Interaction "+(int)p.CustomProperties["InteractionIndex"]+" target "+(int)p.CustomProperties["ActionTargetIndex"]+"\n";
    }
}
