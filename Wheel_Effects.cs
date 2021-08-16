using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel_Effects : MonoBehaviour
{
    //Variables
    public TrailRenderer[] Tire_Marks;
    private bool Tire_Marks_Flag;

    //Update function to check the drifting every frame
    void FixedUpdate(){
        Check_Drift();
    }

    //Check if drifting or braking
    private void Check_Drift(){
        if(Input.GetKey(KeyCode.Space)){
            StartEmitter();
        }

        else{
            StopEmitter();
        }
    }

    //Start Renderring Trail
    private void StartEmitter(){
        if(Tire_Marks_Flag) return;

        foreach (TrailRenderer T in Tire_Marks)
        {
            T.emitting = true;
        }

        Tire_Marks_Flag = true;
    }

    //Stop Renderring Trail
    private void StopEmitter(){
        if(!Tire_Marks_Flag) return;

        foreach (TrailRenderer T in Tire_Marks)
        {
            T.emitting = false;
        }

        Tire_Marks_Flag = false;
    }
}
