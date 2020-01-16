using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using NCMB;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class UserInfo : MonoBehaviourPunCallbacks
{
    private PhotonView photonView;
    private List<MyInfoData> _myData;　//自分のデータを格納
    private List<OtherInfoData> _otherData; //相手のデータを格納
    [SerializeField]
    private Text myUserText;
    [SerializeField]
    private Text opponentUserText;
    public ChatButtonManger chatButtonManger;
    public GameObject UserInfoModal = null;
    public class MyInfoData
    {
        //自分のデータを格納する変数
        public string m_playerName; //プレーヤー名
        public int m_level; //レベル
        public int m_charm; //魅力
        public string m_profile; //自己紹介文
    }
    public class OtherInfoData
    {
        //相手のデータを格納する変数
        public string m_playerName; //プレーヤー名
        public int m_level; //レベル
        public int m_charm; //魅力
        public string m_profile; //自己紹介文
    }
    // Use this for initialization
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        _myData = new List<MyInfoData>();
        _otherData = new List<OtherInfoData>();
    }
    void Start()
    {
        NCMBObject user = new NCMBObject("user");
        user.ObjectId = "yl9wLBnRI2hwVKxF";
        user.FetchAsync((NCMBException e) =>
        {
            if (e != null)
            {
                //エラー処理
            }
            else
            {
                //成功時の処理
                Debug.Log(user["userName"]);
            }
        });
        NCMBObject TestClass = new NCMBObject("TestClass");
        TestClass.ObjectId = "NjgNJsoDLAOtDeR5";
        TestClass.FetchAsync((NCMBException e) =>
        {
            if (e != null)
            {
                //エラー処理
            }
            else
            {
                //成功時の処理
                Debug.Log(TestClass["message"]);
            }
        });

        if (UserInfoModal.activeSelf != false)
        {
            UserInfoModal.SetActive(false);
        }
    }
    public void OpenUserInfoModal()
    {
        // Canvas を有効にする
        if (UserInfoModal.activeSelf == false)
        {
            if (chatButtonManger.ChatMenuModal.activeSelf != false)
            {
                chatButtonManger.ChatMenuModal.SetActive(false);
            }
            UserInfoModal.SetActive(true);
        }
        else
        {
            return;
        }
    }
    public void CloseUserInfoModal()
    {
        // Canvas を有効にする
        if (UserInfoModal.activeSelf != false)
        {
            UserInfoModal.SetActive(false);
        }
    }
    // public void InfoText() //RPC_InfoText()をよぶ(OnPlayerEnteredRoom()から呼ばれる)
    // {
    //     photonView.RPC("RPC_InfoText", RpcTarget.All);
    // }

    // [PunRPC]
    // public void RPC_InfoText(int _otherId) //そのプレーヤーが部屋に入った時に表示される
    // {
    //     // 自分情報の処理
    //     // 自分の情報(UserAuth.currentPlayerName?またはPhotonNetwork.LocalPlayer.NickName?)でニフクラから取得
    //     myUserText = PhotonNetwork.LocalPlayer.NickName;
    //     // 取得した名前、プロフィールを取得
    //     // ラベルに反映自分の名前を反映
    //     _myData = new MyInfoData { m_playerName = myUserText, m_level = 〜, m_charm = 〜, m_profile = 〜 }; // 自分の情報を変数に格納

    //     // 相手情報の処理

    //     // _otherIdの情報でニフクラから取得
    //     // 取得した名前、プロフィールを取得
    //     // ラベルに反映otherInfoの名前を反映
    //     _otherData new OtherInfoData { m_playerName = text, m_level = 〜, m_charm = 〜, m_profile = 〜 }; // 相手の情報を変数に格納
    // }
}
