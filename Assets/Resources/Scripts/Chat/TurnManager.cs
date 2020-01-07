using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PunTurnManager))]
public class TurnManager : MonoBehaviourPunCallbacks, IOnEventCallback, IPunTurnManagerCallbacks
{
    private PhotonView photonView;
    private PunTurnManager turnManager;
    [SerializeField]
    private Text TurnText;//ターン数の表示テキスト
    [SerializeField]
    private Text TimeText = null;//残り時間の表示テキスト
    [SerializeField]
    private Text WaitingText;//"Opponent's turn..."のテキスト
    [SerializeField]
    private Text YourTurnText; //"Your Turn"のテキスト
    [SerializeField]
    private Text GameStart; //"GameStart!"のテキスト
    private int number = 0;
    public bool canChat = false; //チャット可能に
    private bool IsShowingResults; //結果が見えているか
    private bool isAll = false; //全員いるか
    private bool isStartTurn; //全員いるか
    public ScrollerController scrollerController; //チャットが打たれたかを判断するためScrollerControllerを用いる

    // Use this for initialization
    public void Awake()// StartをAwakeにする。
    {
        turnManager = GetComponent<PunTurnManager>();
        photonView = GetComponent<PhotonView>();
        this.turnManager.TurnDuration = 21f;//ターンは30秒にする
        this.turnManager.TurnManagerListener = this;// この実装でイベント関数をコールバックとして呼び出してもらうように登録しています。
    }

    public void OnEvent(EventData photonEvent)
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (this.TurnText != null && isAll && isStartTurn)
        {
            this.TurnText.text = this.turnManager.Turn.ToString();//何ターン目かを表示してくれる
        }
        if (this.turnManager.Turn > 0 || this.TimeText != null && isAll && isStartTurn && !IsShowingResults)//ターンが0以上、TimeTextがnullでない、結果が見えていない場合。
        {
            int flooredToIntValueToInt = Mathf.FloorToInt(this.turnManager.RemainingSecondsInTurn);
            this.TimeText.text = flooredToIntValueToInt.ToString() + " SECONDS";//小数点以下1桁の残り時間を表示。
        }
        if (this.turnManager.IsCompletedByAll) //両方のプレイヤーがターンを終了しているか
        {
            //後に処理を書く予定
        }
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
        IsShowingResults = false;
    }
    public void OnTurnCompleted(int turn)//4 ターン終了時に呼ばれるメソッド（すべてのプレイヤーが終了）
    {
        this.turnManager.BeginTurn();//turnmanagerに新しいターンを始めさせる
        photonView.RPC("RPC_AutomaticSend", RpcTarget.All);
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
    public override void OnPlayerEnteredRoom(Player newPlayer)　// 他のプレイヤーが入室してきた時
    {
        Debug.Log("OnPlayerEnteredRoom");
        Debug.Log("Slots: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers);
        isAll = true;
        photonView.RPC("RPC_GameStart", RpcTarget.All); //前プレーヤーにGameStart!を表示
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)　 // 他のプレイヤーが退室した時
    {
        Debug.Log("OnPlayerLeftRoom");
        Debug.Log("Slots: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers);
        isAll = false;
        photonView.RPC("RPC_DestroyTurnManager", RpcTarget.All); //片方いなくなったらゲームの初期化
    }
    [PunRPC]
    public void RPC_GameStart()
    {
        this.GameStart.text = "GameStart!"; //スタート時に"GameStart！"を表示
        Invoke("DestroyGameStart", 2.5f); //DestroyGameStartを2.5秒後に呼び出す
    }
    void DestroyGameStart() //"GameStart！"テキストを壊す
    {
        Destroy(this.GameStart);
        this.StartTurn();
        isStartTurn = true;
    }
    [PunRPC]
    public void RPC_DestroyTurnManager()
    {
        Destroy(this.turnManager);
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
            this.WaitingText.text = "Opponent's turn...";　//待ちの表示テキスト
            this.YourTurnText.text = "";
        }
        else
        {
            canChat = true; //チャットを可能にする
            this.WaitingText.text = "";
            this.YourTurnText.text = "Your Turn";
        }
    }
}