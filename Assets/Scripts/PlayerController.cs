using KikiNgao.SimpleBikeControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private float speed;
    private float rotationSpeed;
    private Rigidbody playerRB;
    private Animator playerAnimator;
    // Start is called before the first frame update
    void Start()
    {
        speed = player.GetComponent<NavMeshAgent>().speed;
        rotationSpeed = player.GetComponent<NavMeshAgent>().angularSpeed;
        playerRB = player.GetComponent<Rigidbody>();
        playerAnimator = player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    private void Move()
    {
        float moveY = Input.GetAxis("Vertical"); 
        float rotateX = Input.GetAxis("Horizontal");

        Vector3 movement = player.transform.forward * moveY * speed;
        playerRB.velocity = movement;

        // Rotate the player left/right
        if (rotateX != 0)
        {
            Vector3 rotation = new Vector3(0, rotateX * rotationSpeed * Time.deltaTime, 0);
            playerRB.MoveRotation(playerRB.rotation * Quaternion.Euler(rotation));
        }

        playerAnimator.SetFloat("Speed", movement.magnitude);
    }
}
