using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : Raycast
{
    public Vector3 move;
    public LayerMask collisionMask;

    public override void Start()
    {
        base.Start(); //calling Raycast's Start method
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigin();

        Vector3 velocity = move * Time.deltaTime;
        movePlayer(velocity);
        
        transform.Translate(velocity);
    }

    void movePlayer(Vector3 velocity)
    {
        HashSet<Transform> movePlayer = new HashSet<Transform>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //vertical movement
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? RaycastOrigin.bottomLeft : RaycastOrigin.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                if (hit)
                {
                    if (!movePlayer.Contains(hit.transform))
                    {
                        movePlayer.Add(hit.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;
                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }
         
                }

            }
        }
    }
}
