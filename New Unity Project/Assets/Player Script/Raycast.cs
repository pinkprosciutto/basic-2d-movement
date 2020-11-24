using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Raycast : MonoBehaviour
{
    public BoxCollider2D boxCollider;
    public raycastOrigin RaycastOrigin;
    public const float skinWidth = .02f;
    
    [Header("Raycast")]
    public int horizontalRayCount = 3;
    public int verticalRayCount = 3;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    public virtual void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigin()
    {
        Bounds bound = boxCollider.bounds;
        bound.Expand(skinWidth * -2); //shrinking the bound

        RaycastOrigin.topLeft = new Vector2(bound.min.x, bound.max.y); //upper left of the box
        RaycastOrigin.topRight = new Vector2(bound.max.x, bound.max.y); //upper right of the box
        RaycastOrigin.bottomLeft = new Vector2(bound.min.x, bound.min.y); //bottom left of the box
        RaycastOrigin.bottomRight = new Vector2(bound.max.x, bound.min.y); //bottom right of the box
    }

    //To give spacing between the raycast lines
    public void CalculateRaySpacing()
    {
        Bounds bound = boxCollider.bounds;
        bound.Expand(skinWidth * -2); //shrinking the bound

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bound.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bound.size.x / (verticalRayCount - 1);
    }


    //getting corners of BoxCollider2D
    public struct raycastOrigin
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;

    }
}
