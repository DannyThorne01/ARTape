using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Net;
using System.IO;

public class Place : MonoBehaviour
{
    public ARSessionOrigin ar_session_origin;
    private List<ARRaycastHit> raycast_Hit = new List<ARRaycastHit>();
    private float measurementFactor = 34.5f;
    public GameObject start;//represents the startObject Prefab
    public GameObject end;//represents the endObject Prefab
    private GameObject startPoint; //Initialised start Object
    private GameObject endPoint;//Initialised end Object
    private bool drag = false; //determines when to place initial sphere and when you want to drag
    private LineRenderer line;//renders the measuring line 
    public int intDistance;//stores the distance as an int and not a Vector3
    public Text distance;  //displays the distance between points 
    public Text fact; // displays the random fact about the distance



   
    void Start()
    {
        /*
        Start is called before the first frame update.
        This function intialises my startPoint, Endpoint and LineRenderer, but does not display
        them as yet 
        */
        
        startPoint = Instantiate(start,Vector3.zero,Quaternion.identity);
        endPoint = Instantiate(end,Vector3.zero,Quaternion.identity);
        line = ar_session_origin.GetComponent<LineRenderer>();

        startPoint.SetActive(false);
        endPoint.SetActive(false);
        line.enabled = false;

        fact.enabled = false;
    }

    void Update()
    {
        /*
        Update is called once per frame
        Waits user touch: either "First Touch" or "Drag Touch"
        First Touch: Activates the startPoint 
        Drag Touch: Activates the endPoint and the measureLine and displays the distance between the two points
        */
        if(drag == false)
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                bool collision = ar_session_origin.GetComponent<ARRaycastManager>().Raycast(Input.mousePosition,raycast_Hit, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);
            
                if (collision)
                {
                    startPoint.SetActive(true);
                    startPoint.transform.position = raycast_Hit[0].pose.position;
                    drag = true; 
                } 
            }
        }

        else if (Input.GetMouseButtonDown(0))
        {
            bool collision = ar_session_origin.GetComponent<ARRaycastManager>().Raycast(Input.mousePosition,raycast_Hit, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);
            if (collision)
            {
                endPoint.SetActive(true);
                endPoint.transform.position = raycast_Hit[0].pose.position;

            }
        }
        else if (Input.GetMouseButton(0))
        { 
            bool collision = ar_session_origin.GetComponent<ARRaycastManager>().Raycast(Input.mousePosition,raycast_Hit, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);
            endPoint.transform.position = raycast_Hit[0].pose.position;
            line.enabled =true; 
            line.SetPosition(0,startPoint.transform.position);
            line.SetPosition(1,endPoint.transform.position);
            distance.text = $"Distance: {((Vector3.Distance(startPoint.transform.position,  endPoint.transform.position) * measurementFactor)  .ToString("F2"))}";

            //tried to obtain distance as an int so I could pass it through the HTTP request but my intDistance is always zero
            //var successParse = int.TryParse(Vector3.Distance(startPoint.transform.position,  endPoint.transform.position).ToString("F2"), out intDistance);
        } 
    } 

    public void Reset(){
        /*
        When pressed it deactivates the startPoint, endPoint measureLine and Fact so the 
        user can take another measurement
        */
        startPoint.SetActive(false);
        endPoint.SetActive(false);
        line.enabled = false;
        drag = false; 
        fact.enabled = false; 
    }
    
    public Fact APIHelper(){
        /*
        An API helper function that retrieves the return json from the numbers api and 
        converts the json to a Fact class using JsonUtility
        */

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"http://numbersapi.com/random/year?json");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();
        return JsonUtility.FromJson<Fact>(json);
    }

    public void NewFact(){
        /*
        Displays the random fact when the Fact button is pressed
        */
        fact.enabled = true; 
        Fact f = APIHelper();
        fact.text = f.text;
    }
}


 

