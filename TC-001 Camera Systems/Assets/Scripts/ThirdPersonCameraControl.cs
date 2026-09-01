using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Cinemachine.TargetTracking;
using UnityEngine;

public class ThirdPersonCameraControl : MonoBehaviour
{
    //Rotation
    public Transform orient;
    public Transform player;
    public Transform playerObj;
    public Transform reticle;
    public float playerRotateSpd;

    //additional scripts
    InputAbstract controls;

    //required variables
    float TimeSinceCombat = 0;
    bool outOfCombat;

    //the camera and the styles in use
    public GameObject exploreCam;
    public CameraStyle currentStyle;
    public enum CameraStyle
    {
        Explore,
        Combat
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controls = new InputAbstract();
        controls.Enable();
        outOfCombat = true;
    }

    // Update is called once per frame
    void Update()
    {
        //switch cam style
        if (controls.Player.Attack.ReadValue<float>() > 0.5f||controls.Player.Secondary.ReadValue<float>() > 0.5f||controls.Player.Ultimate.ReadValue<float>() > 0.5f)
        { 
            outOfCombat = false; 
            if (currentStyle != CameraStyle.Combat)
            {
                SwitchCamerStyle(CameraStyle.Combat); 
                Debug.Log("Switching to Combat Mode");
            }
        }

        if (!outOfCombat)
        {
            if (TimeSinceCombat < 1f) { TimeSinceCombat += Time.deltaTime; Debug.Log(TimeSinceCombat);}
            else 
            { 
                outOfCombat = true;  
                TimeSinceCombat = 0f;
                SwitchCamerStyle(CameraStyle.Explore); 
                Debug.Log("Switching to Mining Mode"); 
            }
        }


        //rotate orientation object
        Vector3 viewDirect = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orient.forward = viewDirect.normalized;

        if (currentStyle == CameraStyle.Explore)
        {
            float HorizontalIn = Input.GetAxis("Horizontal");
            float VerticalIn = Input.GetAxis("Vertical");
            Vector3 inputDirect = orient.forward * VerticalIn + orient.right * HorizontalIn;

            if (inputDirect != Vector3.zero)
            { playerObj.forward = Vector3.Slerp(playerObj.forward, inputDirect.normalized, Time.deltaTime * playerRotateSpd); }
        }
        else if (currentStyle == CameraStyle.Combat)
        {
            Vector3 viewCombat = reticle.position - new Vector3(transform.position.x, reticle.position.y, transform.position.z);
            orient.forward = viewCombat.normalized;

            playerObj.forward = viewCombat.normalized;
        }
    }

    void SwitchCamerStyle(CameraStyle style)
    {
        //exploreCam.GetComponent<CinemachineCamera>().LookAt
        if (style == CameraStyle.Explore) 
        { 
            exploreCam.GetComponent<CinemachineCamera>().LookAt = player;
        }
        if (style == CameraStyle.Combat) 
        {  
            exploreCam.GetComponent<CinemachineCamera>().LookAt = reticle; 
        }

        currentStyle = style;
    }
}
