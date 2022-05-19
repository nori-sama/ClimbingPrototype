using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimerScript : MonoBehaviour
{
    private PlayerBallController ballCtrl;
    private float inputU, inputV;
    public float sensitivity = 6;

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
        //Debug.Log("input: "+input);

        //Smoothing for ray rotation
        float stepSmoothing = sensitivity * Time.fixedDeltaTime;
        
         // Calculate a rotation a step closer to the target and applies rotation to this object 
        // Should take the local direction and rotate it towards whatever the input direction is. Local space outputted as world space by function TransformDirection() and .forward
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, transform.parent.TransformDirection(input), stepSmoothing, 0.0f);

        //Matches rotation to desired rotation by world space rotation
        transform.rotation = Quaternion.LookRotation(newDirection, transform.parent.up);

        //For now the aiming is handled in this script and added onto this child object instead of directly to the main game object (parent)
        ballCtrl.aim = transform.forward; //Returns local space forward direction in world space coordinates
    }

    Vector3 SquareToCircle(Vector3 input)
    {
        //Makes sure that the input returns with equal force in any direction
        return (input.sqrMagnitude >= 1f) ? input.normalized : input;
    }

    private void ProcessInputs()
    {
        inputU = Input.GetAxis("Horizontal"); //U as in UV coordinates and X in 3D coordinates
        inputV = Input.GetAxis("Vertical"); //V as in UV coordinates and Y in 3D coordinates
    }
}
