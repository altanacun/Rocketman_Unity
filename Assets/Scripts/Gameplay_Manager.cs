using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cinemachine;
public class Gameplay_Manager : MonoBehaviour
{
    [SerializeField] Transform rocketman_Transform;
    [SerializeField] Animator rocketman_Animator;
    [SerializeField] Animator stick_Animator;
    [SerializeField] Text fpsUIText;
    [SerializeField] GameObject SmokeLeft,SmokeRight;
    [SerializeField] Transform RocketMan_Stick_Start_Transform;
    private float deltaTime;

    [SerializeField] CinemachineFreeLook throwCam;
    [SerializeField] CinemachineFreeLook flyCam;

    // GAMEOVER UI
    [SerializeField] GameObject GameOverUI;
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
    //GAME OVER
    private bool gameOverBool;

    private void Awake()
    {
        Application.targetFrameRate = 60; // LIMIT 60 FPS
    }
    private void Start()
    {
        GameOverUI.SetActive(false);
        playerRigidbody = rocketman_Transform.gameObject.GetComponent<Rigidbody>();
        SmokeLeft.SetActive(false);
        SmokeRight.SetActive(false);
        flyCam.LookAt = rocketman_Transform;
        flyCam.Follow = rocketman_Transform;
        setCams();
    }
    void Update()
    {
        playerRigidbody.velocity = Vector3.ClampMagnitude(playerRigidbody.velocity, 150);
        // COUNT & DISPLAY FPS
        deltaTime += (Time.deltaTime - deltaTime) * .1f;
        float FPS = 1f / deltaTime;
        fpsUIText.text = Mathf.Ceil(FPS).ToString();

        // FIRST SECTION --- THROW PHASE
        if (throwPhaseBool && !gameOverBool)
        {
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
        if (flyPhaseBool && !gameOverBool)
        {
            // ROTATE ROCKETMAN
            if (!wingsOpen)
                rocketman_Transform.Rotate(Vector3.right * Time.deltaTime * 1000f);
            else if (wingsOpen)
                rocketman_Transform.rotation = Quaternion.Lerp(rocketman_Transform.rotation, Quaternion.Euler(45, 0, 0), .05f);

            if (Input.GetMouseButtonDown(0))
            {
                // WINGS & SMOKE
                wingsOpen = true;
                rocketman_Animator.SetBool("wingsActive", true);
                playerRigidbody.velocity = playerRigidbody.velocity - new Vector3(0,0,15); // slowdown
                SmokeRight.SetActive(true);
                SmokeLeft.SetActive(true);
                rocketman_Transform.rotation = Quaternion.Lerp(rocketman_Transform.rotation, Quaternion.Euler(45, 0, 0), .05f);
                // MOVEMENT
                flyStartPosX = Input.mousePosition.x;

            }
            // LEFT & RIGHT MOVEMENT
            if (Input.GetMouseButton(0))
            {
                flyMousePosX = Input.mousePosition.x;

                if (flyStartPosX - flyMousePosX > 300)
                {
                    if (flyStartPosX > flyMousePosX) // MOVE LEFT
                    {
                        rocketman_Transform.position = Vector3.Lerp(rocketman_Transform.position, rocketman_Transform.position + new Vector3(-10f, 0, 0), .02f);
                        rocketman_Transform.rotation = Quaternion.Lerp(rocketman_Transform.rotation, Quaternion.Euler(45, 50, 50), .05f);
                    }
                }
                if (flyStartPosX - flyMousePosX < 300)
                {
                    if (flyStartPosX < flyMousePosX) // MOVE RIGHT
                    {
                        rocketman_Transform.position = Vector3.Lerp(rocketman_Transform.position, rocketman_Transform.position + new Vector3(10f, 0, 0), .02f);
                        rocketman_Transform.rotation = Quaternion.Lerp(rocketman_Transform.rotation, Quaternion.Euler(45, -50, -50), .05f);
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                // WINGS & SMOKE
                wingsOpen = false;
                playerRigidbody.velocity = playerRigidbody.velocity + new Vector3(0, 0, 15); // restore speed
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
        // SET CAMERAS
        flyCam.enabled = true;
        throwCam.enabled = false;
        // !!! FLY !!!
        playerRigidbody.mass = 1f;
        playerRigidbody.drag = .1f;
        playerRigidbody.AddForce(transform.up * thrust * 3000f);
        playerRigidbody.AddForce(transform.forward * thrust * 3000f);
        rocketman_Transform.GetComponent<SphereCollider>().enabled = true;
        flyPhaseBool = true;
        throwPhaseBool = false;
    }

    public void GameOver()
    {
        GameOverUI.SetActive(true);
        SmokeLeft.SetActive(false);
        SmokeRight.SetActive(false);
        rocketman_Animator.SetBool("wingsActive", false);

        flyPhaseBool = false;
        gameOverBool = true;
        stick_Animator.SetBool("bend_Stick", false);
        stick_Animator.SetBool("release_Stick", false);
        stick_Animator.Rebind(); // reset anim
    }

    public void Restart()
    {
        Time.timeScale = 0; // pause game
        
        rocketman_Transform.SetParent(null); // unparent
        rocketman_Animator.Rebind();
        rocketman_Transform.SetParent(RocketMan_Stick_Start_Transform);
        stick_Animator.Play("Ready");
        rocketman_Animator.Play("Ready");
        GameOverUI.SetActive(false);
        rocketman_Animator.Rebind();
        rocketman_Transform.position = Vector3.zero;
        rocketman_Transform.position = new Vector3(0,16,0); // reset to start pos
        rocketman_Transform.rotation = Quaternion.identity; // startRot
        playerRigidbody.useGravity = false;
        playerRigidbody.isKinematic = true;
        rocketman_Transform.GetComponent<SphereCollider>().enabled = false;
        thrust = 0;
        throwPhaseBool = true;
        gameOverBool = false;
        Time.timeScale = 1; // pause game
        setCams();
    }
    private void setCams()
    {
        flyCam.enabled = false;
        throwCam.enabled = true;
    }
} 