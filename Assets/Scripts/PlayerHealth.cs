using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class PlayerHealth : MonoBehaviourPun
{
    public Image fill;

    public PlayerScript PImove;
    public GameObject PlayerCanvas;

    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public SpriteRenderer sr;
    


    private void Awake()
    {
        if(photonView.IsMine)
        {
            Manager.Instance.LocalPlayer = this.gameObject;
        }
    }


    [PunRPC]
    public void ReduceHealth(float amount)
    {
        ModifyHealth(amount);
    }

    private void CheckHealth()
    {
       
        if (photonView.IsMine && fill.fillAmount <= 0)
        {
            Manager.Instance.EnableRespawn();
            PImove.DisableInput = true;
            this.GetComponent<PhotonView>().RPC("Die", RpcTarget.AllBuffered);
        }

    }
    private void ModifyHealth(float amount)
    {
        if (photonView.IsMine)
        {
           
            fill.fillAmount -= amount;
           
        }
        else
        {
           
            fill.fillAmount -= amount;
        }
        CheckHealth();
    }

    public void EnableInput()
    {
        PImove.DisableInput = false;
    }
    [PunRPC]
    private void Die()
    {
        rb.gravityScale= 0f;
        bc.enabled= false;
        sr.enabled= false;
        PlayerCanvas.SetActive(false);
    }
    [PunRPC]
    private void Respawn()
    {
        rb.gravityScale = 1f;
        bc.enabled = true;
        sr.enabled = true;
        PlayerCanvas.SetActive(true);
        fill.fillAmount = 1f;
        
    }

}
