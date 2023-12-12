using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class SkillDataEditor : EditorWindow
{
    // 対象のデータベース
    static SkillDataBase m_skillDataBase;
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
    [MenuItem("Window/SkillDataBase")]
    static void Open()
    {
        // 読み込み
        m_skillDataBase = AssetDatabase.LoadAssetAtPath<SkillDataBase>("Assets/Data/SkillData.asset");
        // 名前を変更
        GetWindow<SkillDataEditor>("スキルデータベース");
        // 変更を通知
        EditorUtility.SetDirty(m_skillDataBase);

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
            m_skillDataBase.skillDataList[m_selectNumber].SkillNumber = m_selectNumber;
            GUILayout.Label("ID:" + m_skillDataBase.skillDataList[m_selectNumber].SkillNumber + "   Name:" + m_nameList[m_selectNumber]);

            // 空白
            EditorGUILayout.Space();

            // 設定欄を表示
            // 名前
            m_skillDataBase.skillDataList[m_selectNumber].SkillName =
                EditorGUILayout.TextField(
                    "名前",
                    m_skillDataBase.skillDataList[m_selectNumber].SkillName
                    );
            // 画像
            m_skillDataBase.skillDataList[m_selectNumber].SkillSprite =
                EditorGUILayout.ObjectField(
                    "画像",
                    m_skillDataBase.skillDataList[m_selectNumber].SkillSprite,
                    typeof(Sprite), true) as Sprite;
            // 属性
            m_skillDataBase.skillDataList[m_selectNumber].SkillElement =
                 (ElementType)EditorGUILayout.Popup(
                     "属性",
                     (int)m_skillDataBase.skillDataList[m_selectNumber].SkillElement,
                     new string[] { "炎", "氷", "風", "雷", "光", "闇", "無", "--" });
            // タイプ
            m_skillDataBase.skillDataList[m_selectNumber].SkillType =
                 (SkillType)EditorGUILayout.Popup(
                     "スキルタイプ",
                     (int)m_skillDataBase.skillDataList[m_selectNumber].SkillType,
                     new string[] { "攻撃", "バフ", "デバフ", "回復", "復活" });

            EditorGUILayout.Space();

            // 追加効果
            Draw();

            EditorGUILayout.Space();

            // 消費するデータ
            m_skillDataBase.skillDataList[m_selectNumber].Type =
                (NecessaryType)EditorGUILayout.Popup(
                    "消費するデータの種類",
                    (int)m_skillDataBase.skillDataList[m_selectNumber].Type,
                    new string[] { "SP", "HP" });
            m_skillDataBase.skillDataList[m_selectNumber].SkillNecessary =
                EditorGUILayout.IntField(
                    "消費する値",
                m_skillDataBase.skillDataList[m_selectNumber].SkillNecessary);
            // 基礎攻撃力・回復力
            m_skillDataBase.skillDataList[m_selectNumber].POW =
                EditorGUILayout.IntField(
                    "基礎値(成功率)",
                    m_skillDataBase.skillDataList[m_selectNumber].POW);

            EditorGUILayout.Space();
            // スキルの効果範囲
            m_skillDataBase.skillDataList[m_selectNumber].EffectRange =
                (EffectRange)EditorGUILayout.Popup(
                    "効果範囲",
                    (int)m_skillDataBase.skillDataList[m_selectNumber].EffectRange,
                    new string[] { "単体", "全体" });
            // バフのタイプ
            if (m_skillDataBase.skillDataList[m_selectNumber].SkillType != SkillType.enBuff
                || m_skillDataBase.skillDataList[m_selectNumber].SkillType != SkillType.enDeBuff)
            {
                // タイプ無し
                m_skillDataBase.skillDataList[m_selectNumber].BuffType = BuffType.enNull;
            }
            m_skillDataBase.skillDataList[m_selectNumber].BuffType =
                (BuffType)EditorGUILayout.Popup(
                    "バフタイプ",
                    (int)m_skillDataBase.skillDataList[m_selectNumber].BuffType,
                    new string[] { "ATK", "DEF", "SPD", "--" });

            EditorGUILayout.Space();

            // スキル開放に必要なEP
            m_skillDataBase.skillDataList[m_selectNumber].EnhancementPoint =
                EditorGUILayout.IntField(
                    "必要EP",
                    m_skillDataBase.skillDataList[m_selectNumber].EnhancementPoint
                    );

            EditorGUILayout.Space();

            // 図鑑説明
            GUILayout.Label("図鑑説明");
            m_skillDataBase.skillDataList[m_selectNumber].SkillDetail =
                EditorGUILayout.TextArea(m_skillDataBase.skillDataList[m_selectNumber].SkillDetail);
        }
        EditorGUILayout.EndVertical();

        // 保存
        Undo.RegisterCompleteObjectUndo(m_skillDataBase, "SkillDataBase");
    }

    /// <summary>
    /// 追加効果を表示する
    /// </summary>
    private void Draw()
    {
        // 名前
        m_skillDataBase.skillDataList[m_selectNumber].StateAbnormalData.StateName =
                EditorGUILayout.TextField(
                    "追加効果:",
                    m_skillDataBase.skillDataList[m_selectNumber].StateAbnormalData.StateName
                    );
        // 画像
        m_skillDataBase.skillDataList[m_selectNumber].StateAbnormalData.StateImage = 
            EditorGUILayout.ObjectField(
                    "画像",
                    m_skillDataBase.skillDataList[m_selectNumber].SkillSprite,
                    typeof(Sprite), true) as Sprite;
        // 必要ターン数
        m_skillDataBase.skillDataList[m_selectNumber].StateAbnormalData.RequiredTime =
            EditorGUILayout.IntField(
                "解除までのターン数",
                m_skillDataBase.skillDataList[m_selectNumber].StateAbnormalData.RequiredTime
                );
    }

    /// <summary>
    /// 名前一覧の作成
    /// </summary>
    static void ResetNameList()
    {
        m_nameList.Clear();

        // 名前を入力する
        foreach (SkillData skill in m_skillDataBase.skillDataList)
        {
            m_nameList.Add(skill.SkillName);
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
        SkillData newSkillData = new SkillData();

        // 追加
        m_skillDataBase.skillDataList.Add(newSkillData);
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
        m_skillDataBase.skillDataList.Remove(m_skillDataBase.skillDataList[m_selectNumber]);
        // 調整
        m_selectNumber -= 1;
        m_selectNumber = Mathf.Max(m_selectNumber, 0);
    }
}
