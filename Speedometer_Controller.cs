using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Speedometer_Controller : MonoBehaviour
{
    //Public Vars
    [Header("General Settings")]
    public Rigidbody Car_Rb;

    public float Maximum_Speed = 0.0f;

    public float Minimum_Speed_Arrow_Angle;
    public float Maximum_Speed_Arrow_Angle;

    public bool Speed_In_KPH;

    [Header("Gears Setting")]
    public List<int> Gear_Speeds;

    [Header("UI Settings")]
    public TextMeshProUGUI Speed_Text;
    public RectTransform Arrow;
    public TextMeshProUGUI Gear_Text;

    [Header("Audio Settings")]

    public bool Use_Audio_Settings;
    public AudioSource Gear_Shift_Up;
    public AudioSource Gear_Shift_Down;

    [Header("Script Reference(s)")]
    public Car_Controller car_controller_script;

    [Header("Debug")]
    public string Current_Gear;
    public int Current_Gear_num;
    public bool is_reversing;
    public bool is_braking;
    public bool speed_is_zero;

    //Private Vars
    private float speed = 0.0f;
    private int Speed_To_Display;

    private void Start(){
        //Set Gears to 0:
        Current_Gear = "0";
        Current_Gear_num = 0;
    }

    private void Update()
    {
        //Show speed in KPH
        if(Speed_In_KPH){
            speed = Car_Rb.velocity.magnitude * 3.6f;
        }

        //Show speed in MPH
        if(!Speed_In_KPH){
            speed = Car_Rb.velocity.magnitude * 2.237f;
        }

        //If speed_text var is assigned
        if (Speed_Text != null){
            //Change text of the Text var to the speed
            Speed_To_Display = (int)speed;
            Speed_Text.SetText(Speed_To_Display.ToString());
        }

        //if arrow rect var is assigned
        if (Arrow != null){
            //Rotate the arrow
            Arrow.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(Minimum_Speed_Arrow_Angle, Maximum_Speed_Arrow_Angle, speed / Maximum_Speed));
        }

        //do this if the speed is NOT equal to 0
        if(!speed_is_zero){
            //Setting the gear text to the current gear
            Gear_Text.SetText(Current_Gear);
        }
    }

    private void FixedUpdate(){
        //Changing Gears

        //Check is car isn't braking or reversing
        if(!is_braking && !is_reversing){   
            //When the current speed is more than the speed of the current gear
            if(Gear_Speeds[Current_Gear_num] < speed && Current_Gear_num != Gear_Speeds.Count){
                Current_Gear_num++;
                Current_Gear = (Current_Gear_num + 1).ToString();
                
                if(Use_Audio_Settings){
                    //Play gear shift up effect
                    Gear_Shift_Up.Play();
                }
            }

            //When current speed is less than the speed of the current gear
            if(Gear_Speeds[Current_Gear_num] > speed && Current_Gear_num != 0){
                Current_Gear_num--;
                Current_Gear = Current_Gear_num.ToString();
                
                if(Use_Audio_Settings){
                    //Play gear shift down audio
                    Gear_Shift_Down.Play();
                }
            }

            //When the last gear speed is the same as the current speed
            //This basically limits the speed of the car, so the car has a max speed and does not exceed it
            if(Gear_Speeds.Last() <= speed){
                Car_Rb.velocity = Vector3.ClampMagnitude(Car_Rb.velocity, Maximum_Speed);
            }

            //When speed is equal to 0
            if(speed == 0){
                speed_is_zero = true;
            }

            //When speed is not equal to 0
            if(speed != 0){
                speed_is_zero = false;
            }     
        }

        //When car is reversing
        if(Input.GetKey(KeyCode.S)){
            is_reversing = true;
        }

        //When Car is Braking
        if(Input.GetKey(KeyCode.Space)){
            is_braking = true;
        }

        //When Car is not braking
        if(!Input.GetKey(KeyCode.Space)){
            is_braking = false;
        }

        //When it is forward movement
        if(Input.GetKey(KeyCode.W)){
            is_reversing = false;
        }

        if(Input.GetKey(KeyCode.UpArrow)){
            is_reversing = false;
        }

        //Check if car is braking or reversing and set the gear accordingly
        if(is_reversing){
            //Change gear to "R"
            Current_Gear = "R";
        }

        if(is_braking){
            //Set the Current Gear to B
            Current_Gear = "B";
        }

        //Change gear to zero if the bool is true
        if(speed_is_zero){
            Current_Gear = "0";
        }
    }
}
