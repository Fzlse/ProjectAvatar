using System.Collections;
using System.Collections.Generic;
using UnityEngine; // Add UnityEngine namespace to use UnityEngine.Vector2

public class player : MonoBehaviour
{
    public float gravity;
    public Vector2 velocity; // UnityEngine.Vector2
    public float maxXvelocity = 100;
    public float maxacceleration = 10;
    public float acceleration = 10;
    public float distance = 0;
    public float jumpvelocity = 20;
    public float groundheight =10;
    public bool isgrounded = false; // Corrected the variable name to isgrounded
    public bool isholdingjump = false;
    public float maxholdjumptime = 0.4f;
    public float maxMAXholdjumptime = 0.4f;
    public float holdjumptimer = 0.0f;
    public float jumpgroundthreshold = 1;
    
    public bool isdead = false;
    public LayerMask groundlayermask;
    public LayerMask obstaclelayermask;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;
        float grounddistance = Mathf.Abs(pos.y - groundheight);

        if (isgrounded || grounddistance <= jumpgroundthreshold)
        {
            if (Input.GetKeyDown(KeyCode.Space)) // Corrected KeyCode.Space
            {
                isgrounded = false;
                velocity.y = jumpvelocity;
                isholdingjump = true;
                holdjumptimer = 0;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isholdingjump = false;
        }
    }

    private void FixedUpdate() // Corrected method name to FixedUpdate
    {
        Vector2 pos = transform.position; // UnityEngine.Vector2

       if (isdead)
       {
        return;
       }

       if(pos.y < -20)
       {
        isdead = true;
       }

        if (!isgrounded)
        {
            if (isholdingjump)
            {
                holdjumptimer += Time.fixedDeltaTime;
                if (holdjumptimer >= maxholdjumptime)
                {
                    isholdingjump = false;
                }
            }
            pos.y += velocity.y * Time.fixedDeltaTime;
            if (!isholdingjump)
            {
            velocity.y += gravity * Time.fixedDeltaTime;
            }
Vector2 rayOrigin = new Vector2(pos.x + 0.7f, pos.y);
Vector2 rayDirection = Vector2.up;
float rayDistance = velocity.y * Time.fixedDeltaTime;
RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance,groundlayermask);
if (hit2D.collider !=null)
{
    Ground1 ground = hit2D.collider.GetComponent<Ground1>();
    if (ground != null)
    {
        groundheight = ground.groundheight + -6f;
        pos.y = groundheight;
        isgrounded = true;
    }
} 
Debug.DrawRay(rayOrigin,rayDirection * rayDistance,Color.red);
  //          if (pos.y <= groundheight)
  //          {
  //              pos.y = groundheight;
  //              isgrounded = true; 
  //          }
  Vector2 wallorigin = new Vector2(pos.x,pos.y);
  RaycastHit2D wallhit = Physics2D.Raycast(wallorigin,Vector2.right,velocity.x*Time.fixedDeltaTime,groundlayermask);
        if (wallhit.collider !=null)
        {
            Ground1 ground = wallhit.collider.GetComponent<Ground1>();
            if (ground !=null)
            {
               if (pos.y < ground.groundheight)
               {
                velocity.x = 0;
               } 
            }
        }
        }

        distance += velocity.x * Time.fixedDeltaTime;

        if (isgrounded)
        {
            float velocityRatio = velocity.x / maxXvelocity;
            acceleration = maxacceleration * (1 - velocityRatio);
            maxholdjumptime = maxMAXholdjumptime * velocityRatio;
            
            velocity.x += acceleration * Time.fixedDeltaTime;
            
            if (velocity.x >= maxXvelocity)
        {
            velocity.x = maxXvelocity;
        }
Vector2 rayOrigin = new Vector2(pos.x - 0.7f, pos.y);
Vector2 rayDirection = Vector2.up;
float rayDistance = velocity.y * Time.fixedDeltaTime;
RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
if (hit2D.collider ==null)
{
        isgrounded = false;
    
} 
Debug.DrawRay(rayOrigin,rayDirection * rayDistance,Color.yellow);
        }
        transform.position = pos;

     // Obstacle collision detection
    Vector2 forwardRayOrigin = new Vector2(pos.x + 0.5f, pos.y); // Adjust the origin as needed
    Vector2 forwardRayDirection = Vector2.right; // Raycast to the right for forward obstacles
    float forwardRayDistance = 0.1f; // Set a small distance for the raycast
    RaycastHit2D forwardHit = Physics2D.Raycast(forwardRayOrigin, forwardRayDirection, forwardRayDistance);
    
    if (forwardHit.collider != null && forwardHit.collider.tag == "Obstacle2")
    {
        // If we hit an obstacle, stop the player's horizontal movement
        velocity.x = 0;
        // Optionally, you can also set isgrounded to false if you want the player to fall after hitting an obstacle
        // isgrounded = false;
    }
    
    Debug.DrawRay(forwardRayOrigin, forwardRayDirection * forwardRayDistance, Color.blue);

    
    }

}
