using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class UserInfo : MonoBehaviourPunCallbacks
{
    private PhotonView phView;
    private UserInfoData _myData;　//自分のデータを格納
    private UserInfoData _otherData;　//相手のデータを格納
    [SerializeField] private Text myUserText;
    [SerializeField] private Text OpponentUserText;
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text charmText;
    [SerializeField] private Text profileText;
    public ChatButtonManger chatButtonManger;
    public GameObject UserInfoModal = null;
    private PhotonMessageInfo info;
    public class UserInfoData
    {
        //自分のデータを格納する変数
        public string m_playerName; //プレーヤー名
        public int m_level; //レベル
        public int m_charm; //魅力
        public string m_profile; //自己紹介文
    }
    // Use this for initialization
    void Awake()
    {
        phView = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (UserInfoModal.activeSelf != false)
        {
            UserInfoModal.SetActive(false);
        }
        Debug.Log("PhotonNetwork.LocalPlayer.NickName: " + PhotonNetwork.LocalPlayer.NickName);
        myUserText.text = PhotonNetwork.LocalPlayer.NickName;
        StoreMyData(); //マスターだけ
    }
    public void OpenUserInfoModal()//UserInfoModaを開く
    {
        if (chatButtonManger.ChatMenuModal.activeSelf != false)
        {
            chatButtonManger.ChatMenuModal.SetActive(false);
        }
        if (UserInfoModal.activeSelf == false)
        {
            UserInfoModal.SetActive(true);
        }
    }
    public void CloseUserInfoModal()//UserInfoModaを閉じる
    {
        // Canvas を有効にする
        if (UserInfoModal.activeSelf != false)
        {
            UserInfoModal.SetActive(false);
        }
    }
    public void DrawMyData()//自分のデータを描画
    {
        playerNameText.text = "Name: " + _myData.m_playerName;
        levelText.text = "Level： " + _myData.m_level.ToString();
        charmText.text = "Charm： " + _myData.m_charm.ToString();
        profileText.text = "AboutMe：\n" + _myData.m_profile;
    }
    public void DrawOtherData()//相手のデータを描画
    {
        playerNameText.text = "Name: " + _otherData.m_playerName;
        levelText.text = "Level： " + _otherData.m_level.ToString();
        charmText.text = "Charm： " + _otherData.m_charm.ToString();
        profileText.text = "自己紹介： \n" + _otherData.m_profile;
    }
    public void StoreMyData()//Start(), RPC_IsAll()で呼ばれる
    {
        string userId = NCMBUser.CurrentUser.ObjectId;
        // PlayerProfileを検索するクラスを作成
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("PlayerProfile");
        query.WhereEqualTo("userId", userId);
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
                            int level = (int)(long)playerProfile["level"];
                            int charm = (int)(long)playerProfile["charm"];
                            string profileText = (string)playerProfile["profileText"];
                            Debug.Log("検索→取得成功 ,level: " + level + " ,charm: " + charm + " ,profile: " + profileText);
                            _myData = new UserInfoData { m_playerName = PhotonNetwork.LocalPlayer.NickName, m_level = level, m_charm = charm, m_profile = profileText }; // 自分の情報を変数に格納
                            Debug.Log("自分のデータ：　m_playerName: " + _myData.m_playerName + " ,m_level: " + _myData.m_level + " ,charm: " + _myData.m_charm + " ,profile: " + _myData.m_profile);
                            DrawMyData();
                            if (!PhotonNetwork.IsMasterClient)
                            {
                                SendMyData(userId);
                            }
                        }
                    });
                }
            }
        });
    }
    public void SendMyData(string myUserId) //相手のRPC_RecieveOtherData()をよぶ(RoomInfo.MaxPlayersが揃ったら呼ばれる)
    {
        photonView.RPC("RPC_RecieveOtherData", RpcTarget.Others, myUserId);
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
                            int level = (int)(long)playerProfile["level"];
                            int charm = (int)(long)playerProfile["charm"];
                            string profileText = (string)playerProfile["profileText"];
                            Debug.Log("検索→取得成功 ,level: " + level + " ,charm: " + charm + " ,profile: " + profileText);
                            _otherData = new UserInfoData { m_playerName = _otherData.m_playerName, m_level = _otherData.m_level, m_charm = _otherData.m_charm, m_profile = _otherData.m_profile }; // 相手の情報を変数に格納
                            OpponentUserText.text = _otherData.m_playerName;
                            Debug.Log("相手のデータ：　m_playerName: " + _otherData.m_playerName + " ,m_level: " + _otherData.m_level + " ,charm: " + _otherData.m_charm + " ,profile: " + _otherData.m_profile);
                            DrawOtherData();
                        }
                    });
                }
            }
        });
    }
}
