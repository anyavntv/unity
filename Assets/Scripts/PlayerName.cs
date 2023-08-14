using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerName : MonoBehaviour
{
    public InputField nametf;
    public Button startBTN;
    public GameObject usernamePanel, connectedScreen;

    public void OnTFSubmit()
    {
        if (nametf.text.Length > 3)
        {
            OnClick_SetName();
        }
    }

    public void OnTFChange()
    {
        if(nametf.text.Length >3)
        {
            startBTN.interactable = true;
            
        }
        else
        { 
            startBTN.interactable = false;
            
        }
    }

    public void OnClick_SetName()
    {
        usernamePanel.SetActive(false);
            PhotonNetwork.NickName = nametf.text;
        connectedScreen.SetActive(true);
    }

   
}
