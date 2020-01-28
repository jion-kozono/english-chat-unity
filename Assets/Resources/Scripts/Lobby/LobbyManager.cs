using Photon.Pun;
using NCMB;
using UnityEngine;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    bool initialSignUp = false;
    // Use this for initialization
    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        GameObject go = GameObject.Find("UserAuth");
        UserAuth ua = go.GetComponent<UserAuth>();
        initialSignUp = ua.initialSignUp;
    }
    // Update is called once per frame
    private void Update()
    {
        if (NCMBUser.CurrentUser == null)
        {
            return;
        }
        if (initialSignUp)//signUpがされた時にしか呼ばない
        {
            SaveInitialData();
            initialSignUp = false;//次回以降呼ばないようにする
        }
    }
    private void SaveInitialData()
    {
        Debug.Log("SaveInitialData");

        NCMBObject playerProfile = new NCMBObject("PlayerProfile");
        playerProfile.Add("userId", NCMBUser.CurrentUser.ObjectId);
        playerProfile.Add("level", 0);
        playerProfile.Add("charm", 0);
        playerProfile.Add("profileText", "");
        playerProfile.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                Debug.Log("取得失敗");
            }
            else
            {
                //成功時の処理
                Debug.Log("userId: " + playerProfile["userId"] + " ,level: " + playerProfile["level"] + " ,charm: " + playerProfile["charm"] + " ,profileText: " + playerProfile["profileText"]);
            }
        });
    }
}
