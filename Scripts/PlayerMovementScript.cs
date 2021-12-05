using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class PlayerMovementScript : MonoBehaviour
{
    public Transform moveTargetTransform;
    public CameraController myCam;
    public LayerMask navigationLayers;
    public InteractionMenuScript IntMenuScript;
    private NavMeshAgent myAgent;
    private Animator myAnimator;
    [HideInInspector]
    public bool touchDown;
    private Vector3 touchStartPos;
    public InteractableScript currentInteractable;
    public Interaction currentInteraction;
    public InteractableType actionState = InteractableType.Close;
    public Interaction.ActionTarget currentActionTarget;
    private bool isOverUI;
    public bool actionTransfer;


    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        myAnimator = GetComponent<Animator>();
    }
    private void Update() {
        TouchControl();
        AnimationController();
        PathControl();
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        isOverUI = IsPointerOverUIObject();
    }
    void TouchControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
        }
        if(Input.GetMouseButton(0))
        {
            if(Vector2.Distance(touchStartPos,Input.mousePosition) > 100)
                touchDown = true;
        }
        if(Input.GetMouseButtonUp(0) && !isOverUI)
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hitInfo,Mathf.Infinity,navigationLayers) && !touchDown)
            {   
                transform.LookAt(hitInfo.point);
                if(myAgent.isActiveAndEnabled&& !hitInfo.transform.CompareTag("Player"))
                    myAgent.SetDestination(hitInfo.point);
                if(hitInfo.transform.CompareTag("Interactable") || hitInfo.transform.CompareTag("Player"))
                {
                    InteractableScript interactable = hitInfo.transform.GetComponent<InteractableScript>();
                    Transform intMenuTarget = interactable.GetClosestFreeTarget(hitInfo.point);
                    if(intMenuTarget == null)
                        intMenuTarget = interactable.transform.Find("#InteractionMenuTarget");
                    IntMenuScript.transform.position = intMenuTarget.position;

                    IntMenuScript.Interacted(intMenuTarget.gameObject);
                }
                else
                {
                    if(actionState != InteractableType.Close)
                        IntMenuScript.ClearOccupancy();
                    IntMenuScript.ClearInteraction();
                    myAgent.enabled = true;
                    myAgent.SetDestination(hitInfo.point);
                    // Debug.Log(hitInfo.transform.name);
                    actionState = InteractableType.Close;
                }
            }

            touchDown = false;
        }
    }
    void AnimationController()
    {
        if(myAgent.velocity.magnitude > 0 && Vector3.Distance(transform.position,myAgent.destination) > 0.1f)
        { 
            myAnimator.SetInteger("State",1);
            myAnimator.SetBool("PlayingAction",false);
            moveTargetTransform.gameObject.SetActive(true);
            moveTargetTransform.position = myAgent.destination;
            moveTargetTransform.rotation = Quaternion.identity;   
        }
        else if(actionState == InteractableType.Sit && currentActionTarget != null)
        {    
            myAnimator.SetBool("PlayingAction",false);
            if(Vector3.Distance(transform.position,currentActionTarget.myTransform.position) < 2f)
            {
                myAgent.enabled = false;
                transform.position = currentActionTarget.myTransform.position + currentActionTarget.positionOffset;
                transform.eulerAngles = currentActionTarget.myTransform.eulerAngles + currentActionTarget.rotationOffset;
                myAnimator.SetInteger("State",2);
                myAnimator.SetInteger("Interaction",currentActionTarget.animationState);
            }
            else
            {
                myAgent.enabled = true;
                myAgent.SetDestination(currentActionTarget.myTransform.position);
            }
        }
        else if(actionState == InteractableType.Action && (!myAnimator.GetBool("PlayingAction")))
        {   
            // Debug.Log(1232);
            myAgent.enabled = true;
            if(currentActionTarget.myTransform != null)
            {
                transform.position = currentActionTarget.myTransform.position + currentActionTarget.positionOffset;
                transform.eulerAngles = currentActionTarget.myTransform.eulerAngles + currentActionTarget.rotationOffset;
            }
            myAnimator.SetBool("PlayingAction",true);
            myAnimator.SetInteger("Action",currentActionTarget.animationState);
        }
        else{
            myAnimator.SetInteger("State",0);
            // myAnimator.SetBool("PlayingAction",false);
        }
    }
    void PathControl()
    {
         if ((myAgent.pathStatus==NavMeshPathStatus.PathComplete && myAgent.remainingDistance==0) || !myAgent.hasPath)
         {
             moveTargetTransform.gameObject.SetActive(false);
         }
    }
    public void PerformPlayerInteraction(Interaction interaction, Interaction.ActionTarget actionTarget)
    {
        Debug.Log(interaction.interactableType.ToString());
        myAnimator.SetBool("PlayingAction",false);
        actionState = interaction.interactableType;
        currentInteraction = interaction;
        currentActionTarget = actionTarget;
    }

     private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    
}
