using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class LevelDataEditor : EditorWindow
{
    private const int MAX_TEXTNUM = 14;

    // 対象のデータベース
    private static LevelDataBase m_levelDataBase;
    // 名前一覧
    private static List<string> m_nameList = new List<string>();
    // スクロール位置
    private Vector2 m_leftScrollPosition = Vector2.zero;
    // 選択中ナンバー
    private int m_selectNumber = -1;
    // 検索欄
    private SearchField m_searchField;
    private string m_searchText = "";

    // ウィンドウを作成
    [MenuItem("Window/LevelDataBase")]
    private static void Open()
    {
        // 読み込み
        m_levelDataBase = AssetDatabase.LoadAssetAtPath<LevelDataBase>("Assets/Data/LevelData.asset");
        // 名前を変更
        GetWindow<LevelDataEditor>("レベルデータベース");
        // 変更を通知
        EditorUtility.SetDirty(m_levelDataBase);

        ResetNameList();
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
    private void LeftUpdate()
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
                    if (GUILayout.Button($"{i}:{m_nameList[i]}"))
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
            GUILayout.Label($"項目数: {m_nameList.Count}");
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// ビュー右側の更新処理
    /// </summary>
    private void NameViewUpdate()
    {
        if (m_selectNumber < 0)
        {
            return;
        }

        // 右側を更新
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            // 基礎情報を表示
            GUILayout.Label($"ID:{m_selectNumber}   Name:{m_nameList[m_selectNumber]}");

            // 空白
            EditorGUILayout.Space();

            // 設定欄を表示
            // 名前
            m_levelDataBase.levelDataList[m_selectNumber].LevelName =
                EditorGUILayout.TextField(
                    "名前",
                    m_levelDataBase.levelDataList[m_selectNumber].LevelName
                    );
            // 難易度
            m_levelDataBase.levelDataList[m_selectNumber].LevelState = 
                (LevelState)EditorGUILayout.Popup(
                    "難易度",
                    (int)m_levelDataBase.levelDataList[m_selectNumber].LevelState,
                    new string[] { "★☆☆☆☆", "★★☆☆☆", "★★★☆☆", "★★★★☆", "★★★★★" }
                    );
            EditorGUILayout.Space();
            // 説明
            GUILayout.Label("説明");
            m_levelDataBase.levelDataList[m_selectNumber].LevelDetail =
                EditorGUILayout.TextArea(m_levelDataBase.levelDataList[m_selectNumber].LevelDetail);

            // 警告を表示
            if (m_levelDataBase.levelDataList[m_selectNumber].LevelName.Length >= MAX_TEXTNUM)
            {
                EditorGUILayout.HelpBox("警告：クエスト名の文字数が多すぎます！", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();
        // 保存
        Undo.RegisterCompleteObjectUndo(m_levelDataBase, "LevelDataBase");
    }

    /// <summary>
    /// 名前一覧の作成
    /// </summary>
    private static void ResetNameList()
    {
        m_nameList.Clear();
        // 名前を入力する
        foreach (var level in m_levelDataBase.levelDataList)
        {
            m_nameList.Add(level.LevelName);
        }
    }

    /// <summary>
    /// 検索の処理
    /// </summary>
    private void Search()
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
    private void AddData()
    {
        var newLevelData = new LevelData();
        // 追加
        m_levelDataBase.levelDataList.Add(newLevelData);
    }

    /// <summary>
    /// データの削除処理
    /// </summary>
    private void DeleteData()
    {
        if (m_selectNumber == -1)
        {
            return;
        }
        // 選択位置のデータを削除
        m_levelDataBase.levelDataList.Remove(m_levelDataBase.levelDataList[m_selectNumber]);
        // 調整
        m_selectNumber -= 1;
        m_selectNumber = Mathf.Max(m_selectNumber, 0);
    }
}
