using UnityEngine;
using UnityEngine.SceneManagement;
public class UserInfoModalController : MonoBehaviour
{
    [SerializeField] private GameObject userInfoModal;
    private UserInfo userInfo;
    private TradeUserInfo tradeUserInfo;
    public bool IsUserInfoModalSetActive = false;
    void Start()
    {
        GameObject go1 = GameObject.Find("UserInfo");
        userInfo = go1.GetComponent<UserInfo>();
        if (SceneManager.GetActiveScene().name == "Chat")//チャットの時だけ
        {
            GameObject go2 = GameObject.Find("TradeUserInfo");
            tradeUserInfo = go2.GetComponent<TradeUserInfo>();
        }
        if (userInfoModal.activeSelf != false)
        {
            userInfoModal.SetActive(false);
            IsUserInfoModalSetActive = false;
        }
    }
    public void OpenMuUserInfoModal()//自分のUserInfoModalを開く
    {
        if (!IsUserInfoModalSetActive && userInfo.isGetMyData)
        {
            userInfoModal.SetActive(true);
            userInfo.DrawMyData();//自分のデータを描画
            IsUserInfoModalSetActive = true;
        }
        else
        {
            return;
        }
    }
    public void OpenOtherUserInfoModal()//相手のUserInfoModalを開く
    {
        Debug.Log(tradeUserInfo.isGetOtherData);
        if (!IsUserInfoModalSetActive && tradeUserInfo.isGetOtherData)
        {
            userInfoModal.SetActive(true);
            tradeUserInfo.DrawOtherData();//相手のデータを描画
            IsUserInfoModalSetActive = true;
        }
        else
        {
            return;
        }
    }
    public void CloseUserInfoModal()//UserInfoModalを閉じる
    {
        if (IsUserInfoModalSetActive)
        {
            userInfoModal.SetActive(false);
            userInfo.isDrawMyData = false;
            if (SceneManager.GetActiveScene().name == "Chat")//チャットの時だけ
            {
                tradeUserInfo.isDrawOtherData = false;
            }
            IsUserInfoModalSetActive = false;
        }
    }
}
