using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))] //automatically adds the "BoxCollider2D" component
public class Controller : Raycast
{
    public collisionInfo collisions;
    float maxClimbAngle = 80f;
    float maxDescendAngle = 75f;

    public LayerMask collisionMask;

    public override void Start()
    {
        base.Start(); //calling Raycast's start method
        collisions.faceDirection = 1;
    }

    //movement of the character
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigin();
        collisions.reset();
        collisions.oldVelocity = velocity;

        if (velocity.x != 0)
        {
            collisions.faceDirection = (int)Mathf.Sign(velocity.x);
        }

        //call descendSlope method when the character is walking down a slope
        if (velocity.y < 0)
        {
            descendSlope(ref velocity);
        }

        HorizontalCollision(ref velocity);

        if (velocity.y != 0)
        {
            VerticalCollision(ref velocity);
        }
        transform.Translate(velocity);
        
    }

    void HorizontalCollision(ref Vector3 velocity)
    {
        float directionX = collisions.faceDirection;
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        if (Mathf.Abs(velocity.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? RaycastOrigin.bottomLeft : RaycastOrigin.bottomRight; //checks if the character is moving left or right
            rayOrigin += Vector2.up * (horizontalRaySpacing * i); //calculating ray position when the character has moved
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask); //collision detection below the player

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.green); //draw raycast vertically

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisions.descendSlope)
                    {
                        collisions.descendSlope = false;
                        velocity = collisions.oldVelocity;
                    }

                    //Allows the character to walk on the slope more precisely (ie doesn't hover above the slope)
                    float distanceToStartSlope = 0f;
                    if (slopeAngle != collisions.oldSlopeAngle)
                    {
                        distanceToStartSlope = hit.distance - skinWidth;
                        velocity.x -= distanceToStartSlope * directionX;
                    }
                    climbSlope(ref velocity, slopeAngle); //allows the character to walk on slopes
                    velocity.x += distanceToStartSlope * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX; //stops the character from going through walls
                    rayLength = hit.distance; //prevents the character from falling when only half of the hitbox is in the sky

                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);

                    }
                    collisions.left = directionX == -1; //if moving left and collided with left wall, collisions is true
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    //mathematical calculations to allows smoother climbing on the slopes
    void climbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        //allows the character to jump while walking and standing on slopes
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x); //prevents the character from rapidly shaking when colliding with walls on slopes
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
      
    }

    //allows the character to move down a slope smoothly
    void descendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? RaycastOrigin.bottomRight : RaycastOrigin.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        //check if we're moving down the slope
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if ((hit.distance - skinWidth) <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendSlope = true;
                        collisions.below = true;
                        
                    }
                }
            }
        }
    }

    void VerticalCollision(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? RaycastOrigin.bottomLeft : RaycastOrigin.topLeft; //if falling down, start from bottom left, otherwise top left
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x); //calculating ray position when the character has moved
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask); //collision detection below the player

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.green); //draw raycast below the character

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY; //stops the character from falling further
                rayLength = hit.distance; //prevents the character from falling when only half of the hitbox is in the sky

                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x); //prevents the character from rapidly shaking when colliding with walls on slopes
                }
                collisions.above = directionY == 1;
                collisions.below = directionY == -1;
            }
        }

        //prevents the character from getting lagging in-between slopes
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) * skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? RaycastOrigin.bottomLeft : RaycastOrigin.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }


    public struct collisionInfo
    {
        public bool above, below, left, right;
        public bool climbingSlope, descendSlope;
        public float slopeAngle, oldSlopeAngle;
        public Vector3 oldVelocity;
        public int faceDirection;

        public void reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendSlope = false;
            oldSlopeAngle = slopeAngle;
            slopeAngle = 0;
        }
    }
    
 
}
