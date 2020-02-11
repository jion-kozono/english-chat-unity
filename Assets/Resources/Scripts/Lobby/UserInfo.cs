using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
public class UserInfo : MonoBehaviourPunCallbacks
{
    private UserInfoData _myData; //自分のデータを格納
    public ExitGames.Client.Photon.Hashtable localPlayerProperties;
    public bool isGetMyData = false; //自分のデータを受け取ったか
    public string[] myStrArray;
    public int[] myIntArray;
    public string[] otherStrArray;
    public int[] otherIntArray;
    private UserAuth ua;
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
    void Start()
    {
        GameObject go = GameObject.Find("UserAuth");
        ua = go.GetComponent<UserAuth>();
        if (!ua.initialSignUp)//signUp時以外で
        {
            StoreMyData();
        }
    }
    void Update()
    {
        if (NCMBUser.CurrentUser == null)
        {
            return;
        }
    }
    public void StoreMyData()//ログインしてロビーに入ったら呼ばれる
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
                            //NCMB
                            string status = (string)playerProfile["status"];
                            int score = (int)(long)playerProfile["score"];
                            int level = (int)(long)playerProfile["level"];
                            int charm = (int)(long)playerProfile["charm"];
                            string profileText = (string)playerProfile["profileText"];
                            // //Photonにも一応代入
                            // localPlayerProperties = new ExitGames.Client.Photon.Hashtable();
                            // localPlayerProperties["status"] = status;
                            // localPlayerProperties["score"] = score;
                            // localPlayerProperties["level"] = level;
                            // localPlayerProperties["charm"] = charm;
                            // localPlayerProperties["profileText"] = profileText;
                            // PhotonNetwork.LocalPlayer.SetCustomProperties(localPlayerProperties);
                            _myData = new UserInfoData { m_playerName = NCMBUser.CurrentUser.UserName, m_status = status, m_score = score, m_level = level, m_charm = charm, m_profile = profileText }; // 自分の情報を変数に格納
                            //RPCで相手に送るために各型の配列に格納
                            myStrArray = new string[] { _myData.m_playerName, _myData.m_status, _myData.m_profile, userId };
                            myIntArray = new int[] { _myData.m_score, _myData.m_level, _myData.m_charm };
                            isGetMyData = true;//自分のデータをゲットした
                        }
                    });
                }
            }
        });
    }
}
