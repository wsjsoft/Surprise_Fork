using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cinemachine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    PlayerInput playerInput;

    public TextMesh playerName;
    Animator animator;
    Rigidbody rb;

    float moveSpeed = 1f;

    //변경 변수
    CharacterController controller = null;
    VirtualJoystick virtualJoystick = null;
    Transform cam = null;

    Vector3 joystickDirection;
    Vector3 moveDirection;

    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    //
    public bool isMove { get; private set; }
    public bool isReady { get; private set; }

    void Awake()
    {
        if(PhotonNetwork.IsConnected)
            pv.RPC("GetPlayerName", RpcTarget.All);
        //변경전
        /*playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();*/
        //변경후
        if(photonView.IsMine)
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            virtualJoystick = FindObjectOfType<VirtualJoystick>();
            cam = FindObjectOfType<CinemachineFreeLook>().transform;
        }

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
        //변경전
        /*if (playerInput.move != 0 || playerInput.rotate !=0)
        {
            isMove = true;
            Vector3 direction = playerInput.move * Vector3.right + playerInput.rotate * Vector3.forward; // 방향정하기
            transform.forward = direction;
            rb.velocity = transform.forward * moveSpeed *(playerInput.run ? 8f : 5f);
        }
        else isMove = false;*/

        //변경 후
        
        if(virtualJoystick != null)
        {
            joystickDirection = virtualJoystick.Dir;

            if(joystickDirection.magnitude>=0.1f)
            {
                float targetAngle = cam.eulerAngles.y + Mathf.Atan2(joystickDirection.x, joystickDirection.z) * Mathf.Rad2Deg;

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothTime, turnSmoothVelocity);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                isMove = true;
            }
            else
            {
                moveDirection = Vector3.zero;
                isMove = false;
            }

            controller.SimpleMove(moveDirection.normalized * speed);
        }
    }
}
