using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main script for player movement
public class PlayerBallController : MonoBehaviour
{
    public Rigidbody rb;
    public bool modGravity = false;
    public float artificialG = 1.5f;
    public bool isGrounded;
    public bool isAirbourne;
    public bool isClimbing;
    public float moveSpeed = 20f; 
    public float climbFactor = 1.6f; //Factor to multiply moveSpeed by when climbing
    public float jumpForce = 100f; 
    public float brakeFactor = 1.2f; //Factor to multiply rb.velocity by when using brakes
    private float inputU, inputV;
    public Vector3 aim;
    private Vector3 input;

    [SerializeField] private float inputSensitivity;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        //Debug.DrawRay(transform.position, transform.forward * 10, Color.green);
    }

    private void FixedUpdate()
    {
        
        input = SquareToCircle(new Vector3(inputU, 0, inputV));

        //if(modGravity)
        //    rb.AddForce(-Physics.gravity *(rb.mass * artificialG));
    
        if (Input.GetKeyDown(KeyCode.Space)){
            rb.AddForce(transform.up * jumpForce);
            Debug.Log("jump");
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, aim, out hit) && isClimbing)
        {
                Climb(hit); //XY plane relative to hit.normal
        }
        else if(isGrounded && isClimbing)
        {
            Climb(hit); //XZ plane
        }
        else if(isGrounded && !isClimbing)
        {
            Move();
        }
        else
        {
           if(input != Vector3.zero)
           {
                Move();
           }
        }

        if(input == Vector3.zero)
            Brake();

    }

    private void ProcessInputs()
    {
        inputU = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
    }

    private void Brake()
    {
        rb.AddForce(rb.velocity*-brakeFactor);
    }
    
    private void Move()
    {
        modGravity = false;
        transform.forward = Vector3.forward;
        rb.AddForce(input*moveSpeed); //Move on X and Z (Horizontal plane)
    }

    private void Climb(RaycastHit hit)
    {
        modGravity = true;
        transform.forward = -hit.normal;
        rb.AddRelativeForce(new Vector3(inputU, inputV, 0)*moveSpeed*climbFactor); //Move on X an Y (Vertical Z plane)

    }

    void OnTriggerEnter(Collider collision)
     {
        //Ground layer = 6
        if (collision.gameObject.layer == 6 && !isGrounded)
        { //check the int value in layer manager(User Defined starts at 6)    
             isGrounded = true;
        }
        //Wall layer = 7
        if (collision.gameObject.layer == 7 && !isClimbing)
        {
             isClimbing = true;
        }
     }

    void OnTriggerExit(Collider collision)
     {
         if (collision.gameObject.layer == 6 && isGrounded)
         {
             isGrounded = false;
         }
         if (collision.gameObject.layer == 7 && isClimbing)
        {
             isClimbing = false;
        }

     }

     void OnTriggerStay(Collider collision)
     {
         if (collision.gameObject.layer == 7 && isClimbing)
         {
             isClimbing = true;
         }
          if (collision.gameObject.layer == 8 && !isGrounded && !isClimbing)
        { //check the int value in layer manager(User Defined starts at 6)    
             isAirbourne = true;
        }
     }

     Vector2 SquareToCircle(Vector2 input)
    {
        return (input.sqrMagnitude >= 1f) ? input.normalized : input;
    }

    Vector3 SquareToCircle(Vector3 input)
    {
        return (input.sqrMagnitude >= 1f) ? input.normalized : input;
    }
}