using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Cinemachine.TargetTracking;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraControl : MonoBehaviour
{
    //public variables
    public Transform orient;
    public Transform player;
    public Transform playerObj;
    public Transform combatTarget;
    public float playerRotateSpd;
    public float windDown;

    //additional scripts
    InputAbstract controls;

    //private variables
    float TimeSinceCombat = 0;
    bool outOfCombat;
    InputAction lookAction;

    //the camera and the styles in use
    public GameObject ExploreCamera;
    public GameObject CombatCamera;
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

        lookAction = InputSystem.actions.FindAction("Look");

        CombatCamera.GetComponent<CinemachineCamera>().LookAt = combatTarget;
        CombatCamera.GetComponent<CinemachineCamera>().Follow = player;

        ExploreCamera.GetComponent<CinemachineCamera>().LookAt = player;
        ExploreCamera.GetComponent<CinemachineCamera>().Follow = player;
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
            if (TimeSinceCombat < windDown) { TimeSinceCombat += Time.deltaTime; Debug.Log(TimeSinceCombat);}
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
            Vector2 moveVector = lookAction.ReadValue<Vector2>();

            float HorizontalIn = moveVector.x;
            float VerticalIn = moveVector.y;
            Vector3 inputDirect = orient.forward * VerticalIn + orient.right * HorizontalIn;

            if (inputDirect != Vector3.zero)
            { playerObj.forward = Vector3.Slerp(playerObj.forward, inputDirect.normalized, Time.deltaTime * playerRotateSpd); }
        }
        else if (currentStyle == CameraStyle.Combat)
        {
            Vector3 viewCombat = combatTarget.position - new Vector3(transform.position.x, combatTarget.position.y, transform.position.z);
            orient.forward = viewCombat.normalized;

            playerObj.forward = viewCombat.normalized;
        }
    }

    void SwitchCamerStyle(CameraStyle style)
    {
        CombatCamera.SetActive(false);
        ExploreCamera.SetActive(false);

        //PlayerCamera.GetComponent<CinemachineCamera>().
        if (style == CameraStyle.Explore)
        {
            ExploreCamera.SetActive(true);
        }
        if (style == CameraStyle.Combat) 
        {  
            CombatCamera.SetActive(true);
        }

        currentStyle = style;
    }
}
