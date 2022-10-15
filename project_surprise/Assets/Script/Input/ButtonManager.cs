using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable; // 유니티에서 제공하는 hashtable과 겹치기 때문에 필요!

public class ButtonManager : MonoBehaviourPunCallbacks//,IPunObservable
{
    PhotonView pv;
    PlayerInput playerInput;
    public Text readyText;
    int readyButton = 0;
    int readyCnt = 0;

    [Header("Run")]
    [SerializeField] Button runButton;

    Hashtable temp = new Hashtable();
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        temp.Add("준비완료", 0);


        //PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "준비", num } }); 
        //ht = PhotonNetwork.LocalPlayer.CustomProperties;

    }

    public void RunButtonDown()
    {
        playerInput.run = true; // PlayerInput의 Update에서 매번 검사하지않아도되서 여기서 넣는게 나은거같기도,,
        Debug.Log("버튼Down 눌림");
    }
    public void RunButtonUp()
    {
        playerInput.run = false;
        Debug.Log("버튼Up 눌림");
    }
    public void AttackButtonDown()
    {
        playerInput.attack = true;
        Debug.Log("버튼Down 눌림");
    }
    public void AttackButtonUp()
    {
        playerInput.attack = false;
        Debug.Log("버튼Up 눌림");
    }
    public void ReadyButton()
    {
        ++readyButton;
        if (readyButton % 2 == 1)
        {
            readyText.text = "준비완료";
            temp["준비완료"] = 1;
        }
        else
        {
            readyText.text = "준비하기";
            temp["준비완료"] = 0;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(temp);
    }
    void ReadyStatusRenew()
    {
        readyCnt = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log("PlayerLength : " + PhotonNetwork.PlayerList.Length);
            readyCnt += (int)PhotonNetwork.PlayerList[i].CustomProperties["준비완료"];
        }

        if(PhotonNetwork.IsMasterClient)
        {
            if(readyCnt == PhotonNetwork.PlayerList.Length)
                PhotonNetwork.LoadLevel("GameScene");
        }
        Debug.Log("readyCnt : " + readyCnt);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) // 프로퍼티 변경되면 자동으로 호출됨.
    {
        if(targetPlayer == PhotonNetwork.LocalPlayer)
        {
            if(changedProps != null)
            {
                ReadyStatusRenew();
            }
        }

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
           ReadyStatusRenew();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
           ReadyStatusRenew();
    }
}