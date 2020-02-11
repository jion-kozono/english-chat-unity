using NCMB;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text myUserText;
    // Use this for initialization
    void Start()
    {
        myUserText.text = NCMBUser.CurrentUser.UserName;
        Debug.Log("myUserText.text: " + myUserText.text);
    }
}
