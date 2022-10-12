using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    PlayerInput playerInput;

    public TextMesh playerName;
    Animator animator;
    Rigidbody rb;

    float moveSpeed = 1f;

    //변경 변수
    public CharacterController controller;
    public Transform cam;

    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;

    public bool isMove { get; private set; }
    public bool isReady { get; private set; }

    void Awake()
    {
        if(PhotonNetwork.IsConnected)
            pv.RPC("GetPlayerName", RpcTarget.All);

        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        Move();
        animator.SetBool("Walk", isMove);
    }

    [PunRPC]
    void GetPlayerName()
    {
        if (photonView.IsMine)
            playerName.text = PhotonNetwork.LocalPlayer.NickName;
        else
            playerName.text = pv.Owner.NickName;
    }

    void Move()
    {
        //float ms = moveSpeed * (playerInput.run ? 8f : 4f);
        if (playerInput.move != 0 || playerInput.rotate !=0)
        {
            isMove = true;
            Vector3 direction = playerInput.move * Vector3.right + playerInput.rotate * Vector3.forward; // 방향정하기
            transform.forward = direction;
            rb.velocity = transform.forward * moveSpeed *(playerInput.run ? 8f : 5f);
            //transform.position += direction * moveSpeed * (playerInput.run ? 8f : 4f) * Time.deltaTime;
        }
        else isMove = false;
    }
}
