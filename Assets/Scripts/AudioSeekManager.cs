using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class AudioSeekManager : MonoBehaviour
{

    public bool currentlyPlaying = false;
    public float currentTime = 0;
    public string[] instrument_names2 = { "Bass", "Piano", "Drum", "Vocal", "Misc" };


    private float interval_tracker = 0.0f;       // tracks how long current interval has been
    private float replay_interval = 0.1f;        // how long before we replay song
    private float min_dif_delay = 0.0001f;

    private Slider level_difference_slider;
    private Slider time_difference_slider;
    private Slider distance_difference_slider;
    private Slider echo_dampening_slider;
    private Slider between_instrument_time_slider;

    private int frame_counter = 0;


    // Static singleton property
    public static AudioSeekManager Instance { get; private set; }

    void Awake()
    {

        level_difference_slider = GameObject.FindGameObjectsWithTag("Slider_Level_Difference")[0].GetComponent<Slider>();
        time_difference_slider = GameObject.FindGameObjectsWithTag("Slider_Time_Difference")[0].GetComponent<Slider>();
        distance_difference_slider = GameObject.FindGameObjectsWithTag("Slider_Distance_Difference")[0].GetComponent<Slider>();
        echo_dampening_slider = GameObject.FindGameObjectsWithTag("Slider_Echo_Dampening")[0].GetComponent<Slider>();
        between_instrument_time_slider = GameObject.FindGameObjectsWithTag("Slider_Between_Instrument_Time")[0].GetComponent<Slider>();

        // First we check if there are any other instances conflicting
        if (Instance != null && Instance != this)
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

        if (currentlyPlaying && placedInstrument())
        { // currently playing audio, update currentTime
            currentTime += Time.deltaTime; // Better way is to set currentTime to one of the instrumenttracks.time.
            //Debug.Log("The current time is updated");
            interval_tracker += Time.deltaTime;
            if (interval_tracker > replay_interval)
            {
                updateSongDelay();
                interval_tracker = 0.0f;
            }
            //updateSongDelay();

        }
    }

    public void toggleGreenNote(bool isVisible)
    {
        GameObject[] NoteObjects = GameObject.FindGameObjectsWithTag("Playing_Note");
        foreach (GameObject NoteObject in NoteObjects)
        {
            NoteObject.GetComponent<MeshRenderer>().enabled = isVisible;
        }
    }

    public void setPlay(bool play)
    {
        currentlyPlaying = play;
    }

    public void setSongPosition(float newTime)
    {
        currentTime = newTime;
    }


    public bool placedInstrument()
    {
        foreach (string instr in instrument_names2)
        {
            Debug.Log(instr);
            if (GameObject.FindGameObjectsWithTag(instr).Length != 0)
            {
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
        if (position_vec2.magnitude == 0)
        {
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

    private float[] calculate_level_difference(Vector3 camera_pos, Vector3 instrument_pos, Vector3 camera_dir)
    {
        Vector3 position_vec1 = instrument_pos - camera_pos;
        Vector2 position_vec = new Vector2(position_vec1.x, position_vec1.z);
        Vector2 camera_vec = new Vector2(camera_dir.x, camera_dir.z);

        float min_distance = 1;
        float max_distance = 500;
        //Vector2 position_vec2 = new Vector2 
        Debug.Log("Position Difference Magnitude: " + position_vec.ToString());

        if (position_vec.magnitude == 0)
        {
            return new float[2] { 1.0f, 1.0f };            // if position vector is 0, then they are on same location so no delay
        }
        if (position_vec.magnitude < 1)
        {
            position_vec = position_vec.normalized; // Gives us a unit vector in that direction
        }
        camera_vec = camera_vec.normalized;

        float angle = Mathf.Acos(Vector3.Dot(position_vec, camera_vec) / (position_vec.magnitude * camera_vec.magnitude));


        float distance = position_vec.magnitude;
        float volume = 1 / Mathf.Pow(distance, distance_difference_slider.value);
        float PI = (float)Math.PI;
        float theta2 = Mathf.Abs(PI / 2.0f - angle);
        float closer_ear = (PI - theta2) / PI;
        float further_ear = theta2 / PI;


        //  Calculate which ear is delayed (negative is left, positive is right)
        float sub_dot = (position_vec.x * camera_vec.y) - (position_vec.y * camera_vec.x);
        if (sub_dot == 0.0f)
        {
            sub_dot = 1.0f;
        }
        float direction = sub_dot / Mathf.Abs(sub_dot);

        // Dampen if source is behind camera normal
        if (angle > PI / 2.0f)
        {
            volume *= 0.9f;
        }

        // Get result
        float interaural_level_difference_amount = level_difference_slider.value;
        float further_ear_offset = (further_ear / closer_ear) * interaural_level_difference_amount + 1.0f - interaural_level_difference_amount;
        float[] result = new float[2] { volume, volume };
        if (direction < 0)
        {
            result[1] = volume * further_ear_offset;
        }
        else
        {
            result[0] = volume * further_ear_offset;
        }

        return result;
    }

    public void playSong()
    {
        if (currentlyPlaying)
        {
            foreach (string instr in instrument_names2)
            {
                GameObject[] instrumentObjects = GameObject.FindGameObjectsWithTag(instr);
                GameObject[] echoObjects = GameObject.FindGameObjectsWithTag(instr + "_echo");
                GameObject[] soundObjects = instrumentObjects.Concat(echoObjects).ToArray();

                foreach (GameObject instrumentObject in soundObjects)
                {
                    AudioSource[] audioSources = instrumentObject.GetComponents<AudioSource>();

                    audioSources[0].Play();
                    audioSources[1].Play();

                    audioSources[0].time = currentTime;
                    audioSources[1].time = currentTime;
                }
            }


            toggleGreenNote(true);
        }
    }

    public void setTracks(string song)
    {
        var clip = Resources.Load(song) as AudioClip;

        string[] instrument_types = new string[] { "bass", "piano", "drums", "vocals", "other" }; // For the filenames of the left/right audio sources
        for (int i = 0; i < 5; i++)
        {
            string instr = instrument_names2[i];
            string instrument_type = instrument_types[i];

            //Debug.Log ("Before FindGameObjectsWithTag");
            GameObject[] instrumentObjects = GameObject.FindGameObjectsWithTag(instr);
            GameObject[] echoObjects = GameObject.FindGameObjectsWithTag(instr + "_echo");
            GameObject[] soundObjects = instrumentObjects.Concat(echoObjects).ToArray();

            foreach (GameObject instrumentObject in soundObjects)
            {
                AudioSource audio_left = instrumentObject.GetComponents<AudioSource>()[0];
                AudioSource audio_right = instrumentObject.GetComponents<AudioSource>()[1];
                audio_left.Pause();
                audio_right.Pause();

                audio_left.clip = Resources.Load("allOutput/" + song + "/" + instrument_type + "_left") as AudioClip;
                audio_right.clip = Resources.Load("allOutput/" + song + "/" + instrument_type + "_right") as AudioClip;

                Debug.Log(audio_left.ToString());
                Debug.Log(audio_right.ToString());

                if (audio_left.clip == null)
                {
                    Debug.Log("Audio" + (i + 1).ToString() + "_left was null");
                    audio_left.clip = clip;
                }
                if (audio_right.clip == null)
                {
                    Debug.Log("Audio" + (i + 1).ToString() + "_right was null");
                    audio_right.clip = clip;
                }
            }
        }
    }

    public void updateSongDelay()
    {
        if (currentlyPlaying)
        {

            frame_counter += 1;

            Vector3 camera_direction = Camera.main.transform.forward;
            Vector3 camera_position = Camera.main.transform.position;

            GameObject closest_instrument = null;
            float closest_distance = -1.0f;
            foreach (string instr in instrument_names2)
            {
                GameObject[] instrumentObjects = GameObject.FindGameObjectsWithTag(instr);
                GameObject[] echoObjects = GameObject.FindGameObjectsWithTag(instr + "_echo");
                GameObject[] soundObjects = instrumentObjects.Concat(echoObjects).ToArray();

                foreach (GameObject instrumentObject in soundObjects)
                {
                    float instrument_distance = Vector3.Distance(instrumentObject.transform.position, camera_position);
                    if (closest_distance == -1.0f || instrument_distance < closest_distance)
                    {
                        closest_instrument = instrumentObject;
                        closest_distance = instrument_distance;
                    }

                }
            }

            foreach (string instr in instrument_names2)
            {
                GameObject[] instrumentObjects = GameObject.FindGameObjectsWithTag(instr);
                GameObject[] echoObjects = GameObject.FindGameObjectsWithTag(instr + "_echo");
                GameObject[] soundObjects = instrumentObjects.Concat(echoObjects).ToArray();

                foreach (GameObject instrumentObject in soundObjects)
                {
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
                    Debug.Log("Before If statement");
                    //Debug.Log (track_delay);

                    float echoDampen = echo_dampening_slider.value;
                    float[] volume = calculate_level_difference(camera_position, instrument_position, camera_direction);

                    if (instrumentObject.tag == instr)
                    {
                        echoDampen = 1.0f;
                    }

                    audioSources[0].volume = volume[0] * echoDampen; // left
                    audioSources[1].volume = volume[1] * echoDampen; // right

                    track_delay *= time_difference_slider.value;

                    // Closest's instruments time
                    AudioSource[] closest_audiosource = closest_instrument.GetComponents<AudioSource>();
                    float closest_instrument_time = (closest_audiosource[0].time + closest_audiosource[1].time) / 2.0f;


                    // Distance between current instrument and closest instrument.
                    float distance_difference = Mathf.Abs(closest_distance - Vector3.Distance(instrumentObject.transform.position, camera_position));
                    float delay_between_instruments = distance_difference / 343 * between_instrument_time_slider.value;

                    // Get prev_itd and prev_instrument_delay
                    float current_instrument_time = (audioSources[0].time + audioSources[1].time) / 2.0f;
                    float prev_instrument_delay = current_instrument_time - closest_instrument_time;

                    float prev_itd = audioSources[1].time - audioSources[0].time;

                    if (Mathf.Abs(track_delay - prev_itd) > min_dif_delay || Mathf.Abs(delay_between_instruments - prev_instrument_delay) > min_dif_delay)
                    {
                        //Debug.Log ("Updated delay");

                        float left_ear = closest_instrument_time + delay_between_instruments;
                        float right_ear = closest_instrument_time + delay_between_instruments;
                        if (track_delay < 0)
                        {
                            left_ear += Mathf.Abs(track_delay) / 2.0f;
                            right_ear -= Mathf.Abs(track_delay) / 2.0f;
                        }
                        else
                        {
                            left_ear -= Mathf.Abs(track_delay) / 2.0f;
                            right_ear += Mathf.Abs(track_delay) / 2.0f;
                        }

                        audioSources[0].time = left_ear;
                        audioSources[1].time = right_ear;
                    }
                    Debug.Log("DELAY NUMBERS: " + frame_counter.ToString());
                    Debug.Log("track delay: " + track_delay.ToString());
                    Debug.Log("prev_itd: " + prev_itd.ToString());
                    Debug.Log("delay_between_instruments: " + delay_between_instruments.ToString());
                    Debug.Log("prev_instrument_delay: " + prev_instrument_delay.ToString());


                    /*track_delay *= time_difference_slider.value;
                    float ear_diff = audioSources[1].time - audioSources[0].time;
                    if (Mathf.Abs(track_delay - ear_diff) < (1.0 / 32.0f))
                    {
                        //Debug.Log ("Updated delay");
                        //Debug.Log (track_delay);

                        float delay_diff = track_delay - ear_diff;
                        float pitch_multiplier = Mathf.Abs(delay_diff) / 2.0f / Time.deltaTime;
                        if (delay_diff < 0)
                        {
                            audioSources[0].pitch = 1 + pitch_multiplier;
                            audioSources[1].pitch = 1 - pitch_multiplier;
                        }
                        else
                        {
                            audioSources[0].pitch = 1 - pitch_multiplier;
                            audioSources[1].pitch = 1 + pitch_multiplier;
                        }
                        prev_itd[instr] = track_delay;
                    }
                    else
                    {
                        audioSources[0].pitch = 1.0f;
                        audioSources[1].pitch = 1.0f;
                    }*/
                    //Debug.Log(ear_diff);

                }
            }
        }
    }

    public void pauseSong()
    {
        foreach (string instr in instrument_names2)
        {
            GameObject[] instrumentObjects = GameObject.FindGameObjectsWithTag(instr);
            GameObject[] echoObjects = GameObject.FindGameObjectsWithTag(instr + "_echo");
            GameObject[] soundObjects = instrumentObjects.Concat(echoObjects).ToArray();

            foreach (GameObject instrumentObject in soundObjects)
            {
                AudioSource audioSource_left = instrumentObject.GetComponents<AudioSource>()[0];
                AudioSource audioSource_right = instrumentObject.GetComponents<AudioSource>()[1];
                //Debug.Log("Pause Song");
                audioSource_left.Stop();
                audioSource_right.Stop();
                currentTime = (audioSource_left.time + audioSource_right.time) / 2.0f;
            }

            // TEST TEST TEST. testing code snippet from ARTapToPlaceObjects for repositioning newly spawned objects
            /*foreach (GameObject instrumentObject in instrumentObjects)
            {
                Camera m_MainCamera = Camera.main;
                Vector2 initial_direction = new Vector2(0, 1);
                Vector2 camera_direction = new Vector2(m_MainCamera.transform.forward.x, m_MainCamera.transform.forward.z);
                float rotation_angle = Mathf.Acos(Vector2.Dot(initial_direction, camera_direction)) * 180 / Mathf.PI;
                if (m_MainCamera.transform.forward.x < 0)
                {
                    rotation_angle = 360 - rotation_angle;
                }
                instrumentObject.transform.eulerAngles = new Vector3(0.0f, 0.0f + rotation_angle, 0.0f);
                Debug.Log(initial_direction);
                Debug.Log(camera_direction);
                Debug.Log(Vector2.Dot(initial_direction, camera_direction));
                Debug.Log(rotation_angle);
            }*/
        }


        toggleGreenNote(false);

    }


}
