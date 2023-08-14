using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using  Photon.Pun;


public class ChatManager : MonoBehaviourPun, IPunObservable
{
    public PhotonView pv;
    public GameObject bubbleSpeechObject;
    public Text updatedText;

    private InputField chatInputField;
    private bool disableSend;
    public PlayerScript PImove;
    private void Awake()
    {
        chatInputField = GameObject.Find("ChatInputField").GetComponent<InputField>();

    }

    private void Update()
    {
       if (pv.IsMine) 
        { 
            if(!disableSend&&chatInputField.isFocused)
            {
                if(chatInputField.text != ""&& chatInputField.text.Length> 0&&Input.GetKeyDown(KeyCode.RightAlt))
                {
                    pv.RPC("SendMessage", RpcTarget.AllBuffered, chatInputField.text);
                    bubbleSpeechObject.SetActive(true);

                    chatInputField.text = "";
                    disableSend = true;
                }
            }
        }
    }

    [PunRPC]

    private void SendMessage(string mes)
    {
        updatedText.text = mes;
        StartCoroutine("Remove");
    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(4f);
        bubbleSpeechObject.SetActive(false);
        disableSend= false;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(bubbleSpeechObject.active);
        }
        else if (stream.IsReading)
        {
            bubbleSpeechObject.SetActive((bool)stream.ReceiveNext());
        }
    }
}
