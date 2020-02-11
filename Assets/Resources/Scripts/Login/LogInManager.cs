using UnityEngine;
using NCMB;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LogInManager : MonoBehaviour
{
    [SerializeField] private GameObject guiTextLogIn;   // ログイン画面のゲームオブジェクト
    [SerializeField] private GameObject guiTextSignUp;  // 新規登録画面のゲームオブジェクト
    [SerializeField] private InputField m_userIdForSignUp;
    [SerializeField] private InputField m_userIdForLogin;
    [SerializeField] private InputField m_passwordForSignUp;
    [SerializeField] private InputField m_passwordForLogin;
    [SerializeField] private InputField m_mailInputField;
    [SerializeField] private Toggle m_toggleForLogin;
    [SerializeField] private Toggle m_toggleForSignUp;
    public UserAuth userAuth;
    private LoadingScene loadingScene;
    // テキストボックスで入力される文字列を格納
    public string id;
    public string pw;
    public string mail;
    void Awake()
    { //ここは後で消す予定(ログイン関係のデバッグが終わったら)
        // userAuth.logOut(); //ここは後で消す予定(ログイン関係のデバッグが終わったら)
    }
    void Start()
    {
        GameObject go = GameObject.Find("LoadingScene");
        loadingScene = go.GetComponent<LoadingScene>();
        // if (NCMBUser.CurrentUser != null && !loadingScene.isToLobby && !loadingScene.isOnLobby)
        // // if (NCMBUser.CurrentUser.IsAuthenticated())
        // {
        //     loadingScene.isToLobby = true;
        //     loadingScene.LoadNextScene();
        //     SceneManager.UnloadSceneAsync("LogIn");
        //     // Debug.Log("currentPlayerを毎フレーム監視し、ログインが完了したら");
        // }
        guiTextSignUp.SetActive(false);
        guiTextLogIn.SetActive(true);
    }
    void Update()
    {
        // currentPlayerを毎フレーム監視し、ログインが完了したら
        if (NCMBUser.CurrentUser != null && !loadingScene.isToLobby && !loadingScene.isOnLobby)
        // if (NCMBUser.CurrentUser.IsAuthenticated())
        {
            loadingScene.isToLobby = true;
            loadingScene.LoadNextScene();
            SceneManager.UnloadSceneAsync("LogIn");
            // Debug.Log("currentPlayerを毎フレーム監視し、ログインが完了したら");
        }
    }
    private void drawLogInMenu()
    {
        // テキスト切り替え
        guiTextSignUp.SetActive(false);
        guiTextLogIn.SetActive(true);
    }
    private void drawSignUpMenu()
    {
        // テキスト切り替え
        guiTextLogIn.SetActive(false);
        guiTextSignUp.SetActive(true);
    }
    public void ClickSignUpMenu()// 新規登録画面に移動するボタンが押されたら
    {
        m_userIdForSignUp.text = m_userIdForLogin.text;
        m_passwordForSignUp.text = m_passwordForLogin.text;
        if (this.m_passwordForLogin.contentType == InputField.ContentType.Password)
        {
            this.m_toggleForSignUp.isOn = false;
            this.m_passwordForSignUp.contentType = InputField.ContentType.Password;
        }
        else
        {
            this.m_toggleForSignUp.isOn = true;
            this.m_passwordForSignUp.contentType = InputField.ContentType.Standard;
        }
        drawSignUpMenu();
    }
    public void ClickBack()// 戻るボタンが押されたら
    {
        m_userIdForLogin.text = m_userIdForSignUp.text;
        m_passwordForLogin.text = m_passwordForSignUp.text;
        if (this.m_passwordForSignUp.contentType == InputField.ContentType.Password)
        {
            this.m_toggleForLogin.isOn = false;
            this.m_passwordForLogin.contentType = InputField.ContentType.Password;
        }
        else
        {
            this.m_toggleForLogin.isOn = true;
            this.m_passwordForLogin.contentType = InputField.ContentType.Standard;
        }
        drawLogInMenu();
    }
    public void ClickSignUp()// 新規登録ボタンが押されたら
    {
        id = m_userIdForSignUp.text;
        pw = m_passwordForSignUp.text;
        mail = m_mailInputField.text;
        SignUp(id, mail, pw);
    }
    void SignUp(string id, string mail, string pw)
    {
        Debug.Log("id: " + id + "pw: " + pw + "mail: " + mail);
        userAuth.signUp(id, mail, pw);
    }
    public void ClickLogin() // ログインボタンが押されたら
    {
        id = m_userIdForLogin.text;
        pw = m_passwordForLogin.text;
        Login(id, pw);
    }
    void Login(string id, string pw)
    {
        userAuth.logIn(id, pw);
    }
}