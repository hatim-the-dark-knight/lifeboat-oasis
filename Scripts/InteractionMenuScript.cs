using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Photon.Pun;
using TMPro;
public class InteractionMenuScript : MonoBehaviour
{
    [System.Serializable]
    public struct InteractionButton{
        public InteractableType interactableType;
        public GameObject button;
    }

    public PlayerMovementScript PMScript;
    public CinemachineTargetGroup CMTargetGroup;
    public Transform[] buttonGroups;        //All button groups(for positioning references)
    public Vector3 interactionPosition;
    public InteractableScript interactedObject;     //current menu target
    public InteractionButton[] interactionButtons;      //All types of interactable buttons are configured here
    public InteractionHandler interactionHandler;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform.position);
    }

    //Clicked on an object (Not button press rather, menu is targetted to an object)
    public void Interacted(GameObject GO)
    {
        ClearInteraction();
        interactedObject = GO.transform.parent.GetComponent<InteractableScript>();
        interactionPosition = GO.transform.position;
        gameObject.SetActive(true);
        GenerateButtons();
    }

    // Clear menu targetting
    public void ClearInteraction()
    {
        if(interactedObject != null){
            ClearButtons();
            interactionPosition = PMScript.transform.position;
            interactedObject = null;
            gameObject.SetActive(false);
        }
    }
    //Interaction button functions are set here for each interactable type
    public void ButtonPressed(Interaction interaction)
    {
        //All Interactions with scene objects should satisfy this condition(InteractionType, 'Sit' will be changed to 'Interaction' and will act contain all similar interactions such as sit)
        if(interaction.interactableType == InteractableType.Sit)
        {
            int InteractionIndex = Array.FindIndex(interactedObject.interactions,x => x.interactableType == interaction.interactableType);

            //if the interaction on the interacted object is not occupied by another player
            if(interactedObject.usedBy < interactedObject.maxUsedBy)
            {
                if(interactedObject.interactions[InteractionIndex].usedBy < interactedObject.interactions[InteractionIndex].maxUsedBy)
                {
                    // Clear occupancy of player's current interacted object
                    ClearOccupancy();
                    // Use different targets for different users in case of multi interactable object.
                    int targetIndex = (interaction.actionTarget.Length >= interactedObject.maxUsedBy)?interactedObject.GetClosestFreeTargetIndex(InteractionIndex, transform.position):0;
                    PMScript.PerformPlayerInteraction(interactedObject.interactions[InteractionIndex],interactedObject.interactions[InteractionIndex].actionTarget[targetIndex]);
                    interactedObject.photonView.RPC("OccupyInteraction",RpcTarget.AllBufferedViaServer,InteractionIndex,targetIndex);
                    // int interactableIndex = Array.IndexOf(interactionHandler.interactables,interactedObject);
                    //Occupy a target index inside an interactable for all networked players
                    // interactionHandler.photonView.RPC("OccupyInteraction",RpcTarget.AllViaServer,interactableIndex,InteractionIndex,targetIndex);
                    //Change the player's photon custom property accordingly
                    
                    PMScript.currentInteractable = interactedObject;
                }
            }
            ClearButtons();
            gameObject.SetActive(false);
        }
        else if(interaction.interactableType == InteractableType.Action)
        {
            int InteractionIndex = Array.FindIndex(interactedObject.interactions,x => x.actionTarget == interaction.actionTarget);
            if(interactedObject.interactions[InteractionIndex].usedBy <= interactedObject.interactions[InteractionIndex].maxUsedBy)
            {
                // Clear occupancy of player's current interacted object
                ClearOccupancy();
                // Use different targets for different users in case of multi interactable object.
                PMScript.PerformPlayerInteraction(interactedObject.interactions[InteractionIndex],interactedObject.interactions[InteractionIndex].actionTarget[0]); 
                PMScript.currentInteractable = interactedObject;
            }
            
            ClearButtons();
            gameObject.SetActive(false);
        }
        else if(interaction.interactableType == InteractableType.Close)
        {
            ClearInteraction();
            PMScript.currentInteractable = null;
        }
    }
    public void ClearOccupancy()
    {
        //If the player is interacting & also interaction is not of type action
        if(PMScript.currentInteractable != null && PMScript.currentInteraction.interactableType != InteractableType.Action)
        {
            int InteractionIndex = Array.FindIndex(PMScript.currentInteractable.interactions,x => x.interactableType == PMScript.actionState);
            int targetIndex = Array.IndexOf(PMScript.currentInteractable.interactions[InteractionIndex].actionTarget,PMScript.currentActionTarget);
            
            // //Remove occupied target inside an interactable for all networked players
            PMScript.currentInteractable.photonView.RPC("NetworkedClearOccupancy",RpcTarget.AllBufferedViaServer,InteractionIndex,targetIndex);
            
            PMScript.currentInteractable = null;
        }
    }
    
    void GenerateButtons()
    {
        if(interactedObject.gameObject == PMScript.gameObject || !interactedObject.gameObject.CompareTag("Player"))
        {
            foreach(Interaction interaction in interactedObject.interactions)
            {
                GameObject go = Instantiate(interactionButtons[Array.FindIndex(interactionButtons,x => x.interactableType == interaction.interactableType)].button,interactedObject.transform.position,Quaternion.identity,transform.GetChild(interactedObject.interactions.Length-2));
                //Find button inside the prefab and add ButtonPressed as an onclick function
                go.GetComponentInChildren<Button>().onClick.AddListener(() => ButtonPressed(interaction));
                //Change text of button
                go.GetComponentInChildren<TextMeshProUGUI>().text = (interaction.interactionName == "")?interaction.interactableType.ToString():interaction.interactionName;
            }
            PositionButtons();
        }
    }

    //Destroy all current menu buttons
    void ClearButtons()
    {
        int i =0;
        foreach(Transform buttonTransform in transform.GetChild(interactedObject.interactions.Length-2)){
            if(buttonTransform.CompareTag("InteractableButton")){
                Destroy(buttonTransform.gameObject);
                StopAllCoroutines();
                // StopCoroutine(MoveTo(buttonTransform,buttonGroups[interactedObject.interactions.Length-2].GetChild(i).localPosition));
                i++;
            }
        }
    }
    void PositionButtons()
    {
        int buttonCount = interactedObject.interactions.Length;
        int i =0;
        foreach(Transform buttonTransform in transform.GetChild(interactedObject.interactions.Length-2)){
            if(buttonTransform.CompareTag("InteractableButton")){
                StartCoroutine(MoveTo(buttonTransform,buttonGroups[buttonCount-2].GetChild(i).localPosition));
                // buttonTransform.parent = buttonGroups[buttonCount-2].GetChild(i);
                buttonTransform.localRotation = Quaternion.identity;
                i++;
            }
        }
    }
    //Temporary until new UI animations are made
    IEnumerator MoveTo(Transform t, Vector3 pos)
    {
        if(t != null){
            while (Vector3.Distance(t.localPosition,pos) > 0.01f){
                t.localPosition = Vector3.Lerp(t.localPosition, pos, 5*Time.deltaTime);
                yield return null;
            }
        }
    }
}
