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
    public TextMesh playerName;
    Animator animator;

    //변경 변수
    CharacterController controller = null;
    VirtualJoystick virtualJoystick = null;
    Camera cam;

    Vector3 joystickDirection;
    Vector3 moveDirection;

    public float speed = 12f;

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
            cam = Camera.main;
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
        if (virtualJoystick != null)
        {
            joystickDirection = virtualJoystick.Dir;

            if (joystickDirection.magnitude >= 0.1f)
            {
                isMove = true; // 애니메이션을 위한 bool값

                float targetAngle = Mathf.Atan2(joystickDirection.x, joystickDirection.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                // Mathf.Atan2(x,y) : x와 y의 값으로 아크탄젠트함수를 이용해 연산해 결과를 라디안값으로 반환
                // Mathf.Rad2Deg : 일반 각도를 라디안값으로 변환
                // cam 회전값이 기본인 상태(cam.eulerAngles.y)에서 플레이어 회전 인풋값(Mathf.Atan2(direction.x, direction.z))을 넣는다
                // AcrTan(x/y)를 의미 -> 유니티 좌표계는 y축 + 방향이 0도로 시계방향 좌표계를 쓰기 때문에 y/x가 아닌 x/y
                // Atan는 절대각을 -π/2 ~ π/2의 라디안 값으로 반환
                // Atan2는 두 점 사이의 상대좌표(x, y)를 받아 절대각을 -π ~ π의 라디안 값으로 반환 -> 데카르트 좌표에 유용. 음수를 허용

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                //현재각과 목표 회전값까지 사이의 자연스러운 회전을 위한 각도를 계속해서 계산
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                //targetangle 방향의 z축 값(앞 방향)만을 취한다

                controller.SimpleMove(moveDirection.normalized * speed * Time.deltaTime); //controller.move와 다른 점은 Time.deltaTime를 곱해주지 않아도 됨. 또 지면 방향 설정을 해주면 중력은 자동 계산 해준다
            }
            else
            {
                isMove = false;//애니메이션을 위한 bool값

                moveDirection = Vector3.zero;
            }

            controller.SimpleMove(moveDirection.normalized * speed * Time.deltaTime); //controller.move와 다른 점은 Time.deltaTime를 곱해주지 않아도 됨. 또 지면 방향 설정을 해주면 중력은 자동 계산 해준다
        }
    }
}
