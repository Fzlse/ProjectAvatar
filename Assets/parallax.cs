using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
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

        if (pos.x <= -60)
        pos.x = 110;
        transform.position = pos;
    }
}
