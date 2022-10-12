using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// MonoBehaviourPun : photonView 프로퍼티를 통해 게임 오브젝트의 Photon View 컴포넌트에 접근을 위해 사용
public class PlayerInput : MonoBehaviourPun //IPunObservable
{ 
    public float move { get; private set; }
    public float rotate { get; private set; }
    public bool attack { get; set; }
    public bool run { get; set; }
    public bool ready { get; set; }

    

    public void Movement(Vector2 inputDirection)
    {
        Vector2 moveInput = inputDirection;
        move =  moveInput.x;
        rotate = moveInput.y;
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        ready = false;

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
        GameReady();
    }
    void Keyboard()
    {
        move = -1*Input.GetAxisRaw("Horizontal");
        rotate = -1*Input.GetAxisRaw("Vertical");
    }

    //[PunRPC]
    void GameReady()
    {
        if(PhotonNetwork.IsMasterClient && ready)
            PhotonNetwork.LoadLevel("GameScene");
    }

}
