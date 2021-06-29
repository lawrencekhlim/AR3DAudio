using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
    Camera m_MainCamera;
    private bool isClicked = false;
    public string selectedInstrument = "";
    private GameObject selectedObject = null;
    public int delete = 0;
    public int play_pause_button_state = 1; // 0 is showing pause (currently playing), 1 is showing play (currently paused)
    public bool togglePitchOn = false;
    
    public void Hello()
    {
        isClicked = true;
    }

    public void togglePitchSetting()
    {

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            GameObject pitchButton = EventSystem.current.currentSelectedGameObject;
            togglePitchOn = !togglePitchOn;
            Color myColor = new Color();
            if (togglePitchOn)
            {
                ColorUtility.TryParseHtmlString("#87b5ff", out myColor);
            }
            else
            {
                ColorUtility.TryParseHtmlString("#FFFFFF", out myColor);
            }
            pitchButton.GetComponent<Image>().color = myColor;
            Debug.Log(Camera.main.transform.position);
        }
    }

    void Update()
    {
        if (isClicked == true)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                Debug.Log(EventSystem.current.currentSelectedGameObject.ToString());

                if(EventSystem.current.currentSelectedGameObject.ToString().Contains("Delete"))
                {
                    if(selectedObject != null)
                    {
                        delete = 1;
                    }
                }
                else if(EventSystem.current.currentSelectedGameObject.ToString().Contains("Playpause"))
                {
                    isClicked = false;
                    Debug.Log(EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text);
                    if (play_pause_button_state == 1) {
                        EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text = "Pause";
                        play_pause_button_state = 0;
                        AudioSeekManager.Instance.setPlay (true);
                        if (AudioSeekManager.Instance.placedInstrument()) {
                            AudioSeekManager.Instance.playSong();
                        }

                        

                    }
                    else {
                        EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text = "Play";
                        play_pause_button_state = 1;
                        AudioSeekManager.Instance.setPlay (false);
                        AudioSeekManager.Instance.pauseSong();


                    }
                }
                else
                {
                    // make previously selected object white. Then replace it with new currently selected object
                    if (selectedObject != null)
                    {
                        selectedObject.GetComponent<Image>().color = Color.white;
                    }
                    GameObject newSelectedObject = EventSystem.current.currentSelectedGameObject;

                    if (selectedObject == newSelectedObject)
                    {
                        selectedObject = null;
                    } 
                    else
                    {
                        selectedObject = newSelectedObject;
                        Color myColor = new Color();
                        ColorUtility.TryParseHtmlString("#87b5ff", out myColor);
                        selectedObject.GetComponent<Image>().color = myColor;
                    }

                    if (selectedObject)
                    {
                        string[] name_split = selectedObject.name.Split('_');
                        selectedInstrument = name_split[1];
                    }
                    else
                    {
                        selectedInstrument = "";
                    }

                    Debug.Log(selectedInstrument);
                }

            }
            isClicked = false;
        }
    }
}
