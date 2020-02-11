using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountDownTimer : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    private float totalTime;//　トータル制限時間
    [SerializeField] private int minute;//　制限時間（分）
    [SerializeField] private float seconds;//　制限時間（秒）
    private float oldSeconds;//　前回Update時の秒数
    [SerializeField] private Text timerText;
    void Start()
    {
        totalTime = minute * 60 + seconds;
        oldSeconds = 0f;
    }

    void Update()
    {
        //　制限時間が0秒以下、ゲームがはじまっていないなら何もしない
        // Debug.Log("gameController.isStart: " + gameController.isStart);
        // Debug.Log("totalTime: " + totalTime);
        if (!gameController.isStart)
        {
            return;
        }
        //　一旦トータルの制限時間を計測；
        totalTime = minute * 60 + seconds;
        totalTime -= Time.deltaTime;

        //　再設定
        minute = (int)totalTime / 60;
        seconds = totalTime - minute * 60;

        //　タイマー表示用UIテキストに時間を表示する
        if ((int)seconds != (int)oldSeconds)
        {
            timerText.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00");
        }
        oldSeconds = seconds;
        //　制限時間以下になったら表示する
        if (totalTime <= 0f)
        {
            gameController.GameOver();//GameOverの流れを開始
            Debug.Log("制限時間終了");
        }
    }
}