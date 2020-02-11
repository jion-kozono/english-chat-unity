using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LoadingScene : MonoBehaviour
{

    private AsyncOperation async;
    [SerializeField] private GameObject LoadingUi;
    [SerializeField] private RectTransform LoadingUiRect;
    [SerializeField] private Slider Slider;
    public bool isToChat;
    public bool isToLobby;
    public bool isToLogin;
    // public bool isToLogin;
    public bool isOnChat;
    public bool isOnLobby;
    public bool isOnLogin;
    public bool isStart;
    void Start()
    {
        isToLogin = true;
        LoadNextScene();
        isStart = true;
    }

    public void LoadNextScene()
    {
        LoadingUi.SetActive(true);
        Slider.value = 0f;
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(0.4f);// 0.8f秒待つ
        if (isToChat)
        {
            async = SceneManager.LoadSceneAsync("chat", LoadSceneMode.Additive);
            isToChat = false;
            isOnChat = true;
        }
        if (isToLobby)
        {
            async = SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Additive);
            isToLobby = false;
        }
        if (isToLogin)
        {
            async = SceneManager.LoadSceneAsync("LogIn", LoadSceneMode.Additive);
            isToLogin = false;
        }
        // while (!async.isDone)
        while (!async.isDone)
        {
            Slider.value = async.progress;
            // Slider.value += 0.18f;
            yield return null;
        }
        Slider.value = 1.0f;
        yield return new WaitForSeconds(1f);// 1f秒待つ
        //シーンの切り替えが完了したら
        LoadingUi.SetActive(false);
        // yield return new WaitForSeconds(1f);// 1f秒待つ
        yield return async;
    }
}
