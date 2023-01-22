using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move Variables")]
    public float moveSpeed;
    
    [Header("Other Variables")]
    private Rigidbody rb;
    public Animator anim;
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform groundCheck;
    [SerializeField] LayerMask ground;
    public float rotateSpeed;
    public GameObject model;

    [Header("Jump Variables")]
    public float jumpForce;
    private int jumpCount;
    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;

    [Header("Item Variables")]
    public Transform spawnPoint;
    public GameObject bomb;
    public float launchSpeed = 3f;
    public static int nBombs = 3;
    public GameObject jetpackModel;
    private bool jetpackMode = false;
    public static int jetpackForce = 1000;

    void Start()
    {
       rb = GetComponent<Rigidbody>();
       groundCheck = GameObject.Find("GroundCheck").transform;
       pivot = GameObject.Find("Main Camera/Pivot").transform;
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //rb.velocity = new Vector3(horizontal, rb.velocity.y, vertical);

        float yStore = rb.velocity.y;
        Vector3 velocity = (transform.forward * vertical) + (transform.right * horizontal);
        velocity = velocity.normalized * moveSpeed;
        velocity.y = yStore;
        rb.velocity = velocity;
            
        // Move the player in different directions based on camera look direction
        if(horizontal != 0 || vertical != 0) {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(rb.velocity.x, 0f, rb.velocity.z));
            model.transform.rotation = Quaternion.Slerp(model.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }

        anim.SetBool("isGrounded", isGrounded());
        anim.SetFloat("speed", Mathf.Abs(vertical) + Mathf.Abs(horizontal));

        if(isGrounded()) jumpCount = 0;
    }
    
    void Update() {
        if(!jetpackMode) {
            jetpackModel.SetActive(false);
            if(Input.GetButtonDown("Jump") && jumpCount < 1) {
                //anim.SetTrigger("jump");
                jumpCount += 1;
                anim.Play("Jump");
                isJumping = true;
                jumpTimeCounter = jumpTime;
                rb.velocity = Vector3.up * jumpForce;
            }

            if(Input.GetButton("Jump") && isJumping) {
                if(jumpTimeCounter > 0) {
                    rb.velocity = Vector3.up * jumpForce;
                    jumpTimeCounter -= Time.deltaTime;
                } else isJumping = false;
            }
        
            if(Input.GetButtonUp("Jump")) isJumping = false;
        } else {
            jetpackModel.SetActive(true);
            if(Input.GetButton("Jump")) {
                if(jetpackForce > 0) {
                    rb.velocity = Vector3.up * jumpForce;
                    jetpackForce -= 1;
                } else {
                    jumpCount = 1;
                    jetpackMode = false;
                }
            }
        }
        if(Input.GetButtonDown("Fire1") && nBombs > 0) Launch();
    }

    bool isGrounded() {
        return Physics.CheckSphere(groundCheck.position, .1f, ground);
    }

    void Launch() {
        spawnPoint.eulerAngles = model.transform.eulerAngles;
        GameObject bombInstance = Instantiate(bomb, spawnPoint.position, spawnPoint.rotation);
        bombInstance.GetComponent<Rigidbody>().AddForce(spawnPoint.forward * launchSpeed, ForceMode.Impulse);
        nBombs--;
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Item") {
            Item i = other.gameObject.GetComponent<Item>();
            switch(i.id) {
                case 0:
                    GameController.StartCounter();
                    break;
                case 1:
                    nBombs += Random.Range(1, 3);
                    break;
                case 2:
                    StartJetpack();
                    break;
                default:
                    break;
            }
        }
    }

    void StartJetpack() {
        jetpackMode = true;
        jetpackForce = 1000;
    }
}