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
    public string [] instrument_names = { "Instrument1", "Instrument2", "Instrument3", "Instrument4", "Instrument5" };

    private float interval_tracker = 0.0f;       // tracks how long current interval has been
    private float replay_interval = 1.0f;        // how long before we replay song


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
            //Debug.Log("The current time is updated");
            interval_tracker += Time.deltaTime;
            if (interval_tracker > replay_interval)
            {
                playSong();
                interval_tracker = 0.0f;
            }
            
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

    private float calculate_delay(Vector3 camera_pos, Vector3 instrument_pos, Vector3 camera_dir)
    {
        
        Vector3 position_dir = instrument_pos - camera_pos;

        Vector2 position_vec2 = new Vector2(position_dir.x, position_dir.z);
        if (position_vec2.magnitude == 0) {
            return 0.0f;            // if position vector is 0, then they are on same location so no delay
        }
        position_vec2 = position_vec2.normalized;
        Vector2 camera_vec2 = new Vector2(camera_dir.x, camera_dir.z).normalized;
       
        // Calculate delay from that online equation
        float degree = Mathf.Acos(Vector2.Dot(position_vec2, camera_vec2));
        float radius = 0.0875f;     // 8.75cm
        float C = 343;              // speed of sound, 343 m/s
        float delay = radius * ((degree % (3.141593f / 2)) + Mathf.Sin(degree)) / C;

        //  Calculate which ear is delayed (negative is left, positive is right)
        float sub_dot = (position_vec2.x * camera_vec2.y) - (position_vec2.y * camera_vec2.x);
        if (sub_dot == 0.0f)
        {
            sub_dot = 1.0f;
        }
        float direction = sub_dot / Mathf.Abs(sub_dot); 

        return delay * direction;
    }

    public void playSong() {
        if (currentlyPlaying) {

            Vector3 camera_direction = Camera.main.transform.forward;
            Vector3 camera_position = Camera.main.transform.position;

            foreach (string instr in instrument_names) {
                if (GameObject.FindGameObjectsWithTag(instr).Length != 0) {
                    GameObject instrumentObject = GameObject.FindGameObjectWithTag(instr);
                    AudioSource audioSource = instrumentObject.GetComponent<AudioSource>();
                    Vector3 instrument_position = instrumentObject.transform.position;
                    Debug.Log(instr);
                    float track_delay = calculate_delay(camera_position, instrument_position, camera_direction);
                    // TODO: if track_delay < 0, delay left ear. if track_delay > 0, delay right ear. 


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
