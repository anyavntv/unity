using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Magic : MonoBehaviourPun
{
    public float speed = 9f;
    public float destroyTime = 2f;
    public bool shootLeft = false;
    public SpriteRenderer sp;
    public float damage = 0.1f;
    

    IEnumerator destroyMagic()
    {
        yield return new WaitForSeconds(destroyTime);
        this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered);

    }
    //tart is called before the first frame update
     void Start()
    {
        StartCoroutine(destroyMagic());
    }

    // Update is called once per frame
    void Update()
    {
        if (!shootLeft)
        {
            transform.Translate(Vector2.right * Time.deltaTime * speed);
            
        }
        else
        {
            transform.Translate(Vector2.left * Time.deltaTime * speed);
            
        }
    }
    
    [PunRPC]
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
    [PunRPC]
    public void ChangeDirection()
    {
        shootLeft= true;
        sp.flipX = true;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!photonView.IsMine) return;
        else
        {
            PhotonView target = col.gameObject.GetComponent<PhotonView>();
            if(target != null&& (!target.IsMine|| target.IsRoomView)) 
            {
                if (target.gameObject.CompareTag("Player"))
                {
                    target.RPC("ReduceHealth", RpcTarget.AllBuffered, damage);
                }
                this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered);
            }
        }
    }

}
