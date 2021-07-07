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

    public GameObject pianoObjectToInstantiate;
    public GameObject drumObjectToInstantiate;
    public GameObject bassObjectToInstantiate;
    public GameObject vocalObjectToInstantiate;
    public GameObject miscObjectToInstantiate;

    public GameObject echoObjectsToInstantiate;

    public Dictionary<string, GameObject> spawnedObjects =
        new Dictionary<string, GameObject>(){
            {"Piano", null},
            {"Drum", null},
            {"Bass", null},
            {"Vocal", null},
            {"Misc", null},
        };

    public Dictionary<TrackableId, Dictionary<string, GameObject>> echoObjects; // AR plane ID first, then instrument

    public Dictionary<string, float> objectScale =
        new Dictionary<string, float>(){
            {"Piano", 0.45f},
            {"Drum", 0.075f},
            {"Bass", 0.3f},
            {"Vocal", 0.1f},
            {"Misc", 0.025f},
        };

    /*public Dictionary <string, string> instrument_names = 
        new Dictionary <string, string>(){ 
            {"Instrument1", "Bass"}, 
            {"Instrument2", "Piano"}, 
            {"Instrument3", "Drum"}, 
            {"Instrument4", "Vocal"}, 
            {"Instrument5", "Misc"} 
        };*/

    public ButtonManager buttonManagerScript;
    public DropdownManager dropdownManagerScript;
    private float instrumentPitch;
    AudioMixer pitchBendMixer;


    private GameObject spawnedObject;
    
    private ARRaycastManager _arRaycastManager;
    
    private Vector2 touchPosition;
    
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Slider object_scale_slider;

    private int counter_after_touch = 0;

    void Awake()
    {
        m_MainCamera = Camera.main;
        pitchBendMixer = Resources.Load<AudioMixer>("AudioMixer/TrackMixer");
        object_scale_slider = GameObject.FindGameObjectsWithTag("Slider_Object_Scaling")[0].GetComponent<Slider>();
        _arRaycastManager = GetComponent<ARRaycastManager>();
        echoObjects = new Dictionary<TrackableId, Dictionary<string, GameObject>>();
    }


    
    void UpdateEchoes (string planeId, string instr)
    {
        UnityEngine.XR.ARFoundation.ARPlaneManager planeManager = GetComponent<UnityEngine.XR.ARFoundation.ARPlaneManager>();
        foreach (var plane in planeManager.trackables)
        {
            Debug.Log(plane.transform.position);
            Debug.Log(plane.infinitePlane);
        }
        Debug.Log("Number of trackables: " + planeManager.trackables.count.ToString());
    }

    bool TryGetTouchPosition (out Vector2 touchPosition)
    {
        if (Input.touchCount > 0) {

            //Debug.Log("THERE IS MORE THAN 0 TOUCHES");
            
            Touch touch = Input.GetTouch(0);
            touchPosition = touch.position;
            int touchId = touch.fingerId;

            if (EventSystem.current.IsPointerOverGameObject(touchId))
            {
                counter_after_touch = 0;
                //Debug.Log("touch is over an ui object");
                touchPosition = default;

                return false;
            }

            if (counter_after_touch < 2) {
                //Debug.Log("waiting a few frames");
                touchPosition = default;
                counter_after_touch += 1;
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

        /*
        UnityEngine.XR.ARFoundation.ARPlaneManager planeManager = GetComponent<UnityEngine.XR.ARFoundation.ARPlaneManager>();
        foreach (var plane in planeManager.trackables)
        {
            Debug.Log(plane.transform.position);
            Debug.Log(plane.infinitePlane);
        }
        Debug.Log("Number of trackables: " + planeManager.trackables.count.ToString());
        */

        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            if (buttonManagerScript.delete == 1)
            {
                // Debug.Log("About to delete");
                if (instrument.Equals(""))
                    return;

                if (spawnedObjects[instrument] != null)
                {
                    Destroy(spawnedObjects[instrument]);
                    spawnedObjects[instrument] = null;


                    foreach (var planeID in echoObjects.Keys)
                    {
                        Destroy(echoObjects[planeID][instrument]);
                        echoObjects[planeID].Remove(instrument);
                        // I still see echo objects on the screen.
                    }
                }
                buttonManagerScript.delete = 0;
            }
            return;
        }

        
        if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            Debug.Log (hits);

            Debug.Log ("Hitpose");
            Debug.Log (hitPose);

            bool newObjectAdded = false;

            if (instrument.Equals(""))
                return;

            if (spawnedObjects[instrument] == null)
            {
                spawnedObjects[instrument] = Instantiate(getObjectToInstantiate(instrument), hitPose.position, hitPose.rotation);
                //string tag;
                //Debug.Log(instrument);
                //Debug.Log(dropdownManagerScript.song);  
                newObjectAdded = true;          
            }
            else
            {
                spawnedObjects[instrument].transform.position = hitPose.position;
            }

            UnityEngine.XR.ARFoundation.ARPlaneManager planeManager = GetComponent<UnityEngine.XR.ARFoundation.ARPlaneManager>();
            foreach (var plane in planeManager.trackables)
            {
                var planeId = plane.trackableId;
                if (!echoObjects.ContainsKey (planeId)) {
                    echoObjects.Add (planeId, new Dictionary<string, GameObject>());
                }
                foreach (string instr in spawnedObjects.Keys)
                {
                    if (!echoObjects[planeId].ContainsKey(instr) && spawnedObjects[instr] != null) {
                        echoObjects[planeId].Add(instr, Instantiate (echoObjectsToInstantiate));
                        echoObjects[planeId][instr].tag = instr + "_echo";
                        Debug.Log (echoObjects[planeId][instr].tag);
                        newObjectAdded = true;
                    }
                }
            }

            foreach (TrackableId planeId in echoObjects.Keys)
            {
                var plane = planeManager.GetPlane(planeId);
                var infPlane = plane.infinitePlane;
                foreach (string instr in echoObjects[planeId].Keys) 
                {
                    var pos = spawnedObjects[instr].transform.position;
                    Vector3 closestPoint = infPlane.ClosestPointOnPlane(pos);
                    //float distanceToPoint = infPlane.GetDistanceToPoint(hitPose);
                    var newPoint = pos+ 2 * (closestPoint - pos);
                    /*
                    Debug.Log ("newPoint");
                    Debug.Log (newPoint);
                    Debug.Log ("closestPoint");
                    Debug.Log (closestPoint);
                    Debug.Log ("hitPose.position");
                    Debug.Log (pos);       */
                    echoObjects[planeId][instr].transform.position = newPoint;
                    Debug.Log ("Inside double for loop");
                }
                
            }
            //Debug.Log("Number of trackables: " + planeManager.trackables.count.ToString());
            if (newObjectAdded)
            {
                AudioSeekManager.Instance.setTracks (dropdownManagerScript.song);   
                AudioSeekManager.Instance.playSong(); 
            }
        }
        
    }
}
