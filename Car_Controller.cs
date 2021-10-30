using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Car_Controller : MonoBehaviour
{
    //Public Variables
    [Header("Wheel Colliders")]
    public List<WheelCollider> Front_Wheels;
    public List<WheelCollider> Back_Wheels;

    [Header("Wheel Transforms")]
    public List<Transform> Wheel_Transforms;

    [Header("Wheel Transforms Rotations")]
    public List<Vector3> Wheels_Rotation;

    [Header("Car Settings")]
    public float Motor_Torque = 100f;
    public float Max_Steer_Angle = 20f;
    public float  BrakeForce = 150f;
    public float Maximum_Speed;

    [Space(15)]

    public float handBrakeFrictionMultiplier = 2;
    private float handBrakeFriction  = 0.05f;
    public float tempo;

    [Header("Boost Settings")]
    public float Boost_Motor_Torque = 300f;
    public float Motor_Torque_Normal = 100f;

    [Header("Audio Settings")]
    public bool Enable_Audio;
    public bool Enable_Engine_Audio;
    public AudioSource Engine_Sound;
    public float Minimum_Pitch_Value;
    public float Maximum_Pitch_Value;

    [Space(15)]

    public bool Enable_Horn;
    public AudioSource Horn_Source;
    public KeyCode Car_Horn_Key;

    [Header("Crash System")]
    public bool Enable_Crash_Noise;
    public string[] Crash_Object_Tags;
    public AudioSource Crash_Sound;

    [Header("Drift Settings")]
    public bool Set_Drift_Settings_Automatically = true;
    public float Forward_Extremium_Value_When_Drifting;
    public float Sideways_Extremium_Value_When_Drifting;

    [Header("Light Setting(s)")]

    [Header("Light Settings (With Light Objects)")]
    public bool Enable_Headlights_Lights;
    public bool Enable_Brakelights_Lights;
    public bool Enable_Reverselights_Lights;
    public KeyCode Headlights_Key;
    

    public Light[] HeadLights;
    public Light[] BrakeLights;
    public Light[] ReverseLights;

    [Space(15)]

    [Header("Light Settings (With MeshRenderers)")]
    public bool Enable_Headlights_MeshRenderers;
    public bool Enable_Brakelights_MeshRenderers;
    public bool Enable_Reverselights_MeshRenderers;

    public MeshRenderer[] HeadLights_MeshRenderers;
    public MeshRenderer[] BrakeLights_MeshRenderers;
    public MeshRenderer[] ReverseLights_MeshRenderers;

    [Header("Particle System(s) Settings")]
    public bool Use_Particle_Systems;
    public ParticleSystem[] Car_Smoke_From_Silencer;//Sorry, couldn't think of a better name :P

    [Header("Radio Settings")]
    public bool Enable_Radio;
    public KeyCode Next_Song_Key;
    public KeyCode Prev_Song_Key;
    public List<AudioClip> Tracks;
    public AudioSource Player_Source;

    [Header("Other Settings")]
    public Transform Center_of_Mass;
    public  float frictionMultiplier = 3f;
    public Rigidbody Car_Rigidbody;

    [Header("Debug")] //These are variables that are read only so dont chnage them, they are only there if u wanna use them for UI like speed or RPM;
    public float RPM_FL;
    public float RPM_FR;
    public float RPM_BL;
    public float RPM_BR;

    [Space(15)]

    public float Car_Speed_KPH;
    public float Car_Speed_MPH;
    
    [Space(15)]

    public bool HeadLights_On;

    //Debug Values in Int Form
    public int Car_Speed_In_KPH;
    public int Car_Speed_In_MPH;

    //private Variables
    private Rigidbody rb;
    private float Brakes = 0f;
    private WheelFrictionCurve  Wheel_forwardFriction, Wheel_sidewaysFriction;

    //Private Audio Variables
    private float pitch;
    private int Current_Track;

    //Hidden Variables (not private, but hidden in inspector)
    [HideInInspector] public float currSpeed;

    void Start(){
        //To Prevent The Car From Toppling When Turning Too Much
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Center_of_Mass.localPosition;

        //Play Car Smoke Particle System
        if(Use_Particle_Systems){
            foreach(ParticleSystem P in Car_Smoke_From_Silencer){
                P.Play();
            }
        }

        //Play the first song
        if(Enable_Radio){
            Player_Source.enabled = false;
            Player_Source.clip = Tracks[Current_Track];
            Player_Source.enabled = true;
        }
        
        //Here we just set the lights to turn on and off at play.

        //We turn the headlights on/off here
        if(Enable_Headlights_Lights && HeadLights_On){
            foreach(Light H in HeadLights){
                H.enabled = true;
            }
        }

        if(Enable_Headlights_MeshRenderers && HeadLights_On){
            foreach(MeshRenderer HM in HeadLights_MeshRenderers){
                HM.enabled = true;
            }
        }

        //Here we turn the reverse light(s) off
        if(Enable_Reverselights_Lights){
            foreach(Light R in ReverseLights){
                R.enabled = false;
            }
        }

        if(Enable_Reverselights_MeshRenderers){
            foreach(MeshRenderer RM in ReverseLights_MeshRenderers){
                RM.enabled = false;
            }
        }

        //Here we turn off the brakelights
        if(Enable_Brakelights_Lights){
            foreach(Light B in BrakeLights){
                B.enabled = false;
            }
        }

        if(Enable_Brakelights_MeshRenderers){
            foreach(MeshRenderer BM in BrakeLights_MeshRenderers){
                BM.enabled = true;
            }
        }

        //Turning some things off if their options are disabled
        if(!Enable_Horn && Horn_Source != null){
            Horn_Source.gameObject.SetActive(false);
        }

        if(!Enable_Engine_Audio && Engine_Sound != null){
            Engine_Sound.gameObject.SetActive(false);
        }

        if(!Enable_Audio && (Engine_Sound != null || Horn_Source != null)){
            Horn_Source.gameObject.SetActive(false);
            Engine_Sound.gameObject.SetActive(false);
        }
    }

    public void FixedUpdate(){
        //Check the keys for headlights and turn them off/on
        if(Input.GetKeyDown(Headlights_Key) && (Enable_Headlights_Lights || Enable_Brakelights_MeshRenderers)){
            if(Input.GetKeyUp(Headlights_Key) && (Enable_Headlights_Lights || Enable_Brakelights_MeshRenderers)){
                if(HeadLights_On){
                    HeadLights_On = false;
                }

                if(!HeadLights_On){
                    HeadLights_On = true;
                }
            }
        }

        //Check if the headlights were turned on or off
        if(Enable_Headlights_Lights && HeadLights_On){
            foreach(Light H in HeadLights){
                H.enabled = true;
            }
        }

        if(Enable_Headlights_MeshRenderers && HeadLights_On){
            foreach(MeshRenderer HM in HeadLights_MeshRenderers){
                HM.enabled = true;
            }
        }

        if(Enable_Headlights_Lights && !HeadLights_On){
            foreach(Light H in HeadLights){
                H.enabled = false;
            }
        }

        if(Enable_Headlights_MeshRenderers && !HeadLights_On){
            foreach(MeshRenderer HM in HeadLights_MeshRenderers){
                HM.enabled = false;
            }
        }

        //The radio
        if(Enable_Radio){
            //Checking if the current song has finished playing
            if(!Player_Source.isPlaying){

                //Checking if the track was the last one
                if(Tracks.Last() == Tracks[Current_Track]){
                    Current_Track = 0;
                    Player_Source.enabled = false;
                    Player_Source.clip = Tracks[Current_Track];
                    Player_Source.enabled = true;
                }

                else{
                    Current_Track++;
                    Player_Source.enabled = false;
                    Player_Source.clip = Tracks[Current_Track];
                    Player_Source.enabled = true;
                }
            }

            //Checking if player went to the next song/track
            if(Input.GetKeyDown(Next_Song_Key)){
                if(Input.GetKeyUp(Next_Song_Key)){
                    if(Tracks.Last() == Tracks[Current_Track]){
                        Current_Track = 0;
                        Player_Source.enabled = false;
                        Player_Source.clip = Tracks[Current_Track];
                        Player_Source.enabled = true;
                    }

                    else{
                        Current_Track++;
                        Player_Source.enabled = false;
                        Player_Source.clip = Tracks[Current_Track];
                        Player_Source.enabled = true;
                    }
                }
            }

            //Checking if the player went to the previous song/track
            if(Input.GetKeyDown(Prev_Song_Key)){
                if(Input.GetKeyUp(Prev_Song_Key)){
                    if(Tracks.Last() == Tracks[Current_Track]){
                        Current_Track = 0;
                        Player_Source.enabled = false;
                        Player_Source.clip = Tracks[Current_Track];
                        Player_Source.enabled = true;
                    }

                    else{
                        Current_Track++;
                        Player_Source.enabled = false;
                        Player_Source.clip = Tracks[Current_Track];
                        Player_Source.enabled = true;
                    }
                }
            }
        }
        //Applying Maximum Speed
        if(Car_Speed_In_KPH < Maximum_Speed){

            //Making The Car Move Forward or Backward
            foreach(WheelCollider Wheel in Back_Wheels){
                Wheel.motorTorque = Input.GetAxis("Vertical") * Motor_Torque;
            }
        }

        if(Car_Speed_In_KPH > Maximum_Speed){
            foreach(WheelCollider Wheel in Back_Wheels){
                Wheel.motorTorque = 0;
            }
        }

        //Making The Car Turn/Steer
        foreach(WheelCollider Wheel in Front_Wheels){
            Wheel.steerAngle = Input.GetAxis("Horizontal") * Max_Steer_Angle;
        }

        //Changing speed of the car
        Car_Speed_KPH = Car_Rigidbody.velocity.magnitude * 3.6f;
        Car_Speed_MPH = Car_Rigidbody.velocity.magnitude * 2.237f;

        Car_Speed_In_KPH = (int) Car_Speed_KPH;
        Car_Speed_In_MPH = (int) Car_Speed_MPH;

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
        WheelHit wheelHit;

        foreach(WheelCollider Wheel in Back_Wheels){
            Wheel.GetGroundHit(out wheelHit);

            if(wheelHit.sidewaysSlip < 0 )	
                tempo = (1 + -Input.GetAxis("Horizontal")) * Mathf.Abs(wheelHit.sidewaysSlip *handBrakeFrictionMultiplier);

                if(tempo < 0.5) tempo = 0.5f;

            if(wheelHit.sidewaysSlip > 0 )	
                tempo = (1 + Input.GetAxis("Horizontal") )* Mathf.Abs(wheelHit.sidewaysSlip *handBrakeFrictionMultiplier);

                if(tempo < 0.5) tempo = 0.5f;

            if(wheelHit.sidewaysSlip > .99f || wheelHit.sidewaysSlip < -.99f){
                //handBrakeFriction = tempo * 3;
                float velocity = 0;
                handBrakeFriction = Mathf.SmoothDamp(handBrakeFriction,tempo* 3,ref velocity ,0.1f * Time.deltaTime);
                }

            else{
                handBrakeFriction = tempo;
            }
        }

        foreach(WheelCollider Wheel in Front_Wheels){
            Wheel.GetGroundHit(out wheelHit);

            if(wheelHit.sidewaysSlip < 0 )	
                tempo = (1 + -Input.GetAxis("Horizontal")) * Mathf.Abs(wheelHit.sidewaysSlip *handBrakeFrictionMultiplier);

                if(tempo < 0.5) tempo = 0.5f;

            if(wheelHit.sidewaysSlip > 0 )	
                tempo = (1 + Input.GetAxis("Horizontal") )* Mathf.Abs(wheelHit.sidewaysSlip *handBrakeFrictionMultiplier);

                if(tempo < 0.5) tempo = 0.5f;

            if(wheelHit.sidewaysSlip > .99f || wheelHit.sidewaysSlip < -.99f){
                //handBrakeFriction = tempo * 3;
                float velocity = 0;
                handBrakeFriction = Mathf.SmoothDamp(handBrakeFriction,tempo* 3,ref velocity ,0.1f * Time.deltaTime);
                }

            else{
                handBrakeFriction = tempo;
            }
        }

        if(Input.GetKey(KeyCode.S)){
            //Enable reverse lights when car is reversing
            if(Enable_Reverselights_Lights){
                foreach(Light RL in ReverseLights){
                    RL.enabled = true;
                }
            }

            if(Enable_Reverselights_MeshRenderers){
                foreach(MeshRenderer RM in ReverseLights_MeshRenderers){
                    RM.enabled = true;
                }
            }
        }

        if(!Input.GetKey(KeyCode.S)){
            if(Enable_Reverselights_Lights){
                foreach(Light Rl in ReverseLights){
                    Rl.enabled = false;
                }
            }

            if(Enable_Reverselights_MeshRenderers){
                foreach(MeshRenderer RM in ReverseLights_MeshRenderers){
                    RM.enabled = false;
                }
            }
        }
    }

    public void Update(){

        //Rotating The Wheels Meshes so they have the same position and rotation as the wheel colliders
        var pos = Vector3.zero;
        var rot = Quaternion.identity;
        
        foreach(WheelCollider Wheel_Col in Back_Wheels){
            foreach(Transform Wheel in Wheel_Transforms){
                foreach(Vector3 Wheel_Rotation in Wheels_Rotation){
                    Wheel_Col.GetWorldPose(out pos, out rot);
                    Wheel.position = pos;
                    Wheel.rotation = rot * Quaternion.Euler(Wheel_Rotation);
                }
            }
        }

        foreach(WheelCollider Wheel_Col in Front_Wheels){
            foreach(Transform Wheel in Wheel_Transforms){
                foreach(Vector3 Wheel_Rotation in Wheels_Rotation){
                    Wheel_Col.GetWorldPose(out pos, out rot);
                    Wheel.position = pos;
                    Wheel.rotation = rot * Quaternion.Euler(Wheel_Rotation);
                }
            }
        }

        //Make Car Brake
        if(Input.GetKey(KeyCode.Space) == true){
            Brakes = BrakeForce;

            //Turn on brake lights
            if(Enable_Brakelights_Lights){
                foreach(Light L in BrakeLights){
                    L.enabled = true;
                }
            }

            if(Enable_Brakelights_MeshRenderers){
                foreach(MeshRenderer BM in BrakeLights_MeshRenderers){
                    BM.enabled = true;
                }
            }

            //Drifting and changing wheel collider values
            if(Set_Drift_Settings_Automatically){
                foreach(WheelCollider Wheel in Back_Wheels){
                    Wheel_forwardFriction = Wheel.forwardFriction;
                    Wheel_sidewaysFriction = Wheel.sidewaysFriction;

                    Wheel_forwardFriction.extremumValue = Wheel_forwardFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
                    Wheel_sidewaysFriction.extremumValue = Wheel_sidewaysFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
                }

                foreach(WheelCollider Wheel in Front_Wheels){
                    Wheel_forwardFriction = Wheel.forwardFriction;
                    Wheel_sidewaysFriction = Wheel.sidewaysFriction;

                    Wheel_forwardFriction.extremumValue = Wheel_forwardFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
                    Wheel_sidewaysFriction.extremumValue = Wheel_sidewaysFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
                }
            }

            if(!Set_Drift_Settings_Automatically){
                foreach(WheelCollider Wheel in Back_Wheels){
                    //Variables getting assigned
                    Wheel_forwardFriction = Wheel.forwardFriction;
                    Wheel_sidewaysFriction = Wheel.sidewaysFriction;

                    //Setting The Extremium values to the ones that the user defined
                    Wheel_forwardFriction.extremumValue = Forward_Extremium_Value_When_Drifting;
                    Wheel_sidewaysFriction.extremumValue = Sideways_Extremium_Value_When_Drifting;
                }

                foreach(WheelCollider Wheel in Front_Wheels){
                    //Variables getting assigned
                    Wheel_forwardFriction = Wheel.forwardFriction;
                    Wheel_sidewaysFriction = Wheel.sidewaysFriction;

                    //Setting The Extremium values to the ones that the user defined
                    Wheel_forwardFriction.extremumValue = Forward_Extremium_Value_When_Drifting;
                    Wheel_sidewaysFriction.extremumValue = Sideways_Extremium_Value_When_Drifting;
                }
            }
        }

        else{
            Brakes = 0f;
        }

        //Apply brake force
        foreach(WheelCollider Wheel in Front_Wheels){
            Wheel.brakeTorque = Brakes;
        }

        foreach(WheelCollider Wheel in Back_Wheels){
            Wheel.brakeTorque = Brakes;
        }

        if(!Input.GetKey(KeyCode.Space)){
            //Turn off brake lights
            if(Enable_Brakelights_Lights){
                foreach(Light L in BrakeLights){
                    L.enabled = false;
                }
            }

            if(Enable_Brakelights_MeshRenderers){
                foreach(MeshRenderer BM in BrakeLights_MeshRenderers){
                    BM.enabled = false;
                }
            }
        }

        //Audio System
        if(Enable_Audio){
            if(Enable_Engine_Audio){
                //Setting the pitch according to the speed of the car.
                pitch = Car_Speed_In_KPH/Maximum_Speed + 1f;
                
                //Do this if the pitch variable exceeds the maximum pitch value
                if(pitch > Maximum_Pitch_Value){
                    pitch = Maximum_Pitch_Value;
                }

                //Do this if the pitch variable is lower than the minimum pitch value
                else if(pitch < Minimum_Pitch_Value){
                    pitch = Minimum_Pitch_Value;
                }

                //This actually sets the audio source pitch
                Engine_Sound.pitch = pitch;
            }

            //Car Horn
            if(Enable_Horn){
                if(Input.GetKey(Car_Horn_Key) && !Horn_Source.isPlaying){
                    //Play the sound
                    Horn_Source.Play();
                }

                if(!Input.GetKey(Car_Horn_Key)){
                    //Stop playing the sound
                    Horn_Source.Stop();
                }
            }
        }
    }

    void OnCollisionEnter(Collision col){
        //Play the crash sound when car crashes into an object with the tag in the "Crash_Object_Tags" list
        if(Enable_Crash_Noise && Enable_Audio){
            foreach (string tag in Crash_Object_Tags){
                if(col.gameObject.tag == tag){
                    //Play the crash sound:
                    Crash_Sound.Play();
                }

                else{
                    //Stop playing the crash sound
                    Crash_Sound.Stop();
                }
            }
        }
    }
}
