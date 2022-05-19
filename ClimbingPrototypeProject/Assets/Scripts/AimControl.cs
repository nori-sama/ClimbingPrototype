using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimControl : MonoBehaviour
{
    private PlayerBallController ballCtrl;
    private float inputU, inputV;
    public float sensitivity = 6;

    private Quaternion psuedoRot;

    void Awake()
    {
        //To assign values to our aim variable. Aim variable not used in current version of PlayerBallController script.
        ballCtrl = GetComponentInParent<PlayerBallController>();
    }
    
    void Update()
    {
        ProcessInputs();
        Debug.DrawRay(transform.position, transform.forward * 10, Color.magenta);
    }

    void FixedUpdate()
    {
        Vector3 input = SquareToCircle(new Vector3(inputU, 0, inputV));
        //Vector3 sqrInput = new Vector3(inputU, 0, inputV);
        Debug.Log("input: "+input);
        //Vector3 relativeInput = transform.parent.InverseTransformDirection(input);
        //Debug.Log("relative: "+ relativeInput);

        float stepSmoothing = sensitivity * Time.fixedDeltaTime;
        
        // Should take the local rotation/direction and rotate it towards whatever the input vector/direction is
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, transform.parent.TransformDirection(input), stepSmoothing, 0.0f);

        // Calculate a rotation a step closer to the target and applies rotation to this object (local rotation)
        transform.rotation = Quaternion.LookRotation(newDirection, transform.parent.up);

        //For now the aiming is handled in this script and added onto a child object instead of directly to the main game object
        ballCtrl.aim = transform.forward; //Returns in world space coordinates
    }

    Vector3 SquareToCircle(Vector3 input)
    {
        //Makes sure that the input returns with equal force in any direction
        return (input.sqrMagnitude >= 1f) ? input.normalized : input;
    }

    private void ProcessInputs()
    {
        inputU = Input.GetAxis("Horizontal"); //U
        inputV = Input.GetAxis("Vertical"); //V
    }
}
