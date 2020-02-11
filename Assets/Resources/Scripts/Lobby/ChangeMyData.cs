using System.Collections.Generic;
using UnityEngine;
using NCMB;
using UnityEngine.UI;
public class ChangeMyData : MonoBehaviour
{
    [SerializeField] private InputField profileText;
    [SerializeField] private GameObject ChangeUserInfoModal;
    [SerializeField] private UserInfoModalController userInfoModalController;
    private UserInfo userInfo;

    // Use this for initialization
    void Start()
    {
        GameObject go1 = GameObject.Find("UserInfo");
        userInfo = go1.GetComponent<UserInfo>();
        if (ChangeUserInfoModal.activeSelf)
        {
            ChangeUserInfoModal.SetActive(false);
        }
    }
    void Update()
    {
        if (profileText.textComponent.cachedTextGenerator.lineCount == 7)
        {
            Debug.Log("7行目に行きました。");
            return;
        }
    }
    public void DefaultText()//現在のProfile情報を表示
    {
        profileText.text = userInfo.myStrArray[2];
    }
    public void OnClickChange()//Changeボタンが確定された時
    {
        if (profileText.text != userInfo.myStrArray[2])
        {
            ChangeMyProfileData(profileText.text);
        }
    }
    public void OpenChangeModal()//ChangeUserInfoModalを開く
    {
        if (!ChangeUserInfoModal.activeSelf)
        {
            ChangeUserInfoModal.SetActive(true);
            DefaultText();
        }
    }
    public void CloseChangeModal()//ChangeUserInfoModalを閉じる
    {
        if (ChangeUserInfoModal.activeSelf)
        {
            ChangeUserInfoModal.SetActive(false);
        }
    }
    public void ChangeMyProfileData(string profileText)//自分のProfile情報を書き換える
    {
        string userId = userInfo.myStrArray[3];
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
                    playerProfile.Add("profileText", profileText);
                    playerProfile.SaveAsync((NCMBException e2) =>
                    {
                        if (e != null)
                        {
                            Debug.Log("取得失敗");
                        }
                        else
                        {
                            //成功時の処理
                            Debug.Log("profileText: " + playerProfile["profileText"]);
                            userInfo.myStrArray[2] = profileText;
                            DefaultText();
                            ChangeUserInfoModal.SetActive(false);
                            userInfoModalController.DrawMyData();
                        }
                    });
                }
            }
        });
    }
}
