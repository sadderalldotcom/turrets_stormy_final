using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{

    [SerializeField] private float lookInterval = 0.1f;
    [Range(30,110)]
    [SerializeField] private float fieldOfView = 75;
    private Transform emitter;
    private GameObject[] badGuys;

    // Start is called before the first frame update
    void Start()
    {
        emitter = this.transform.GetChild(0);
        badGuys = GameObject.FindGameObjectsWithTag("ShultsCube");
        StartCoroutine(CheckForBadGuys());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CheckForBadGuys()
    {
        while(true)
        {
            yield return new WaitForSeconds(lookInterval);

            // check for bad guys.
            // draw a ray from the emitter to each bad guy.
            foreach(GameObject guy in badGuys)
            {
                Ray ray = new Ray(emitter.position, guy.transform.position - emitter.position);
                RaycastHit hit;
               if(Physics.Raycast(ray, out hit, 1000)) 
               {
                if(hit.transform.gameObject.CompareTag("ShultsCube"))
                {
                    // testing to see if the bad guy is in front of the camera.
                    Vector3 targetDir = guy.transform.position - emitter.position;
                    float angle = Vector3.Angle(targetDir, emitter.forward);

                    if(angle < fieldOfView)
                    {
                    Debug.Log("Found a bad guy!");
                    Debug.DrawRay(emitter.position, guy.transform.position - emitter.position, Color.green, 4);
                    }
                    else
                    {
                       Debug.DrawRay(emitter.position, guy.transform.position - emitter.position, Color.yellow, 4); 
                    }
                } 
                else
                {
                    Debug.DrawRay(emitter.position, guy.transform.position - emitter.position, Color.red, 4);
                }
               }
            }
        }
    }
}
