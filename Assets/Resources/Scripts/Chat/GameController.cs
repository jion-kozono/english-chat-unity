using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using NCMB;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class GameController : MonoBehaviourPunCallbacks
{
    private PhotonView phoView;
    [SerializeField] private TradeUserInfo tradeUserInfo;
    [SerializeField] private NetworkManager nManager;
    [SerializeField] private TurnManager turnManager;
    private bool isAll = false; //全員いるか
    private bool isOtherPlayerEntered = false;//他プレーヤーが入ったかどうか
    private bool isOtherPlayerLeft = false;//他プレーヤーが抜けたかどうか
    private bool isGameStart = false;//他プレーヤーが抜けたかどうか
    private bool isGameOver = false;//他プレーヤーが抜けたかどうか

    // Use this for initialization
    public void Awake()// StartをAwakeにする。
    {
        phoView = GetComponent<PhotonView>();
    }
    private GUIStyle style;
    void Start()
    {
        style = new GUIStyle();
        style.fontSize = 30;
        style.fontStyle = FontStyle.Normal;
    }
    void OnGUI()
    {
        if (isOtherPlayerEntered)
        {
            int txtX = Screen.width * 1 / 11, txtY = 15, txtW = Screen.width * 3 / 5, txtH = 40;
            GUI.Label(new Rect(txtX, Screen.height * 4 / txtY - txtH * 1 / 2, txtW, txtH), "Other Player Entered!");
        }
        if (isOtherPlayerLeft)
        {
            int txtX = Screen.width * 1 / 11, txtY = 15, txtW = Screen.width * 3 / 5, txtH = 40;
            GUI.Label(new Rect(txtX, Screen.height * 4 / txtY - txtH * 1 / 2, txtW, txtH), "Other Player Left!");
        }
        if (isGameStart)
        {
            style.fontSize = 40;
            style.alignment = TextAnchor.MiddleCenter;
            int txtX = Screen.width * 1 / 11, txtY = 15, txtW = Screen.width * 3 / 5, txtH = 50;
            GUI.Label(new Rect(txtX, Screen.height * 4 / txtY - txtH * 1 / 2, txtW, txtH), "GameStart!");
        }
        if (isGameOver)
        {
            style.fontSize = 40;
            style.alignment = TextAnchor.MiddleCenter;
            int txtX = Screen.width * 1 / 11, txtY = 15, txtW = Screen.width * 3 / 5, txtH = 50;
            GUI.Label(new Rect(txtX, Screen.height * 4 / txtY - txtH * 1 / 2, txtW, txtH), "GameOver!");
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) // 他のプレイヤーが入室してきた時
    {
        Debug.Log("OnPlayerEnteredRoom");
        // Debug.Log("Slots: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers);
        phoView.RPC("RPC_IsAll", RpcTarget.All); //全プレーヤーがいる
        isOtherPlayerEntered = true;//他プレーヤーが入ったことを表示
        Invoke("DestroyOtherPlayerEntereOrLeft", 2.5f); //DestroyOtherPlayerEntereOrLeftを2.5秒後に呼び出す
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)  // 他のプレイヤーが退室した時
    {
        Debug.Log("OnPlayerLeftRoom");
        phoView.RPC("RPC_NotIsAll", RpcTarget.All); //全プレーヤーがいない
        isOtherPlayerLeft = true;//他プレーヤーが抜けたことを表示
        Invoke("DestroyOtherPlayerEntereOrLeft", 2.5f); //DestroyOtherPlayerEntereOrLeftを2.5秒後に呼び出す
    }
    [PunRPC]
    public void RPC_IsAll()
    {
        isAll = true; //全員いる
        Debug.Log("SendMyData();");
        tradeUserInfo.SendMyData();
    }
    [PunRPC]
    public void RPC_NotIsAll()
    {
        isAll = false; //全員いない
        tradeUserInfo.HideOpponentUserWindow();//相手のボタンを隠す
    }
    private void DestroyOtherPlayerEntereOrLeft() //プレーヤーが入った、抜けた時に表示するテキストを壊す
    {
        if (!isAll)
        {
            isOtherPlayerLeft = false;//他プレーヤーが抜けたことの表示を消す
            ReGame(); //新しくゲームを作る
        }
        else
        {
            isOtherPlayerEntered = false;//他プレーヤーが入ったことの表示を消す
        }
    }
    public void GameStart()//相手に情報が渡ったらマスターがゲーム開始
    {
        phoView.RPC("RPC_GameStart", RpcTarget.All); //全プレーヤーにGameStart!を表示
    }
    [PunRPC]
    public void RPC_GameStart()
    {
        isGameStart = true;//"GameStart！"を表示
        Invoke("NullGameStartText", 2.5f); //DestroyGameStartを2.5秒後に呼び出す
    }
    private void NullGameStartText()
    {
        isGameStart = false;//"GameStart！"をnullにする
        turnManager.StartTurn();//ターンを開始する
    }
    public void GameOver()
    {
        isGameOver = true;//"GameOver！"を表示
        Invoke("NullGameOver", 2.5f); //DestroyGameOverを2.5秒後に呼び出す
    }
    private void NullGameOver()
    {
        isGameOver = false;//"GameOver！"をnullにする
        Invoke("LeaveRoomInGameOver", 1.5f); //DestroyGameOverを2.5秒後に呼び出す
    }
    private void LeaveRoomInGameOver() //"GameOver！"テキストをnullにする
    {
        PhotonNetwork.LeaveRoom(); //両プレーヤーを退出させる（ロビーに行く)
        // SceneManager.LoadScene("Lobby");
    }
    private void ReGame()
    {
        nManager.ToLobby = false;
        PhotonNetwork.LeaveRoom(); //ロビーに行かない
        Invoke("CreateRoom", 2.5f); //CreateRoomを2.5秒後に呼び出す
    }
    public void CreateRoom()
    {
        SceneManager.LoadScene("Chat"); //シーンをリロードする
        nManager.CreateAndJoinRoom();
    }
}
