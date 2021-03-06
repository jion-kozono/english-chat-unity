using ExitGames.Client.Photon;
using NCMB;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///
/// Unity 2019.1.11f1
///
/// Pun: 2.4
///
/// Photon lib: 4.1.2.4
///
/// </summary>

public class NetworkManager : MonoBehaviourPunCallbacks
{
    /////////////////////////////////////////////////////////////////////////////////////
    // Field ////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////

    [Header("DefaultRoomSettings")]

    // 最大人数
    [SerializeField] private int maxPlayers = 2;

    // 公開・非公開
    [SerializeField] private bool isVisible = true;

    // 入室の可否
    [SerializeField] private bool isOpen = true;

    // 部屋名
    // [SerializeField] private string roomName = "Knohhoso's Room";

    // ステージ
    [SerializeField] private string DisplayName;

    // 難易度
    [SerializeField] private string stageDifficulty = "Easy";

    // ルームリスト
    private List<RoomInfo> roomInfoList = new List<RoomInfo>();

    // 待機用
    // private WaitForSeconds loopWait = new WaitForSeconds(1.0f);
    // bool onJoinedRoom = false; //部屋に入ったかどうか
    /////////////////////////////////////////////////////////////////////////////////////
    // Awake & Start ////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////

    // Awake
    private void Awake()
    {
        // シーンの自動同期: 有効
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    private LoadingScene loadingScene;
    private void Start()
    {
        Connect("1.0");// Photonに接続
        GameObject go = GameObject.Find("LoadingScene");
        loadingScene = go.GetComponent<LoadingScene>();
    }
    private void Update()
    {
        if (NCMBUser.CurrentUser == null)
        {
            return;
        }
        else
        {
            // ニックネームを付ける
            SetMyNickName(NCMBUser.CurrentUser.UserName); //ログインしているプレーヤーをphotonのプレーヤーネームにする.id);
            DisplayName = PhotonNetwork.NickName + "の部屋";
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Connect //////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////

    // Photonに接続する

    private void Connect(string gameVersion)
    {
        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // ニックネームを付ける
    private void SetMyNickName(string nickName)
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = nickName;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Join Lobby ///////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////

    // ロビーに入る
    public void JoinLobby()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Join Room ////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////
    public void UnloadScene(string sceneNameData)//シーンをアンロード
    {
        SceneManager.UnloadSceneAsync(sceneNameData);
    }
    public void ToNextScene()//次のシーンに切替
    {
        loadingScene.LoadNextScene();
    }

    // 1. 部屋を作成して入室する
    public void CreateAndJoinRoom()
    {
        // ルームオプションの基本設定
        RoomOptions roomOptions = new RoomOptions
        {
            // 部屋の最大人数
            MaxPlayers = (byte)maxPlayers,

            // 公開
            IsVisible = isVisible,

            // 入室可
            IsOpen = isOpen
        };

        // ルームオプションにカスタムプロパティを設定
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "DisplayName", DisplayName },
            { "Difficulty", stageDifficulty }
        };
        roomOptions.CustomRoomProperties = customRoomProperties;

        // ロビーに公開するカスタムプロパティを指定
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "DisplayName", "Difficulty" };

        // 部屋を作成して入室する
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.CreateRoom(null, roomOptions);
        }
    }
    public void ClickCreateAndJoinRoom()//ボタンをクリックしてCreateAndJoinRoom()を呼ぶ
    {
        loadingScene.isToChat = true;
        CreateAndJoinRoom();
    }

    // 2. 部屋に入室する （存在しなければ作成して入室する）
    public void JoinOrCreateRoom()
    {
        // ルームオプションの基本設定
        RoomOptions roomOptions = new RoomOptions
        {
            // 部屋の最大人数
            MaxPlayers = (byte)maxPlayers,

            // 公開
            IsVisible = isVisible,

            // 入室可
            IsOpen = isOpen
        };

        // ルームオプションにカスタムプロパティを設定
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "DisplayName", DisplayName },
            { "Difficulty", stageDifficulty }
        };
        roomOptions.CustomRoomProperties = customRoomProperties;

        // ロビーに公開するカスタムプロパティを指定
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "DisplayName", "Difficulty" };

        // 入室 (存在しなければ部屋を作成して入室する)
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinOrCreateRoom(null, roomOptions, TypedLobby.Default);
        }
    }

    // 3. 特定の部屋に入室する
    public void JoinRoom(string targetRoomName)
    {
        if (PhotonNetwork.InLobby)
        {
            loadingScene.isToChat = true;
            PhotonNetwork.JoinRoom(targetRoomName);
        }
    }

    // 4. ランダムな部屋に入室する
    public void JoinRandomRoom()
    {
        if (PhotonNetwork.InLobby)
        {
            loadingScene.isToChat = true;
            PhotonNetwork.JoinRandomRoom();
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////
    // Leave Room ///////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////

    // 部屋から退室する
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            // 退室
            PhotonNetwork.LeaveRoom();
        }
    }
    public void ClickLeaveRoom()//ボタンをクリックしてLeaveRoom()を呼ぶ
    {
        loadingScene.isToLobby = true;
        LeaveRoom();
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Update RoomProperties ////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////

    // ルームオプションを更新する
    private void UpdateRoomOptions(int newMaxPlayers, bool newIsVisible, bool newIsOpen)
    {
        if (PhotonNetwork.InRoom)
        {
            // 最大人数
            PhotonNetwork.CurrentRoom.MaxPlayers = (byte)newMaxPlayers;
            // 公開・非公開
            PhotonNetwork.CurrentRoom.IsVisible = newIsVisible;
            // 入室の可否
            PhotonNetwork.CurrentRoom.IsOpen = newIsOpen;
        }
    }

    // カスタムプロパティを更新する
    private void UpdateRoomCustomProperties(string newDisplayName, string newStageDifficulty)
    {
        if (PhotonNetwork.InRoom)
        {
            // ステージと難易度を更新
            ExitGames.Client.Photon.Hashtable customRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            customRoomProperties["DisplayName"] = newDisplayName;
            customRoomProperties["Difficulty"] = newStageDifficulty;
            PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Find Room ////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////

    // 部屋を探す
    public void FindRoom()
    {
        if (roomInfoList != null)
        {
            // ルームリストを展開して目的の部屋を探す
            foreach (RoomInfo roomInfo in roomInfoList)
            {
                // ステージで絞り込み
                if (roomInfo.CustomProperties["DisplayName"] as string == DisplayName)
                {
                    // 難易度で絞り込み
                    if (roomInfo.CustomProperties["Difficulty"] as string == stageDifficulty)
                    {
                        // 条件に合致した部屋の情報を表示
                        Debug.Log("RoomName: " + roomInfo.Name);
                        Debug.Log("Slots: " + roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers);
                    }
                }
            }
        }
    }

    // 部屋を探して入室する
    public void FindAndJoinRoom()
    {
        if (roomInfoList != null)
        {
            // ルームリストを展開して目的の部屋を探す
            foreach (RoomInfo roomInfo in roomInfoList)
            {
                // ステージで絞り込み
                if (roomInfo.CustomProperties["Stage"] as string == DisplayName)
                {
                    // 難易度で絞り込み
                    if (roomInfo.CustomProperties["Difficulty"] as string == stageDifficulty)
                    {
                        // 満員でなければ部屋に入室する
                        if (roomInfo.PlayerCount < roomInfo.MaxPlayers)
                        {
                            JoinRoom(roomInfo.Name);
                        }
                    }
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Pun Callbacks ////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////

    // Photonに接続した時

    public override void OnConnected()
    {
        Debug.Log("OnConnected");
    }

    // Photonから切断された時
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected");
    }

    // マスターサーバーに接続した時
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        JoinLobby();// ロビーに入る
    }

    // ロビーに入った時
    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        // if (loadingScene.isToLobby)
        // {
        //     ToNextScene();
        //     UnloadScene("chat");
        // }
        // else
        // {
        //     return;
        // }
    }

    // ロビーから出た時
    public override void OnLeftLobby()
    {
        Debug.Log("OnLeftLobby");
    }

    // 部屋を作成した時
    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
    }

    // 部屋の作成に失敗した時
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        loadingScene.isToChat = false;
        Debug.Log("OnCreateRoomFailed");
    }

    // 部屋に入室した時
    public override void OnJoinedRoom()
    {
        if (loadingScene.isToChat)
        {
            ToNextScene();
            UnloadScene("Lobby");
        }
        else
        {
            return;
        }
        Debug.Log("OnJoinedRoom");
        // 部屋の情報を表示
        Debug.Log("RoomName: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("HostName: " + PhotonNetwork.MasterClient.NickName);
        Debug.Log("Stage: " + PhotonNetwork.CurrentRoom.CustomProperties["Stage"] as string);
        Debug.Log("Difficulty: " + PhotonNetwork.CurrentRoom.CustomProperties["Difficulty"] as string);
        Debug.Log("Slots: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers);
        return;
    }


    // 特定の部屋への入室に失敗した時
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed");
        loadingScene.isToChat = false;
        JoinLobby();
    }

    // ランダムな部屋への入室に失敗した時
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");
        loadingScene.isToChat = false;
        JoinLobby();
    }

    // 部屋から退室した時
    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        if (loadingScene.isToLobby)
        {
            ToNextScene();
            UnloadScene("chat");
        }
        else
        {
            return;
        }
    }

    // // 他のプレイヤーが入室してきた時
    // public override void OnPlayerEnteredRoom(Player newPlayer)
    // {
    //     Debug.Log("OnPlayerEnteredRoom");
    //     Debug.Log("Slots: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers);
    // }

    // // 他のプレイヤーが退室した時
    // public override void OnPlayerLeftRoom(Player otherPlayer)
    // {
    //     Debug.Log("OnPlayerLeftRoom");
    // }

    // マスタークライアントが変わった時
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("OnMasterClientSwitched");
    }

    // ロビーに更新があった時
    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        Debug.Log("OnLobbyStatisticsUpdate");
    }

    // ルームリストに更新があった時
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");

        // 既存の部屋リストをクリア
        if (roomInfoList != null)
        {
            roomInfoList.Clear();
        }
        // 新しいルームリストに更新
        roomInfoList = roomList;
    }

    // ルームプロパティが更新された時
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log("OnRoomPropertiesUpdate");

        // 部屋の情報を表示
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("RoomName: " + PhotonNetwork.CurrentRoom.Name);
            Debug.Log("HostName: " + PhotonNetwork.MasterClient.NickName);
            Debug.Log("Stage: " + PhotonNetwork.CurrentRoom.CustomProperties["Stage"] as string);
            Debug.Log("Difficulty: " + PhotonNetwork.CurrentRoom.CustomProperties["Difficulty"] as string);
            Debug.Log("Slots: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers);
        }
    }

    // プレイヤープロパティが更新された時
    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("OnPlayerPropertiesUpdate");
    }

    // フレンドリストに更新があった時
    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        Debug.Log("OnFriendListUpdate");
    }

    // 地域リストを受け取った時
    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("OnRegionListReceived");
    }

    // WebRpcのレスポンスがあった時
    public override void OnWebRpcResponse(OperationResponse response)
    {
        Debug.Log("OnWebRpcResponse");
    }

    // カスタム認証のレスポンスがあった時
    public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        Debug.Log("OnCustomAuthenticationResponse");
    }

    // カスタム認証が失敗した時
    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        Debug.Log("OnCustomAuthenticationFailed");
    }
}