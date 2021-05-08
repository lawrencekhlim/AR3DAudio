using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{

    bool isClicked = false;

    public void Hello()
    {
        isClicked = true;
    }

    void Update()
    {
        if (isClicked == true)
        {
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                selectedObject.GetComponent<Image>().color = Color.red;
                Debug.Log(EventSystem.current.currentSelectedGameObject.name);
            }
            isClicked = false;
        }
    }
}
