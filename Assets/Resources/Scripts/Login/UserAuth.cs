using UnityEngine;
using NCMB;
public class UserAuth : MonoBehaviour
{

    private string currentPlayerName;
    public bool initialSignUp = false; //SignUpは一度だけ

    // mobile backendに接続してログイン ------------------------

    public void logIn(string id, string pw)
    {

        NCMBUser.LogInAsync(id, pw, (NCMBException e) =>
        {
            // 接続成功したら
            if (e == null)
            {
                currentPlayerName = id;
            }
        });
    }

    // mobile backendに接続して新規会員登録 ------------------------

    public void signUp(string id, string mail, string pw)
    {

        NCMBUser user = new NCMBUser();
        user.UserName = id;
        user.Email = mail;
        user.Password = pw;
        user.SignUpAsync((NCMBException e) =>
        {
            if (e == null)
            {
                currentPlayerName = id;
                logIn(id, pw);
                initialSignUp = true;
            }
        });
    }

    // mobile backendに接続してログアウト ------------------------

    public void logOut()
    {

        NCMBUser.LogOutAsync((NCMBException e) =>
        {
            if (e == null)
            {
                currentPlayerName = null;
            }
        });
    }

    // 現在のプレイヤー名を返す --------------------
    public string currentPlayer()
    {
        return currentPlayerName;
    }

    // シングルトン化する ------------------------
    private UserAuth instance = null;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            string name = gameObject.name;
            gameObject.name = name + "(Singleton)";

            GameObject duplicater = GameObject.Find(name);
            if (duplicater != null)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.name = name;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}