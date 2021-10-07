using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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

    [Header("Wheel Transforms Rotations")]
    public Vector3 FL_Rotation;
    public Vector3 FR_Rotation;
    public Vector3 BL_Rotation;
    public Vector3 BR_Rotation;

    [Header("Car Settings")]
    public float Motor_Torque = 100f;
    public float Max_Steer_Angle = 20f;
    public float  BrakeForce = 150f;

    public float handBrakeFrictionMultiplier = 2;
    private float handBrakeFriction  = 0.05f;
    public float tempo;

    [Header("Boost Settings")]
    public float Boost_Motor_Torque = 300f;
    public float Motor_Torque_Normal = 100f;

    [Header("Audio Settings (Beta)")]
    public bool Enable_Audio;
    public AudioSource Engine_Sound;
    public float Max_Engine_Audio_Pitch;
    public float Min_Engine_Audio_Pitch;
    public float Min_Volume;
    public float Max_Volume;

    [Header("Drift Settings")]
    public bool Set_Drift_Settings_Automatically = true;
    public float Forward_Extremium_Value_When_Drifting;
    public float Sideways_Extremium_Value_When_Drifting;

    [Header("Light Setting(s)")]
    public bool Enable_Headlights;
    public bool Enable_Brakelights;
    public bool Enable_Reverselights;

    public Light[] HeadLights;
    public Light[] BrakeLights;
    public Light[] ReverseLights;

    [Header("Particle System(s) Settings")]
    public bool Use_Particle_Systems;
    public ParticleSystem[] Car_Smoke_From_Silencer;//Sorry, couldn't think of a better name :P

    [Header("Other Settings")]
    public Transform Center_of_Mass;
    public  float frictionMultiplier = 3f;

    //private Variables
    private Rigidbody rb;
    private float Brakes = 0f;
    private WheelFrictionCurve  FLforwardFriction, FLsidewaysFriction;
    private WheelFrictionCurve  FRforwardFriction, FRsidewaysFriction;
    private WheelFrictionCurve  BLforwardFriction, BLsidewaysFriction;
    private WheelFrictionCurve  BRforwardFriction, BRsidewaysFriction;

    //Private Audio Variables
    private float Forward_volume;
    private float Reverse_volume;
    private float Reverse_pitch;
    private float Forward_pitch;

    //Hidden Variables
    [HideInInspector]public float currSpeed;

    void Start(){
        //To Prevent The Car From Toppling When Turning Too Much
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Center_of_Mass.localPosition;

        //Play Car Particle System
        if(Use_Particle_Systems){
            foreach(ParticleSystem P in Car_Smoke_From_Silencer){
                P.Play();
            }
        }

        if(Enable_Headlights){
            foreach(Light H in HeadLights){
                H.enabled = true;
            }
        }

        if(Enable_Reverselights){
            foreach(Light R in ReverseLights){
                R.enabled = false;
            }
        }

        if(Enable_Brakelights){
            foreach(Light B in BrakeLights){
                B.enabled = false;
            }
        }
    }

    public void FixedUpdate(){
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

        //Make Car Drift
        WheelHit wheelHit1;
        WheelHit wheelHit2;
        WheelHit wheelHit3;
        WheelHit wheelHit4;

        FL.GetGroundHit(out wheelHit1);
        FR.GetGroundHit(out wheelHit2);
        BL.GetGroundHit(out wheelHit3);
        BR.GetGroundHit(out wheelHit4);

        if(wheelHit1.sidewaysSlip < 0 )	
            tempo = (1 + -Input.GetAxis("Horizontal")) * Mathf.Abs(wheelHit1.sidewaysSlip *handBrakeFrictionMultiplier);
            if(tempo < 0.5) tempo = 0.5f;
        if(wheelHit1.sidewaysSlip > 0 )	
            tempo = (1 + Input.GetAxis("Horizontal") )* Mathf.Abs(wheelHit1.sidewaysSlip *handBrakeFrictionMultiplier);
            if(tempo < 0.5) tempo = 0.5f;
        if(wheelHit1.sidewaysSlip > .99f || wheelHit1.sidewaysSlip < -.99f){
            //handBrakeFriction = tempo * 3;
            float velocity = 0;
            handBrakeFriction = Mathf.SmoothDamp(handBrakeFriction,tempo* 3,ref velocity ,0.1f * Time.deltaTime);
            }

        if(wheelHit2.sidewaysSlip < 0 )	
            tempo = (1 + -Input.GetAxis("Horizontal")) * Mathf.Abs(wheelHit2.sidewaysSlip *handBrakeFrictionMultiplier) ;
            if(tempo < 0.5) tempo = 0.5f;
        if(wheelHit2.sidewaysSlip > 0 )	
            tempo = (1 + Input.GetAxis("Horizontal") )* Mathf.Abs(wheelHit2.sidewaysSlip *handBrakeFrictionMultiplier);
            if(tempo < 0.5) tempo = 0.5f;
        if(wheelHit2.sidewaysSlip > .99f || wheelHit2.sidewaysSlip < -.99f){
            //handBrakeFriction = tempo * 3;
            float velocity = 0;
            handBrakeFriction = Mathf.SmoothDamp(handBrakeFriction,tempo* 3,ref velocity ,0.1f * Time.deltaTime);
            }

        if(wheelHit3.sidewaysSlip < 0 )	
            tempo = (1 + -Input.GetAxis("Horizontal")) * Mathf.Abs(wheelHit3.sidewaysSlip *handBrakeFrictionMultiplier) ;
            if(tempo < 0.5) tempo = 0.5f;
        if(wheelHit3.sidewaysSlip > 0 )	
            tempo = (1 + Input.GetAxis("Horizontal") )* Mathf.Abs(wheelHit3.sidewaysSlip *handBrakeFrictionMultiplier);
            if(tempo < 0.5) tempo = 0.5f;
        if(wheelHit3.sidewaysSlip > .99f || wheelHit3.sidewaysSlip < -.99f){
            //handBrakeFriction = tempo * 3;
            float velocity = 0;
            handBrakeFriction = Mathf.SmoothDamp(handBrakeFriction,tempo* 3,ref velocity ,0.1f * Time.deltaTime);
            }

        if(wheelHit4.sidewaysSlip < 0 )	
            tempo = (1 + -Input.GetAxis("Horizontal")) * Mathf.Abs(wheelHit4.sidewaysSlip *handBrakeFrictionMultiplier) ;
            if(tempo < 0.5) tempo = 0.5f;
        if(wheelHit4.sidewaysSlip > 0 )	
            tempo = (1 + Input.GetAxis("Horizontal") )* Mathf.Abs(wheelHit4.sidewaysSlip *handBrakeFrictionMultiplier);
            if(tempo < 0.5) tempo = 0.5f;
        if(wheelHit4.sidewaysSlip > .99f || wheelHit4.sidewaysSlip < -.99f){
            //handBrakeFriction = tempo * 3;
            float velocity = 0;
            handBrakeFriction = Mathf.SmoothDamp(handBrakeFriction,tempo* 3,ref velocity ,0.1f * Time.deltaTime);
            }

        else{
            handBrakeFriction = tempo;
        }

        //Enable reverse lights when car is reversing
        if(Input.GetKey(KeyCode.S)){
            if(Enable_Reverselights){
                foreach(Light RL in ReverseLights){
                    RL.enabled = true;
                }
            }
        }

        if(!Input.GetKey(KeyCode.S)){
            if(Enable_Reverselights){
                foreach(Light Rl in ReverseLights){
                    Rl.enabled = false;
                }
            }
        }
    }

    public void Update(){

        //Rotating The Wheels So They Don't Slide
        var pos = Vector3.zero;
        var rot = Quaternion.identity;
        
        FL.GetWorldPose(out pos, out rot);
        Fl.position = pos;
        Fl.rotation = rot * Quaternion.Euler(FL_Rotation);

        FR.GetWorldPose(out pos, out rot);
        Fr.position = pos;
        Fr.rotation = rot * Quaternion.Euler(FR_Rotation);

        BL.GetWorldPose(out pos, out rot);
        Bl.position = pos;
        Bl.rotation = rot * Quaternion.Euler(BL_Rotation);

        BR.GetWorldPose(out pos, out rot);
        Br.position = pos;
        Br.rotation = rot * Quaternion.Euler(BR_Rotation);

        //Make Car Brake
        if(Input.GetKey(KeyCode.Space) == true){
            Brakes = BrakeForce;

            //Turn on brake lights
            if(Enable_Brakelights){
                foreach(Light L in BrakeLights){
                    L.enabled = true;
                }
            }

            //Drifting and changing wheel collider values
            if(Set_Drift_Settings_Automatically){
                FLforwardFriction = FL.forwardFriction;
                FLsidewaysFriction = FL.sidewaysFriction;

                FLforwardFriction.extremumValue = FLforwardFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
                FLsidewaysFriction.extremumValue = FLsidewaysFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;

                FRforwardFriction = FR.forwardFriction;
                FRsidewaysFriction = FR.sidewaysFriction;

                FRforwardFriction.extremumValue = FRforwardFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
                FRsidewaysFriction.extremumValue = FRsidewaysFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;

                BLforwardFriction = BL.forwardFriction;
                BLsidewaysFriction = BL.sidewaysFriction;

                BLforwardFriction.extremumValue = BLforwardFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
                BLsidewaysFriction.extremumValue = BLsidewaysFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;

                BRforwardFriction = BR.forwardFriction;
                BRsidewaysFriction = BR.sidewaysFriction;

                BRforwardFriction.extremumValue = BRforwardFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
                BRsidewaysFriction.extremumValue = BRsidewaysFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
            }

            if(!Set_Drift_Settings_Automatically){
                //Variables
                FLforwardFriction = FL.forwardFriction;
                FLsidewaysFriction = FL.sidewaysFriction;

                FRforwardFriction = FR.forwardFriction;
                FRsidewaysFriction = FR.sidewaysFriction;

                BLforwardFriction = BL.forwardFriction;
                BLsidewaysFriction = BL.sidewaysFriction;

                BRforwardFriction = BR.forwardFriction;
                BRsidewaysFriction = BR.sidewaysFriction;

                //Setting The Extremium values to the ones that the user defined
                FLforwardFriction.extremumValue = Forward_Extremium_Value_When_Drifting;
                FLsidewaysFriction.extremumValue = Sideways_Extremium_Value_When_Drifting;

                FRforwardFriction.extremumValue = Forward_Extremium_Value_When_Drifting;
                FRsidewaysFriction.extremumValue = Sideways_Extremium_Value_When_Drifting;

                BLforwardFriction.extremumValue = Forward_Extremium_Value_When_Drifting;
                BLsidewaysFriction.extremumValue = Sideways_Extremium_Value_When_Drifting;

                BRforwardFriction.extremumValue = Forward_Extremium_Value_When_Drifting;
                BRsidewaysFriction.extremumValue = Sideways_Extremium_Value_When_Drifting;
            }
        }

        else{
            Brakes = 0f;
        }

        FL.brakeTorque = Brakes;
        FR.brakeTorque = Brakes;
        BL.brakeTorque = Brakes;
        BR.brakeTorque = Brakes;

        if(!Input.GetKey(KeyCode.Space)){
            //Turn off brake lights
            if(Enable_Brakelights){
                foreach(Light L in BrakeLights){
                    L.enabled = false;
                }
            }
        }


        if(Enable_Audio == true){
                //Play Car Audio
            if(Input.GetKey(KeyCode.W)){
                //Play Engine Sound
                Engine_Sound.Play();

                //Adjust Engine Sound Volume To Car Motor Torque
                Forward_volume = -1f * (Motor_Torque/BR.motorTorque);

                //Adjust Engine Speed
                Forward_pitch = -1f * (BR.motorTorque/Motor_Torque);

                if(Forward_volume > Max_Volume){
                    Forward_volume = Max_Volume;

                    if(Forward_pitch > Max_Engine_Audio_Pitch){
                        Forward_pitch = Max_Engine_Audio_Pitch;

                        Engine_Sound.volume = Forward_volume;
                        Engine_Sound.pitch = Forward_pitch;
                    }

                    if(Forward_pitch < Min_Engine_Audio_Pitch){
                        Forward_pitch = Min_Engine_Audio_Pitch;

                        Engine_Sound.volume = Forward_volume;
                        Engine_Sound.pitch = Forward_pitch;
                    }

                    else{
                        Engine_Sound.volume = Forward_volume;
                        Engine_Sound.pitch = Forward_pitch;
                    }
                }

                if(Forward_volume < Min_Volume){
                    Forward_volume = Min_Volume;

                    if(Forward_pitch > Max_Engine_Audio_Pitch){
                        Forward_pitch = Max_Engine_Audio_Pitch;

                        Engine_Sound.volume = Forward_volume;
                        Engine_Sound.pitch = Forward_pitch;
                    }

                    if(Forward_pitch < Min_Engine_Audio_Pitch){
                        Forward_pitch = Min_Engine_Audio_Pitch;

                        Engine_Sound.volume = Forward_volume;
                        Engine_Sound.pitch = Forward_pitch;
                    }

                    else{
                        Engine_Sound.volume = Forward_volume;
                        Engine_Sound.pitch = Forward_pitch;
                    }
                }
            }

            if(Input.GetKey(KeyCode.S)){
                //Play Engine Sound
                Engine_Sound.Play();

                //Adjust Engine Sound Volume To Car Motor Torque
                Reverse_volume = Motor_Torque/BR.motorTorque;

                //Adjust Audio To Engine Speed
                Reverse_pitch = -1f * (BR.motorTorque/Motor_Torque);

                if(Forward_volume > Max_Volume){
                    Forward_volume = Max_Volume;

                    if(Forward_pitch > Max_Engine_Audio_Pitch){
                        Forward_pitch = Max_Engine_Audio_Pitch;

                        Engine_Sound.volume = Forward_volume;
                        Engine_Sound.pitch = Forward_pitch;
                    }

                    if(Forward_pitch < Min_Engine_Audio_Pitch){
                        Forward_pitch = Min_Engine_Audio_Pitch;

                        Engine_Sound.volume = Forward_volume;
                        Engine_Sound.pitch = Forward_pitch;
                    }

                    else{
                        Engine_Sound.volume = Forward_volume;
                        Engine_Sound.pitch = Forward_pitch;
                    }
                }

                if(Forward_volume < Min_Volume){
                    Forward_volume = Min_Volume;

                    if(Forward_pitch > Max_Engine_Audio_Pitch){
                        Forward_pitch = Max_Engine_Audio_Pitch;

                        Engine_Sound.volume = Forward_volume;
                        Engine_Sound.pitch = Forward_pitch;
                    }

                    if(Forward_pitch < Min_Engine_Audio_Pitch){
                        Forward_pitch = Min_Engine_Audio_Pitch;

                        Engine_Sound.volume = Forward_volume;
                        Engine_Sound.pitch = Forward_pitch;
                    }

                    else{
                        Engine_Sound.volume = Forward_volume;
                        Engine_Sound.pitch = Forward_pitch;
                    }
                }
            }
        }
        
    }
}
