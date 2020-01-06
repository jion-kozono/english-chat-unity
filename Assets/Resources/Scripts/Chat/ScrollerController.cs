using Photon.Pun;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class ScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private PhotonView photonView;
    public EnhancedScroller m_scroller;
    public CellView m_cellPrefab;
    public TextGenerationSettings settings;
    private List<ScrollerData> _data;　// インプットしたデータのリスト
    private float _height;
    private float _width;
    [SerializeField] private InputField field;
    [SerializeField] private RectTransform fieldRect;
    public TurnManager turnManager;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        // 各データリストを作成
        _data = new List<ScrollerData>();

        m_scroller.Delegate = this;　// Scrollerにデリゲート登録
        m_scroller.ReloadData();　// ReloadDataをするとビューが更新される
    }
    private void OnSubmit() // テキストを入力し、Sendボタンを押したら呼ばれる
    {
        if (turnManager.canChat == true && field.text != string.Empty) // チャットが打てる、field.textが空白の時は呼ばない
        {
            photonView.RPC("OnRecieve", RpcTarget.All, field.text);
            field.text = string.Empty; // 入力フィールドは初期化する
            turnManager.MakeTurn(1);
        }
    }

    [PunRPC]
    private void OnRecieve(string text, PhotonMessageInfo info) // textにはphotonView.RPC関数の引数が入っている（https://doc.photonengine.com/ja-jp/pun/current/gameplay/rpcsandraiseevent）
    {
        // RpcTarget.Allで呼び出すと自分と他の人のOnRecieveが呼び出される
        // この関数でデータの追加と画面の更新の処理を行う
        Debug.LogFormat("Info: {0} {1} {2}", info.Sender, info.photonView.IsMine, info.SentServerTime);
        Vector2 size = GetTextSize(text, fieldRect.rect.width - 20);
        _height = size.y + 80;
        _width = size.x;
        _data.Add(new ScrollerData { m_input_data = text, m_height = _height, m_width = _width }); // インプットしたデータを格納する
        m_scroller.ReloadData(); // ReloadDataをするとビューが更新さ
        m_scroller.JumpToDataIndex(_data.Count - 1, 1, 1, true, EnhancedScroller.TweenType.easeInSine, 0); //ここにスクローラーを下にする処理を書くれる
    }
    private Vector2 GetTextSize(string str, float maxWidth)
    {
        TextGenerationSettings settings = field.textComponent.GetGenerationSettings(new Vector2(maxWidth, 0));
        // ここはサイズを求めたいテキストの設定にする。
        settings.alignByGeometry = false;
        settings.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        settings.fontSize = 22;
        settings.fontStyle = FontStyle.Normal;
        settings.generateOutOfBounds = false;
        settings.generationExtents = new Vector2(maxWidth, 32.0F);
        settings.horizontalOverflow = HorizontalWrapMode.Wrap;
        settings.pivot = new Vector2(0.5F, 0.5F);
        settings.resizeTextForBestFit = false;
        settings.richText = true;
        settings.textAnchor = TextAnchor.MiddleCenter;
        settings.updateBounds = false;
        settings.verticalOverflow = VerticalWrapMode.Truncate;

        TextGenerator generator = new TextGenerator();
        float width = Mathf.Min(generator.GetPreferredWidth(str, settings), maxWidth);
        float height = generator.GetPreferredHeight(str, settings);
        return new Vector2(width, height);
    }

    public int GetNumberOfCells(EnhancedScroller scroller)　//自動で呼ばれる
    {
        return _data.Count; //データの数を渡す
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) //自動で呼ばれる
    {
        return _data[dataIndex].m_height;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) //自動で呼ばれる
    {
        var cellView = scroller.GetCellView(m_cellPrefab) as CellView; //CellViewプレハブを取得し、cellViewに格納
        cellView.SetData(_data[dataIndex]); //cellViewにインデックスに合ったデータを渡す
        return cellView;　//データの入ったCellViewが生成される
    }
}