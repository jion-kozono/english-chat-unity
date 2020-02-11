using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PwController : MonoBehaviour
{

    [SerializeField] private UnityEngine.UI.InputField passwordInputForSignUp = null;
    [SerializeField] private UnityEngine.UI.InputField passwordInputForLogin = null;
    // void Start(){

    // }
    // void Update(){

    // }
    public void ToggleInputType()
    {
        if (this.passwordInputForSignUp != null)//signup画面
        {
            if (this.passwordInputForSignUp.contentType == InputField.ContentType.Password)
            {
                this.passwordInputForSignUp.contentType = InputField.ContentType.Standard;
            }
            else
            {
                this.passwordInputForSignUp.contentType = InputField.ContentType.Password;
            }
            this.passwordInputForSignUp.ForceLabelUpdate();
        }
        if (this.passwordInputForLogin != null)//ログイン画面
        {
            if (this.passwordInputForLogin.contentType == InputField.ContentType.Password)
            {
                this.passwordInputForLogin.contentType = InputField.ContentType.Standard;
            }
            else
            {
                this.passwordInputForLogin.contentType = InputField.ContentType.Password;
            }
            this.passwordInputForLogin.ForceLabelUpdate();
        }
    }
}
