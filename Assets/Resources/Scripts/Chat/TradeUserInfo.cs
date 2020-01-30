using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class TradeUserInfo : MonoBehaviourPunCallbacks
{
    private PhotonView phView;
    public TurnManager turnManager;
    private UserInfo userInfo;
    private UserInfoData _otherData;　//相手のデータを格納
    private GameObject userInfoModal;
    [SerializeField] private GameController gameController;
    [SerializeField] private Text myUserText;
    [SerializeField] private GameObject opponentUserWindow;
    [SerializeField] private Text opponentUserText;
    public bool isGetOtherData; //相手のデータを受け取ったかどうか
    public bool isDrawOtherData;//相手のデータを描画するか
    // Use this for initialization
    void Awake()
    {
        phView = GetComponent<PhotonView>();
        GameObject go = GameObject.Find("UserInfo");
        userInfo = go.GetComponent<UserInfo>();
    }
    private GUIStyle style;
    void Start()
    {
        style = new GUIStyle();
        style.fontSize = 30;
        style.fontStyle = FontStyle.Normal;
        HideOpponentUserWindow(); //相手のボタンを隠す
        myUserText.text = NCMBUser.CurrentUser.UserName;
    }
    void OnGUI()
    {
        if (isDrawOtherData)
        {
            int txtX = Screen.width * 1 / 11, txtY = 15, txtW = Screen.width * 3 / 5, txtH = 40;
            GUI.Label(new Rect(txtX, Screen.height * 4 / txtY - txtH * 1 / 2, txtW, txtH), "Name: " + _otherData.m_playerName, style);
            GUI.Label(new Rect(txtX, Screen.height * 5 / txtY - txtH * 1 / 2, txtW, txtH), "Status: " + _otherData.m_status);
            GUI.Label(new Rect(txtX, Screen.height * 6 / txtY - txtH * 1 / 2, txtW, txtH), "Score: " + _otherData.m_score.ToString(), style);
            GUI.Label(new Rect(txtX, Screen.height * 7 / txtY - txtH * 1 / 2, txtW, txtH), "Level： " + _otherData.m_level.ToString(), style);
            GUI.Label(new Rect(txtX, Screen.height * 8 / txtY - txtH * 1 / 2, txtW, txtH), "Charm： " + _otherData.m_charm.ToString(), style);
            GUI.Label(new Rect(txtX, Screen.height * 9 / txtY - txtH * 1 / 2, txtW, txtH * 5), "AboutMe：\n" + _otherData.m_profile, style);
        }
    }
    public void DrawOtherData()//相手のデータを描画
    {
        isDrawOtherData = true;
    }
    public void HideOpponentUserWindow()
    {
        opponentUserWindow.SetActive(false);
        Debug.Log("opponentUserWindow" + opponentUserWindow);
        isGetOtherData = false;//相手のデータを削除
    }
    public void SendMyData() //相手のRPC_RecieveOtherData()をよぶ(RoomInfo.MaxPlayersが揃ったら呼ばれる)
    {
        phView.RPC("RPC_RecieveOtherData", RpcTarget.Others, NCMBUser.CurrentUser.ObjectId);
    }
    [PunRPC]
    public void RPC_RecieveOtherData(string otherUserId)
    {
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("PlayerProfile");
        query.WhereEqualTo("userId", otherUserId);
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                Debug.Log("検索失敗時");//検索失敗時の処理
            }
            else
            {
                foreach (NCMBObject obj in objList)
                {
                    string playerProfileId = obj.ObjectId;
                    NCMBObject playerProfile = new NCMBObject("PlayerProfile");
                    playerProfile.ObjectId = playerProfileId;
                    playerProfile.FetchAsync((NCMBException e2) =>
                    {
                        if (e2 != null)
                        {
                            //エラー処理
                        }
                        else
                        {
                            string userName = (string)playerProfile["userName"];
                            string status = (string)playerProfile["status"];
                            int score = (int)(long)playerProfile["score"];
                            int level = (int)(long)playerProfile["level"];
                            int charm = (int)(long)playerProfile["charm"];
                            string profileText = (string)playerProfile["profileText"];
                            // Debug.Log("検索→取得成功 ,level: " + level + " ,charm: " + charm + " ,profile: " + profileText);
                            _otherData = new UserInfoData { m_playerName = userName, m_status = status, m_score = score, m_level = level, m_charm = charm, m_profile = profileText }; // 相手の情報を変数に格納
                            Debug.Log("相手のデータ：　m_playerName: " + _otherData.m_playerName + " ,m_level: " + _otherData.m_level + " ,charm: " + _otherData.m_charm + " ,profile: " + _otherData.m_profile);
                            isGetOtherData = true;//相手のデータを受け取った
                            Debug.Log("opponentUserWindow: " + opponentUserWindow);
                            opponentUserWindow.SetActive(true);//opponentUserWindowを表示
                            opponentUserText.text = _otherData.m_playerName;//相手の名前を表示
                            //後から入ったほうが相手のデータを受け取ったらそれをマスターに伝える
                            if (!PhotonNetwork.IsMasterClient)
                            {
                                photonView.RPC("RPC_OtherGetMyData", RpcTarget.Others, isGetOtherData);
                            }
                        }
                    });
                }
            }
        });
    }
    [PunRPC]
    public void RPC_OtherGetMyData(bool isOtherGetMyData)//マスターにゲームスタートさせる
    {
        if (isOtherGetMyData)
        {
            gameController.GameStart();
        }
    }
}
