using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler,IPointerUpHandler
{
    [SerializeField] private GameObject movingStar;
    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        transform.localScale = new Vector3(1.1f,1.1f,1.1f);
        movingStar.SetActive(true);
        Debug.Log("Cursor Entering " + name + " GameObject");
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        transform.localScale = new Vector3(1,1,1);
        movingStar.SetActive(false);
        //Output the following message with the GameObject's name
        Debug.Log("Cursor Exiting " + name + " GameObject");
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        
        //Output the name of the GameObject that is being clicked
        Debug.Log(name + "Game Object Click in Progress");
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        Debug.Log(name + "No longer being clicked");
    }
}