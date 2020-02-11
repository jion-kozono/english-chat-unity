using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NCMB;
public class VersionChecker : MonoBehaviour
{
    [SerializeField] private GameObject UpdateModal;
    private string version;
    void Start()
    {
        if (UpdateModal.activeSelf != false)
        {
            UpdateModal.SetActive(false);
        }
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            VersionCheckForAndroid();
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            VersionCheckForAndroid();
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VersionCheckForIOS();
        }
    }
    public void VersionCheckForAndroid()//ncmbとバージョン比較
    {
        bool isLatest = true;
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("AndroidVersion");
        query.WhereEqualTo("isLatest", isLatest);
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
                    string AndroidVersionId = obj.ObjectId;
                    NCMBObject AndroidVersionObject = new NCMBObject("AndroidVersion");
                    AndroidVersionObject.ObjectId = AndroidVersionId;
                    AndroidVersionObject.FetchAsync((NCMBException e2) =>
                    {
                        if (e2 != null)
                        {
                            Debug.Log("取得失敗時");//取得失敗時の処理
                        }
                        else
                        {
                            version = (string)AndroidVersionObject["version"];
                            if (version != Application.version)
                            {
                                SetUpdateModal();
                            }
                        }
                    });
                }
            }
        });
    }
    public void VersionCheckForIOS()//ncmbとバージョン比較
    {
        bool isLatest = true;
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("IosVerison");
        query.WhereEqualTo("isLatest", isLatest);
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
                    string IosVerisonId = obj.ObjectId;
                    NCMBObject IosVerisonObject = new NCMBObject("IosVerison");
                    IosVerisonObject.ObjectId = IosVerisonId;
                    IosVerisonObject.FetchAsync((NCMBException e2) =>
                    {
                        if (e2 != null)
                        {
                            Debug.Log("取得失敗時");//取得失敗時の処理
                        }
                        else
                        {
                            version = (string)IosVerisonObject["version"];
                            if (version != Application.version)
                            {
                                SetUpdateModal();
                            }
                        }
                    });
                }
            }
        });
    }
    public void SetUpdateModal()
    {
        UpdateModal.SetActive(true);
    }
    public void ClickUpdateBtn()
    {
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.installerName);
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.installerName);
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Application.OpenURL(string.Format("itms-apps://itunes.apple.com/app/id{0}?mt=8", version));
        }
    }
}