using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Drive : MonoBehaviour
{
     public Vector2 controlInput;
     public Rigidbody rb;
     public Transform[] tires;
     public Transform[] frontTires;
     public Transform[] rearTires;
     public float rayDistance;
     public float springStrength;
     public float springDamper;
     public float restDistance;
     public float tireGrip;
     public float tireMass;
     public float speedMultiplier;




    void Start()
    {
        rb = GetComponent<Rigidbody>();
        

    }



    void FixedUpdate()
    {
        
        controlInput[0] = Input.GetAxisRaw("Horizontal");
        controlInput[1] = Input.GetAxisRaw("Vertical");

        Suspension();
        Friction();
        Steer();
        Accelerate();
    }

    void Suspension()
    {
        foreach(Transform tire in tires)
        {
            if(tire == null)
                return;


            RaycastHit hit;

            if(Physics.Raycast(tire.transform.position, -transform.up, out hit, rayDistance))
            {
                Vector3 springDir = transform.up;
                Vector3 tireWorldVelocity = rb.GetPointVelocity(tire.transform.position);
                float offset = restDistance - hit.distance;
                float velocity = Vector3.Dot(springDir, tireWorldVelocity);
                float force = (offset * springStrength) - (velocity * springDamper);
                rb.AddForceAtPosition(springDir * force, tire.transform.position);

            }
            
            
                 Debug.DrawRay(tire.transform.position, -tire.up * rayDistance, Color.red);

        }

    }


    void Friction()
    {
        foreach(Transform tire in tires)
        {
            if(tire == null)
                return;

            RaycastHit hit;

            if(Physics.Raycast(tire.transform.position, -transform.up, out hit, rayDistance))
            {
                Vector3 steeringDir = tire.transform.right;
                Vector3 tireWorldVelocity= rb.GetPointVelocity(tire.transform.position);
                float steeringVelocity = Vector3.Dot(steeringDir, tireWorldVelocity);
                float desiredVelocityChange = -steeringVelocity * tireGrip;
                float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;

                rb.AddForceAtPosition(steeringDir * tireMass * desiredAcceleration, tire.transform.position);

            }
        }
    }

    void Steer()
    {
        foreach(Transform tire in frontTires)
        {
            tire.localRotation = Quaternion.Euler(tire.transform.rotation.x, 45 * controlInput[0], tire.transform.rotation.z);
        }

        
    }

    void Accelerate()
    {
        foreach(Transform tire in rearTires)
        {
            if(tire == null)
                return;

            RaycastHit hit;

            if(Physics.Raycast(tire.transform.position, -transform.up, out hit, rayDistance))
            {
                Vector3 accelerationDir = tire.transform.forward;


                if(controlInput[1] > 0f)
                {
                    float currentSpeed = Vector3.Dot(rb.transform.forward, rb.velocity);
                    float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(currentSpeed) / 100); //This is divided by top speed;

                    float torque = controlInput[1] * speedMultiplier;

                    rb.AddForceAtPosition(accelerationDir * torque, tire.transform.position, ForceMode.Acceleration);

                }
                else if(controlInput[1] < 0)
                {
                    float currentSpeed = Vector3.Dot(rb.transform.forward, rb.velocity);
                    float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(currentSpeed) / 100); //This is divided by top speed;

                    float torque = controlInput[1] * speedMultiplier;

                    rb.AddForceAtPosition(accelerationDir * torque, tire.transform.position, ForceMode.Acceleration);

                }

            }
        }
    }

}






