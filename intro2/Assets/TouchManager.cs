using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    Gestures currentGesture = Gestures.NONE;


    IControllable selectedObject;
    GameObject ourCameraPlane;


    float beginningDistanceBetweenFingers;
    Vector3 objectBeginningScale;
    Quaternion objectStartingRotation;
    float startingAngle;
    

    // Start is called before the first frame update
    void Start()
    {
        ourCameraPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ourCameraPlane.transform.position = new Vector3(0, Camera.main.transform.position.y, 0);
        ourCameraPlane.transform.up = (Camera.main.transform.position - ourCameraPlane.transform.position).normalized;
        
        //to change plane to angled
        ourCameraPlane.transform.eulerAngles = new Vector3(
            ourCameraPlane.transform.eulerAngles.x + 70,
            ourCameraPlane.transform.eulerAngles.y,
            ourCameraPlane.transform.eulerAngles.z
        );

        ourCameraPlane.transform.position = new Vector3(
            ourCameraPlane.transform.position.x,
            ourCameraPlane.transform.position.y - 1,
            ourCameraPlane.transform.position.z
        );
    }

    // Update is called once per frame
    void Update()
    {
        //reset currentGesture
        if (Input.touchCount == 0)
            currentGesture = Gestures.NONE;


        //Single touch
        if(Input.touchCount == 1)
        {
            
            Ray ourRay = Camera.main.ScreenPointToRay(Input.touches[0].position);

            Debug.DrawRay(ourRay.origin, 30 * ourRay.direction);
            IControllable object_hit;
            

            RaycastHit info;
            Touch currentTouch = Input.GetTouch(0);
            if (currentTouch.phase == TouchPhase.Moved )
            {
                currentGesture = Gestures.DRAG;
                
                if (Physics.Raycast(ourRay, out info) && selectedObject !=null)
                {
                    if(info.transform == ourCameraPlane.transform)
                    {
                        Vector3 pointOnPlane = info.point;
                        selectedObject.moveTo(pointOnPlane);
                    }
                }
                else if (Physics.Raycast(ourRay, out info) && selectedObject == null)
                {
                    Camera.main.transform.position = info.point;
                }

                //Moving equal distance from the camera
                //float starting_distance_to_selected_object = Vector3.Distance(Camera.main.transform.position, selectedObject.gameObject.transform.position);
                //selectedObject.moveTo(ourRay.GetPoint(starting_distance_to_selected_object));



            }
            if(currentTouch.phase == TouchPhase.Ended && currentGesture != Gestures.DRAG)
            {
                
                if (Physics.Raycast(ourRay, out info))
                {
                    object_hit = info.transform.GetComponent<IControllable>();
                    if (object_hit != null && selectedObject == null)
                    {                        
                        selectedObject = object_hit;
                        if(selectedObject != null)
                            Debug.Log("object selected");
                        Renderer selectedObjectColour = selectedObject.gameObject.GetComponent<Renderer>();
                        selectedObjectColour.material.SetColor("_Color", Color.blue);
                    }
                    else if(object_hit == null && selectedObject != null)
                    {
                        Deselect();
                    }
                    else if(selectedObject == object_hit && object_hit != null)
                    {
                        currentGesture = Gestures.TAP;

                        selectedObject.youve_been_tapped();

                        Deselect();
                    }
                }

                
            }
            if (currentTouch.phase == TouchPhase.Ended && currentGesture == Gestures.DRAG && selectedObject != null)
            {
                Debug.Log("Not tap");
                


                Deselect();
            }

        }

        //multi touch
        if(Input.touchCount == 2)
        {
            
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                beginningDistanceBetweenFingers = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

                startingAngle = Mathf.Atan2(touch1.position.y - touch0.position.y, touch1.position.x - touch0.position.x);



                if (selectedObject != null)
                {
                    objectBeginningScale = selectedObject.gameObject.transform.localScale;

                    objectStartingRotation = selectedObject.gameObject.transform.rotation;
                }
                else
                {
                    objectStartingRotation = Camera.main.transform.rotation;
                }

            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {

                float movedDistanceBetweenFingers = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                
                Vector3 newPositions = touch1.position - touch0.position;
                float newAngle = Mathf.Atan2(newPositions.y, newPositions.x);
                float difference = newAngle - startingAngle;
                float differenceAsDegrees = Mathf.Rad2Deg * difference;

                //for changing objects
                if (selectedObject != null)
                {

                    //logic for if zoom or rotate
                    if(Quaternion.Angle(objectStartingRotation, objectStartingRotation * Quaternion.AngleAxis(differenceAsDegrees, Camera.main.transform.forward)) > 15)
                    {
                        currentGesture = Gestures.ROTATE;
                        Debug.Log("should rotate");
                        selectedObject.gameObject.transform.rotation = objectStartingRotation * Quaternion.AngleAxis(differenceAsDegrees, Camera.main.transform.forward);
                        
                    }
                    else if(currentGesture != Gestures.ROTATE)
                    {
                        currentGesture = Gestures.ZOOM;
                        float amountToScale = movedDistanceBetweenFingers / beginningDistanceBetweenFingers;
                        Debug.Log("object should scale: " + amountToScale);
                        selectedObject.scaleTo(objectBeginningScale, amountToScale);
                    }
                    

                }

                //for changing camera
                else if (selectedObject == null)
                {
                    if(Quaternion.Angle(objectStartingRotation, objectStartingRotation * Quaternion.AngleAxis(differenceAsDegrees, Camera.main.transform.forward)) > 1)
                    {
                        currentGesture = Gestures.ROTATE;

                        Camera.main.transform.rotation = objectStartingRotation * Quaternion.AngleAxis(differenceAsDegrees, Camera.main.transform.forward);
                    }
                    else if(currentGesture != Gestures.ROTATE)
                    {
                        currentGesture = Gestures.ZOOM;
                        float distanceDifference = movedDistanceBetweenFingers - beginningDistanceBetweenFingers;
                        Camera.main.transform.position += (distanceDifference / 500) * transform.forward;
                    }
                    

                }
            }
        }

    }

    void Deselect()
    {
        Renderer selectedObjectColour = selectedObject.gameObject.GetComponent<Renderer>();
        selectedObjectColour.material.SetColor("_Color", Color.gray);
        selectedObject = null;
    }

    private enum Gestures
    {
        TAP,
        LONGTOUCH,
        DRAG,
        ZOOM,
        ROTATE,
        DECIDING,
        NONE
    }
}
