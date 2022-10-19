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
    PlayerInput playerInput;

    //변경 변수
    CharacterController controller = null;
    VirtualJoystick virtualJoystick = null;
    Camera cam;

    Vector3 joystickDirection;
    Vector3 moveDirection;

    public float speed;
    public float speed_walk = 6f;
    public float speed_run = 20f;

    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    

    //공격
    [SerializeField] GameObject atkCooltimePanel;
    float atkCooltime = 3f;
    public GameObject attackFX;
    public Transform attackPos;
    public BoxCollider meleeArea;
    AudioSource audioSource;
    public AudioClip hitSound;

    //Run
    GameObject runCooltimePanel;
    float runTime = 0;
    float runMaxTime = 4f;
    float runCooltime = 3f;
    public ParticleSystem runFX;

    public bool isMove { get; private set; }
    public bool isReady { get; private set; }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (PhotonNetwork.IsConnected)
            pv.RPC("GetPlayerName", RpcTarget.All);

        if(photonView.IsMine)
        {
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            virtualJoystick = FindObjectOfType<VirtualJoystick>();
            cam = Camera.main;

            //공격
            atkCooltimePanel = GameObject.Find("AtkCoolTime_Panel");
            
            atkCooltimePanel.transform.parent.GetComponent<Button>().onClick.AddListener(() => Attack());
            //run
            runCooltimePanel = GameObject.Find("RunCoolTime_Panel");
            
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
                // Mathf.Rad2Deg : 라디안을 일반각도로 변환
                // cam 회전값이 기본인 상태(cam.eulerAngles.y)에서 플레이어 회전 인풋값(Mathf.Atan2(direction.x, direction.z))을 넣는다
                // AcrTan(x/y)를 의미 -> 유니티 좌표계는 y축 + 방향이 0도로 시계방향 좌표계를 쓰기 때문에 y/x가 아닌 x/y
                // Atan는 절대각을 -π/2 ~ π/2의 라디안 값으로 반환
                // Atan2는 두 점 사이의 상대좌표(x, y)를 받아 절대각을 -π ~ π의 라디안 값으로 반환 -> 데카르트 좌표에 유용. 음수를 허용

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                //현재각과 목표 회전값까지 사이의 자연스러운 회전을 위한 각도를 계속해서 계산
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                //targetangle 방향의 z축 값(앞 방향)만을 취한다

                Run();
            }
            else
            {
                isMove = false;//애니메이션을 위한 bool값

                moveDirection = Vector3.zero;
            }
            controller.SimpleMove(moveDirection.normalized * speed * Time.deltaTime); //controller.move와 다른 점은 Time.deltaTime를 곱해주지 않아도 됨. 또 지면 방향 설정을 해주면 중력은 자동 계산 해준다
        }
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        StartCoroutine("Swing");
        atkCooltimePanel.GetComponent<CoolTime>().SetCoolTime(atkCooltime);
        atkCooltimePanel.SetActive(true);
    }

    void Run()
    {
        if(playerInput.run)
        {
            if(1 < runTime / runMaxTime)
            {
                runFX.Play();
                playerInput.run = false;
                runTime = runMaxTime;
                runCooltimePanel.GetComponent<CoolTime>().SetCoolTime(runCooltime);
                runCooltimePanel.SetActive(true);
            }
            runTime += Time.deltaTime;
        }
        else if(!playerInput.run && runTime > 0)
        {
            if(runTime < 0)
            {
                runTime = 0f;
            }
            else
            {
                runTime -= Time.deltaTime;
            }
        }
        else runFX.Stop();
        speed = playerInput.run ? speed_run : speed_walk;
    }

    IEnumerator Die()
    {
        animator.SetTrigger("Die");

        yield return new WaitForSeconds(1.2f);
        PhotonNetwork.Destroy(gameObject);
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        audioSource.PlayOneShot(hitSound);

        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = false;
        Instantiate(attackFX, attackPos.position, attackPos.rotation);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Attack"))
        {
            StartCoroutine("Die");
        }
    }

}
