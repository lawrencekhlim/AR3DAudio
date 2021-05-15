using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]


public class ARTapToPlaceObject : MonoBehaviour
{

    public GameObject gameObjectToInstantiate;

    public GameObject pianoObjectToInstantiate;
    public GameObject drumObjectToInstantiate;
    public GameObject bassObjectToInstantiate;
    public GameObject vocalObjectToInstantiate;
    public GameObject miscObjectToInstantiate;

    private Dictionary<string, GameObject> spawnedObjects =
        new Dictionary<string, GameObject>(){
            {"Piano", null},
            {"Drum", null},
            {"Bass", null},
            {"Vocal", null},
            {"Misc", null},
        };

    //public string instrument = "Piano";
    public ButtonManager buttonManagerScript;




    private GameObject spawnedObject;
    
    private ARRaycastManager _arRaycastManager;
    
    private Vector2 touchPosition;
    
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();


    void Awake()
    {
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
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            if(buttonManagerScript.delete == 1)
            {
                Debug.Log("About to delete");
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
            }
            else
            {
                spawnedObjects[instrument].transform.position = hitPose.position;
            }

        }
    }
}
