using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public Dictionary<string, float> objectScale =
        new Dictionary<string, float>(){
            {"Piano", 0.45f},
            {"Drum", 0.075f},
            {"Bass", 0.3f},
            {"Vocal", 0.1f},
            {"Misc", 0.025f},
        };

    public ButtonManager buttonManagerScript;
    public DropdownManager dropdownManagerScript;
    private float instrumentPitch;
    AudioMixer pitchBendMixer;


    private GameObject spawnedObject;
    
    private ARRaycastManager _arRaycastManager;
    
    private Vector2 touchPosition;
    
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Slider object_scale_slider;

    void Awake()
    {
        m_MainCamera = Camera.main;
        pitchBendMixer = Resources.Load<AudioMixer>("AudioMixer/TrackMixer");
        object_scale_slider = GameObject.FindGameObjectsWithTag("Slider_Object_Scaling")[0].GetComponent<Slider>();
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }


    bool TryGetTouchPosition (out Vector2 touchPosition)
    {
        if (Input.touchCount > 0) {
            touchPosition = Input.GetTouch(0).position;

            if(touchPosition.y > Screen.height * 0.8 || touchPosition.y < Screen.height*0.15){
                touchPosition = default;
                return false;
            }

            //Log("I sense a touch");
            //Debug.Log(touchPosition);
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
        instrumentPitch = Mathf.Pow(2.0f, m_MainCamera.transform.position.y / 2.0f);
        if (buttonManagerScript.togglePitchOn)
        {
            pitchBendMixer.SetFloat("pitchBend", Mathf.Clamp(instrumentPitch, 0.5f, 2.0f));
        }
        else
        {
            pitchBendMixer.SetFloat("pitchBend", 1.0f);
        }

        string instrument = buttonManagerScript.selectedInstrument;

        if (!instrument.Equals(""))
        {
            float newSize = objectScale[instrument] * object_scale_slider.value;
            if (spawnedObjects[instrument] != null) {
                spawnedObjects[instrument].transform.localScale = new Vector3(newSize, newSize, newSize);
            }
        }

        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            if(buttonManagerScript.delete == 1)
            {
                // Debug.Log("About to delete");
                if (instrument.Equals(""))
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

        if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            Debug.Log (hits);


            if (instrument.Equals(""))
                return;

            if (spawnedObjects[instrument] == null)
            {
                spawnedObjects[instrument] = Instantiate(getObjectToInstantiate(instrument), hitPose.position, hitPose.rotation);
                //string tag;
                //Debug.Log(instrument);
                //Debug.Log(dropdownManagerScript.song);


                AudioSeekManager.Instance.setTracks (dropdownManagerScript.song);
                AudioSeekManager.Instance.playSong();
                
            }
            else
            {
                spawnedObjects[instrument].transform.position = hitPose.position;
            }

        }
        
    }
}
