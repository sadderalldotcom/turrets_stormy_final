using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycasterItem : MonoBehaviour, IItem
{
    [Tooltip("How long to wait between increasing the size of the cube.")]
    [SerializeField] float interval = 0.025f;

    private Rigidbody rb;
    private Collider col;
    private Transform emitter;
    private bool selecting = false ;
    private LineRenderer Ir;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        col = this.GetComponent<Collider>();
        emitter = this.transform.GetChild(0);
        Ir = this.GetComponent<LineRenderer>();
        StartCoroutine(Select());
    }

    public void Pickup(Transform hand)
    {
        Debug.Log("Picking up Raycaster");
        // make kinematic rigidbody
        rb.isKinematic = true;
        // move to hand and match rotation
        this.transform.SetParent(hand);
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        // turn off collision so it doesn't push the player off the map
        col.enabled = false;
    }

    public void Drop()
    {
        Debug.Log("Dropping Flashlight");
        // make dynamic rigidbody
        rb.isKinematic = false;
        // throw it away from the player
        rb.AddRelativeForce(Vector3.forward * 10, ForceMode.Impulse);
        // set this parent to null
        this.transform.SetParent(null);
        col.enabled = true;
    }

    public void PrimaryAction()
    {
        Debug.Log("Casting a ray!");
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        // Debug.DrawRay(emitter.position, forward, Color.red, 2);

        // this will store the details about what we hit with our raycast.
        RaycastHit hit;

        // casting a real raycast!
        if(Physics.Raycast(emitter.position, forward, out hit, 599))
        {
            // if it DID hit something
            if(hit.distance < 2)
            {
                Debug.DrawRay(emitter.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.cyan, 2);
                Debug.Log("Within Arm's Reach!");
            }
            else
            {
                Debug.DrawRay(emitter.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.green, 2);
                // use the normal to create a cube on the surface of the object.
                Debug.DrawRay(hit.point, hit.normal, Color.blue, 100);
                if(hit.collider.gameObject.CompareTag("Cube"))
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.tag = "Cube";
                    StartCoroutine(CubeRise(cube, hit));
                    cube.transform.LookAt(hit.normal * 10);
                    cube.GetComponent<Renderer>().material.color = Random.ColorHSV();
                }
                else
                {
                    Debug.Log("Can't spawn cube there.");
                }
            }
        }
        else
        {
            // if it did NOT hit anything.
            Debug.DrawRay(emitter.position, transform.TransformDirection(Vector3.forward) * 100, Color.white, 2);
        }
    }

    public void SecondaryAction()
    {
        // spherecasting. It's just a raycast, but it has radius!
        selecting = !selecting;     // flip the boolean
    }

    private GameObject lastTouchedCube;

    IEnumerator Select()
    {
        while(true)
        {
            yield return new WaitForSeconds(.05f);
            if(selecting)
            {
            Vector3 endPoint = emitter.forward * 100;
                Debug.DrawRay(emitter.position, endPoint, Color.magenta, 1);
                Ir.SetPosition(0, emitter.position);
                Ir.SetPosition(1, endPoint);
            }

                 RaycastHit hit;
                if(Physics.SphereCast(emitter.position, 2, emitter.forward, out hit, 100))
                {
                    if(hit.transform.gameObject.CompareTag("Cube"))
                        {
                        if(lastTouchedCube == hit.transform.gameObject)
                        {
                            // we can do nothing because it is the same object we hit last time.
                        }
                        else
                        {
                            // reset the color of the previous item we had selected
                            if(lastTouchedCube) lastTouchedCube.GetComponent<Renderer>().material.color = Color.gray;
                            // change 'lastTouchedCub' to the cube we're Looking at now
                            lastTouchedCube = hit.transform.gameObject;
                            // change the color of this newly selected cube to red.
                            lastTouchedCube.GetComponent<Renderer>().material.color = Color.red;
                        }
                        hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.red;
                        } 
                        else
                            { 
                             // reset the color of the previous item we had selected
                            if(lastTouchedCube) lastTouchedCube.GetComponent<Renderer>().material.color = Color.gray;
                            // change 'lastTouchedCub' to the cube we're Looking at now
                            lastTouchedCube = null;
                            }
                }
            else
            {
                Ir.SetPosition(0, Vector3.zero);
                Ir.SetPosition(1, Vector3.zero);
                // reset the color of the previous item we had selected
                if(lastTouchedCube) lastTouchedCube.GetComponent<Renderer>().material.color = Color.gray;
                // change 'lastTouchedCub' to the cube we're Looking at now
                lastTouchedCube = null;
            }
        }
    }

    IEnumerator CubeRise(GameObject cube, RaycastHit hit)
    {
        float timer = 0;
        while(timer < 2)
        {
            timer += interval;
            cube.transform.localScale = new Vector3(1, 1, timer);
            
            yield return new WaitForSeconds(interval);
            cube.transform.position = hit.point + (hit.normal * (timer / 2f));
        }
    }
}
