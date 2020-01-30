using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using UnityEngine.SceneManagement;

public class UserInfo : MonoBehaviourPunCallbacks
{
    private UserInfoData _myData; //自分のデータを格納
    public bool isGetMyData = false; //自分のデータを受け取ったか
    public bool isDrawMyData = false;//自分のデータを描画するか
    private UserInfo instance = null;
    void Awake()
    {
        // シングルトン化する ------------------------
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            string name = gameObject.name;
            gameObject.name = name + "(Singleton)";
            GameObject duplicater = GameObject.Find(name);
            if (duplicater != null)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.name = name;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    private GUIStyle style;//GUIクラスのスタイル
    void Start()
    {
        style = new GUIStyle();
        style.fontSize = 30;
        style.fontStyle = FontStyle.Normal;
        StoreMyData();
    }
    void Update()
    {
        if (NCMBUser.CurrentUser == null || !isGetMyData)
        {
            return;
        }
    }
    void OnGUI()
    {
        if (isDrawMyData)
        {
            int txtX = Screen.width * 1 / 9, txtY = 15, txtW = Screen.width * 3 / 5, txtH = 40;
            GUI.Label(new Rect(txtX, Screen.height * 4 / txtY - txtH * 1 / 2, txtW, txtH), "Name: " + _myData.m_playerName, style);
            GUI.Label(new Rect(txtX, Screen.height * 5 / txtY - txtH * 1 / 2, txtW, txtH), "Status: " + _myData.m_status, style);
            GUI.Label(new Rect(txtX, Screen.height * 6 / txtY - txtH * 1 / 2, txtW, txtH), "Score: " + _myData.m_score.ToString(), style);
            GUI.Label(new Rect(txtX, Screen.height * 7 / txtY - txtH * 1 / 2, txtW, txtH), "Level： " + _myData.m_level.ToString(), style);
            GUI.Label(new Rect(txtX, Screen.height * 8 / txtY - txtH * 1 / 2, txtW, txtH), "Charm： " + _myData.m_charm.ToString(), style);
            GUI.Label(new Rect(txtX, Screen.height * 9 / txtY - txtH * 1 / 2, txtW, txtH * 5), "AboutMe：\n" + _myData.m_profile, style);
        }
    }
    public void DrawMyData()//自分のデータを描画
    {
        isDrawMyData = true;
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
                            string status = (string)playerProfile["status"];
                            int score = (int)(long)playerProfile["score"];
                            int level = (int)(long)playerProfile["level"];
                            int charm = (int)(long)playerProfile["charm"];
                            string profileText = (string)playerProfile["profileText"];
                            _myData = new UserInfoData { m_playerName = NCMBUser.CurrentUser.UserName, m_status = status, m_score = score, m_level = level, m_charm = charm, m_profile = profileText }; // 自分の情報を変数に格納
                            isGetMyData = true;
                            if (SceneManager.GetActiveScene().name == "Lobby")//ロビーの時だけ
                            {
                                GameObject go = GameObject.Find("LobbyManager");
                                LobbyManager lobbyManager = go.GetComponent<LobbyManager>();
                                lobbyManager.OnRecieveMyData();
                            }
                        }
                    });
                }
            }
        });
    }
}
