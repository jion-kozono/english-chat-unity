using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatButtonManger : MonoBehaviour
{
    public GameObject ChatMenuModal = null;
    public UserInfo userInfo;
    // Use this for initialization
    void Start()
    {
        if (ChatMenuModal.activeSelf != false)
        {
            ChatMenuModal.SetActive(false);
        }
    }
    public void OpenChatMenuModal()
    {
        // Canvas を有効にする
        if (ChatMenuModal.activeSelf == false)
        {
            if (userInfo.UserInfoModal.activeSelf != false)
            {
                userInfo.UserInfoModal.SetActive(false);
            }
            ChatMenuModal.SetActive(true);
        }
        else
        {
            return;
        }
    }
    public void CloseChatMenuModal()
    {
        // Canvas を有効にする
        if (ChatMenuModal.activeSelf != false)
        {
            ChatMenuModal.SetActive(false);
        }
    }
}
