using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerScript : MonoBehaviourPun, IPunObservable
{
    public PhotonView pv;

    public Text nameText;
    public float moveSpeed = 10;
    public float jumpforce = 800;

    private GameObject sceneCamera;
    public GameObject playerCamera;

    public SpriteRenderer spriteRenderer;

    private Vector3 smoothMove;

    private Rigidbody2D rigidbody;
    private bool isGrounded;

    public GameObject magicPrefab;
    public Transform magicSpawn;
    public Transform magicSpawnleft;

    public Animator anim;

    public bool DisableInput = false;


    void Start() 
    {

        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 15;
        if (pv.IsMine)
        {
            nameText.text = PhotonNetwork.NickName;

            rigidbody = GetComponent<Rigidbody2D>();
            sceneCamera = GameObject.Find("Main Camera");
            sceneCamera.SetActive(false);
            playerCamera.SetActive(true);
        }
        else
        {
            nameText.text = pv.Owner.NickName;
            nameText.color = Color.cyan;
        }
    }

    private void Update()
    {
        if(photonView.IsMine&&!DisableInput) 
        {
            ProcessInputs();
        }
        else
        {
            SmoothMovement();
        }
    }

    private void SmoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position , smoothMove,Time.deltaTime*10);
    }

    private void ProcessInputs()
    {
        var move = new Vector3(Input.GetAxisRaw("Horizontal"), 0);
        transform.position += move * moveSpeed * Time.deltaTime;

        if (transform.position.x <= -4.3) playerCamera.transform.position = new Vector2(-4.3f, playerCamera.transform.position.y);
        if (transform.position.x >= 16) playerCamera.transform.position = new Vector2(15f, playerCamera.transform.position.y);

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            spriteRenderer.flipX = false;
            
            pv.RPC("OnDirectionChange_RIGHT", RpcTarget.Others);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            spriteRenderer.flipX = true;
            
            pv.RPC("OnDirectionChange_LEFT", RpcTarget.Others);
        }
        if( Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            Jump();
            

        }
        if(Input.GetKeyDown(KeyCode.RightControl))
        {
            Shoot();
        }

        if(Input.GetKey(KeyCode.RightArrow) ||
            Input.GetKey(KeyCode.LeftArrow))
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        if ( Input.GetKey(KeyCode.UpArrow) && isGrounded)
        {
            anim.SetBool("isJumping", true);
        }

    }

    public void Shoot()
    {
       
        if (spriteRenderer.flipX)
        {
            GameObject magic = PhotonNetwork.Instantiate(magicPrefab.name, magicSpawnleft.position, Quaternion.identity);
            magic.GetComponent<PhotonView>().RPC("ChangeDirection", RpcTarget.AllBuffered);
        }
        else
        {
            GameObject magic = PhotonNetwork.Instantiate(magicPrefab.name, magicSpawn.position, Quaternion.identity);

        }

       

    }

    void Jump()
    {
        rigidbody.AddForce(Vector2.up * jumpforce);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (pv.IsMine)
        {
            if (col.gameObject.CompareTag("ground"))
            {
                isGrounded = true;
                anim.SetBool("isJumping", false);
            }
           
        }
    }
    void OnCollisionExit2D(Collision2D col)
    {
        if (pv.IsMine)
        {
            if (col.gameObject.CompareTag("ground"))
            {
                isGrounded = false;
            }
        }
    }
    

   

    [PunRPC] 
    void OnDirectionChange_LEFT()
    {
        spriteRenderer.flipX = true;
    }

    [PunRPC]
    void OnDirectionChange_RIGHT()
    {
        spriteRenderer.flipX = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else if(stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
        }
    }
}
