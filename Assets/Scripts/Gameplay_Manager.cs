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
    [SerializeField] GameObject SmokeLeft,SmokeRight;
    private float deltaTime;

    Rigidbody playerRigidbody;

    //THROW PHASE CONTROLS
    private float throwStartPosX;
    private bool throwPhaseBool = true;
    private float throwMousePosX;
    private float thrust;
    //FLY PHASE CONTROLS
    private bool flyPhaseBool = false;
    private bool wingsOpen = false;
    private float flyStartPosX;
    private float flyMousePosX;

    private void Awake()
    {
        Application.targetFrameRate = 60; // LIMIT 60 FPS
    }
    private void Start()
    {
        playerRigidbody = rocketman_Transform.gameObject.GetComponent<Rigidbody>();
        SmokeLeft.SetActive(false);
        SmokeRight.SetActive(false);
    }
    void Update()
    {
        // COUNT & DISPLAY FPS
        deltaTime += (Time.deltaTime - deltaTime) * .1f;
        float FPS = 1f / deltaTime;
        fpsUIText.text = Mathf.Ceil(FPS).ToString();

        // FIRST SECTION --- THROW PHASE
        if (throwPhaseBool){
            if (Input.GetMouseButton(0))
            {
                stick_Animator.SetBool("bend_Stick", true);

                if (throwMousePosX > 0)
                {
                    thrust = (throwMousePosX - throwStartPosX) / -1000;
                    stick_Animator.SetFloat("bend_Stick_Time", thrust); // -1000 cause of screen pixel values
                }
                throwMousePosX = Input.mousePosition.x;

            }
            if (Input.GetMouseButtonDown(0))
            {
                throwStartPosX = throwMousePosX;
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
        if (flyPhaseBool)
        {
            // SET CAMERA
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, rocketman_Transform.position + new Vector3(0, 5, -20), 0.05f);
            Camera.main.transform.LookAt(rocketman_Transform);

            // ROTATE ROCKETMAN
            if (!wingsOpen)
                rocketman_Transform.Rotate(Vector3.right * Time.deltaTime * 1000f);
            else if (wingsOpen)
                rocketman_Transform.rotation = Quaternion.Lerp(rocketman_Transform.rotation, Quaternion.Euler(45,0,0), .05f);

            if (Input.GetMouseButtonDown(0))
            {
                // WINGS & SMOKE
                wingsOpen = true;
                rocketman_Animator.SetBool("wingsActive",true);
                playerRigidbody.mass = .01f;
                playerRigidbody.drag = 1f;
                SmokeRight.SetActive(true);
                SmokeLeft.SetActive(true);
                // MOVEMENT
                flyStartPosX = Input.mousePosition.x;


            }
            if (Input.GetMouseButton(0))
            {
                flyMousePosX = Input.mousePosition.x;

                if (flyStartPosX - flyMousePosX > 300) 
                {
                    if (flyStartPosX > flyMousePosX) // MOVE LEFT
                    {
                        //playerRigidbody.AddForce(Vector3.left * 30 * Input.GetAxis("Horizontal") * Time.deltaTime);
                        rocketman_Transform.rotation = Quaternion.Lerp(rocketman_Transform.rotation, Quaternion.Euler(45, 50, 50), .05f);
                    }
                }
                if (flyStartPosX - flyMousePosX < 300)
                {
                    if (flyStartPosX < flyMousePosX) // MOVE RIGHT
                    {
                        //playerRigidbody.AddForce(Vector3.right * 30 * Input.GetAxis("Horizontal") * Time.deltaTime);
                        rocketman_Transform.rotation = Quaternion.Lerp(rocketman_Transform.rotation, Quaternion.Euler(45, -50, -50), .05f);
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                // WINGS & SMOKE
                wingsOpen = false;
                playerRigidbody.mass = 1f;
                playerRigidbody.drag = -1f;
                rocketman_Animator.SetBool("wingsActive", false);
                SmokeRight.SetActive(false);
                SmokeLeft.SetActive(false);
            }
        }
    }

    public void ThrowPhase()
    {
        // Play throw animation
        stick_Animator.SetBool("release_Stick", true);
        stick_Animator.SetBool("bend_Stick", false);
        // UNPARENT PLAYER FROM STICK
        rocketman_Transform.SetParent(null);
        // RIGIDBODY SETTINGS
        playerRigidbody.isKinematic = false;
        playerRigidbody.useGravity = true;
        // !!! FLY !!!
        playerRigidbody.AddForce(transform.up * thrust * 3000f);
        playerRigidbody.AddForce(transform.forward * thrust * 3000f);
        throwPhaseBool = false;
        flyPhaseBool = true;
        FlyPhaseOperations();
    }

    public void FlyPhaseOperations()
    {   
    }

    public void Restart()
    {
        throwPhaseBool = true;
        // camera pos = new Vector3();
        // caemra rot = new vector3();
        stick_Animator.SetBool("bendStick", false);
        stick_Animator.SetBool("releaseStick", false);
    }
} 