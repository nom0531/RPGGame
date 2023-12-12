using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class PlayerDataEditor : EditorWindow
{
    // 対象のデータベース
    static PlayerDataBase m_playerDataBase;
    // 名前一覧
    static List<string> m_nameList = new List<string>();
    // スクロール位置
    Vector2 m_leftScrollPosition = Vector2.zero;
    // 選択中ナンバー
    int m_selectNumber = -1;
    // 検索欄
    SearchField m_searchField;
    string m_searchText = "";

    // ウィンドウを作成
    [MenuItem("Window/PlayerDataBase")]
    static void Open()
    {
        // 読み込み
        m_playerDataBase = AssetDatabase.LoadAssetAtPath<PlayerDataBase>("Assets/Data/PlayerData.asset");
        // 名前を変更
        GetWindow<PlayerDataEditor>("プレイヤーデータベース");

        ResetNameList();

        // 変更を通知
        EditorUtility.SetDirty(m_playerDataBase);
    }

    /// <summary>
    /// ウィンドウのGUI処理
    /// </summary>
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        {
            LeftUpdate();
            NameViewUpdate();
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// ビュー左側の更新処理
    /// </summary>
    void LeftUpdate()
    {
        // サイズを調整
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(160), GUILayout.Height(400));
        {
            // 検索欄
            m_searchField ??= new SearchField();
            GUILayout.Label("名前検索");
            m_searchText = m_searchField.OnToolbarGUI(m_searchText);

            Search();

            m_leftScrollPosition = EditorGUILayout.BeginScrollView(m_leftScrollPosition, GUI.skin.box);
            {
                // データリスト
                for (int i = 0; i < m_nameList.Count; i++)
                {
                    // 色変更
                    if (m_selectNumber == i)
                    {
                        GUI.backgroundColor = Color.cyan;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.white;
                    }

                    // ボタンが押された時の処理
                    if (GUILayout.Button(i + ":" + m_nameList[i]))
                    {
                        // 対象変更
                        m_selectNumber = i;
                        GUI.FocusControl("");
                        Repaint();
                    }
                }
                // 色を戻す
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndScrollView();

            // 項目操作ボタン
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("追加", EditorStyles.miniButtonLeft))
                {
                    AddData();
                }
                if (GUILayout.Button("削除", EditorStyles.miniButtonRight))
                {
                    DeleteData();
                }
            }
            EditorGUILayout.EndHorizontal();

            // 項目数
            GUILayout.Label("項目数:" + m_nameList.Count);
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// ビュー右側の更新処理
    /// </summary>
    void NameViewUpdate()
    {
        if (m_selectNumber < 0)
        {
            return;
        }

        // 右側を更新
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            // 基礎情報を表示
            GUILayout.Label("ID:" + m_selectNumber + "   Name:" + m_nameList[m_selectNumber]);

            // 空白
            EditorGUILayout.Space();

            // 設定欄を表示
            // 名前
            m_playerDataBase.playerDataList[m_selectNumber].PlayerName =
                EditorGUILayout.TextField(
                    "名前",
                    m_playerDataBase.playerDataList[m_selectNumber].PlayerName
                    );
            // 画像
            m_playerDataBase.playerDataList[m_selectNumber].PlayerSprite =
                EditorGUILayout.ObjectField(
                    "画像",
                    m_playerDataBase.playerDataList[m_selectNumber].PlayerSprite,
                    typeof(Sprite), true) as Sprite;
            // 自身のロール
            m_playerDataBase.playerDataList[m_selectNumber].PlayerRoll = 
                (PlayerRoll)EditorGUILayout.Popup(
                    "ロール",
                    (int)m_playerDataBase.playerDataList[m_selectNumber].PlayerRoll,
                    new string[] { "アタッカー", "バッファー", "ヒーラー" }
                    );

            EditorGUILayout.Space();
            ElementViewUpdate();
            EditorGUILayout.Space();

            // ステータス欄
            m_playerDataBase.playerDataList[m_selectNumber].HP =
                EditorGUILayout.IntField(
                    "HP",
                    m_playerDataBase.playerDataList[m_selectNumber].HP
                    );
            m_playerDataBase.playerDataList[m_selectNumber].SP =
                EditorGUILayout.IntField(
                    "SP",
                    m_playerDataBase.playerDataList[m_selectNumber].SP
                    );
            m_playerDataBase.playerDataList[m_selectNumber].ATK =
                EditorGUILayout.IntField(
                    "ATK",
                    m_playerDataBase.playerDataList[m_selectNumber].ATK
                    );
            m_playerDataBase.playerDataList[m_selectNumber].DEF =
                EditorGUILayout.IntField(
                    "DEF",
                    m_playerDataBase.playerDataList[m_selectNumber].DEF
                    );
            m_playerDataBase.playerDataList[m_selectNumber].SPD =
                EditorGUILayout.IntField(
                    "SPD",
                    m_playerDataBase.playerDataList[m_selectNumber].SPD
                    );
            m_playerDataBase.playerDataList[m_selectNumber].LUCK =
                EditorGUILayout.IntField(
                    "LUCK",
                    m_playerDataBase.playerDataList[m_selectNumber].LUCK
                    );

            EditorGUILayout.Space();

            // 値が異常な場合は警告を表示する
            if (m_playerDataBase.playerDataList[m_selectNumber].HP <= 0)
            {
                EditorGUILayout.HelpBox("警告：初期体力が0以下です！", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();

        // 保存
        Undo.RegisterCompleteObjectUndo(m_playerDataBase, "EnemyDataBase");
    }

    /// <summary>
    /// 属性耐性のビューの更新処理
    /// </summary>
    void ElementViewUpdate()
    {
        {
            // 属性耐性
            string[] elementText = { "炎", "氷", "風", "雷", "光", "闇", "無" };

            for (int i = 0; i < (int)ElementType.enNum; i++)
            {
                m_playerDataBase.playerDataList[m_selectNumber].PlayerElement[i] =
                     (ElementResistance)EditorGUILayout.Popup(
                         elementText[i],
                         (int)m_playerDataBase.playerDataList[m_selectNumber].PlayerElement[i],
                         new string[] { "耐性", "弱点", "--" }
                         );
            }

            // 一定以上設定された場合は警告を表示する
            if (m_playerDataBase.playerDataList[m_selectNumber].PlayerElement.Length > (int)ElementType.enNum)
            {
                EditorGUILayout.HelpBox("警告：属性の種類が定義より多く設定されています！", MessageType.Warning);
            }
        }
    }

    /// <summary>
    /// 名前一覧の作成
    /// </summary>
    static void ResetNameList()
    {
        m_nameList.Clear();

        // 名前を入力する
        foreach (PlayerData player in m_playerDataBase.playerDataList)
        {
            m_nameList.Add(player.PlayerName);
        }
    }

    /// <summary>
    /// 検索の処理
    /// </summary>
    void Search()
    {
        if (m_searchText == "")
        {
            return;
        }

        // 初期化
        int startNumber = m_selectNumber;
        startNumber = Mathf.Max(startNumber, 0);

        for (int i = startNumber; i < m_nameList.Count; i++)
        {
            if (m_nameList[i].Contains(m_searchText))
            {
                // ヒットしたら終了
                m_selectNumber = i;
                GUI.FocusControl("");
                Repaint();
                return;
            }
            // ヒットしない場合は-1
            m_selectNumber = -1;
        }
    }

    /// <summary>
    /// データの追加処理
    /// </summary>
    void AddData()
    {
        PlayerData newPlayerData = new PlayerData();

        // 追加
        m_playerDataBase.playerDataList.Add(newPlayerData);
    }

    /// <summary>
    /// データの削除処理
    /// </summary>
    void DeleteData()
    {
        if (m_selectNumber == -1)
        {
            return;
        }

        // 選択位置のデータを削除
        m_playerDataBase.playerDataList.Remove(m_playerDataBase.playerDataList[m_selectNumber]);
        // 調整
        m_selectNumber -= 1;
        m_selectNumber = Mathf.Max(m_selectNumber, 0);
    }
}
