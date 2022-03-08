using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    [Tooltip("Maximum slope the character can jump on")]
    [Range(5f, 60f)]
    public float slopeLimit = 45f;
    [Tooltip("Move speed in meters/second")]
    public float moveSpeed = 2f;
    [Tooltip("Turn speed in degrees/second, left (+) or right (-)")]
    public float turnSpeed = 300;
    [Tooltip("Whether the character can jump")]
    public bool allowJump = true;
    [Tooltip("Upward speed to apply when jumping in meters/second")]
    public float jumpSpeed = 4f;

    public bool IsGrounded { get; private set; }
    public bool IsWallContact { get; private set; }
    public float ForwardInput { get; set; }
    public float TurnInput { get; set; }
    public bool JumpInput { get; set; }

    new private Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        ProcessActions();
    }

    /// <summary>
    /// Checks whether the character is on the ground and updates <see cref="IsGrounded"/>
    /// </summary>
    private void CheckGrounded()
    {
        IsGrounded = false;
        IsWallContact = false;
        float capsuleHeight = Mathf.Max(capsuleCollider.radius * 2f, capsuleCollider.height);
        Vector3 capsuleBottom = transform.TransformPoint(capsuleCollider.center - Vector3.up * capsuleHeight / 2f);
        float radius = transform.TransformVector(capsuleCollider.radius, 0f, 0f).magnitude;

        Ray ray1 = new Ray(capsuleBottom + transform.up * .01f, -transform.up);
        Ray ray2 = new Ray(capsuleBottom - transform.forward * .01f, transform.forward);
        RaycastHit hit1;
        RaycastHit hit2;

        if (Physics.Raycast(ray1, out hit1, radius * 5f))
        {
            float normalAngle = Vector3.Angle(hit1.normal, transform.up);
            //Debug.DrawRay(ray1.origin, ray1.direction * 10f, Color.blue, 5f);
            if (normalAngle < slopeLimit)
            {
                float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + .02f;
                if (hit1.distance < maxDist)
                    IsGrounded = true;
            }
        }
        
        if (Physics.Raycast(ray2, out hit2, radius * 5f))
        {
            float normalAngle = Vector3.Angle(hit2.normal, transform.forward);
            Debug.DrawRay(ray2.origin, ray2.direction * 10f, Color.red, 5f);
            if (normalAngle > slopeLimit)
            {
                float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + .02f;
                
                if (hit2.distance < maxDist)
                    IsWallContact = true;   
            }
        }
    }

    /// <summary>
    /// Processes input actions and converts them into movement
    /// </summary>
    private void ProcessActions()
    {
        // Turning
       // if (TurnInput != 0f)
        //{
        //    float angle = Mathf.Clamp(TurnInput, -1f, 1f) * turnSpeed;
        //    transform.Rotate(Vector3.up, Time.fixedDeltaTime * angle); //Character is just rotating if you use A or D key, which is weird
        //}

        // Movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f ){

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            rigidbody.MovePosition(transform.position + (direction * moveSpeed * Time.deltaTime)); 
            
            //Vector3 move = transform.forward * Mathf.Clamp(ForwardInput, -1f, 1f) * moveSpeed * Time.fixedDeltaTime;
            //rigidbody.MovePosition(transform.position + move); //Only movement for forward transformation, which is weird
        } 

        if(direction.magnitude >= 0.1f && IsWallContact){

            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            direction = new Vector3(horizontal, vertical, 0f);

            rigidbody.MovePosition(transform.position + (direction * moveSpeed * Time.deltaTime)); 
            
            //Vector3 move = transform.forward * Mathf.Clamp(ForwardInput, -1f, 1f) * moveSpeed * Time.fixedDeltaTime;
            //rigidbody.MovePosition(transform.position + move); //Only movement for forward transformation, which is weird
        } 

        // Jump
        if (JumpInput && allowJump && IsGrounded)
        {
            print("Jumped");
            rigidbody.AddForce(transform.up * jumpSpeed, ForceMode.VelocityChange);
        }
    }

}

/*

MIT License

Copyright (c) 2021 Immersive Limit LLC

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/