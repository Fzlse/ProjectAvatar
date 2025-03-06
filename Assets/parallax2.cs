using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax2 : MonoBehaviour
{
    public float depth = 1;

    player player;
    private void Awake()
    {
        player = GameObject.Find("player").GetComponent<player>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float realvelocity = player.velocity.x / depth;
        Vector2 pos = transform.position;

         pos.x -= realvelocity * Time.fixedDeltaTime;

        if (pos.x <= -49)
        pos.x = 234;
        transform.position = pos;
    }
}
