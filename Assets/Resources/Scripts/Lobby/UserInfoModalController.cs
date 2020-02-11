using UnityEngine;
using UnityEngine.UI;
public class UserInfoModalController : MonoBehaviour
{
    [SerializeField] private GameObject userInfoModal;
    [SerializeField] private Text PlayerNameText;
    [SerializeField] private Text StatusText;
    [SerializeField] private Text ScoreText;
    [SerializeField] private Text LevelText;
    [SerializeField] private Text CharmText;
    [SerializeField] private Text ProfileText;
    private UserInfo userInfo;
    private TradeUserInfo tradeUserInfo;
    public bool IsUserInfoModalSetActive = false;
    private bool isGetTradeUserInfo;
    void Start()
    {
        GameObject go1 = GameObject.Find("UserInfo");
        userInfo = go1.GetComponent<UserInfo>();
        if (userInfoModal.activeSelf != false)
        {
            userInfoModal.SetActive(false);
            IsUserInfoModalSetActive = false;
        }
    }
    public void DrawMyData()
    {
        PlayerNameText.text = "Name: " + userInfo.myStrArray[0];
        StatusText.text = "Status: " + userInfo.myStrArray[1];
        ScoreText.text = "Score: " + userInfo.myIntArray[0].ToString();
        LevelText.text = "Level： " + userInfo.myIntArray[1].ToString();
        CharmText.text = "Charm： " + userInfo.myIntArray[2].ToString();
        ProfileText.text = "AboutMe：\n" + userInfo.myStrArray[2];
    }
    private void DrawOtherData()
    {
        PlayerNameText.text = "Name: " + userInfo.otherStrArray[0];
        StatusText.text = "Status: " + userInfo.otherStrArray[1];
        ScoreText.text = "Score: " + userInfo.otherIntArray[0].ToString();
        LevelText.text = "Level： " + userInfo.otherIntArray[1].ToString();
        CharmText.text = "Charm： " + userInfo.otherIntArray[2].ToString();
        ProfileText.text = "AboutMe：\n" + userInfo.otherStrArray[2];
    }
    public void OpenMuUserInfoModal()//自分のUserInfoModalを開く
    {
        // Debug.Log(userInfo.isGetMyData);
        if (!IsUserInfoModalSetActive && userInfo.isGetMyData)//StoreMyData()でuserInfo.isGetMyDataがtrueになっていたら
        {
            userInfoModal.SetActive(true);
            DrawMyData();//自分のデータを描画
            IsUserInfoModalSetActive = true;
        }
        else
        {
            return;
        }
    }
    public void OpenOtherUserInfoModal()//相手のUserInfoModalを開く
    {
        if (!isGetTradeUserInfo)
        {
            GameObject go2 = GameObject.Find("TradeUserInfo");
            tradeUserInfo = go2.GetComponent<TradeUserInfo>();
        }
        if (!IsUserInfoModalSetActive && tradeUserInfo.isGetOtherData)
        {
            userInfoModal.SetActive(true);
            DrawOtherData();//相手のデータを描画
            IsUserInfoModalSetActive = true;
            isGetTradeUserInfo = true;
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
            IsUserInfoModalSetActive = false;
        }
    }
}
