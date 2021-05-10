using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{

    private bool isClicked = false;
    public string selectedInstrument = null;
    private GameObject selectedObject = null;


    public void Hello()
    {
        isClicked = true;
    }

    void Update()
    {
        if (isClicked == true)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
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
            isClicked = false;
        }
    }
}
