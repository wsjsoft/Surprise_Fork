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

    [Header("PlayerList")]
    [SerializeField] Transform scrollContent;
    [SerializeField] GameObject playerList;

    Hashtable temp = new Hashtable();
    void Start()
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