using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
public enum AttachmentRule { KeepRelative, KeepWorld, SnapToTarget }
public class TreasureHunter : MonoBehaviour
{

    public GameObject charCyl;
    public float totScore = 0;
    public int sphereCount = 0;
    public int cubeCount = 0;
    public int capsuleCount = 0;
    public int cylinderCount = 0;
    public TextMesh winmsg;
    
    OVRCameraRig oVRCameraRig;
    OVRManager oVRManager;
    OVRHeadsetEmulator oVRHeadsetEmulator;
    Camera viewpointCamera;

    public bool test;
    //PostProcessLayer postProcessLayer;
    //LocomotionHandler locomotionHandler;
    // Start is called before the first frame update

    public AudioSource audioSource;
    public AudioClip impact;
    public GameObject leftPointerObject;
    public GameObject rightPointerObject;
    public TextMesh outputText;
    public TextMesh outputText2;
    public TextMesh outputText3;
    public LayerMask collectiblesMask;

    GameObject thingOnGun;

    Collectible thingIGrabbed;

    Vector3 previousPointerPos;

    public TextMesh scoreText;
    public float translationalSpeed=0.1f;
    public float rotationalSpeed=2.0f;
    public Camera aerialViewCam;

    public Camera playerCam;
    public bool shouldLockMouse=true;
    Vector3 prevForwardVector;

    Vector3 trajectoryVector;

    Vector3 prevLocation;
    
    public GameObject TrackingSpace;
    float prevYawRelativeToCenter;
    float howMuchUserRotated;
    float directionUserRotated;
    float deltaYawRelativeToCenter;
    float distanceFromCenter;//////
    float longestDimensionOfPE = 5;
    float howMuchToAccelerate;
    Vector3 howMuchToTranslate;
    //FOLLOWING IS PART OF THE REDIRECTION ALGORITHM
    float d(Vector3 pointToTest, Vector3 vectorSource, Vector3 vectorDestination){
        float x = (pointToTest.x-vectorSource.x)*(vectorDestination.z-vectorSource.z)-(pointToTest.z-vectorSource.z)*(vectorDestination.x-vectorSource.x);
        return x;
    }
    float angleBetweenVectors(Vector3 A, Vector3 B){
        A.y = 0;
        B.y = 0;
      return Mathf.Rad2Deg*(Mathf.Acos(Vector3.Dot(Vector3.Normalize(A),Vector3.Normalize(B))));
    }

    void Start()
    {
        //FOLLOWING IS PART OF THE REDIRECTION ALGORITHM
        prevForwardVector=playerCam.transform.forward;
        prevYawRelativeToCenter=angleBetweenVectors(playerCam.transform.forward,TrackingSpace.transform.position-playerCam.transform.position);

        scoreText.transform.LookAt(playerCam.transform);
        scoreText.text = "Ruth & Micah";


        prevLocation = playerCam.transform.position;
        
    }
    // Update is called once per frame
    void Update()
    {

    int counter = 0;

    if(Input.GetMouseButtonDown(0)){
        print("hit 1");
        
        GameObject pickedUpObject;
        RaycastHit outHit;
            if (Physics.Raycast(aerialViewCam.transform.position, aerialViewCam.transform.forward, out outHit, 100.0f)){

            pickedUpObject=outHit.collider.gameObject;   

            //pickedUpObject.GetComponent<AudioSource>().Stop();

            Destroy(pickedUpObject.transform.parent.gameObject);

            Debug.Log(" raycast hit ");

            counter++;
            if(counter>=10){
                winmsg.text = "you win game over";
            }

        }
    }

        
        // FOLLOWING IS PART OF REDIRECTION ALGORITHM
        Vector3 YIdentity = new Vector3(0,1,0);
        howMuchUserRotated=angleBetweenVectors(prevForwardVector,playerCam.transform.forward);

        directionUserRotated=(d(playerCam.transform.position+prevForwardVector, playerCam.transform.position, playerCam.transform.position + playerCam.transform.forward)<0)?-1:1 ;
        deltaYawRelativeToCenter=prevYawRelativeToCenter-angleBetweenVectors(playerCam.transform.forward,TrackingSpace.transform.position-playerCam.transform.position);
        distanceFromCenter=playerCam.transform.localPosition.magnitude;
        howMuchToAccelerate=(((deltaYawRelativeToCenter<0)? -0.13f: 0.3f) * howMuchUserRotated * directionUserRotated * Mathf.Clamp(distanceFromCenter/longestDimensionOfPE/2,0,1));
        if(Mathf.Abs(howMuchToAccelerate)>0){
        TrackingSpace.transform.RotateAround(playerCam.transform.position, YIdentity, howMuchToAccelerate);
        }
        prevForwardVector=playerCam.transform.forward;
        prevYawRelativeToCenter=angleBetweenVectors(playerCam.transform.forward,TrackingSpace.transform.position-playerCam.transform.position);
        

        //this is for translational gain
        trajectoryVector = playerCam.transform.position - prevLocation;
        howMuchToTranslate = trajectoryVector*0.50f;
        TrackingSpace.transform.position += howMuchToTranslate;
        prevLocation = playerCam.transform.position;

        
        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
        {
            Collider[] overlappingThings = Physics.OverlapSphere(rightPointerObject.transform.position, 0.5f, collectiblesMask);
            if (overlappingThings.Length > 0)
            {
                attachGameObjectToAChildGameObject(overlappingThings[0].gameObject, rightPointerObject.gameObject, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld, true);
                //I'm not bothering to check for nullity because layer mask should ensure I only collect collectibles.
                thingIGrabbed = overlappingThings[0].gameObject.GetComponent<Collectible>();
                audioSource.Play();
            }

        }
        else if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
        {
            if (thingIGrabbed.gameObject.GetComponent<Transform>().position.x < (Camera.main.gameObject.transform.position.x + 0.3) &&
            thingIGrabbed.gameObject.GetComponent<Transform>().position.x > (Camera.main.gameObject.transform.position.x - 0.3) &&
            thingIGrabbed.gameObject.GetComponent<Transform>().position.z < (Camera.main.gameObject.transform.position.z + 0.3) &&
            thingIGrabbed.gameObject.GetComponent<Transform>().position.z > (Camera.main.gameObject.transform.position.z - 0.3) &&
            thingIGrabbed.gameObject.GetComponent<Transform>().position.y < (Camera.main.gameObject.transform.position.y - 0.3) &&
            thingIGrabbed.gameObject.GetComponent<Transform>().position.y > (Camera.main.gameObject.transform.position.y - 1.2))
            {
                scoreText.text = "placed" + thingIGrabbed.gameObject.name;
                if(thingIGrabbed.gameObject.name == "Sphere"){
                    GameObject prefab = (GameObject)Resources.Load<GameObject>("ball");
                    this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems.Add(prefab.GetComponent<Collectible>());
                    sphereCount++;
                }
                if(thingIGrabbed.gameObject.name == "Cube"){
                    cubeCount++;
                    GameObject prefab = (GameObject)Resources.Load<GameObject>("cube");
                    
                    this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems.Add(prefab.GetComponent<Collectible>());
                    
                }
                if(thingIGrabbed.gameObject.name == "Cylinder"){
                    GameObject prefab = (GameObject)Resources.Load<GameObject>("cyl");
                    cylinderCount++;
                    this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems.Add(prefab.GetComponent<Collectible>());
                    
                }
                if(thingIGrabbed.gameObject.name == "Capsule"){
                    GameObject prefab = (GameObject)Resources.Load<GameObject>("cap");
                    this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems.Add(prefab.GetComponent<Collectible>());
                    capsuleCount++;
                }
                totScore = calculateScore();
                Destroy(thingIGrabbed.gameObject);
                scoreText.text = "ball-1: " + sphereCount + "\n" + "cube-2: " + cubeCount + "\n" 
                + "cylinder-3: " + cylinderCount + "\n" + "capsule-4: " + capsuleCount +"\n" + "score: " + totScore; 
            }
            else
            {
                letGo();
            }
            //since you can't merge paths the way I did in BP, I need to create a function that does the force grab thing or else I would duplicate code
            //equivalent to ShootAndGrabNoSnap in UE4 version (A)
        }else if (OVRInput.GetDown(OVRInput.RawButton.A)){
            outputText3.text="Ruth Force Grab No Snap";
            forceGrab(true);

        //equivalent to ShootAndGrabSnap in UE4 version (B)
        }else if (OVRInput.GetDown(OVRInput.RawButton.B)){
            outputText3.text="Ruth Force Grab Snap";
            forceGrab(false);

        //equivalent to MagneticGrip in UE4 version (RS/R3)
        }else if (OVRInput.GetDown(OVRInput.RawButton.LThumbstick)){
            outputText3.text="Ruth Magnetic Grip";
            Collider[] overlappingThings=Physics.OverlapSphere(rightPointerObject.transform.position,1,collectiblesMask);
            if (overlappingThings.Length>0){
                Collectible nearestCollectible=getClosestHitObject(overlappingThings);
                attachGameObjectToAChildGameObject(nearestCollectible.gameObject,rightPointerObject,AttachmentRule.SnapToTarget,AttachmentRule.SnapToTarget,AttachmentRule.KeepWorld,true);
                //I'm not bothering to check for nullity because layer mask should ensure I only collect collectibles.
                thingIGrabbed=nearestCollectible.gameObject.GetComponent<Collectible>();
            }
        }



        float calculateScore()
        {
            float score = 0;
            List<Collectible> collectibleTreasures = this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems;
            foreach (Collectible treasure in collectibleTreasures)
            {
                score += treasure.value;
            }
            return score;
        }
    }

    Collectible getClosestHitObject(Collider[] hits)
    {
        float closestDistance = 10000000.0f;
        Collectible closestObjectSoFar = null;
        foreach (Collider hit in hits)
        {
            Collectible c = hit.gameObject.GetComponent<Collectible>();
            if (c)
            {
                float distanceBetweenHandAndObject = (c.gameObject.transform.position - rightPointerObject.gameObject.transform.position).magnitude;
                if (distanceBetweenHandAndObject < closestDistance)
                {
                    closestDistance = distanceBetweenHandAndObject;
                    closestObjectSoFar = c;
                }
            }
        }
        return closestObjectSoFar;
    }
    //could have more easily just passed in attachment rule.... but I wanted to keep the code similar to the BP example
    void forceGrab(bool pressedA)
    {
        RaycastHit outHit;
        //notice I'm using the layer mask again
        if (Physics.Raycast(rightPointerObject.transform.position, rightPointerObject.transform.forward, out outHit, 10000000.0f, collectiblesMask))
        {
            AttachmentRule howToAttach = pressedA ? AttachmentRule.KeepWorld : AttachmentRule.SnapToTarget;
            attachGameObjectToAChildGameObject(outHit.collider.gameObject, rightPointerObject.gameObject, howToAttach, howToAttach, AttachmentRule.KeepWorld, true);
            thingIGrabbed = outHit.collider.gameObject.GetComponent<Collectible>();
        } 
    }
    void letGo()
    {
        if (thingIGrabbed)
        {
            Collider[] overlappingThingsWithLeftHand = Physics.OverlapSphere(leftPointerObject.transform.position, 0.01f, collectiblesMask);

            detachGameObject(thingIGrabbed.gameObject, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld);
            simulatePhysics(thingIGrabbed.gameObject, Vector3.zero / Time.deltaTime, true);
            thingIGrabbed = null;

        }
    }

    //since Unity doesn't have sceneComponents like UE4, we can only attach GOs to other GOs which are children of another GO
    //e.g. attach collectible to controller GO, which is a child of VRRoot GO
    //imagine if scenecomponents in UE4 were all split into distinct GOs in Unity
    public void attachGameObjectToAChildGameObject(GameObject GOToAttach, GameObject newParent, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule, bool weld)
    {
        GOToAttach.transform.parent = newParent.transform;
        handleAttachmentRules(GOToAttach, locationRule, rotationRule, scaleRule);
        if (weld)
        {
            simulatePhysics(GOToAttach, Vector3.zero, false);
        }
    }

    public static void detachGameObject(GameObject GOToDetach, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule)
    {
        //making the parent null sets its parent to the world origin (meaning relative & global transforms become the same)
        GOToDetach.transform.parent = null;
        handleAttachmentRules(GOToDetach, locationRule, rotationRule, scaleRule);
    }

    public static void handleAttachmentRules(GameObject GOToHandle, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule)
    {
        GOToHandle.transform.localPosition =
        (locationRule == AttachmentRule.KeepRelative) ? GOToHandle.transform.position :
        //technically don't need to change anything but I wanted to compress into ternary
        (locationRule == AttachmentRule.KeepWorld) ? GOToHandle.transform.localPosition :
        new Vector3(0, 0, 0);

        //localRotation in Unity is actually a Quaternion, so we need to specifically ask for Euler angles
        GOToHandle.transform.localEulerAngles =
        (rotationRule == AttachmentRule.KeepRelative) ? GOToHandle.transform.eulerAngles :
        //technically don't need to change anything but I wanted to compress into ternary
        (rotationRule == AttachmentRule.KeepWorld) ? GOToHandle.transform.localEulerAngles :
        new Vector3(0, 0, 0);

        GOToHandle.transform.localScale =
        (scaleRule == AttachmentRule.KeepRelative) ? GOToHandle.transform.lossyScale :
        //technically don't need to change anything but I wanted to compress into ternary
        (scaleRule == AttachmentRule.KeepWorld) ? GOToHandle.transform.localScale :
        new Vector3(1, 1, 1);
    }

    public void simulatePhysics(GameObject target, Vector3 oldParentVelocity, bool simulate)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb)
        {
            if (!simulate)
            {
                Destroy(rb);
            }
        }
        else
        {
            if (simulate)
            {
                //there's actually a problem here relative to the UE4 version since Unity doesn't have this simple "simulate physics" option
                //The object will NOT preserve momentum when you throw it like in UE4.
                //need to set its velocity itself.... even if you switch the kinematic/gravity settings around instead of deleting/adding rb
                Rigidbody newRB = target.AddComponent<Rigidbody>();
                newRB.velocity = oldParentVelocity;
            }
        }
    }
}

