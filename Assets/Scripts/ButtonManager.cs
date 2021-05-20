using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
    Camera m_MainCamera;
    private bool isClicked = false;
    public string selectedInstrument = null;
    private GameObject selectedObject = null;
    public int delete = 0;
    public bool togglePitchOn = false;


    public void Hello()
    {
        isClicked = true;
    }

    public void togglePitchSetting()
    {

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            selectedObject = EventSystem.current.currentSelectedGameObject;
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
            selectedObject.GetComponent<Image>().color = myColor;
            Debug.Log(Camera.main.transform.position);
        }
    }

    void Update()
    {
        if (isClicked == true)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                // Debug.Log(EventSystem.current.currentSelectedGameObject.ToString());
                if(EventSystem.current.currentSelectedGameObject.ToString().Contains("Delete"))
                {
                    // Debug.Log("Here in delete");
                    if(selectedObject != null)
                    {
                        delete = 1;
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
                        selectedInstrument = null;
                    }

                    Debug.Log(selectedInstrument);
                }

            }
            isClicked = false;
        }
    }
}
