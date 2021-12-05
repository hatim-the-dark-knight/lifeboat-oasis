using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public PlayerMovementScript PMScript;
    public float minZoom = 2;
    public float maxZoom = 5;
    private CinemachineVirtualCamera vCam;
    private CinemachineOrbitalTransposer vCamOrbTransposer;
    private CinemachineComposer vCamComposer;
    private CinemachineTargetGroup CMTargetGroup;
    private Vector2 prevTouchpos;
    private bool touchDown;
    private Vector3 followOffsetSave;
    private  float prevFingerDistance;
    public float pinchRate;
    void Awake() 
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        CMTargetGroup = vCam.m_Follow.GetComponent<CinemachineTargetGroup>();
    }
    // Start is called before the first frame update
    void Start()
    {
        vCamOrbTransposer = vCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        vCamComposer = vCam.GetCinemachineComponent<CinemachineComposer>();
        followOffsetSave = vCamOrbTransposer.m_FollowOffset;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Inputs();
    }
    void Inputs()
    {

        if (Input.GetMouseButton(0) &&PMScript.touchDown)
        {   
            vCamOrbTransposer.m_XAxis.m_InputAxisName ="Mouse X";

        }
        else
        {
            vCamOrbTransposer.m_XAxis.m_InputAxisName ="";
            vCamOrbTransposer.m_XAxis.m_InputAxisValue = 0;
        }
        if (Input.GetKey(KeyCode.C))
        {   
            if(vCamOrbTransposer.m_FollowOffset.z < -minZoom)
            {
                vCamOrbTransposer.m_FollowOffset -= new Vector3(0,followOffsetSave.y*Time.deltaTime,followOffsetSave.z*Time.deltaTime);
                if(vCamComposer.m_TrackedObjectOffset.y < 1.5f )
                    vCamComposer.m_TrackedObjectOffset += new Vector3(0,Time.deltaTime*1.5f,0);
                else
                    vCamComposer.m_TrackedObjectOffset = new Vector3(0,1.5f,0);
                
            }
        }
        else if (Input.GetKey(KeyCode.X))
        {   
            if(vCamOrbTransposer.m_FollowOffset.z > -maxZoom)
            {
                vCamOrbTransposer.m_FollowOffset += new Vector3(0,followOffsetSave.y*Time.deltaTime,followOffsetSave.z*Time.deltaTime);
                if(vCamComposer.m_TrackedObjectOffset.y > 0)
                    vCamComposer.m_TrackedObjectOffset -= new Vector3(0,Time.deltaTime*1.5f,0);
                else
                    vCamComposer.m_TrackedObjectOffset = new Vector3(0,0,0);
            }
        }

        if (Input.touchCount >= 2)
        {
            // Vector2 touch0, touch1;
            // float distance;
            // touch0 = Input.GetTouch(0).position;
            // touch1 = Input.GetTouch(1).position;
            // distance = Vector2.Distance(touch0, touch1);
            // prevFingerDistance = (prevFingerDistance == 0)?distance:prevFingerDistance;

            // pinchRate = (distance - prevFingerDistance)/100;

             // get current touch positions
            Touch tZero = Input.GetTouch(0);
            Touch tOne = Input.GetTouch(1);
            // get touch position from the previous frame
            Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
            Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

            float oldTouchDistance = Vector2.Distance (tZeroPrevious, tOnePrevious);
            float currentTouchDistance = Vector2.Distance (tZero.position, tOne.position);

            // get offset value
            pinchRate = (oldTouchDistance - currentTouchDistance)/100;
            

            if(vCamOrbTransposer.m_FollowOffset.magnitude > minZoom)
                vCamOrbTransposer.m_FollowOffset += new Vector3(0,pinchRate*followOffsetSave.y*Time.deltaTime,pinchRate*followOffsetSave.z*Time.deltaTime);
            vCamOrbTransposer.m_FollowOffset = Vector3.ClampMagnitude(vCamOrbTransposer.m_FollowOffset,maxZoom);

            float YAim = vCamComposer.m_TrackedObjectOffset.y;
            YAim += pinchRate*Time.deltaTime*4f;
            YAim = Mathf.Clamp(YAim,0,1.5f);
            
            vCamComposer.m_TrackedObjectOffset = new Vector3(0,YAim,0);
        }
    }
    public void ResetPlayer(PlayerMovementScript PMScript)
    {
        if(CMTargetGroup.FindMember(PMScript.transform) == -1)
            CMTargetGroup.AddMember(PMScript.transform,1,0);
    }
}
