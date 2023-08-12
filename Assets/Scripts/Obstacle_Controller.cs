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
            collision.transform.GetComponent<Rigidbody>().AddForce(transform.up * 15f);
            collision.transform.GetComponent<Rigidbody>().AddForce(transform.forward * 5f);

        }
        else if (transform.tag == "Cube_Bounce")
        {
            collision.transform.GetComponent<Rigidbody>().AddForce(transform.up * 10f);
            collision.transform.GetComponent<Rigidbody>().AddForce(transform.forward * 2.5f);

        }
    }
}