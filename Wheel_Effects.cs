using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel_Effects : MonoBehaviour
{
    //Variables
    [Header("Tire Mark Trail Renderers")]
    public TrailRenderer[] Tire_Marks; //The Trail renderer(s) for the tracks

    [Header("Tire Mark Particle Systems")]
    public bool Enable_Particle_System;
    public ParticleSystem[] Skid_Particles; //The particle system(s) for the particles

    [Header("Tire Audio (Basically The Sound When Drifting)")]
    public bool Enable_Audio;
    public AudioSource Drift_Audio; //This would be like a tire screech sound

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
            is_drifting = false;
        }
    }

    //Start Renderring Trail
    public void StartEmitter(){
        if(Tire_Marks_Flag) return;

        //Drift Trail
        foreach (TrailRenderer T in Tire_Marks)
        {
            T.emitting = true;
        }

        //Drift Particles
        if(Enable_Particle_System && is_drifting){
            foreach (ParticleSystem P in Skid_Particles)
            {
                P.Play();
            }
        }

        //Drift Sound
        if(Enable_Audio && is_drifting){
            if(!Drift_Audio.isPlaying){
                //Play the sound
                Drift_Audio.Play();
            }
        }

        Tire_Marks_Flag = true;
    }

    //Stop Renderring Trail
    public void StopEmitter(){
        if(!Tire_Marks_Flag) return;

        //Drift Trai
        foreach (TrailRenderer T in Tire_Marks)
        {
            T.emitting = false;
        }

        //Drift Particles
        if(Enable_Particle_System){
            foreach (ParticleSystem P in Skid_Particles)
            {
                P.Stop();
            }
        }

        //Drift Sound
        if(Enable_Audio){
            //Stop playing the sound
            Drift_Audio.Stop();
        }

        Tire_Marks_Flag = false;
    }
}
