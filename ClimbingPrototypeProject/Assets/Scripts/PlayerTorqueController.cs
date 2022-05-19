using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTorqueController : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed = 10f;
    public float stopSpeed = 0.3f;
    private float inputU, inputV;

    [SerializeField] private Transform PlayerCamera;
    
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        Debug.DrawRay(transform.position, transform.forward * 10, Color.red); //Local x-axis
    }

    private void FixedUpdate()
    {
        Vector2 input = SquareToCircle(new Vector2(inputU, inputV));
        if(input != Vector2.zero)
            Move();
        else
        {
            StopTorque();
        }
    }

    private void ProcessInputs()
    {
        inputU = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
    }
    
    private void StopTorque()
    {
        rb.angularVelocity *= stopSpeed;
    }

    private void Move()
    {
        rb.AddTorque(new Vector3(inputV, 0, -inputU) * moveSpeed);
    }

    Vector2 SquareToCircle(Vector2 input)
    {
        return (input.sqrMagnitude >= 1f) ? input.normalized : input;
    }
}