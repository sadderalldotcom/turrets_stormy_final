using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private Transform emitter;
    [SerializeField] private Transform turretBody;
    [SerializeField] private Transform player;

    [Tooltip("Inactive and Active positions for the turret")]
    [SerializeField] private Transform inactive, active;

    [SerializeField] private Animator anim;

    [SerializeField] private bool canSeePlayer = false;

    [SerializeField] private GameObject laserPrefab;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Queue<Rigidbody> laserPool = new Queue<Rigidbody>();

    // Start is called before the first frame update
    void Start()
    {
       anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(canSeePlayer) turretBody.transform.LookAt(player);
    }

    public void Activate()
    {
        anim.SetTrigger("Activate");
        StartCoroutine(MoveIntoPosition());
        StartCoroutine(LookForPlayer());
    }

    IEnumerator LookForPlayer()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);

                Ray ray = new Ray(emitter.position, player.transform.position - emitter.position);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 100)) 
                {
                if(hit.transform.gameObject.CompareTag("Player"))
                {
                    // testing to see if the bad guy is in front of the camera.
                    Vector3 targetDir = player.transform.position - emitter.position;
                    float angle = Vector3.Angle(targetDir, emitter.forward);

                    if(angle < 45)
                    {
                    Debug.Log("Found a bad guy!");
                    // Start Shooting.
                    FoundPlayer();
                    Debug.DrawRay(emitter.position, player.transform.position - emitter.position, Color.green, 4);
                    }
                    else
                    {
                        LostPlayer();
                        Debug.DrawRay(emitter.position, player.transform.position - emitter.position, Color.yellow, 4); 
                    }
                } 
                else
                {
                    Debug.DrawRay(emitter.position, player.transform.position - emitter.position, Color.red, 4);
                    LostPlayer();
                }
                }
        }

        void FoundPlayer()
        {
            if(canSeePlayer == false)
            {
                anim.SetTrigger("Firing");
                startPosition = turretBody.transform.position;
                startRotation = turretBody.transform.rotation;
                canSeePlayer = true;
            }
        }
    }

    void LostPlayer()
    {
        if(canSeePlayer) 
        {
            anim.SetTrigger("Idle");
            canSeePlayer = false;
            turretBody.transform.position = startPosition;
            turretBody.transform.rotation = startRotation;
        }
    }
    
    void Shoot()
    {
        Debug.Log("Pow!");
        // GameObject laser = Instantiate(laserPrefab, emitter.position, emitter.rotation);
        Rigidbody rb;
        
        if(laserPool.Count > 0)
        {
            Debug.Log("using the laser pool.");
            rb = laserPool.Dequeue();
            rb.gameObject.SetActive(true);
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb = Instantiate(laserPrefab, emitter.position, emitter.rotation).GetComponent<Rigidbody>();
        }

        rb.AddRelativeForce(Vector3.forward * 100, ForceMode.Impulse);
        StartCoroutine(StoreLaser(rb));
    }
    
    IEnumerator MoveIntoPosition()
        {
            float t = 0;
            Transform turretBody = this.transform.GetChild(0);
            while(t < .1)
            {
                turretBody.position = Vector3.Lerp(inactive.position, active.position, t);
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

    IEnumerator StoreLaser(Rigidbody laser)
    {
        yield return new WaitForSeconds(0.5f);
        if(laser.gameObject.activeSelf == true)
        {
            laserPool.Enqueue(laser);
            laser.gameObject.SetActive(false);
            laser.transform.position = emitter.position;
            laser.transform.rotation = emitter.rotation;
        }
    }
}
