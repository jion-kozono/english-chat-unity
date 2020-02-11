using UnityEngine;
using UnityEngine.SceneManagement;

public class ChatButtonManger : MonoBehaviour
{
    [SerializeField] private NetworkManager nManager;
    [SerializeField] private GameObject ChatMenuModal = null;
    void Start()
    {
        if (ChatMenuModal.activeSelf != false)
        {
            ChatMenuModal.SetActive(false);
            // IsChatMenuModalSetActive = false;
        }
    }
    public void OpenChatMenuModal()//ChatMenuModalを開く
    {
        if (ChatMenuModal.activeSelf == false)
        {
            ChatMenuModal.SetActive(true);
        }
        else
        {
            return;
        }
    }
    public void CloseChatMenuModal()//ChatMenuModalを閉じる
    {
        if (ChatMenuModal.activeSelf != false)
        {
            ChatMenuModal.SetActive(false);
        }
    }
    public void LogOut()
    {
        nManager.LeaveRoom();
        GameObject go = GameObject.Find("UserAuth");
        UserAuth ua = go.GetComponent<UserAuth>();
        GameObject go2 = GameObject.Find("LoadingScene");
        LoadingScene loadingScene = go2.GetComponent<LoadingScene>();
        loadingScene.isToLogin = true;
        ua.logOut();
        loadingScene.LoadNextScene();
        SceneManager.UnloadSceneAsync("chat");
    }
}
