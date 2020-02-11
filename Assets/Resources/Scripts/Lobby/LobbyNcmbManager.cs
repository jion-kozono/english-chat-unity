using NCMB;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyNcmbManager : MonoBehaviour
{
    // [SerializeField] private NetworkManager nManager;
    [SerializeField] private Text myUserText;
    private UserInfo userInfo;
    private UserAuth ua;
    // Use this for initialization
    private void Start()
    {
        GameObject go = GameObject.Find("UserAuth");
        ua = go.GetComponent<UserAuth>();
        GameObject go2 = GameObject.Find("UserInfo");
        userInfo = go2.GetComponent<UserInfo>();
        if (ua.initialSignUp)//signUpがされた時にしか呼ばない
        {
            SaveInitialData();
            ua.initialSignUp = false;//一応初期化
        }
        myUserText.text = NCMBUser.CurrentUser.UserName;
    }
    public void LogOut()
    {
        GameObject go = GameObject.Find("LoadingScene");
        LoadingScene loadingScene = go.GetComponent<LoadingScene>();
        loadingScene.isToLogin = true;
        ua.logOut();
        loadingScene.LoadNextScene();
        SceneManager.UnloadSceneAsync("Lobby");
    }
    private void SaveInitialData()
    {
        Debug.Log("SaveInitialData");
        NCMBObject playerProfile = new NCMBObject("PlayerProfile");
        playerProfile.Add("userName", NCMBUser.CurrentUser.UserName);
        playerProfile.Add("userId", NCMBUser.CurrentUser.ObjectId);
        playerProfile.Add("status", "Begginer");
        playerProfile.Add("score", 0);
        playerProfile.Add("level", 0);
        playerProfile.Add("charm", 0);
        playerProfile.Add("profileText", "Notthing....");
        playerProfile.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                Debug.Log("取得失敗");
            }
            else
            {
                //成功時の処理
                Debug.Log("SaveInitialData");
                userInfo.StoreMyData();
            }
        });
    }
}
