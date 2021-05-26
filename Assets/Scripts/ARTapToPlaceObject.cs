using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]


public class ARTapToPlaceObject : MonoBehaviour
{
    Camera m_MainCamera;
    public GameObject gameObjectToInstantiate;

    public GameObject pianoObjectToInstantiate;
    public GameObject drumObjectToInstantiate;
    public GameObject bassObjectToInstantiate;
    public GameObject vocalObjectToInstantiate;
    public GameObject miscObjectToInstantiate;

    public Dictionary<string, GameObject> spawnedObjects =
        new Dictionary<string, GameObject>(){
            {"Piano", null},
            {"Drum", null},
            {"Bass", null},
            {"Vocal", null},
            {"Misc", null},
        };

    public ButtonManager buttonManagerScript;
    public DropdownManager dropdownManagerScript;
    private float instrumentPitch;
    AudioMixer pitchBendMixer;


    private GameObject spawnedObject;
    
    private ARRaycastManager _arRaycastManager;
    
    private Vector2 touchPosition;
    
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();


    void Awake()
    {
        m_MainCamera = Camera.main;
        pitchBendMixer = Resources.Load<AudioMixer>("AudioMixer/TrackMixer");
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }


    bool TryGetTouchPosition (out Vector2 touchPosition)
    {
        if (Input.touchCount > 0) {
            touchPosition = Input.GetTouch(0).position;

            if(touchPosition.y > Screen.height * 0.9 || touchPosition.y < Screen.height*0.1){
                touchPosition = default;
                return false;
            }

            Debug.Log("I sense a touch");
            Debug.Log(touchPosition);
            return true;
        }
        touchPosition = default;
        return false;
    }

    GameObject getObjectToInstantiate(string instrument_name)
    {
        switch (instrument_name)
        {
            case "Piano":
                return pianoObjectToInstantiate;
            case "Drum":
                return drumObjectToInstantiate;
            case "Bass":
                return bassObjectToInstantiate;
            case "Vocal":
                return vocalObjectToInstantiate;
            case "Misc":
                return miscObjectToInstantiate;
            default:
                return null;
        }
    }


    // Update is called once per frame
    void Update()
    {
        // pitch is normalized so that when z=0,pitch=1.0. 
        instrumentPitch = (m_MainCamera.transform.position.y * 1.0f) + 1.0f;
        if (buttonManagerScript.togglePitchOn)
        {
            pitchBendMixer.SetFloat("pitchBend", Mathf.Clamp(instrumentPitch, 0.5f, 2.0f));
        }
        else
        {
            pitchBendMixer.SetFloat("pitchBend", 1.0f);
        }


        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            if(buttonManagerScript.delete == 1)
            {
                // Debug.Log("About to delete");
                string instrument = buttonManagerScript.selectedInstrument;
                if (instrument == null)
                    return;

                if (spawnedObjects[instrument] != null)
                {
                    Destroy(spawnedObjects[instrument]);
                    spawnedObjects[instrument] = null;
                }
                buttonManagerScript.delete = 0;
            }
            return;
        }

        /*if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon)) {
            var hitPose = hits[0].pose;
            
            if (spawnedObject == null) {
                spawnedObject = Instantiate (gameObjectToInstantiate, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
            }
        
        }*/

        if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;


            string instrument = buttonManagerScript.selectedInstrument;
            if (instrument == null)
                return;

            if (spawnedObjects[instrument] == null)
            {
                spawnedObjects[instrument] = Instantiate(getObjectToInstantiate(instrument), hitPose.position, hitPose.rotation);
                //string tag;
                Debug.Log(instrument);
                Debug.Log(dropdownManagerScript.song);


                AudioSeekManager.Instance.setTracks (dropdownManagerScript.song);
                AudioSeekManager.Instance.playSong();
                /*
                // Set Audio Output of Spawned Object
                AudioSource instrumentAudioSource = spawnedObjects[instrument].GetComponent<AudioSource>();
                if (instrumentAudioSource != null)
                {
                    instrumentAudioSource.outputAudioMixerGroup = pitchBendMixer.FindMatchingGroups("Master")[0];
                } else
                {
                    Debug.Log("Error: Couldn't find audio source on instrument");
                }

                // Setup track for each instrument
                
                var clip = Resources.Load(dropdownManagerScript.song) as AudioClip;
                if (instrument.Contains("Vocal"))
                {
                    if(GameObject.FindGameObjectsWithTag("Instrument4").Length != 0)
                    {
                        AudioSource audio = GameObject.FindGameObjectWithTag("Instrument4").GetComponent<AudioSource>();
                        Debug.Log(audio.ToString());
                        audio.Pause();
                        audio.clip = clip;
                        AudioSeekManager.Instance.playSong();
                    }
                }
                else if(instrument.Contains("Bass"))
                {
                    if(GameObject.FindGameObjectsWithTag("Instrument1").Length != 0)
                    {
                        AudioSource audio = GameObject.FindGameObjectWithTag("Instrument1").GetComponent<AudioSource>();
                        Debug.Log(audio.ToString());
                        audio.Pause();
                        audio.clip = clip;
                        AudioSeekManager.Instance.playSong();
                    }
                }
                else if(instrument.Contains("Misc"))
                {
                    if(GameObject.FindGameObjectsWithTag("Instrument5").Length != 0)
                    {
                        AudioSource audio = GameObject.FindGameObjectWithTag("Instrument5").GetComponent<AudioSource>();
                        Debug.Log(audio.ToString());
                        audio.Pause();
                        audio.clip = clip;
                        AudioSeekManager.Instance.playSong();
                    }
                }
                else if(instrument.Contains("Drum"))
                {
                    if(GameObject.FindGameObjectsWithTag("Instrument3").Length != 0)
                    {
                        AudioSource audio = GameObject.FindGameObjectWithTag("Instrument3").GetComponent<AudioSource>();
                        Debug.Log(audio.ToString());
                        audio.Pause();
                        audio.clip = clip;
                        AudioSeekManager.Instance.playSong();
                    }
                }
                else if(instrument.Contains("Piano"))
                {
                    if(GameObject.FindGameObjectsWithTag("Instrument2").Length != 0)
                    {
                        AudioSource audio = GameObject.FindGameObjectWithTag("Instrument2").GetComponent<AudioSource>();
                        Debug.Log(audio.ToString());
                        audio.Pause();
                        audio.clip = clip;
                        AudioSeekManager.Instance.playSong();
                    }
                }

                */
            }
            else
            {
                spawnedObjects[instrument].transform.position = hitPose.position;
            }

        }
    }
}
