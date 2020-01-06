using Photon.Pun;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RoomListScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    public EnhancedScroller m_scroller;
    public CellView m_cellPrefab;
    private List<ScrollerData> _data;　// インプットしたデータのリスト

    // Use this for initialization
    private void Start()
    {
        // 各データリストを作成
        _data = new List<ScrollerData>();
        m_scroller.Delegate = this;　// Scrollerにデリゲート登録
        m_scroller.ReloadData();　// ReloadDataをするとビューが更新される
    }

    private void OnRecieve(string text) // textにはphotonView.RPC関数の引数が入っている（https://doc.photonengine.com/ja-jp/pun/current/gameplay/rpcsandraiseevent）
    {
        // RpcTarget.Allで呼び出すと自分と他の人のOnRecieveが呼び出される
        // この関数でデータの追加と画面の更新の処理を行う

        // _data.Add(new ScrollerData { m_input_data = text, m_height = _height, m_width = _width }); // インプットしたデータを格納する
        m_scroller.ReloadData(); // ReloadDataをするとビューが更新さ
        m_scroller.JumpToDataIndex(_data.Count - 1, 1, 1, true, EnhancedScroller.TweenType.easeInSine, 0); //ここにスクローラーを下にする処理を書くれる
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
