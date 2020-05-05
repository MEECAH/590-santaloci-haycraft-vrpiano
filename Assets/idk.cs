using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using System;

public class idk : MonoBehaviour
{
    public GameObject key;
    private float y;
    // public Text text;
    //get reference to the RectTransform component
    // private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        y = key.transform.position.y;
//  rectTransform = text.GetComponent<RectTransform>();

        //Set the anchor to Left Below Corner
        // rectTransform.anchorMin = new Vector2(0,0);
        // rectTransform.anchorMax = new Vector2(0,0);

    }

    // Update is called once per frame
    void Update()
    { 
        
  

         if (key){
              if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click.");
            // key.y = key.y-.3f;
            Vector3 newpos = key.transform.position;
            print("key:" + key);
            newpos.y -= .1f; // why does this work while 'transform.position.x += 5.0f;' doesn't?
            key.transform.position = newpos;
                //   rectTransform.anchoredPosition3D = Input.mousePosition;
        //display position info on the text
        // text.text = Input.mousePosition.ToString();
            // key.transform.position.y = key.transform.position.y -.3;
        }}
        if (Input.GetMouseButtonUp(0))
        { 
            Vector3 newpos = transform.position;
            newpos.y += .1f; // why does this work while 'transform.position.x += 5.0f;' doesn't?
            transform.position = newpos;
            Debug.Log("Unclick.");
            //    key.transform.position.y = 1;
        }

    }
}
