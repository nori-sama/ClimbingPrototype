using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawnScript2 : MonoBehaviour
{
    [SerializeField] float climbSpeed = 3f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.useGravity = false;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 input = SquareToCircle(new Vector2(h, v));  
        
        //Check walls in a cross pattern
        Vector3 offset = transform.TransformDirection(Vector2.one * 0.5f);
        Vector3 checkDirection = Vector3.zero;
        int k = 0;
        for(int i = 0; i<4; i++)
        {
            RaycastHit checkHit;
            if(Physics.Raycast(transform.position + offset,
                transform.forward,
                out checkHit))
            {
                //Debug.DrawRay(transform.position + offset, transform.forward * 10f, Color.red, 5f);
                checkDirection += checkHit.normal;
                k++;
            }
            // Rotate Offset by 90 degrees
            offset = Quaternion.AngleAxis(90f, transform.forward) * offset;
        }
        checkDirection /= k;

        RaycastHit hit;
        if(Physics.Raycast(transform.position,
            -checkDirection,
            out hit))
        {
            Debug.DrawRay(transform.position, transform.forward * 10f, Color.blue, 5f);
            transform.forward = -hit.normal;
            rb.position = Vector3.Lerp(rb.position,
                hit.point + hit.normal * 0.55f,
                10f * Time.fixedDeltaTime);
        }
    
        rb.velocity = transform.TransformDirection(input) * climbSpeed;
    }

    Vector2 SquareToCircle(Vector2 input)
        {
            return (input.sqrMagnitude >= 1f) ? input.normalized : input;
        }
}
