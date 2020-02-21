using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 25f)] private float movementSpeed = 1;

    float positiveReward = 1;
    float negativeReward = -1;
    float speedReward = -0.01f;
    float proximityReward = 0.05f;

    [SerializeField] int iteration = 1;

    [SerializeField] private TextMeshProUGUI iterationText = null;

    Vector3 spawnPosition = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        fireRaycast();

        moveUp();
    }

    void fireRaycast()
    {
        int layerMask = 1 << 4;
        layerMask = ~layerMask;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.position + transform.up, 0.1f);

        if(hit.collider != null)
        { 
            Debug.DrawRay(transform.position, transform.up * 2.0f, Color.red);
        }

        //Debug.DrawRay(transform.position, (transform.up + -transform.right) * 1.5f, Color.white);

        //Debug.DrawRay(transform.position, (transform.up + transform.right) * 1.5f, Color.white);
        
    }

    private void InitialiseStateMachine()
    {
        //var states = new Dictionary<Type, BaseState>()
        //{
        //    {typeof() }
        //}
    }

    void randomMovement()
    {

    }

    void moveUp()
    {
        transform.position += new Vector3(0, (movementSpeed * Time.deltaTime), 0);
    }

    void moveLeft()
    {
        transform.position += new Vector3(-movementSpeed * Time.deltaTime, 0, 0);
    }

    void moveRight()
    {
        transform.position += new Vector3(movementSpeed * Time.deltaTime, 0, 0);
    }

    void moveDown()
    {
        transform.position += new Vector3(0, (-movementSpeed * Time.deltaTime), 0);
    }

    void respawn()
    {
        iteration++;
        iterationText.text = "Iteration " + iteration;
        transform.position = spawnPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        respawn();
    }
}
