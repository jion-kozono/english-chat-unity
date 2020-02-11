using System.Collections.Generic;
using UnityEngine;
using NCMB;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class Theme : MonoBehaviour
{
    private PhotonView phoView;
    [SerializeField] private Text themeText;
    string theme;
    void Start()
    {
        phoView = GetComponent<PhotonView>();
    }
    public void BringTheme()//テーマを持ってくる
    {
        int id = Random.Range(1, 5);//これではリアルタイムの変更ができない
        bool isVisible = true;
        // PlayerProfileを検索するクラスを作成
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("Theme");
        query.WhereEqualTo("id", id);
        query.WhereEqualTo("isVisible", isVisible);
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
                    string themeId = obj.ObjectId;
                    NCMBObject themeObject = new NCMBObject("Theme");
                    themeObject.ObjectId = themeId;
                    themeObject.FetchAsync((NCMBException e2) =>
                    {
                        if (e2 != null)
                        {
                            Debug.Log("取得失敗時");//取得失敗時の処理
                        }
                        else
                        {
                            theme = (string)themeObject["theme"];
                            phoView.RPC("RPC_SetTheme", RpcTarget.AllViaServer, theme);
                        }
                    });
                }
            }
        });
    }
    [PunRPC]
    public void RPC_SetTheme(string theme)
    {
        themeText.text = "Let's Talk About \"" + theme + "\"!";
    }
}
