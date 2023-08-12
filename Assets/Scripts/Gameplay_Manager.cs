using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class Gameplay_Manager : MonoBehaviour
{
    [SerializeField] Transform rocketman_Transform;
    [SerializeField] Animator rocketman_Animator;
    [SerializeField] Animator stick_Animator;
    [SerializeField] Text fpsUIText;
    private float deltaTime;
    //THROW PHASE CONTROLS
    private float startPosX;
    private float direction;
    private bool throwPhase = true;
    private float mousePosX;
    private float thrust;
    //FLY PHASE CONTROLS
    private bool flyPhase = false;


    //CAMERA LOCATIONS
    private Vector3 camPos_Start;
    private Vector3 camRot_Start;


    private void Awake()
    {
        Application.targetFrameRate = 60; // LIMIT 60 FPS
    }

    void Start()
    {
        camPos_Start = new Vector3(17.5f, 20, -14);
        camRot_Start = new Vector3(15, -50, 0);
    }

    void Update()
    {
        // COUNT & DISPLAY FPS
        deltaTime += (Time.deltaTime - deltaTime) * .1f;
        float FPS = 1f / deltaTime;
        fpsUIText.text = Mathf.Ceil(FPS).ToString();

        // FIRST SECTION --- THROW PHASE
        if (throwPhase){
            if (Input.GetMouseButton(0))
            {
                stick_Animator.SetBool("bend_Stick", true);

                if (mousePosX > 0)
                {
                    thrust = (mousePosX - startPosX) / -1000;
                    stick_Animator.SetFloat("bend_Stick_Time", thrust); // -1000 cause of screen pixel values
                }
                mousePosX = Input.mousePosition.x;

            }
            if (Input.GetMouseButtonDown(0))
            {
                startPosX = mousePosX;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (thrust > .4f) // AVOID FALLING TO GROUND
                {
                    ThrowPhase();
                }
                else
                {
                    stick_Animator.SetFloat("bend_Stick_Time", 0); // RETURN TO START POS
                }
            }
        }

        // SECOND SECTION --- FLY PHASE
        if (flyPhase)
        {
            rocketman_Transform.GetChild(0).transform.Rotate(Vector3.right * Time.deltaTime * 1000f);
        }
        
    }

    public void ThrowPhase()
    {
        // Play throw animation
        stick_Animator.SetBool("release_Stick", true);
        stick_Animator.SetBool("bend_Stick", false);
        // SET CAMERA
   
        Transform GameCam = Camera.main.transform;
        GameCam.SetParent(null); // unparent
        GameCam.SetParent(rocketman_Transform);
        GameCam.transform.position = Vector3.zero;  // reset
        GameCam.transform.rotation = Quaternion.Euler(0, 0, 0); // reset
        Vector3 flyCamPos = new Vector3(0, 25, 0);
        Quaternion flyCamRot = Quaternion.Euler(1.08f, 0, 0);
        GameCam.position = flyCamPos;//Vector3.MoveTowards(GameCam.position, flyCamPos, Time.deltaTime * 300f);
        //GameCam.rotation = Quaternion.Slerp(GameCam.rotation, flyCamRot, Time.deltaTime * 300f);
        // UNPARENT PLAYER FROM STICK
        rocketman_Transform.SetParent(null);
        // RIGIDBODY SETTINGS
        Rigidbody rb = rocketman_Transform.gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        // !!! FLY !!!
        rb.AddForce(transform.up * thrust * 3000f);
        rb.AddForce(transform.forward * thrust * 3000f);
        throwPhase = false;
        flyPhase = true;
        FlyPhaseOperations();
    }

    public void FlyPhaseOperations()
    {


    }

    public void Restart()
    {
        throwPhase = true;
        // camera pos = new Vector3();
        // caemra rot = new vector3();
        stick_Animator.SetBool("bendStick", false);
        stick_Animator.SetBool("releaseStick", false);
    }
}