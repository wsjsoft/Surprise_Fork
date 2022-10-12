using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviourPunCallbacks
{
    public Text playerNameText;
    public Transform[] startPos;
    //public Camera mainCam;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        CreatePlayer();
    }
    void Update()
    {
        playerNameText.text = PhotonNetwork.LocalPlayer.NickName;
    }
    
    void CreatePlayer()
    {
        if (PlayerInfo.instance.GetCurrentPlayer().name == "°¨ÀÚ")
            PhotonNetwork.Instantiate("Potato", startPos[Random.Range(0, startPos.Length)].position, Quaternion.Euler(new Vector3(0, 180, 0)));
        else
            PhotonNetwork.Instantiate("SweetPotato", startPos[Random.Range(0, startPos.Length)].position, Quaternion.Euler(new Vector3(0, 180, 0)));
    }
}
