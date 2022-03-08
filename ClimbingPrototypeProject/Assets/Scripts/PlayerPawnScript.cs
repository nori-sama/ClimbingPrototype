using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawnScript : MonoBehaviour
{
    public enum PlayerState
    {
        WALKING,
        FALLING,
        CLIMBING
    }
    [SerializeField] public PlayerState state = PlayerState.CLIMBING;
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float climbSpeed = 2f;

    Rigidbody rb;

    float h = 0f;
    float v = 0f;
    bool jumpDown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.useGravity = false;
    }

    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        if (!jumpDown)
            jumpDown = Input.GetButtonDown("Jump");
    }

    void FixedUpdate()
    {
        Vector2 input = SquareToCircle(new Vector2(h, v));
        //Transform cam = Camera.main.transform;
        //Vector3 moveDirection = Quaternion.FromToRotation(cam.up, Vector3.up) 
        //* cam.TransformDirection(new Vector3(input.x, 0f, input.y)); 
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);

        switch(state)
        {
            case PlayerState.WALKING: {HandleWalking(moveDirection);} break;
            case PlayerState.FALLING: {HandleFalling();} break;
            case PlayerState.CLIMBING: {HandleClimbing(input);} break;
        }

        RaycastHit hit;
        Ray dwnRay = new Ray(transform.position, Vector3.down); // Position, Direction

        if(Physics.Raycast(dwnRay, out hit, 1.02f)) 
            {
            Debug.DrawRay(dwnRay.origin, dwnRay.direction * 10f, Color.green, 5f);
            state = PlayerState.WALKING;
            }
        else if(state == PlayerState.WALKING)
            {
            state = PlayerState.FALLING;
            }
        rb.useGravity = state != PlayerState.CLIMBING;

        //Reset input
        jumpDown = false;
    }

    void HandleWalking(Vector3 moveDirection)
    {
        Vector3 prevVel = rb.velocity;
        Vector3 nextVel = moveDirection * walkSpeed;
        nextVel.y = prevVel.y;
        if(jumpDown)
        {
            nextVel.y = 5f;
            state = PlayerState.FALLING;
        }
        rb.velocity = nextVel;

        if(moveDirection.sqrMagnitude > 0.01f)
        {
            transform.forward = moveDirection;
        }
    }

    void HandleFalling()
    {
        if(jumpDown && Physics.Raycast(transform.position, transform.forward*0.4f))
            state = PlayerState.CLIMBING;
    }

    void HandleClimbing(Vector2 input)
    {
        //Check walls in cross pattern
        Vector3 offset = transform.TransformDirection(Vector2.one * 0.5f);
        Vector3 checkDirection = Vector3.zero;
        int k = 0;
        for (int i = 0; i < 4; i++)
        {
            Ray fwdTempRay = new Ray(transform.position + offset, transform.forward);
            RaycastHit checkHit;
            Debug.DrawRay(fwdTempRay.origin, fwdTempRay.direction * 10f, Color.blue, 5f);
            if(Physics.Raycast(fwdTempRay, // Ray (Position & Direction)
            out checkHit)) // Hit data
            {
                checkDirection += checkHit.normal;
                k++;
            }
            //Rotate Offset by 90 degrees
            offset = Quaternion.AngleAxis(90f, transform.forward) * offset;
        }
        checkDirection /= k;

        //Check wall directly in front
        Ray fwdRay = new Ray(transform.position,-checkDirection);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, // Position
        -checkDirection, // Direction
        out hit)) // Hit data
        {
            Debug.DrawRay(fwdRay.origin, fwdRay.direction * 10f, Color.blue, 5f);

            float dot = Vector3.Dot(transform.forward, -hit.normal);

            rb.position = Vector3.Lerp(rb.position, 
            hit.point + hit.normal * 0.55f, 
            5f * Time.fixedDeltaTime);

            //Change forward direction according to hit normal
            transform.forward = Vector3.Lerp(transform.forward, 
            -hit.normal, 
            10f * Time.fixedDeltaTime);

            rb.useGravity = false; 
            rb.velocity = transform.TransformDirection(input) * climbSpeed;
            if(jumpDown)
            {
                //With jump
                rb.velocity = Vector3.up * 5f + hit.normal * 2f;
                state = PlayerState.FALLING;
            }
            else
            {
                //No jump
                state = PlayerState.FALLING;
            }
        }

    }

    Vector2 SquareToCircle(Vector2 input)
        {
            return (input.sqrMagnitude >= 1f) ? input.normalized : input;
        }

}
