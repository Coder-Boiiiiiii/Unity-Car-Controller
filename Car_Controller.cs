using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Controller : MonoBehaviour
{
    //Variables
    [Header("Wheel Colliders")]
    public WheelCollider FL;
    public WheelCollider FR;
    public WheelCollider BL;
    public WheelCollider BR;

    [Header("Wheel Transforms")]
    public Transform Fl;
    public Transform Fr;
    public Transform Bl;
    public Transform Br;

    [Header("Car Settings")]
    public float Motor_Torque = 100f;
    public float Max_Steer_Angle = 20f;
    public float  BrakeForce = 150f;

    [Header("Boost Settings")]
    public float Boost_Motor_Torque = 300f;
    public float Motor_Torque_Normal = 100f;

    [Header("Other")]
    public Transform Center_of_Mass;

    //private Variables
    private Rigidbody rb;
    private float Brakes = 0f;

    void Start(){
        //To Prevent The Car From Toppling When Turning Too Much
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Center_of_Mass.localPosition;
    }

    void FixedUpdate(){
        //Making The Car Move Forward or Backward
        BL.motorTorque = Input.GetAxis("Vertical") * Motor_Torque;
        BR.motorTorque = Input.GetAxis("Vertical") * Motor_Torque;

        //Making The Car Turn
        FL.steerAngle = Input.GetAxis("Horizontal") * Max_Steer_Angle;
        FR.steerAngle = Input.GetAxis("Horizontal") * Max_Steer_Angle;

        //Make Car Boost
        if(Input.GetKey(KeyCode.LeftShift)){
            //Setting The Motor Torque To The Boost Torque
            Motor_Torque = Boost_Motor_Torque;
        }

        else{
            //Setting The Motor Torque Back To Normal;
            Motor_Torque = Motor_Torque_Normal;
        }
    }

    public void Update(){

        //Rotating The Wheels So They Don't Slide
        var pos = Vector3.zero;
        var rot = Quaternion.identity;
        
        FL.GetWorldPose(out pos, out rot);
        Fl.position = pos;
        Fl.rotation = rot * Quaternion.Euler(0, -90, 90);

        FR.GetWorldPose(out pos, out rot);
        Fr.position = pos;
        Fr.rotation = rot * Quaternion.Euler(0, 90, -90);

        BL.GetWorldPose(out pos, out rot);
        Bl.position = pos;
        Bl.rotation = rot * Quaternion.Euler(0, -90, 90);

        BR.GetWorldPose(out pos, out rot);
        Br.position = pos;
        Br.rotation = rot * Quaternion.Euler(0, 90, -90);

        //Make Car Brake
        if(Input.GetKey(KeyCode.Space) == true){
            Brakes = BrakeForce;
        }

        else{
            Brakes = 0f;
        }

        FL.brakeTorque = Brakes;
        FR.brakeTorque = Brakes;
        BL.brakeTorque = Brakes;
        BR.brakeTorque = Brakes;
    }
}
