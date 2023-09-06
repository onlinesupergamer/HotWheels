using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.Rendering;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public Vector2 controlInput;
    public Rigidbody rb;
    public Transform[] tires;
    public Transform[] frontTires;
    public Transform[] rearTires;
    public AnimationCurve turningCurve;
    public AnimationCurve speedCurve;
    public AnimationCurve frictionCurve;
    public float rayDistance;
    public float springStrength;
    public float springDamper;
    public float restDistance;
    public float tireGrip;
    public float tireMass;
    public float speedMultiplier;
    public bool bisGrounded;
    public float turnAmount;
    float steeringAmount;
    RaycastHit hit;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //Application.targetFrameRate = 30; //This is for debugging only DO NOT ENABLE FOR SHIPPING
        

    }

    void FixedUpdate()
    {
        
        controlInput[0] = Input.GetAxisRaw("Horizontal");
        controlInput[1] = Input.GetAxisRaw("Vertical");

        Gravity();
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


            

            if(Physics.Raycast(tire.transform.position, -transform.up, out hit, rayDistance))
            {
                Vector3 springDir = transform.up;
                Vector3 tireWorldVelocity = rb.GetPointVelocity(tire.transform.position);
                float offset = restDistance - hit.distance;
                float velocity = Vector3.Dot(springDir, tireWorldVelocity);
                float force = (offset * springStrength) - (velocity * springDamper);
                rb.AddForceAtPosition(springDir * force, tire.transform.position);
                bisGrounded = true;

            }
            else
            {
                bisGrounded = false;
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

            

            if(Physics.Raycast(tire.transform.position, -transform.up, out hit, rayDistance))
            {
                Vector3 steeringDir = tire.transform.right;
                Vector3 tireWorldVelocity = rb.GetPointVelocity(tire.transform.position);
                float sideSpeed = Vector3.Dot(rb.transform.right, rb.velocity);
                Mathf.Clamp01(sideSpeed);
                float rawSpeed = Mathf.InverseLerp(-1, 1, sideSpeed);
                float convertedSpeed = Mathf.Lerp(0, 1, rawSpeed);


                float frictionValue = frictionCurve.Evaluate(convertedSpeed * 3);
                

                float steeringVelocity = Vector3.Dot(steeringDir, tireWorldVelocity);
                float desiredVelocityChange = -steeringVelocity * tireGrip;
                //Debug.Log(steeringVelocity);


                //This method seems scuffed, but it works
                if(steeringVelocity < -3f)
                {
                    //Debug.Log("Sliding Right");
                }

                if(steeringVelocity > 3f)
                {
                    //Debug.Log("Sliding Left");
                }



                float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;
                //Debug.Log(frictionValue);
                

                rb.AddForceAtPosition(steeringDir * tireMass * desiredAcceleration, tire.transform.position);
                
            }
        }
    }

    void Steer()
    {
        foreach(Transform tire in frontTires)
        {
            float wheelSpeed = Mathf.Abs(rb.velocity.magnitude);
            float clampedSpeed = Mathf.Clamp01(Mathf.Abs(wheelSpeed));
            float newSpeed = turningCurve.Evaluate(clampedSpeed);
            float currentTurnAmount = turnAmount * newSpeed;

            float speed = Vector3.Dot(rb.transform.forward, rb.velocity);

            //I hate this, why does this work
            float rawSpeed = Mathf.InverseLerp(-18, 18, speed);
            float convertedSpeed = Mathf.Lerp(0, 1, rawSpeed);

            float clampedConvertedSpeed = Mathf.Clamp01(Mathf.Abs(convertedSpeed * 4));
            float affectedSteering = turningCurve.Evaluate(convertedSpeed);
            
            
            

            tire.localRotation = Quaternion.Euler(tire.transform.rotation.x, controlInput[0] * (affectedSteering * clampedConvertedSpeed) * turnAmount, tire.transform.rotation.z);
            
            
        }

        
    }

    void Accelerate()
    {
        foreach(Transform tire in tires)
        {
            if(tire == null)
                return;

            

            if(Physics.Raycast(tire.transform.position, -transform.up, out hit, rayDistance))
            {
                Vector3 accelerationDir = tire.transform.forward;


                if(controlInput[1] > 0f)
                {
                    if(rb.velocity.magnitude >= 18)
                        return;


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

    void Gravity()
    {
        if(!bisGrounded)
        {
            rb.AddForce(-Vector3.up * 2200f);
            
        }
        else
        {
            rb.AddForce(-hit.normal * 1800f);
        }

    }


    void MovementBuffer()
    {
        Vector3 tmpbuffer = rb.velocity;




        
    }


    void OnCollisionEnter(Collision other)
    {
        //Debug.Log(other.collider.name);


    }


}

