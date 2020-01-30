using UnityEngine;

public class MenuModalController : MonoBehaviour
{
    [SerializeField] private GameObject MenuModal;
    // private UserInfoModalController userInfoModalController;
    // Use this for initialization
    void Start()
    {
        if (MenuModal.activeSelf != false)
        {
            MenuModal.SetActive(false);
        }
    }
    public void OpenMenuModal()//MenuModalを開く
    {
        if (MenuModal.activeSelf == false)
        {
            // if (userInfoModalController.IsUserInfoModalSetActive)
            // {
            //     userInfoModalController.CloseUserInfoModal();
            // }
            MenuModal.SetActive(true);
        }
        else
        {
            return;
        }
    }
    public void CloseMenuModal()//MenuModalを閉じる
    {
        if (MenuModal.activeSelf != false)
        {
            MenuModal.SetActive(false);
        }
    }
}
