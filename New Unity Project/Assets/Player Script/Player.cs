using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof (Controller))] //automatically adds the "Controller" script 
public class Player : MonoBehaviour
{

    Vector3 velocity;
    float gravity;

    [Header("Movement stats")]
    public float speed = 6f;
    float accelerationTimeInAir = .2f;
    float accelerationTimeInGround = .1f;

    [Header("Jump")]
    public float jumpVelocity;
    public float jumpHeight = 4f;
    public float timeToJump = .4f;
    public bool doubleJump;

    [Header("Wall Climb")]
    public float wallSlideSpeedMax = 3f;
    public Vector2 wallClimbOn;
    public Vector2 wallClimbOff;
    public Vector2 wallLeap;

    float velocityXSmoothing;

    Controller controller;

    void Start()
    {
        controller = GetComponent<Controller>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJump, 2); 
        jumpVelocity = Mathf.Abs(gravity) * timeToJump;

    }

    void FixedUpdate()
    {
        controller.Move(velocity * Time.deltaTime);

        //prevents gravity from building up
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        if (controller.collisions.below)
        {
            doubleJump = true; //allows a second jump
        }


    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //Storing input key
        int wallX = ((controller.collisions.left) ? -1 : 1);

        //wall climbing
        bool wallSlide = false;
        if (controller.collisions.left || controller.collisions.right && !controller.collisions.below && velocity.y < 0)
        {
            wallSlide = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }
        }

        float velocityX = input.x * speed;
        velocity.x = Mathf.SmoothDamp(velocity.x, velocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeInGround : accelerationTimeInAir); //vertical movement, slow down smoothly when stopped moving
        velocity.y += gravity * Time.deltaTime; //gravity


        if (Input.GetKey(KeyCode.Z))
        {
            if (wallSlide)
            {
                if (wallX == input.x)
                {
                    velocity.x = -wallX * wallClimbOn.x;
                    velocity.y = wallClimbOn.y;
                } else if (input.x == 0) {
                    velocity.x = -wallX * wallClimbOff.x;
                    velocity.y = wallClimbOff.y;
                } else
                {
                    velocity.x = -wallX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }

            if (controller.collisions.below)
            {
                velocity.y = jumpVelocity;
            }
            else if (doubleJump && Input.GetKeyDown(KeyCode.Z))
            {
                doubleJump = false;
                velocity.y = jumpVelocity;
            }
        }

    }
}
