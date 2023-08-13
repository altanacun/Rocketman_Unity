using UnityEngine;

public class Obstacle_Controller : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (transform.tag == "Kill")
        {
            FindObjectOfType<Gameplay_Manager>().GameOver(); // GAME OVER - RESTART GAME
        }
        else if (transform.tag == "Cylinder_Bounce")
        {
            collision.transform.GetComponent<Rigidbody>().AddForce(transform.up * 2000f);
            collision.transform.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f);

        }
        else if (transform.tag == "Cube_Bounce")
        {
            collision.transform.GetComponent<Rigidbody>().AddForce(transform.up * 1000f);
            collision.transform.GetComponent<Rigidbody>().AddForce(transform.forward * 500f);

        }
    }
}