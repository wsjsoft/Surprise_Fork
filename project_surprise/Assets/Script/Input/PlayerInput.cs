using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable; // 유니티에서 제공하는 hashtable과 겹치기 때문에 필요!


// MonoBehaviourPun : photonView 프로퍼티를 통해 게임 오브젝트의 Photon View 컴포넌트에 접근을 위해 사용
public class PlayerInput : MonoBehaviourPun 
{ 
    public float move { get; private set; }
    public float rotate { get; private set; }
    public bool attack { get; set; }
    public bool run { get; set; }
    public bool ready { get; set; }

    //PhotonView pv;
    //Hashtable ht;


    public void Movement(Vector2 inputDirection)
    {
        Vector2 moveInput = inputDirection;
        move =  moveInput.x;
        rotate = moveInput.y;
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        //pv = GetComponent<PhotonView>();

        //PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "준비", 0 } }); // 일단 시작할 때는 0(준비면 1, 준비X면 1)
        //ht = PhotonNetwork.LocalPlayer.CustomProperties;

        //ready = false;

        //if (GameManager.instance != null && GameManager.instance.isGameOver)
        //{
        //    move = 0;
        //    rotate = 0;
        //    attack = false;
        //    run = false;
        //    return;
        //}
    }

    void Update()
    {
        // 현재 게임 오브젝트가 로컬 게임 오브젝트인 경우에만 true!!
        // 리모트플레이어(내 컴퓨터에 보이는 다른 플레이어)인 경우에는 return됨.
        if (!photonView.IsMine) return; 

        Keyboard();
    }
    void Keyboard()
    {
        move = Input.GetAxisRaw("Horizontal");
        rotate = Input.GetAxisRaw("Vertical");
    }


    //[PunRPC]
    //public void PushReadyButton()
    //{
    //    pv.RPC("ReadyButton", RpcTarget.All);
    //}

    //void ReadyButton()
    //{
    //    readyButton++;

    //    if (photonView.IsMine)
    //    {
    //        if (readyButton % 2 == 1)
    //        {
    //            readyText.text = "준비완료";
    //            ht.Add(PhotonNetwork.LocalPlayer.UserId, 1);
    //            Debug.Log(ht.Count);
    //        }
    //        else
    //        {
    //            readyText.text = "준비하기";
    //            ht.Remove(PhotonNetwork.LocalPlayer.UserId);
    //            Debug.Log(ht.Count);
    //        }
    //    }
    //    else
    //    {
    //        if (readyButton % 2 == 1)
    //        {
    //            readyText.text = "준비완료";
    //            ht.Add(pv.Owner.UserId, 1);
    //            Debug.Log(ht.Count);
    //        }
    //        else
    //        {
    //            readyText.text = "준비하기";
    //            ht.Remove(pv.Owner.UserId);
    //            Debug.Log(ht.Count);
    //        }
    //    }
    //}
    //void Ready()
    //{
    //    int readyCnt = 0;

    //    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
    //    {
    //        if (ht.ContainsKey(PhotonNetwork.PlayerList[i].UserId))
    //            readyCnt += 1;
    //    }
    //    Debug.Log("readyCnt : " + readyCnt);
    //}

    //public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) // 프로퍼티 변경되면 자동으로 호출됨.
    //{
    //    Ready();
    //}
}
