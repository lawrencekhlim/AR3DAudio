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
    private float replay_interval = 0.0f;        // how long before we replay song
    private Dictionary <string, float> prev_itd;
    private float min_dif_delay = 0.0001f;


    // Static singleton property
    public static AudioSeekManager Instance { get; private set; }
     
    void Awake()
    {
        prev_itd = new Dictionary <string, float> ();
        foreach(string instr in instrument_names)  
        {
            prev_itd[instr] = 0.0f;
        }

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
            currentTime += Time.deltaTime; // Better way is to set currentTime to one of the instrumenttracks.time.
            //Debug.Log("The current time is updated");
            interval_tracker += Time.deltaTime;
            if (interval_tracker > replay_interval)
            {
                updateSongDelay();
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

    private float[] calculate_level_difference (Vector3 camera_pos, Vector3 instrument_pos, Vector3 camera_dir) {
        Vector3 position_vec1 = instrument_pos - camera_pos;
        Vector2 position_vec = new Vector2(position_vec1.x, position_vec1.z);
        
        float min_distance = 1;
        float max_distance = 500;
        //Vector2 position_vec2 = new Vector2 
        if (position_vec.magnitude == 0) {
            return new float[2] {1.0f,1.0f};            // if position vector is 0, then they are on same location so no delay
        }
        if (position_vec.magnitude < 1) {
            position_vec = position_vec.normalized; // Gives us a unit vector in that direction
        }
        camera_dir = camera_dir.normalized;

        float angle = Mathf.Acos (Vector3.Dot(position_vec, camera_dir) / (position_vec.magnitude * camera_dir.magnitude));

        /*
        float sub_dot = (position_vec2.x * camera_vec2.y) - (position_vec2.y * camera_vec2.x);
        if (sub_dot == 0.0f)
        {
            sub_dot = 1.0f;
        } */
        //float direction = sub_dot / Mathf.Abs(sub_dot); 
        
        float radius = 0.0875f; 
        float pythagorean = Mathf.Sqrt(position_vec.magnitude * position_vec.magnitude+ radius * radius);
        float temp1 = position_vec.magnitude + radius - pythagorean;
        float left = 1.5f* Mathf.Sin(angle) * temp1 + pythagorean;
        float right = 1.5f * Mathf.Sin(-1 * angle) * temp1 + pythagorean;
        Debug.Log ("Angle: " + angle.ToString());
        Debug.Log ("Left: " + left.ToString());
        Debug.Log ("Right: " + right.ToString());
        return new float [2] { Mathf.Clamp ((1 / left) / left, 0, 1), Mathf.Clamp ((1 / right)/right, 0, 1) };
    }

    public void playSong()
    {
        foreach (string instr in instrument_names)
        {
            if (GameObject.FindGameObjectsWithTag(instr).Length != 0)
            {
                GameObject instrumentObject = GameObject.FindGameObjectWithTag(instr);
                AudioSource[] audioSources = instrumentObject.GetComponents<AudioSource>();

                audioSources[0].Play();
                audioSources[1].Play();

                audioSources[0].time = currentTime;
                audioSources[1].time = currentTime;
            }
        }
    }

    public void setTracks (string song) {
        var clip = Resources.Load(song) as AudioClip;

        string[] instrument_names = new string[] { "bass", "piano", "drums", "vocals", "other" };
        for(int i=0; i < 5; i++)
        {
            string instrument_tag = "Instrument" + (i+1).ToString();
            string instrument_name = instrument_names[i];

            Debug.Log ("Before FindGameObjectsWithTag");

            if (GameObject.FindGameObjectsWithTag(instrument_tag).Length != 0)
            {
                AudioSource audio_left = GameObject.FindGameObjectWithTag(instrument_tag).GetComponents<AudioSource>()[0];
                AudioSource audio_right = GameObject.FindGameObjectWithTag(instrument_tag).GetComponents<AudioSource>()[1];
                audio_left.Pause();
                audio_right.Pause();

                audio_left.clip = Resources.Load("allOutput/" + song + "/" + instrument_name + "_left") as AudioClip;
                audio_right.clip = Resources.Load("allOutput/" + song + "/" + instrument_name + "_right") as AudioClip;

                Debug.Log(audio_left.ToString());
                Debug.Log(audio_right.ToString());

                if (audio_left.clip == null)
                {
                    Debug.Log("Audio" + (i+1).ToString() + "_left was null");
                    audio_left.clip = clip;
                }
                if (audio_right.clip == null)
                {
                    Debug.Log("Audio" + (i + 1).ToString() + "_right was null");
                    audio_right.clip = clip;
                }

                audio_left.Play();
                audio_right.Play();
            }
        }
    }

    public void updateSongDelay() {
        if (currentlyPlaying) {

            Vector3 camera_direction = Camera.main.transform.forward;
            Vector3 camera_position = Camera.main.transform.position;

            foreach (string instr in instrument_names) {
                if (GameObject.FindGameObjectsWithTag(instr).Length != 0) {
                    GameObject instrumentObject = GameObject.FindGameObjectWithTag(instr);
                    AudioSource[] audioSources = instrumentObject.GetComponents<AudioSource>();
                    Vector3 instrument_position = instrumentObject.transform.position;
                    //Debug.Log(instr);

                    // Check instrument has 2 audio sources
                    if (audioSources.Length < 2)
                    {
                        Debug.Log("Instrument Object has less than 2 audioSource components. (missing left and right ear)");
                        continue;
                    }


                    // Get Delay. if track_delay < 0, delay left ear. if track_delay > 0, delay right ear. 
                    // index 0 is left ear. index 1 is right ear. 
                    float track_delay = calculate_delay(camera_position, instrument_position, camera_direction);
                    //Debug.Log("Before If statement");
                    //Debug.Log (track_delay);
                    //Debug.Log (prev_itd[instr]);

                    float [] volume = calculate_level_difference(camera_position, instrument_position, camera_direction);
                    audioSources[0].volume = volume[0]; // left
                    audioSources[1].volume = volume[1]; // right
                    //Debug.Log (audioSources[0].volume);
                    //Debug.Log (audioSources[1].volume);

                    if (Mathf.Abs(track_delay - prev_itd[instr]) > min_dif_delay) {
                        //Debug.Log ("Updated delay");
                        //Debug.Log (track_delay);
                        audioSources[0].time = audioSources[0].time;
                        audioSources[1].time = audioSources[0].time;
                        if (track_delay < 0)
                        {
                            audioSources[0].time += Mathf.Abs(track_delay);
                        }
                        else
                        {
                            audioSources[1].time += Mathf.Abs(track_delay);
                        }
                        prev_itd[instr] = track_delay; 
                    }

                }
            }
        }
    }

    public void pauseSong() {
        foreach (string instr in instrument_names) {
            if (GameObject.FindGameObjectsWithTag(instr).Length != 0) {
                AudioSource audioSource_left = GameObject.FindGameObjectWithTag(instr).GetComponents<AudioSource>()[0];
                AudioSource audioSource_right = GameObject.FindGameObjectWithTag(instr).GetComponents<AudioSource>()[1];
                //Debug.Log("Pause Song");
                audioSource_left.Stop();
                audioSource_right.Stop();
                currentTime = audioSource_left.time;
            }
        }
    }


}
