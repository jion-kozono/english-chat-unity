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
[RequireComponent(typeof(PunTurnManager))]
public class TurnManager : MonoBehaviourPunCallbacks, IOnEventCallback, IPunTurnManagerCallbacks
{
    private PhotonView pView;
    private PunTurnManager turnManager;
    [SerializeField]
    private Text TurnText = null;//ターン数の表示テキスト
    [SerializeField]
    private Text TimeText = null;//残り時間の表示テキスト
    [SerializeField]
    private Text YourTurnText; //"Your Turn, Waiting..."のテキスト
    [SerializeField]
    private Text OtherPlayerEntereOrLeft; //相手が出入りした時に表示するテキスト
    [SerializeField]
    private Text GameStartOrOver; //"GameStart!, GameOver!"のテキスト
    private int number = 0;
    private int turnLimit = 2; //ターンの回数
    public bool canChat = false; //チャット可能に
    private bool isAll = false; //全員いるか
    private bool isStart = false; //全員いるか
    public bool isReGame = false; //ゲームが再開されたか
    public UserInfo userInfo;
    public NetworkManager nManager;
    // Use this for initialization
    public void Awake()// StartをAwakeにする。
    {
        pView = GetComponent<PhotonView>();
        turnManager = GetComponent<PunTurnManager>(); //他プレーヤーが入ってきたらPunTurnManagerコンポーネントをつける
        this.turnManager.TurnDuration = 21f;//ターンは30秒にする
        this.turnManager.TurnManagerListener = this;// この実装でイベント関数をコールバックとして呼び出してもらうように登録しています。
    }
    public void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (this.turnManager.Turn > 0 && this.TurnText != null && isStart) //ターンが0以上、TurnTextがnullでない、ゲームが始まっている場合。
        {
            this.TurnText.text = this.turnManager.Turn.ToString();//何ターン目かを表示してくれる
        }
        if (this.turnManager.Turn > 0 && this.TimeText != null && isStart)//ターンが0以上、TimeTextがnullでない、ゲームが始まっている場合。
        {
            int flooredToIntValueToInt = Mathf.FloorToInt(this.turnManager.RemainingSecondsInTurn);
            this.TimeText.text = flooredToIntValueToInt.ToString() + " S";//小数点以下1桁の残り時間を表示。
        }
    }
    public void OnEvent(EventData photonEvent)
    {
    }
    public void OnPlayerFinished(Player player, int turn, object move) //1 プレイヤーがターンを終えたとき（そのプレイヤーのアクション/移動を含む）
    {
    }
    public void OnPlayerMove(Player player, int turn, object move) //2 プレイヤーが移動したとき（ただし、ターンは終了しない）
    {
        //Debug.Log("Player: " + player + " turn: " + turn + " action: " + move);
    }
    public void OnTurnBegins(int turn)//3 ターンが開始した場合
    {
    }
    public void OnTurnCompleted(int turn)//4 ターン終了時に呼ばれるメソッド（すべてのプレイヤーが終了）
    {
        if (this.turnManager.Turn == turnLimit)　//10ターンでゲーム終了
        {
            isStart = false; //ゲーム終了
            photonView.RPC("RPC_GameOver", RpcTarget.All);
        }
        else
        {
            // StartTurn();
            this.turnManager.BeginTurn();//turnmanagerに新しいターンを始めさせる
            photonView.RPC("RPC_AutomaticSend", RpcTarget.All);
        }
    }
    public void OnTurnTimeEnds(int turn)//5　タイマーが終了した場合
    {
        this.MakeTurn(5);
    }
    public void StartTurn()//ターン開始メソッド（シーン開始時にRPCから呼ばれる呼ばれるようにしてあります。）
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (number == this.turnManager.Turn)//BeginTurnが1ターンに1回しか回らないことのチェックをする。
            {
                this.turnManager.BeginTurn();//turnmanagerに新しいターンを始めさせる
                photonView.RPC("RPC_AutomaticSend", RpcTarget.All);
                number++;//BeginTurnが2回目以降1ターンに回る場合にはこの変数がターンと一致しないようにする
            }
        }
    }

    // public void OnEndTurn()//エンドターンのメソッド
    // {
    //     this.MakeTurn();
    // }
    public void MakeTurn(object index)//ターン移動のメソッド
    {
        this.turnManager.SendMove(index, true); //無条件でターン終了
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) // 他のプレイヤーが入室してきた時
    {
        Debug.Log("OnPlayerEnteredRoom");
        Debug.Log("Slots: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers);
        photonView.RPC("RPC_IsAll", RpcTarget.All); //全プレーヤーにGameStart!を表示
        OnOtherPlayerEntereOrLeft();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)  // 他のプレイヤーが退室した時
    {
        Debug.Log("OnPlayerLeftRoom");
        photonView.RPC("RPC_NotIsAll", RpcTarget.All); //全プレーヤーにGameStart!を表示
        OnOtherPlayerEntereOrLeft();
    }
    [PunRPC]
    public void RPC_IsAll()
    {
        isAll = true; //全員いる
        if (PhotonNetwork.IsMasterClient)
        {
            userInfo.SendMyData(NCMBUser.CurrentUser.ObjectId);
        }
        else
        {
            userInfo.StoreMyData();
        }
    }
    [PunRPC]
    public void RPC_NotIsAll()
    {
        isAll = false; //全員いない
    }
    private void OnOtherPlayerEntereOrLeft() //プレーヤーが入った、抜けた時
    {
        // Debug.Log("IsCompletedByAll: " + this.turnManager.IsCompletedByAll);
        // if (!this.turnManager.IsCompletedByAll) //ゲーム終了時には呼ばない
        // {
        if (isAll)
        {
            this.OtherPlayerEntereOrLeft.text = "Other Player Entered!"; //他プレーヤーが入ったことを表示
        }
        else
        {
            this.OtherPlayerEntereOrLeft.text = "Other Player Left!"; //他プレーヤーが抜けたことを表示
        }
        Invoke("DestroyOtherPlayerEntereOrLeft", 2.5f); //DestroyOtherPlayerEntereOrLeftを2.5秒後に呼び出す
        // }
        // else
        // {
        //     return;
        // }
    }
    void DestroyOtherPlayerEntereOrLeft() //プレーヤーが入った、抜けた時に表示するテキストを壊す
    {
        Destroy(this.OtherPlayerEntereOrLeft);
        if (isAll)
        {
            photonView.RPC("RPC_GameStart", RpcTarget.All); //全プレーヤーにGameStart!を表示
        }
        else
        {
            ReGame(); //新しくゲームを作る
        }
    }
    [PunRPC]
    public void RPC_GameStart()
    {
        isStart = true; //ゲーム開始
        this.GameStartOrOver.text = "GameStart!"; //スタート時に"GameStart！"を表示
        Invoke("NullGameStart", 2.5f); //DestroyGameStartを2.5秒後に呼び出す
    }
    void NullGameStart() //"GameStart！"テキストをnullにする
    {
        this.GameStartOrOver.text = null;
        this.StartTurn();
    }
    [PunRPC]
    public void RPC_GameOver()
    {
        this.GameStartOrOver.text = "GameOver!"; //スタート時に"GameOver!"を表示
        Invoke("NullGameOver", 2.5f); //DestroyGameOverを2.5秒後に呼び出す
    }
    void NullGameOver() //"GameOver！"テキストをnullにする
    {
        this.GameStartOrOver.text = null;
        PhotonNetwork.LeaveRoom(); //両プレーヤーを退出させる（ロビーに行く)
        SceneManager.LoadScene("Lobby");
    }
    private void ReGame()
    {
        isReGame = true;
        PhotonNetwork.LeaveRoom(); //ロビーに行かない
        Invoke("CreateRoom", 2.5f); //CreateRoomを2.5秒後に呼び出す
    }
    public void CreateRoom()
    {
        SceneManager.LoadScene("Chat");
        nManager.CreateAndJoinRoom();
    }
    [PunRPC]
    public void RPC_AutomaticSend()
    {
        if ((this.turnManager.Turn % 2) + 1 == PhotonNetwork.LocalPlayer.ActorNumber)//2ターンに1回自分のターンを無条件で終わらせる
        {
            // Debug.Log("photonView.IsMine: " + photonView.IsMine);
            canChat = false; //チャットを不可能にする
            object index = 0;
            this.turnManager.SendMove(index, true); //無条件でターン終了
            this.YourTurnText.text = "Waitng..."; //待ちの表示テキスト
        }
        else
        {
            canChat = true; //チャットを可能にする
            this.YourTurnText.text = "Your Turn";
        }
    }
}