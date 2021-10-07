using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel_Effects : MonoBehaviour
{
    //Variables
    [Header("Tire Mark Trail Renderers")]
    public TrailRenderer[] Tire_Marks;

    [Header("Tire Mark Particle Systems")]
    public bool Enable_Particle_System;
    public ParticleSystem[] Skid_Particles;

    [Header("Script References")]
    public Car_Controller car_Controller;

    private bool Tire_Marks_Flag;
    private bool is_drifting;

    public void Start(){
        foreach(TrailRenderer T in Tire_Marks){
            T.emitting = false;
        }

        if(Enable_Particle_System){
            foreach(ParticleSystem P in Skid_Particles){
                P.Stop();
            }
        }
    }

    //Update function to check the drifting every frame
    void FixedUpdate(){
        Check_Drift();
    }

    //Check if drifting or braking
    public void Check_Drift(){
        if(Input.GetKey(KeyCode.Space)){
            StartEmitter();
        }

        else if (car_Controller.tempo != 0.5){
            StartEmitter();
            is_drifting = true;
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

        if(Enable_Particle_System && is_drifting){
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
