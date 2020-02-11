using Photon.Pun;
using UnityEngine;
using NCMB;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class SendScore : MonoBehaviour
{
    private PhotonView phView;
    private UserInfo userInfo;
    [SerializeField] private GameObject SendScoreModal;
    [SerializeField] private GameObject SendScorePanel;
    [SerializeField] private GameObject BackRobbyPanel;
    [SerializeField] private GameObject OnSendScorePanel;
    [SerializeField] private GameObject BackLobbyBtn;
    [SerializeField] private GameController gameController;
    private int score;
    private string status;
    private bool isGetOtherSend;
    private bool isSend;

    // Use this for initialization
    private void Awake()
    {
        phView = GetComponent<PhotonView>();
        GameObject go = GameObject.Find("UserInfo");
        userInfo = go.GetComponent<UserInfo>();
    }
    private void Start()
    {
        if (SendScoreModal.activeSelf != false)
        {
            SendScoreModal.SetActive(false);
        }
    }
    public void SetSendScoreModal()//ゲームが終わったら
    {
        if (SendScoreModal.activeSelf == false)
        {
            SendScoreModal.SetActive(true);
            OnSendScorePanel.SetActive(false);
        }
        else
        {
            return;
        }
    }
    private void ChangeOnSendPanel()//パネルの切り替え
    {
        SendScorePanel.SetActive(false);
        OnSendScorePanel.SetActive(true);
        BackLobbyBtn.SetActive(false);
        Invoke("SetDefaultSend", 30f);
    }
    private void ChangeBackRobbyPanel()//パネルの切り替え
    {
        OnSendScorePanel.SetActive(false);
        BackRobbyPanel.SetActive(true);
    }
    private void SendScoreToOther(int score)//相手のスコアを更新
    {
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("PlayerProfile");
        query.WhereEqualTo("userId", userInfo.otherStrArray[3]);
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                Debug.Log("検索失敗"); //検索失敗時の処理
            }
            else
            {
                foreach (NCMBObject obj in objList)
                {
                    string playerProfileId = obj.ObjectId;
                    NCMBObject playerProfile = new NCMBObject("PlayerProfile");
                    playerProfile.ObjectId = playerProfileId;
                    playerProfile.Add("score", score);
                    if (score < 100)
                    {
                        playerProfile.Add("status", "Begginer");//初級者
                        status = "Begginer";
                    }
                    if (score >= 100)
                    {
                        playerProfile.Add("status", "Intermediate");//中級者
                        status = "Intermediate";
                    }
                    if (score >= 500)
                    {
                        playerProfile.Add("status", "Advanced");//上級者
                        status = "Advanced";
                    }
                    if (score >= 1000)
                    {
                        playerProfile.Add("status", "Professional");//プロ級
                        status = "Professional";
                    }
                    playerProfile.SaveAsync((NCMBException e2) =>
                    {
                        if (e != null)
                        {
                            Debug.Log("取得失敗");
                        }
                        else
                        {
                            //成功時の処理
                            SendScoreData(status, score);
                            // gameController.LeaveRoomInGameOver();//部屋から退出
                        }
                    });
                }
            }
        });
    }
    private void SendScoreData(string status, int score) //相手にスコアを渡す
    {
        isSend = true;
        phView.RPC("RPC_RecieveMyScore", RpcTarget.Others, status, score);
        LeaveRoom();//退出できるかどうかを判定しできたら退出
        Invoke("SetDefaultSend", 20f);
    }
    [PunRPC]
    public void RPC_RecieveMyScore(string myStatus, int myscore)
    {
        userInfo.myStrArray[1] = myStatus;
        userInfo.myIntArray[0] = myscore;
        isGetOtherSend = true;
        Debug.Log("RPC_RecieveMyData");
        LeaveRoom();//退出できるかどうかを判定しできたら退出
    }
    public void LeaveRoom()//スコアを渡して受け取ったら退出
    {
        if (isSend && isGetOtherSend)
        {
            StartCoroutine("LeaveRoomCoroutine");
        }
    }
    [PunRPC]
    public void RPC_LeaveRoomDefult()//DefaultSendで退出
    {
        StartCoroutine("LeaveRoomCoroutine");
    }
    IEnumerator LeaveRoomCoroutine()
    {
        SendScorePanel.SetActive(false);
        ChangeBackRobbyPanel();
        yield return new WaitForSeconds(1.4f);
        gameController.LeaveRoomInGameOver();//部屋から退出
    }
    private void SetDefaultSend() //DefaultSendボタンを設置
    {
        BackLobbyBtn.SetActive(true);
    }
    public void DefaultSend()
    {
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("PlayerProfile");
        query.WhereEqualTo("userId", userInfo.myStrArray[3]);
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                Debug.Log("検索失敗"); //検索失敗時の処理
            }
            else
            {
                foreach (NCMBObject obj in objList)
                {
                    string playerProfileId = obj.ObjectId;
                    NCMBObject playerProfile = new NCMBObject("PlayerProfile");
                    score = userInfo.myIntArray[0] + 2;//デフォルトで２ポイントゲット
                    playerProfile.ObjectId = playerProfileId;
                    playerProfile.Add("score", score);
                    if (score < 100)
                    {
                        playerProfile.Add("status", "Begginer");//初級者
                        status = "Begginer";
                    }
                    if (score >= 100)
                    {
                        playerProfile.Add("status", "Intermediate");//中級者
                        status = "Intermediate";
                    }
                    if (score >= 500)
                    {
                        playerProfile.Add("status", "Advanced");//上級者
                        status = "Advanced";
                    }
                    if (score >= 1000)
                    {
                        playerProfile.Add("status", "Professional");//プロ級
                        status = "Professional";
                    }
                    playerProfile.SaveAsync((NCMBException e2) =>
                    {
                        if (e != null)
                        {
                            Debug.Log("取得失敗");
                        }
                        else
                        {
                            //成功時の処理
                            Debug.Log("score: " + playerProfile["score"]);
                            userInfo.myStrArray[1] = status;
                            userInfo.myIntArray[0] = score;
                            phView.RPC("RPC_LeaveRoomDefult", RpcTarget.All);//全員退出
                        }
                    });
                }
            }
        });
    }
    public void OnClickAwsome()
    {
        score = userInfo.otherIntArray[0] + 5;
        ChangeOnSendPanel();
        SendScoreToOther(score);
    }
    public void OnClickExcellent()
    {
        score = userInfo.otherIntArray[0] + 4;
        ChangeOnSendPanel();
        SendScoreToOther(score);
    }
    public void OnClickGood()
    {
        score = userInfo.otherIntArray[0] + 3;
        ChangeOnSendPanel();
        SendScoreToOther(score);
    }
    public void OnClickAverage()
    {
        score = userInfo.otherIntArray[0] + 2;
        ChangeOnSendPanel();
        SendScoreToOther(score);
    }
    public void OnClickPoor()
    {
        score = userInfo.otherIntArray[0] + 1;
        ChangeOnSendPanel();
        SendScoreToOther(score);
    }
}
