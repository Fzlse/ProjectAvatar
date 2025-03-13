using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground1 : MonoBehaviour
{
    public float groundheight;
    public float screenRight;
    new BoxCollider2D collider;

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        screenRight = Camera.main.transform.position.x * 2;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnGroundFall();
    }

    // Update is called once per frame
    void Update()
    {
        groundheight = transform.position.y + (collider.size.y / 2);
    }

    void SpawnGroundFall()
    {
        if (UnityEngine.Random.Range(0, 3) == 0)
        {
            GameObject groundFallObj = new GameObject("GroundFall"); // Create a new GameObject
            groundfall fall = groundFallObj.AddComponent<groundfall>(); // Add groundfall script to it
            fall.player = GameObject.Find("player").GetComponent<player>(); // Assign the player reference
        }
    }
}
