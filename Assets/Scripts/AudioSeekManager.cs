using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AudioSeekManager : MonoBehaviour
{

    public bool currentlyPlaying = false;
    public float currentTime = 0;
    public string [] instrument_names = { "Instrument1", "Instrument2", "Instrument3", "Instrument4", "Instrument5"};

    // Static singleton property
    public static AudioSeekManager Instance { get; private set; }
     
    void Awake()
    {
        // First we check if there are any other instances conflicting
        if(Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }
 
        // Here we save our singleton instance
        Instance = this;
 
        // Furthermore we make sure that we don't destroy between scenes (this is optional)
        DontDestroyOnLoad(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log ("Updated is called in AudioSeekManager.");
        //Debug.Log (currentlyPlaying);
        //Debug.Log (placedInstrument());
        if (currentlyPlaying && placedInstrument()) { // currently playing audio, update currentTime
            currentTime += Time.deltaTime;
            Debug.Log("The current time is updated");
            //playSong();
        }
    }

    public void setPlay (bool play) {
        currentlyPlaying = play;
    }

    public void setSongPosition (float newTime) {
        currentTime = newTime;
    }


    public bool placedInstrument() {
        foreach (string instr in instrument_names) {
            if (GameObject.FindGameObjectsWithTag(instr).Length != 0) {
                //Debug.Log ("A instrument is placed.");
                return true;
            }
        }
        return false;
    }

    public void playSong() {
        if (currentlyPlaying) {
            foreach (string instr in instrument_names) {
                if (GameObject.FindGameObjectsWithTag(instr).Length != 0) {
                    AudioSource audioSource = GameObject.FindGameObjectWithTag(instr).GetComponent<AudioSource>();
                    //Debug.Log("Play Song");
                    audioSource.Play();
                    audioSource.time = currentTime;
                }
            }
        }
    }

    public void pauseSong() {
        foreach (string instr in instrument_names) {
            if (GameObject.FindGameObjectsWithTag(instr).Length != 0) {
                AudioSource audioSource = GameObject.FindGameObjectWithTag(instr).GetComponent<AudioSource>();
                //Debug.Log("Pause Song");
                Debug.Log(audioSource.ToString());
                audioSource.Stop();
                audioSource.time = currentTime;
            }
        }
    }


}
