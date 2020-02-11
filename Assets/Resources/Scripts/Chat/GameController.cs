using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class GameController : MonoBehaviourPunCallbacks
{
    private PhotonView phoView;
    [SerializeField] private TradeUserInfo tradeUserInfo;
    [SerializeField] private GameObject StartBottun;
    [SerializeField] private Text dtawingText;
    [SerializeField] private NetworkManager nManager;
    // [SerializeField] private TurnManager turnManager;
    [SerializeField] private SendScore sendScore;
    [SerializeField] private Theme theme;
    public bool isStart; //スタートしたか
    private bool isOver;
    private bool isOtherPlayerEntered;//他プレーヤーが入ったかどうか
    private bool isOtherPlayerLeft;//他プレーヤーが抜けたかどうか

    // Use this for initialization
    public void Awake()// StartをAwakeにする。
    {
        phoView = GetComponent<PhotonView>();
    }
    // private GUIStyle style;
    private LoadingScene loadingScene;
    void Start()
    {
        GameObject go = GameObject.Find("LoadingScene");
        loadingScene = go.GetComponent<LoadingScene>();
        if (!PhotonNetwork.IsMasterClient)
        {
            phoView.RPC("RPC_IsAll", RpcTarget.AllViaServer);
            DrawWatingStart();
        }
    }
    private void DrawWatingStart()
    {
        dtawingText.text = "Waitng For Master to Start...";
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) // 他のプレイヤーが入室してきた時
    {
        Debug.Log("OnPlayerEnteredRoom");
        dtawingText.text = "Other Player Entered!";
        // Debug.Log("Slots: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers);
        isOtherPlayerEntered = true;//他プレーヤーが入ったことを表示
        Invoke("DestroyOtherPlayerEntereOrLeft", 1f); //DestroyOtherPlayerEntereOrLeftを2.5秒後に呼び出す
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)  // 他のプレイヤーが退室した時
    {
        Debug.Log("OnPlayerLeftRoom");
        dtawingText.text = "Other Player Left!";
        isOtherPlayerLeft = true;//他プレーヤーが抜けたことを表示
        tradeUserInfo.HideOpponentUserWindow();//相手のボタンを隠す
        Invoke("DestroyOtherPlayerEntereOrLeft", 1f); //DestroyOtherPlayerEntereOrLeftを0.5秒後に呼び出す
    }
    [PunRPC]
    public void RPC_IsAll()//全員が揃ったら呼ばれる
    {
        Invoke("CallSendMyData", 1.2f); //DestroyOtherPlayerEntereOrLeftを0.5秒後に呼び出す
        if (PhotonNetwork.IsMasterClient)//マステーだけがよぶ
        {
            theme.BringTheme();//テーマを持ってくる
        }
    }
    private void CallSendMyData()
    {
        tradeUserInfo.SendMyData();//自分のデータを送る
    }
    private void DestroyOtherPlayerEntereOrLeft() //プレーヤーが入った、抜けた時に表示するテキストを壊す
    {
        dtawingText.text = null;//他プレーヤーが入った、抜けたことの表示を消す
        if (isOtherPlayerEntered)//他プレーヤーが入った時
        {
            isOtherPlayerEntered = false;
        }
        if (isOtherPlayerLeft)//他プレーヤーが抜けた時
        {
            isOtherPlayerLeft = false;
            if (!isOver)//ゲーム終了していなかったら
            {
                ReGame(); //新しくゲームを作る
            }
            // if (isOver)
            // {
            //     GameObject go = GameObject.Find("SendScore");
            //     SendScore sendScore = go.GetComponent<SendScore>();
            //     sendScore.
            // }
        }
    }
    public void SetStartBottun()//マスターにStartBottunを設置
    {
        StartBottun.SetActive(true);
    }
    public void ClickStartBottun()//startBottunをクリック時
    {
        GameStart();
        StartBottun.SetActive(false);
    }
    public void GameStart()//GameStart！
    {
        phoView.RPC("RPC_GameStart", RpcTarget.AllViaServer); //全プレーヤーにGameStart!を表示
    }
    [PunRPC]
    public void RPC_GameStart()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            dtawingText.text = null;//"Waitng For Master to Start..."を消す
        }
        dtawingText.text = "GameStart!";//"GameStart！"を表示
        Invoke("NullGameStartText", 2f); //NullGameStartTextを1.5秒後に呼び出す
    }
    private void NullGameStartText()
    {
        dtawingText.text = null;//"GameStart！"をnullにする
        // turnManager.StartTurn();//ターンを開始する
        isStart = true;//ゲーム開始フラグ
    }
    public void GameOver()
    {
        isStart = false;//ゲーム終了フラグ
        isOver = true;
        dtawingText.text = "GameOver!";//"GameOver！"を表示
        Invoke("NullGameOver", 1.5f); //NullGameOverを1.5秒後に呼び出す
    }
    private void NullGameOver()
    {
        dtawingText.text = null;//"GameOver！"をnullにする
        sendScore.SetSendScoreModal();//相手のスコアをいれるモーダルを表示
    }
    public void LeaveRoomInGameOver()
    {
        loadingScene.isToLobby = true;//ロビーに行く
        nManager.LeaveRoom();//両プレーヤーを退出させる
    }
    private void ReGame()//ロビーに入るが、チャットシーンをロードするだけ
    {
        loadingScene.isToChat = true;
        Invoke("CreateAndJoinRoom", 1.5f); //CreateAndJoinRoomを1.5秒後に呼び出す
        nManager.LeaveRoom();
    }
    public void CreateAndJoinRoom()
    {
        nManager.CreateAndJoinRoom();
    }
}
