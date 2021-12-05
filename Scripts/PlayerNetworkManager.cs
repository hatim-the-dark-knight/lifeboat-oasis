using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using ExitGames.Client.Photon;

public class PlayerNetworkManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [HideInInspector]
    public PhotonView photonView;

    public Transform UpperWearParent;
    public Transform LowerWearParent;
    private Player latestPlayer;
    public Renderer skinRenderer;
    private GameSceneManager GSManager;
    public int actorNo;
    public bool isInteracting;
    public int interactionIndex;
    public int interactableIndex;
    public int targetIndex;
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
       PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        //Some Player Spawn
        if (eventCode == 0)
        {
            // if(photonView.IsMine)
            //     photonView.RPC("CustomisePlayer",RpcTarget.Others,PlayerPrefs.GetInt("UpperWear"), PlayerPrefs.GetInt("LowerWear"), PlayerPrefs.GetInt("HeadWear"),PlayerPrefs.GetString("SkinColor"));
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    void Start()
    {
        GSManager = GameObject.FindObjectOfType<GameSceneManager>();
        if(photonView.isRuntimeInstantiated){
            if(!photonView.IsMine)
            {
                GetComponent<PlayerMovementScript>().enabled = false;
                GetComponent<NavMeshAgent>().enabled = false;
            }
            AssignPlayerCustomisationData();
            // interactionHandler = GameObject.FindObjectOfType<InteractionHandler>();
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            if(photonView.IsMine){
                PhotonNetwork.RaiseEvent(0,null,raiseEventOptions,SendOptions.SendReliable);
            }
        }
        else
            AssignPlayerCustomisationData();
    }

    private void Update() {
    }
    public void AssignPlayerCustomisationData()
    {
        if(photonView.isRuntimeInstantiated)
        {
            if(photonView.IsMine)
                photonView.RPC("CustomisePlayer",RpcTarget.AllBufferedViaServer,PlayerPrefs.GetInt("UpperWear"), PlayerPrefs.GetInt("LowerWear"), PlayerPrefs.GetInt("HeadWear"),PlayerPrefs.GetString("SkinColor"),PlayerPrefs.GetString("TopColor"),PlayerPrefs.GetString("BottomColor"));
        }
        else
        {
            CustomisePlayer(PlayerPrefs.GetInt("UpperWear"), PlayerPrefs.GetInt("LowerWear"), PlayerPrefs.GetInt("HeadWear"),PlayerPrefs.GetString("SkinColor","FFFFFF"),PlayerPrefs.GetString("TopColor","FF0000"),PlayerPrefs.GetString("BottomColor","FF0000"));
        }
    }

    [PunRPC]
    public void CustomisePlayer(int upper, int lower, int head,string skin, string top, string bottom)
    {
        int upperWear = upper;
        int lowerWear = lower;
        int headWear = head;
        Color skinColor;
        ColorUtility.TryParseHtmlString("#" + skin, out skinColor);
        
        Color topColor;
        ColorUtility.TryParseHtmlString("#" + top, out topColor);

        Color bottomColor;
        ColorUtility.TryParseHtmlString("#" + bottom, out bottomColor);

        DisableChildren(UpperWearParent);
        DisableChildren(LowerWearParent);

        UpperWearParent.GetChild(upperWear).gameObject.SetActive(true);
        LowerWearParent.GetChild(lowerWear).gameObject.SetActive(true);

        skinRenderer.material.color = skinColor;
        UpperWearParent.GetChild(upperWear).gameObject.GetComponent<SkinnedMeshRenderer>().material.color = topColor;
        LowerWearParent.GetChild(lowerWear).gameObject.GetComponent<SkinnedMeshRenderer>().material.color = bottomColor;
    }

    public void DisableChildren(Transform parent)
    {
        for(int i=0; i< parent.transform.childCount; i++)
        {
            var child = parent.transform.GetChild(i).gameObject;
            if(child != null)
                child.SetActive(false);
        }
    }
    
    void OnPhotonPlayerConnected(Player newPlayer)
    {
        latestPlayer = newPlayer;
    }

    public override void OnLeftRoom()
    {
    //    GSManager.LeaveRoom();
    }

}
