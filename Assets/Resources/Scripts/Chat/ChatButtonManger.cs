using UnityEngine;

public class ChatButtonManger : MonoBehaviour
{
    [SerializeField] private GameObject ChatMenuModal = null;
    // UserInfoModalController userInfoModalController;
    // public bool IsChatMenuModalSetActive = false;

    // Use this for initialization
    void awake()
    {
        // GameObject go = GameObject.Find("UserInfoModalController");
        // userInfoModalController = go.GetComponent<UserInfoModalController>();
    }
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
            // if (userInfoModalController.IsUserInfoModalSetActive)
            // {
            //     userInfoModalController.CloseUserInfoModal();
            // }
            ChatMenuModal.SetActive(true);
            // IsChatMenuModalSetActive = true;
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
            // IsChatMenuModalSetActive = false;
        }
    }
}
