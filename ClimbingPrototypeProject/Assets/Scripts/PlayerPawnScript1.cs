using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawnScript1 : MonoBehaviour
{
    
    [SerializeField] float climbSpeed = 2f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 input = SquareToCircle(new Vector2(h, v));  
        
        RaycastHit hit;
        if(Physics.Raycast(transform.position,
            transform.forward,
            out hit))
        {
            Debug.DrawRay(transform.position, transform.forward * 10f, Color.blue, 5f);
            transform.forward = -hit.normal;
            rb.position = Vector3.Lerp(rb.position,
                hit.point + hit.normal * 0.51f,
                10f * Time.fixedDeltaTime);
        }
    
        rb.velocity = transform.TransformDirection(input) * climbSpeed;
    }

    Vector2 SquareToCircle(Vector2 input)
        {
            return (input.sqrMagnitude >= 1f) ? input.normalized : input;
        }
}
