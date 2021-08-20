using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel_Effects : MonoBehaviour
{
    //Variables
    public TrailRenderer[] Tire_Marks;
    public bool Enable_Particle_System;
    public ParticleSystem[] Skid_Particles;

    private bool Tire_Marks_Flag;

    //Update function to check the drifting every frame
    void FixedUpdate(){
        Check_Drift();
    }

    //Check if drifting or braking
    public void Check_Drift(){
        if(Input.GetKey(KeyCode.Space)){
            StartEmitter();
        }

        else{
            StopEmitter();
        }
    }

    //Start Renderring Trail
    public void StartEmitter(){
        if(Tire_Marks_Flag) return;

        foreach (TrailRenderer T in Tire_Marks)
        {
            T.emitting = true;
        }

        if(Enable_Particle_System){
            foreach (ParticleSystem P in Skid_Particles)
            {
                P.Play();
            }
        }

        Tire_Marks_Flag = true;
    }

    //Stop Renderring Trail
    public void StopEmitter(){
        if(!Tire_Marks_Flag) return;

        foreach (TrailRenderer T in Tire_Marks)
        {
            T.emitting = false;
        }

        if(Enable_Particle_System){
            foreach (ParticleSystem P in Skid_Particles)
            {
                P.Stop();
            }
        }

        Tire_Marks_Flag = false;
    }
}
