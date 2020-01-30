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
    public UserAuth userAuth;
    // ログイン画面のときtrue, 新規登録画面のときfalse
    private bool isLogIn;

    // テキストボックスで入力される文字列を格納
    public string id;
    public string pw;
    public string mail;
    void awake()
    {
        if (NCMBUser.CurrentUser.IsAuthenticated())
        {
            SceneManager.LoadScene("Lobby");
        }
    }
    void Start()
    {
        // userAuth.logOut(); //ここは後で消す予定(ログイン関係のデバッグが終わったら)
        isLogIn = true;
        guiTextSignUp.SetActive(false);
        guiTextLogIn.SetActive(true);
    }

    void Update()
    {
        if (isLogIn)// ログイン画面
        {
            drawLogInMenu();
        }
        else// 新規登録画面
        {
            drawSignUpMenu();
        }
        // currentPlayerを毎フレーム監視し、ログインが完了したら
        if (NCMBUser.CurrentUser != null)
        {
            SceneManager.LoadScene("Lobby");
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
        isLogIn = false;
    }
    public void ClickSignUp()// 新規登録ボタンが押されたら
    {
        string userId = m_userIdForSignUp.text;
        string password = m_passwordForSignUp.text;
        string mail = m_mailInputField.text;
        SignUp(userId, mail, password);
    }
    void SignUp(string id, string mail, string pw)
    {
        Debug.Log("id: " + id + "pw: " + pw + "mail: " + mail);
        userAuth.signUp(id, mail, pw);
    }
    public void ClickLogin() // ログインボタンが押されたら
    {
        string userId = m_userIdForLogin.text;
        string password = m_passwordForLogin.text;
        Login(userId, password);
    }
    void Login(string id, string pw)
    {
        userAuth.logIn(id, pw);
    }
    public void ClickBack()// 戻るボタンが押されたら
    {
        isLogIn = true;
    }
}