using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InteractionHandler : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public InteractionMenuScript interactionMenuScript;
    public InteractableScript[] interactables;
    [HideInInspector]
    public PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        interactionMenuScript = GetComponent<InteractionMenuPositionScript>().interactionMenuScript;
        photonView = GetComponent<PhotonView>();
    }

    // [PunRPC]
    // void OccupyInteraction(int interactableIndex, int interactionIndex, int targetIndex)
    // {
    //     // SetInteractionProperty(interactableIndex,interactionIndex,interactables[interactableIndex].allActionTargets.FindIndex(t =>t == interactables[interactableIndex].interactions[interactionIndex].actionTarget[targetIndex].myTransform));
    //     interactables[interactableIndex].interactions[interactionIndex].usedBy = CurrentUsedBy(interactableIndex,interactionIndex,targetIndex);
    //     interactables[interactableIndex].usedBy = CurrentUsedBy(interactableIndex);
    //     Debug.Log(interactables[interactableIndex].usedBy);
    //     interactables[interactableIndex].AddTargetInUse(interactionIndex,targetIndex);
    // }
    // [PunRPC]
    // public void NetworkedClearOccupancy(int interactableIndex, int interactionIndex, int targetIndex)
    // {
    //     // ResetInteractionProperty();
    //     interactables[interactableIndex].interactions[interactionIndex].usedBy = CurrentUsedBy(interactableIndex,interactionIndex,targetIndex);
    //     interactables[interactableIndex].usedBy = CurrentUsedBy(interactableIndex);
    //     Debug.Log(interactables[interactableIndex].usedBy);
    //     interactables[interactableIndex].RemoveTargetInUse(interactionIndex,targetIndex);
    // }

    // public bool CheckIfActionTargetInUse(int interactableIndex,int interactionIndex, int targetIndex)
    // {
    //     int usedBy = CurrentUsedBy(interactableIndex,interactionIndex,targetIndex);
    //     return (usedBy > interactables[interactableIndex].interactions[interactionIndex].maxUsedBy);
    // }

    // public int CurrentUsedBy(int interactableIndex,int interactionIndex, int targetIndex)
    // {
    //     int usedBy = 0;
    //     foreach(Player player in PhotonNetwork.PlayerList)
    //     {
    //         if(player.CustomProperties.ContainsKey("Interacting"))
    //         {
    //             if((bool)player.CustomProperties["Interacting"])
    //             {
    //                 if(interactableIndex == (int)player.CustomProperties["InteractableIndex"] && interactionIndex == (int)player.CustomProperties["InteractionIndex"] && targetIndex == (int)player.CustomProperties["ActionTargetIndex"])
    //                 {
    //                     usedBy++;
    //                 }
    //             } 
    //         }
    //     }
    //     return usedBy;
    // }

    // public int CurrentUsedBy(int interactableIndex)
    // {
    //     int usedBy = 0;
    //     foreach(Interaction interaction in interactables[interactableIndex].interactions)
    //     {
    //         usedBy += interaction.usedBy;
    //     }
    //     return usedBy;
    // }
}

