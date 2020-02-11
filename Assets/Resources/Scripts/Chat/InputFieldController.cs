using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
public class InputFieldController : MonoBehaviour
{
    public void Alphanumeric(string str)
    {
        // 英数字と改行以外を削除
        string text = Regex.Replace(str, "[^0-9a-zA-Z\n]*$", "");
        // InputFieldのTextに反映　
        gameObject.GetComponent<InputField>().text = text;
    }
}