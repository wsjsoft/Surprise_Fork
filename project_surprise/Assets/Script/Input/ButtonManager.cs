using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable; // 유니티에서 제공하는 hashtable과 겹치기 때문에 필요!

public class ButtonManager : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    PlayerInput playerInput;
    public Text readyText;
    public Text playerList;
    public Transform gridTR;

    Text playerStatus = null;
    int readyButton = 0;
    int readyCnt = 0;
    bool isReady = false;
    public Text[] playerNameTemp;

    Dictionary<string, Text> playerDic = new Dictionary<string, Text>();

    [Header("Run")]
    [SerializeField] Button runButton;

    [Header("PlayerList")]
    [SerializeField] Transform scrollContent;
    [SerializeField] GameObject playerList;

    Hashtable temp = new Hashtable();
    Hashtable playerName = new Hashtable();

    void Awake()
    {
        Debug.Log("씬 전환");
        playerInput = FindObjectOfType<PlayerInput>();
        temp.Add("준비완료", 0);
        Debug.Log("닉네임" + PhotonNetwork.LocalPlayer.NickName);
        
        PlayerState();
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
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("GameScene");

        ++readyButton;
        if (readyButton % 2 == 1)
        {
            readyText.text = "준비완료!";
            temp["준비완료"] = 1;
        }
        else
        {
            readyText.text = "준비하려면 눌러주세요!";
            temp["준비완료"] = 0;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(temp);
    }
    void ReadyStatusRenew()
    {
        Debug.Log((string)PhotonNetwork.LocalPlayer.CustomProperties["닉네임"]);
        readyCnt = 0;
        Debug.Log("PlayerLength : " + PhotonNetwork.PlayerList.Length);
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            readyCnt += (int)PhotonNetwork.PlayerList[i].CustomProperties["준비완료"];
        }
        Debug.Log("readyCnt : " + readyCnt);
        if (PhotonNetwork.IsMasterClient)
        {
            if (!isReady) // 방장 준비버튼이 바로 활성화 되는 걸 막으려고 넣음
            {
                readyCnt -= 1;
                isReady = true;
            }
            if ((readyCnt == PhotonNetwork.CurrentRoom.PlayerCount) && isReady ) // 방장의 레디카운트가 방장빼고 다른 플레어어 수와 같으면
            {
                readyText.gameObject.SetActive(true); // 방장의 준비버튼 활성화
                readyText.text = "게임을 시작하려면 눌러주세요!";
            }
            else if((readyCnt != PhotonNetwork.CurrentRoom.PlayerCount) && isReady) // 모두 레디가되서 방장의 준비버튼이 활성화榮쨉 도중에 다른 플레이어 입장 시 다시 준비버튼 꺼주기
            {
                readyText.gameObject.SetActive(false); // 방장의 준비버튼 활성화
            }
        }
    }

    //[PunRPC]
    void PlayerState()
    {
        Transform[] childList = scrollContent.GetComponentsInChildren<Transform>();
        Debug.Log("자식오브젝트 " + childList.Length);
        if(childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                if(childList[i] != scrollContent)
                {
                    Destroy(childList[i].gameObject);
                    Debug.Log("list 삭제");
                }
            }
        }

        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            GameObject list = Instantiate(playerList, scrollContent);
            Debug.Log("list 생성");
            Text playerName = list.transform.GetChild(0).GetComponent<Text>();
            Text ready = list.transform.GetChild(1).GetComponent<Text>();

            playerName.text = (string)PhotonNetwork.PlayerList[i].CustomProperties["닉네임"];
            Debug.Log((string)PhotonNetwork.PlayerList[i].CustomProperties["닉네임"]);
            
            bool isReady = 1 == (int)PhotonNetwork.PlayerList[i].CustomProperties["준비완료"];
            ready.text = isReady ? "Ready" : "not Ready";
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) // 프로퍼티 변경되면 자동으로 호출됨.
    {
        Debug.Log("플레이어 상태 업데이트");
        PlayerState();
        ReadyStatusRenew();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("플레이어 입장");
        PlayerState();
        ReadyStatusRenew();

    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("플레이어 퇴장");
        PlayerState();
        ReadyStatusRenew();
    }
}