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

    Dictionary<string, Text> playerDic = new Dictionary<string, Text>();

    Hashtable temp = new Hashtable();
    Hashtable playerName = new Hashtable();

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();

        playerName.Add("닉네임", PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerName);

        if (PhotonNetwork.IsMasterClient)
        {
            temp.Add("준비완료", 1);
            readyText.gameObject.SetActive(false);
        }
        else
        {
            temp.Add("준비완료", 0);
            readyText.gameObject.SetActive(true);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(temp);
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
            else if((readyCnt != PhotonNetwork.CurrentRoom.PlayerCount) && isReady) // 모두 레디가되서 방장의 준비버튼이 활성화됬는데 도중에 다른 플레이어 입장 시 다시 준비버튼 꺼주기
            {
                readyText.gameObject.SetActive(false); // 방장의 준비버튼 활성화
            }
        }
    }

    //[PunRPC]
    void PlayerStatus(Photon.Realtime.Player p)
    {
        Debug.Log("플레이어 리스트 생성");
        int tmp = (int)p.CustomProperties["준비완료"];
        string readyStatus = new string("");
        readyStatus = tmp == 1 ? "준비완료" : "아직 준비 중..";

        List<string> playerNameList = new List<string>();

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playerNameList.Add((string)PhotonNetwork.PlayerList[i].CustomProperties["닉네임"]);

        }

        Text tempPlayerStatus = null;
        if (!playerNameList.Contains(p.NickName)) // 플레이어가 나간 경우
        {
            playerDic.TryGetValue(p.NickName, out tempPlayerStatus);
            Destroy(tempPlayerStatus);
        }
        else // 플레이어 정보가 갱신된 경우
        {
            if (playerDic.ContainsKey(p.NickName) == false) // 플레이어가 새로 들어온 경우
            {
                Text playerStatus = Instantiate(playerList, gridTR);

                if (p.IsMasterClient) playerStatus.text = "<color=red>[방장]</color>" + p.NickName + "님 " + "<color=red>" + readyStatus + "</color>";
                else playerStatus.text = p.NickName + "님 " + "<color=red>" + readyStatus + "</color>";

                playerDic.Add(p.NickName, playerStatus);
            }
            else // 기존의 상태 정보가 갱신된 경우
            {
                playerDic.TryGetValue(p.NickName, out tempPlayerStatus);

                if (p.IsMasterClient) playerStatus.text = "<color=red>[방장]</color>" + p.NickName + "님 " + "<color=red>" + readyStatus + "</color>";
                else tempPlayerStatus.text = p.NickName + "님 " + "<color=red>" + readyStatus + "</color>";
            }
        }
    }


    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) // 프로퍼티 변경되면 자동으로 호출됨.
    {
        ReadyStatusRenew();
        PlayerStatus(targetPlayer);
        //photonView.RPC("PlayerStatus", RpcTarget.All, targetPlayer);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        ReadyStatusRenew();
        PlayerStatus(newPlayer);
        //photonView.RPC("PlayerStatus", RpcTarget.All, newPlayer);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        ReadyStatusRenew();
        PlayerStatus(otherPlayer);
        //photonView.RPC("PlayerStatus", RpcTarget.All, otherPlayer);
    }
}