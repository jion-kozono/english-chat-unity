using Photon.Pun;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class CellView : EnhancedScrollerCellView
{
    private PhotonView photonView;
    [SerializeField] private RectTransform m_leftRootUI = null;
    [SerializeField] private RectTransform m_rightRootUI = null;
    [SerializeField] private Image m_leftArrowUI = null;
    [SerializeField] private Image m_rightArrowUI = null;
    [SerializeField] private Image m_leftFrameUI = null;
    [SerializeField] private Image m_rightFrameUI = null;
    [SerializeField] private RectTransform m_leftFrameRectUI = null;
    [SerializeField] private RectTransform m_rightFrameRectUI = null;
    [SerializeField] private Text m_leftTextUI = null;
    [SerializeField] private Text m_rightTextUI = null;
    [SerializeField] private RectTransform m_leftTextRectUI = null;
    [SerializeField] private RectTransform m_rightTextRectUI = null;

    // データに保存したテキストを表示するテキストに置換
    public void SetData(ScrollerData data)
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log(photonView.IsMine);
        if (photonView.IsMine == false)
        {
            m_rightRootUI.gameObject.SetActive(false);
            m_leftTextUI.text = data.m_input_data;
            m_leftTextRectUI.sizeDelta = new Vector2(data.m_width, data.m_height - 80);
            // m_leftTextRectUI.sizeDelta = new Vector2(m_leftTextUI.preferredWidth, m_leftTextUI.preferredHeight);//再度、ピッタリ収まるようにサイズ変更

            var callOutContentSize = m_leftTextRectUI.sizeDelta; //吹き出しの中身の大きさ
            var callOutSize = callOutContentSize + new Vector2(20, 20); //吹き出しの大きさ
            var color = new Color32(255, 255, 255, 255);

            m_leftFrameRectUI.sizeDelta = callOutSize;
            m_leftFrameUI.color = color;
            m_leftArrowUI.color = color;
        }
        else
        {
            m_leftRootUI.gameObject.SetActive(false);
            m_rightTextUI.text = data.m_input_data;
            m_rightTextRectUI.sizeDelta = new Vector2(data.m_width, data.m_height - 80);
            // m_rightTextRectUI.sizeDelta = new Vector2(m_rightTextUI.preferredWidth, m_rightTextUI.preferredHeight);//再度、ピッタリ収まるようにサイズ変更

            var callOutContentSize = m_rightTextRectUI.sizeDelta; //吹き出しの中身の大きさ
            var callOutSize = callOutContentSize + new Vector2(20, 20); //吹き出しの大きさ
            var color = new Color32(255, 255, 255, 255);

            m_rightFrameRectUI.sizeDelta = callOutSize;
            m_rightFrameUI.color = color;
            m_rightArrowUI.color = color;
        }
    }

}