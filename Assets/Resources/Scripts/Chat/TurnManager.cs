using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PunTurnManager))]
public class TurnManager : MonoBehaviourPunCallbacks, IOnEventCallback, IPunTurnManagerCallbacks
{
    private PhotonView pView;
    private PunTurnManager turnManager;
    [SerializeField] private GameController gameController;
    [SerializeField] private Text TurnText = null;//ターン数の表示テキスト
    [SerializeField] private Text TimeText = null;//残り時間の表示テキスト
    [SerializeField] private Text YourTurnText; //"Your Turn, Waiting..."のテキスト
    private int number = 0;
    private int turnLimit = 2; //ターンの回数
    private bool isStartTurn = false; //turn開始しているか
    public bool canChat = false; //チャット可能に
    // Use this for initialization
    public void Awake()
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
        if (this.turnManager.Turn > 0 && this.TurnText != null && isStartTurn) //ターンが0以上、TurnTextがnullでない、turn開始の場合。
        {
            this.TurnText.text = this.turnManager.Turn.ToString();//何ターン目かを表示してくれる
        }
        if (this.turnManager.Turn > 0 && this.TimeText != null && isStartTurn)//ターンが0以上、TimeTextがnullでない、turn開始の場合。
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
            isStartTurn = false; //turn終了
            pView.RPC("RPC_OverTurn", RpcTarget.All);
        }
        else
        {
            // StartTurn();
            this.turnManager.BeginTurn();//turnmanagerに新しいターンを始めさせる
            pView.RPC("RPC_AutomaticSend", RpcTarget.All);
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
                isStartTurn = true; //turn開始
                pView.RPC("RPC_AutomaticSend", RpcTarget.All);
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

    [PunRPC]
    public void RPC_AutomaticSend()
    {
        if ((this.turnManager.Turn % 2) + 1 == PhotonNetwork.LocalPlayer.ActorNumber)//2ターンに1回自分のターンを無条件で終わらせる
        {
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
    [PunRPC]
    public void RPC_OverTurn()//全てのターン終了時
    {
        gameController.GameOver();//GameOverの流れを開始
    }
}