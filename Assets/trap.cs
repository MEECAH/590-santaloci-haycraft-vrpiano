using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trap : MonoBehaviour
{
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other){
        print("trigger");
        audioSource.Play();
        GameObject body = GameObject.Find("OVRPlayerController");
        Rigidbody gameObjectRigidBody = body.AddComponent<Rigidbody>();
        gameObjectRigidBody.mass = 5;
    }
}
