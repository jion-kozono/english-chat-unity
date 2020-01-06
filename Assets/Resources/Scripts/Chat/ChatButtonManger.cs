using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatButtonManger : MonoBehaviour
{
    public GameObject ChatMenuModal = null;
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
            ChatMenuModal.SetActive(true);
        }
        else
        {
            ChatMenuModal.SetActive(false);
        }
    }

    // Update is called once per frame
    public void CloseChatMenuModal()
    {
        // Canvas を有効にする
        if (ChatMenuModal.activeSelf != false)
        {
            ChatMenuModal.SetActive(false);
        }
    }
}
