using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class UserInfo : MonoBehaviourPunCallbacks
{
    private PhotonView photonView;
    [SerializeField]
    private Text myUserText;
    [SerializeField]
    private Text opponentUserText;
    public ChatButtonManger chatButtonManger;
    public GameObject UserInfoModal = null;
    // Use this for initialization
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    void Start()
    {
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
    void InfoText() //RPC_InfoText()をよぶ
    {
        photonView.RPC("RPC_InfoText", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_InfoText()//そのプレーヤーが部屋に入った時に表示される
    {
        if (photonView.IsMine == false)
        {
            this.opponentUserText.text = "相手の名前";
        }
        else
        {
            this.myUserText.text = "自分の名前";
        }
    }
    private void MyUserInfo() //myUserButtonをクリックした時に自分プレーヤープロフィールが書かれた情報を表示
    {
    }
    private void OpponentUserInfo() //opponentUserButtonをクリックした時に相手プレーヤープロフィールが書かれた情報を表示
    {
    }
}
