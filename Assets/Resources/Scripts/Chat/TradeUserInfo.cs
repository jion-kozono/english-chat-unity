using Photon.Pun;
using UnityEngine;
using NCMB;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class TradeUserInfo : MonoBehaviourPunCallbacks
{
    private PhotonView phView;
    private UserInfo userInfo;
    private UserInfoData _myData; //自分のデータを格納
    private UserInfoData _otherData;　//相手のデータを格納
    // [SerializeField] private UserInfoModalController UserInfoModalController;
    [SerializeField] private GameObject userInfoModal;
    [SerializeField] private GameController gameController;
    [SerializeField] private Text myUserText;
    [SerializeField] private GameObject opponentUserWindow;
    [SerializeField] private Text opponentUserText;
    public bool isGetOtherData; //相手のデータを受け取ったかどうか
    // Use this for initialization
    void Awake()
    {
        phView = GetComponent<PhotonView>();
        GameObject go = GameObject.Find("UserInfo");
        userInfo = go.GetComponent<UserInfo>();
    }
    void Start()
    {
        HideOpponentUserWindow(); //相手のボタンを隠す
        myUserText.text = NCMBUser.CurrentUser.UserName;
    }
    //photonを使って相手のデータを取得する流れ
    public void SendMyData() //相手のRPC_RecieveOtherData()をよぶ(RoomInfo.MaxPlayersが揃ったら呼ばれる)
    {
        phView.RPC("RPC_RecieveOtherData", RpcTarget.Others, userInfo.myStrArray, userInfo.myIntArray);
    }
    [PunRPC]
    public void RPC_RecieveOtherData(string[] otherStrArray, int[] otherIntArray)
    {
        string userName = otherStrArray[0];
        string status = otherStrArray[1];
        string profileText = otherStrArray[2];
        string userId = otherStrArray[3];
        int score = otherIntArray[0];
        int level = otherIntArray[1];
        int charm = otherIntArray[2];

        _otherData = new UserInfoData { m_playerName = userName, m_status = status, m_profile = profileText, m_score = score, m_level = level, m_charm = charm }; // 相手の情報を変数に格納
        userInfo.otherStrArray = new string[] { userName, status, profileText, userId };
        userInfo.otherIntArray = new int[] { score, level, charm };
        SetOpponentUserWindow();
        //後から入ったほうが相手のデータを受け取ったらそれをマスターに伝える
        if (!PhotonNetwork.IsMasterClient)
        {
            phView.RPC("RPC_OtherGetMyData", RpcTarget.Others, isGetOtherData);
        }
    }

    [PunRPC]
    public void RPC_OtherGetMyData(bool isOtherGetMyData)//マスターにゲームスタートさせる
    {
        if (isOtherGetMyData)
        {
            gameController.SetStartBottun();//マスターにスタートボタンを配置
        }
    }
    public void SetOpponentUserWindow()
    {
        isGetOtherData = true;//相手のデータを受け取った
        opponentUserWindow.SetActive(true);//opponentUserWindowを表示
        opponentUserText.text = _otherData.m_playerName;//相手の名前を表示
    }
    public void HideOpponentUserWindow()
    {
        opponentUserWindow.SetActive(false);
    }

}
