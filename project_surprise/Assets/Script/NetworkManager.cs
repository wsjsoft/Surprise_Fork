using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1.0"; //  게임버전 : 같은 버전끼리 매칭할 때 사용

    public Text connectionInfoText; // 네트워크 연결 정보를 표시할 텍스트
    public Button connectServerButton; // 서버 접속 버튼
    public Button connectRoomButton; // 서버 접속 버튼
    public InputField nicknameInput; // 닉네임 입력할 인풋필드
    public InputField roomNameInput; // 방이름 입력할 인풋필드
    public GameObject nicknameInputPanel;
    public GameObject joinRoomPanel;

    public GameObject room;
    public Transform gridTR;
    AudioSource audioSource;
    public AudioClip clickClip;

    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>(); // 룸 목록을 저장하기위한 딕셔너리 자료형(key, value)

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // 방장이 혼자 씬을 로딩하면, 나머지 사람들 자동으로 싱크됨

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        PhotonNetwork.GameVersion = gameVersion; // 접속에 필요한 게임버전 설정

        audioSource = FindObjectOfType<AudioSource>();
    }
    public void Connect()
    {
        audioSource.PlayOneShot(clickClip);

        if (string.IsNullOrEmpty(nicknameInput.text)) // 이름의 인풋필드가 비어있다면
        {
            nicknameInput.text = PlayerInfo.instance.playerName + $"_{Random.Range(1, 100):000}"; // 랜덤으로 이름 부여
        }
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;

        PhotonNetwork.LocalPlayer.CustomProperties.Add("닉네임", PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("준비완료", 0);

        PlayerPrefs.SetString("PlayerName", nicknameInput.text);

        PhotonNetwork.ConnectUsingSettings(); // 닉네임 입렧하고 Go!버튼 누르면 설정한 정보로 마스터서버 접속 시도

        connectionInfoText.text =  "구황작물 만드는 중..."; // 고구마 생성중...감자 무럭무럭 자라는 중...?
    }

    public override void OnConnectedToMaster() // 포톤 마스터 서버에 접속 성공하면 자동으로 실행됨
    {
        connectionInfoText.text = "온라인 : 구황작물 만들기 성공!"; // 감자or고구마 생성 완료!

        nicknameInputPanel.SetActive(false);
        joinRoomPanel.SetActive(true);

        PhotonNetwork.JoinLobby(); // 로비에 들어가야 룸리스트 콜백함수가 불린다~!!!!
    }

    // 마스터 서버 접속에 실패했거나, 이미 마스터 서버에 접속된 상태에서 어떠한 이유로 접속이 끊긴경우 자동 실행
    // 접속이 끊기면, 접속이 끊긴 상태를 표시라고 룸 접속 버튼을 비활성화. 그리고 마스터 서버로 재접속 시도
    public override void OnDisconnected(DisconnectCause cause)
    {
        connectServerButton.interactable = false;
        connectionInfoText.text = "오프라인 : 구황작물 만들기 실패!\n다시 만드는 중..."; // 고구마 생성 실패! 다시 만드는 중!
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속");
    }
    public void ConnectRandomRoom()
    {
        audioSource.PlayOneShot(clickClip);
        connectRoomButton.interactable = false; // 중복 접속 시도를 막기위해 잠시 비활성화

        if(PhotonNetwork.IsConnected) // 마스터 서버에 접속 중이라면(마스터 서버에 접속이 안 된 상태에서 접속시도예외를 막기위해)
        {
            //룸 접속 실행
            connectionInfoText.text = "농장으로 가는 중..."; // 농장으로 가는 중...
            PhotonNetwork.JoinRandomRoom();
        }
        else // 아니면 마스터 서버에 접속 시도
        {
            connectionInfoText.text = "오프라인 : 구황작물 만들기 실패!\n다시 만드는 중..."; // 고구마 생성 실패! 다시 만드는 중!
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // 랜덤 룸 접속 실패한 경우에 자동 실행. 마스터 서버와의 접속이 끊긴 것이 아님!!!
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "빈 농장이 없어요! 새로운 농장 탐색 중..."; // 빈 농장이 없어요! 새로운 농장 탐색 중...
        Invoke("CreateRoom", 1.5f);
    }

    public void CreateRoom() // 방생성
    {
        audioSource.PlayOneShot(clickClip);
        connectionInfoText.text = "새로운 농장 짓는 중...";

        RoomOptions option = new RoomOptions();
        option.IsOpen = true;
        option.IsVisible = true;
        option.MaxPlayers = 10;

        if (string.IsNullOrEmpty(roomNameInput.text)) // 방이름의 인풋필드가 비어있다면
        {
            roomNameInput.text = $"ROOM_{Random.Range(1, 100):000}"; // 랜덤으로 이름 부여
        }
        PhotonNetwork.CreateRoom(roomNameInput.text, option);
        //Invoke(GetCreateRoom(roomNameInput.text, option), 1.5f);
    }

    #region 방만들기완료
    public override void OnCreatedRoom() // 방생성완료되면 불림
    {
        Debug.Log("방생성완료");
    }
    #endregion
    // 로비에 접속했을 때 자동을 호출됨
    public override void OnRoomListUpdate(List<RoomInfo> roomList) // 대기 중인 방리스트 업데이트
    {
        Debug.Log("방리스트 업데이트");

        GameObject tempRoom = null;
        foreach (RoomInfo roomInfo in roomList)
        {
            if(roomInfo.RemovedFromList == true) // 룸이 삭제된 경우
            {
                roomDict.TryGetValue(roomInfo.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(roomInfo.Name);
            }
            else // 룸 정보가 갱신된 경우
            {
                if (roomDict.ContainsKey(roomInfo.Name) == false) // 룸이 처음 생성된 경우
                {
                    GameObject _room = Instantiate(room, gridTR); // 방목록 나타내는 패널생성
                    SetRoomInfo(_room, roomInfo); // 방정보 업데이트 후
                    roomDict.Add(roomInfo.Name, _room); // 방정보가담긴 딕셔너리에 넣어주기
                }
                else // 갱신된경우 (기존 방의 정보가 변경된 경우)
                {
                    roomDict.TryGetValue(roomInfo.Name, out tempRoom); // 룸이름에 맞는 값을 가져옴. 룸이름에 해당하는 방목록 오브젝트가져오기
                    SetRoomInfo(tempRoom, roomInfo); // 그리고 다시 정보 갱신
                }
            }
        }
    }

    void SetRoomInfo(GameObject rommD, RoomInfo roomI) // 방정보 세팅 후, 그 방 누르면 방으로 들어가는 함수 델리게이트 실행
    {
        RoomData roomData = rommD.GetComponent<RoomData>();
        roomData.roomName = roomI.Name;
        roomData.maxPlayer = roomI.MaxPlayers;
        roomData.playerCount = roomI.PlayerCount;
        roomData.UpdateInfo();
        roomData.GetComponent<Button>().onClick.AddListener(delegate { OnClickRoom(roomData.roomName); }); 
        // 익명 메서드 : 이름이 없고 메서드의 몸체(내용)만 있는 것. 이를 델리게이트에 사용가능.
        // 델리게이트에 미리 정의된 메서드를 저장하는 것이 아니라 '이름없는'메서드를 만들어서 전달하는 것!!!
        // => 메서드명 대신에 delegate키워드와 함께 익명 메서드의 형태를 넣으면 됨! delegate(매개변수) {내용;};
    }
    void OnClickRoom(string roomN) // 이미 있는 방에 들어갈 때, 클릭시 방이름으로 들어가기
    {
        PhotonNetwork.JoinRoom(roomN); // 방이름으로 들어가기
        Debug.Log(roomN);
    }
    public override void OnJoinedRoom() // 룸 참가에 성공한 경우 자동으로 실행됨(자기가 방을 CreateRoom()으로 만들어도), 여기서 게임준비 후 플레이씬으로 이동
    {
        Debug.Log("방입장완료");
        connectionInfoText.text = "농장 입성 성공!";
        Invoke("GetOnJoinedRoom", 1.5f);
        connectionInfoText.text = "농장으로 가는 중...";
    }

    void GetOnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("RoomScene");
    }

}


