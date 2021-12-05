using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
public enum InteractableType{Sit,Close,Action};
[System.Serializable]
public class Interaction{
    public string interactionName;
    [System.Serializable]
    public class ActionTarget{
        public Transform myTransform;
        public Vector3 positionOffset;
        public Vector3 rotationOffset; 
        public int animationState;
    }
    public InteractableType interactableType;
    public ActionTarget[] actionTarget;
    
    public int usedBy;
    public int maxUsedBy;

    
    
}
public class InteractableScript : MonoBehaviourPunCallbacks
{
    public int usedBy; // total users on all interactions combined
    public int maxUsedBy = 1; // maximum total users on all interactions combined
    public Interaction[] interactions;
    // [HideInInspector]
    public List<Transform> actionTargetsInUse;
    public List<Transform> allActionTargets;
    private GameSceneManager GSManagerScript;

    private void Start() {
        GSManagerScript = GameObject.FindObjectOfType<GameSceneManager>();
        GetAllActionTargets();
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.F))
            usedBy++;
    }
    void GetAllActionTargets()
    {
        foreach(Interaction interaction in interactions)
            foreach(Interaction.ActionTarget at in interaction.actionTarget)
                if(!CheckIfTransformIsAnActionTarget(at.myTransform))
                    allActionTargets.Add(at.myTransform);
    }

    bool CheckIfTransformIsAnActionTarget(Transform t)
    {
        foreach(Transform t2 in allActionTargets)
            if(t2 == t)
                return true;

        return false;
    }

    public int GetOrderedFreeTargetIndex(int interactionIndex)
    {
        int i = 0;
        foreach(Interaction.ActionTarget at in interactions[interactionIndex].actionTarget){
            if(actionTargetsInUse.IndexOf(at.myTransform) == -1)
                return i;
            i++;
        }
        return -1;
    }
    public int GetClosestFreeTargetIndex(int interactionIndex, Vector3 targetPos)
    {
        int i = 0;
        float prevDistanceFromPlayer = Mathf.Infinity;
        int targetIndex = 0;
        foreach(Interaction.ActionTarget at in interactions[interactionIndex].actionTarget){
            if(actionTargetsInUse.IndexOf(at.myTransform) == -1){
                float distanceFromPlayer  = Vector3.Distance(targetPos, at.myTransform.position);
                if(distanceFromPlayer < prevDistanceFromPlayer)
                {
                    prevDistanceFromPlayer = distanceFromPlayer;
                    targetIndex = i;
                }
            }
            i++;
        }
        return targetIndex;
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {   
        
    }

    public Transform GetClosestFreeTarget(Vector3 targetPos)
    {
        int i = 0,j = 0;
        float prevDistanceFromPlayer = Mathf.Infinity;
        int targetIndex = 0;
        int interactionIndex = 0;
        foreach(Interaction interaction in interactions){
            foreach(Interaction.ActionTarget at in interaction.actionTarget){
                if(actionTargetsInUse.IndexOf(at.myTransform) == -1){
                    float distanceFromPlayer = 0;
                    if(at.myTransform != null)
                        distanceFromPlayer  = Vector3.Distance(targetPos, at.myTransform.position);

                    if(distanceFromPlayer < prevDistanceFromPlayer)
                    {
                        prevDistanceFromPlayer = distanceFromPlayer;
                        targetIndex = i;
                        interactionIndex = j;
                    }
                }
                i++;
            }
            j++;
        }
        Transform t = (allActionTargets.Count == 0)?transform.Find("#InteractionMenuTarget"):interactions[interactionIndex].actionTarget[targetIndex].myTransform;
        return t;
    }
    public void AddTargetInUse(int interactionIndex,int targetIndex)
    {
        actionTargetsInUse.Add(interactions[interactionIndex].actionTarget[targetIndex].myTransform);
    }
    public void RemoveTargetInUse(int interactionIndex,int targetIndex)
    {
        if(targetIndex != -1&& interactionIndex != -1)
        {
            int x = actionTargetsInUse.IndexOf(interactions[interactionIndex].actionTarget[targetIndex].myTransform);
            actionTargetsInUse.Remove(actionTargetsInUse[x]);
        }

    }
    [PunRPC]
    public void OccupyInteraction(int interactionIndex, int actionTargetIndex)
    {
        interactions[interactionIndex].usedBy++;
        usedBy = CurrentUsedBy();
        AddTargetInUse(interactionIndex,actionTargetIndex);
    }

     [PunRPC]
    public void NetworkedClearOccupancy(int interactionIndex, int actionTargetIndex)
    {
        interactions[interactionIndex].usedBy--;
        usedBy = CurrentUsedBy();
        RemoveTargetInUse(interactionIndex,actionTargetIndex);
    }

    public int CurrentUsedBy()
    {
        int usedBy = 0;
        foreach(Interaction interaction in interactions)
        {
            usedBy += interaction.usedBy;
        }
        return usedBy;
    }

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if(stream.IsWriting)
    //     {
    //         foreach(Interaction interaction in interactions)
    //             stream.SendNext(interaction.usedBy);
    //         stream.SendNext(usedBy);
            
    //     }
    //     else if(stream.IsReading)
    //     {
    //         foreach(Interaction interaction in interactions)
    //             interaction.usedBy = (int)stream.ReceiveNext();
    //         usedBy = (int)stream.ReceiveNext();
    //     }
    // }
    
}
