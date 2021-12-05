using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMenuPositionScript : MonoBehaviour
{
    public InteractionMenuScript interactionMenuScript;
    // Start is called before the first frame update
    void Start()
    {
        interactionMenuScript.interactionHandler = GetComponent<InteractionHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(interactionMenuScript.interactedObject == null && interactionMenuScript.PMScript != null)
            interactionMenuScript.interactionPosition = interactionMenuScript.PMScript.transform.position;
        transform.position = Vector3.MoveTowards(transform.position,interactionMenuScript.interactionPosition,Time.deltaTime*15);
    }
}
