using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class EnemyDataEditor : EditorWindow
{
    // 対象のデータベース
    private static EnemyDataBase m_enemyDataBase;
    private static SkillDataBase m_skillDataBase;
    private static EnemyMoveDataBase m_enemyMoveDataBase;
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
    [MenuItem("Window/EnemyDataBase")]
    private static void Open()
    {
        // 読み込み
        m_enemyDataBase = AssetDatabase.LoadAssetAtPath<EnemyDataBase>("Assets/Data/EnemyData.asset");
        m_skillDataBase = AssetDatabase.LoadAssetAtPath<SkillDataBase>("Assets/Data/SkillData.asset");
        m_enemyMoveDataBase = AssetDatabase.LoadAssetAtPath<EnemyMoveDataBase>("Assets/Data/EnemyMoveData.asset");
        // 名前を変更
        GetWindow<EnemyDataEditor>("エネミーデータベース");
        // 変更を通知
        EditorUtility.SetDirty(m_enemyDataBase);

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
                for(int i = 0; i < m_nameList.Count; i++)
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
            m_enemyDataBase.enemyDataList[m_selectNumber].ID = m_selectNumber;
            GUILayout.Label($"ID:{m_enemyDataBase.enemyDataList[m_selectNumber].ID}   Name:{m_nameList[m_selectNumber]}");

            // 空白
            EditorGUILayout.Space();

            // 設定欄を表示
            // 名前
            m_enemyDataBase.enemyDataList[m_selectNumber].EnemyName =
                EditorGUILayout.TextField(
                    "名前",
                    m_enemyDataBase.enemyDataList[m_selectNumber].EnemyName
                    );
            // 画像
            m_enemyDataBase.enemyDataList[m_selectNumber].EnemySprite =
                EditorGUILayout.ObjectField(
                    "画像",
                    m_enemyDataBase.enemyDataList[m_selectNumber].EnemySprite,
                    typeof(Sprite), true) as Sprite;
            // サイズ
            m_enemyDataBase.enemyDataList[m_selectNumber].EnemySize =
                (EnemySize)EditorGUILayout.Popup(
                    "サイズ",
                    (int)m_enemyDataBase.enemyDataList[m_selectNumber].EnemySize,
                    new string[] { "極小", "小", "中", "大" }
                    );
            // 難易度
            m_enemyDataBase.enemyDataList[m_selectNumber].LevelState =
                (LevelState)EditorGUILayout.Popup(
                    "難易度",
                    (int)m_enemyDataBase.enemyDataList[m_selectNumber].LevelState,
                    new string[] { "★☆☆☆☆", "★★☆☆☆", "★★★☆☆", "★★★★☆", "★★★★★" }
                    );
            // EP
            m_enemyDataBase.enemyDataList[m_selectNumber].EnhancementPoint =
               EditorGUILayout.IntField(
                   "ドロップするEP",
                   m_enemyDataBase.enemyDataList[m_selectNumber].EnhancementPoint
                   );

            EditorGUILayout.Space();
            DrawElement();
            EditorGUILayout.Space();

            // ステータス欄
            m_enemyDataBase.enemyDataList[m_selectNumber].HP =
                EditorGUILayout.IntField(
                    "HP",
                    m_enemyDataBase.enemyDataList[m_selectNumber].HP
                    );
            m_enemyDataBase.enemyDataList[m_selectNumber].ATK =
                EditorGUILayout.IntField(
                    "ATK",
                    m_enemyDataBase.enemyDataList[m_selectNumber].ATK
                    );
            m_enemyDataBase.enemyDataList[m_selectNumber].DEF =
                EditorGUILayout.IntField(
                    "DEF",
                    m_enemyDataBase.enemyDataList[m_selectNumber].DEF
                    );
            m_enemyDataBase.enemyDataList[m_selectNumber].SPD =
                EditorGUILayout.IntField(
                    "SPD",
                    m_enemyDataBase.enemyDataList[m_selectNumber].SPD
                    );
            m_enemyDataBase.enemyDataList[m_selectNumber].LUCK =
                EditorGUILayout.IntField(
                    "LUCK",
                    m_enemyDataBase.enemyDataList[m_selectNumber].LUCK
                    );

            m_enemyDataBase.enemyDataList[m_selectNumber].HP = 50;
            m_enemyDataBase.enemyDataList[m_selectNumber].DEF = 5;
            m_enemyDataBase.enemyDataList[m_selectNumber].ATK = 10;

            EditorGUILayout.Space();

            DrawSkill();
            DrawMove();

            // 図鑑説明
            GUILayout.Label("図鑑説明");
            m_enemyDataBase.enemyDataList[m_selectNumber].EnemyDetail =
                EditorGUILayout.TextArea(m_enemyDataBase.enemyDataList[m_selectNumber].EnemyDetail);

            // 値が異常な場合は警告を表示する
            if (m_enemyDataBase.enemyDataList[m_selectNumber].HP <= 0)
            {
                EditorGUILayout.HelpBox("警告：初期体力が0以下です！", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();

        // 保存
        Undo.RegisterCompleteObjectUndo(m_enemyDataBase, "EnemyDataBase");
    }

    /// <summary>
    /// 属性耐性
    /// </summary>
    private void DrawElement()
    {
        // 属性耐性
        string[] elementText = { "炎", "氷", "風", "雷", "光", "闇", "無" };

        for (int i = 0; i < (int)ElementType.enNum; i++)
        {
            m_enemyDataBase.enemyDataList[m_selectNumber].EnemyElement[i] =
                 (ElementResistance)EditorGUILayout.Popup(
                     elementText[i],
                     (int)m_enemyDataBase.enemyDataList[m_selectNumber].EnemyElement[i],
                     new string[] { "耐性", "弱点", "--" }
                     );
        }

        // 一定以上設定された場合は警告を表示する
        if (m_enemyDataBase.enemyDataList[m_selectNumber].EnemyElement.Length > (int)ElementType.enNum)
        {
            EditorGUILayout.HelpBox("警告：属性の種類が定義より多く設定されています！", MessageType.Warning);
        }
    }

    /// <summary>
    /// 使用可能スキル
    /// </summary>
    private void DrawSkill()
    {
        for (int skillNumber = 0; skillNumber < m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < m_skillDataBase.skillDataList.Count; dataNumber++)
            {
                // 識別番号が同じならデータを初期化する
                if (m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].ID != m_skillDataBase.skillDataList[dataNumber].ID)
                {
                    continue;
                }
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].SkillName = m_skillDataBase.skillDataList[dataNumber].SkillName;
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].SkillSprite = m_skillDataBase.skillDataList[dataNumber].SkillSprite;
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].POW = m_skillDataBase.skillDataList[dataNumber].POW;
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].SkillElement = m_skillDataBase.skillDataList[dataNumber].SkillElement;
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].EnhancementPoint = m_skillDataBase.skillDataList[dataNumber].EnhancementPoint;
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].SkillNecessary = m_skillDataBase.skillDataList[dataNumber].SkillNecessary;
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].SkillEffect = m_skillDataBase.skillDataList[dataNumber].SkillEffect;
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].EffectScale = m_skillDataBase.skillDataList[dataNumber].EffectScale;
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].Type = m_skillDataBase.skillDataList[dataNumber].Type;
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].BuffType = m_skillDataBase.skillDataList[dataNumber].BuffType;
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].SkillType = m_skillDataBase.skillDataList[dataNumber].SkillType;
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].EffectRange = m_skillDataBase.skillDataList[dataNumber].EffectRange;
                m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList[skillNumber].TargetState = m_skillDataBase.skillDataList[dataNumber].TargetState;
            }
        }
    }

    /// <summary>
    /// 行動パターン
    /// </summary>
    private void DrawMove()
    {
        for (int moveNumber = 0; moveNumber < m_enemyDataBase.enemyDataList[m_selectNumber].enemyMoveList.Count; moveNumber++)
        {
            for (int dataNumber = 0; dataNumber < m_enemyMoveDataBase.enemyMoveDataList.Count; dataNumber++)
            {
                // 識別番号が同じならデータを初期化する
                if (m_enemyDataBase.enemyDataList[m_selectNumber].enemyMoveList[moveNumber].ID != m_enemyMoveDataBase.enemyMoveDataList[dataNumber].ID)
                {
                    continue;
                }
                m_enemyDataBase.enemyDataList[m_selectNumber].enemyMoveList[moveNumber].MoveName = m_enemyMoveDataBase.enemyMoveDataList[dataNumber].MoveName;
                m_enemyDataBase.enemyDataList[m_selectNumber].enemyMoveList[moveNumber].ActorHPState = m_enemyMoveDataBase.enemyMoveDataList[dataNumber].ActorHPState;
                m_enemyDataBase.enemyDataList[m_selectNumber].enemyMoveList[moveNumber].ActorAbnormalState = m_enemyMoveDataBase.enemyMoveDataList[dataNumber].ActorAbnormalState;
                m_enemyDataBase.enemyDataList[m_selectNumber].enemyMoveList[moveNumber].ActionType = m_enemyMoveDataBase.enemyMoveDataList[dataNumber].ActionType;
            }
        }
    }

    /// <summary>
    /// 名前一覧の作成
    /// </summary>
    private static void ResetNameList()
    {
        m_nameList.Clear();

        // 名前を入力する
        foreach (var enemy in m_enemyDataBase.enemyDataList)
        {
            m_nameList.Add(enemy.EnemyName);
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

        for(int i = startNumber; i < m_nameList.Count; i++)
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
        var newEnamyData = new EnemyData();

        // 追加
        m_enemyDataBase.enemyDataList.Add(newEnamyData);
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
        m_enemyDataBase.enemyDataList.Remove(m_enemyDataBase.enemyDataList[m_selectNumber]);
        // 調整
        m_selectNumber -= 1;
        m_selectNumber = Mathf.Max(m_selectNumber, 0);
    }
}
