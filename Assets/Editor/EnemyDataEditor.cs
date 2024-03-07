using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class EnemyDataEditor : EditorWindow
{
    // �Ώۂ̃f�[�^�x�[�X
    private static EnemyDataBase m_enemyDataBase;
    private static SkillDataBase m_skillDataBase;
    private static EnemyMoveDataBase m_enemyMoveDataBase;
    // ���O�ꗗ
    private static List<string> m_nameList = new List<string>();
    // �X�N���[���ʒu
    private Vector2 m_leftScrollPosition = Vector2.zero;
    // �I�𒆃i���o�[
    private int m_selectNumber = -1;
    // ������
    private SearchField m_searchField;
    private string m_searchText = "";

    // �E�B���h�E���쐬
    [MenuItem("Window/EnemyDataBase")]
    private static void Open()
    {
        // �ǂݍ���
        m_enemyDataBase = AssetDatabase.LoadAssetAtPath<EnemyDataBase>("Assets/Data/EnemyData.asset");
        m_skillDataBase = AssetDatabase.LoadAssetAtPath<SkillDataBase>("Assets/Data/SkillData.asset");
        m_enemyMoveDataBase = AssetDatabase.LoadAssetAtPath<EnemyMoveDataBase>("Assets/Data/EnemyMoveData.asset");
        // ���O��ύX
        GetWindow<EnemyDataEditor>("�G�l�~�[�f�[�^�x�[�X");
        // �ύX��ʒm
        EditorUtility.SetDirty(m_enemyDataBase);

        ResetNameList();
    }

    /// <summary>
    /// �E�B���h�E��GUI����
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
    /// �r���[�����̍X�V����
    /// </summary>
    private void LeftUpdate()
    {
        // �T�C�Y�𒲐�
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(160), GUILayout.Height(400));
        {
            // ������
            m_searchField ??= new SearchField();
            GUILayout.Label("���O����");
            m_searchText = m_searchField.OnToolbarGUI(m_searchText);

            Search();

            m_leftScrollPosition = EditorGUILayout.BeginScrollView(m_leftScrollPosition, GUI.skin.box);
            {
                // �f�[�^���X�g
                for(int i = 0; i < m_nameList.Count; i++)
                {
                    // �F�ύX
                    if (m_selectNumber == i)
                    {
                        GUI.backgroundColor = Color.cyan;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.white;
                    }

                    // �{�^���������ꂽ���̏���
                    if (GUILayout.Button($"{i}:{m_nameList[i]}"))
                    {
                        // �ΏەύX
                        m_selectNumber = i;
                        GUI.FocusControl("");
                        Repaint();
                    }
                }
                // �F��߂�
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndScrollView();

            // ���ڑ���{�^��
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("�ǉ�", EditorStyles.miniButtonLeft))
                {
                    AddData();
                }
                if (GUILayout.Button("�폜", EditorStyles.miniButtonRight))
                {
                    DeleteData();
                }
            }
            EditorGUILayout.EndHorizontal();

            // ���ڐ�
            GUILayout.Label($"���ڐ�: {m_nameList.Count}");
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// �r���[�E���̍X�V����
    /// </summary>
    private void NameViewUpdate()
    {
        if (m_selectNumber < 0)
        {
            return;
        }

        // �E�����X�V
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            // ��b����\��
            m_enemyDataBase.enemyDataList[m_selectNumber].ID = m_selectNumber;
            GUILayout.Label($"ID:{m_enemyDataBase.enemyDataList[m_selectNumber].ID}   Name:{m_nameList[m_selectNumber]}");

            // ��
            EditorGUILayout.Space();

            // �ݒ藓��\��
            // ���O
            m_enemyDataBase.enemyDataList[m_selectNumber].EnemyName =
                EditorGUILayout.TextField(
                    "���O",
                    m_enemyDataBase.enemyDataList[m_selectNumber].EnemyName
                    );
            // �摜
            m_enemyDataBase.enemyDataList[m_selectNumber].EnemySprite =
                EditorGUILayout.ObjectField(
                    "�摜",
                    m_enemyDataBase.enemyDataList[m_selectNumber].EnemySprite,
                    typeof(Sprite), true) as Sprite;
            // �T�C�Y
            m_enemyDataBase.enemyDataList[m_selectNumber].EnemySize =
                (EnemySize)EditorGUILayout.Popup(
                    "�T�C�Y",
                    (int)m_enemyDataBase.enemyDataList[m_selectNumber].EnemySize,
                    new string[] { "�ɏ�", "��", "��", "��" }
                    );
            // ��Փx
            m_enemyDataBase.enemyDataList[m_selectNumber].LevelState =
                (LevelState)EditorGUILayout.Popup(
                    "��Փx",
                    (int)m_enemyDataBase.enemyDataList[m_selectNumber].LevelState,
                    new string[] { "����������", "����������", "����������", "����������", "����������" }
                    );
            // EP
            m_enemyDataBase.enemyDataList[m_selectNumber].EnhancementPoint =
               EditorGUILayout.IntField(
                   "�h���b�v����EP",
                   m_enemyDataBase.enemyDataList[m_selectNumber].EnhancementPoint
                   );

            EditorGUILayout.Space();
            DrawElement();
            EditorGUILayout.Space();

            // �X�e�[�^�X��
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

            // �}�Ӑ���
            GUILayout.Label("�}�Ӑ���");
            m_enemyDataBase.enemyDataList[m_selectNumber].EnemyDetail =
                EditorGUILayout.TextArea(m_enemyDataBase.enemyDataList[m_selectNumber].EnemyDetail);

            // �l���ُ�ȏꍇ�͌x����\������
            if (m_enemyDataBase.enemyDataList[m_selectNumber].HP <= 0)
            {
                EditorGUILayout.HelpBox("�x���F�����̗͂�0�ȉ��ł��I", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();

        // �ۑ�
        Undo.RegisterCompleteObjectUndo(m_enemyDataBase, "EnemyDataBase");
    }

    /// <summary>
    /// �����ϐ�
    /// </summary>
    private void DrawElement()
    {
        // �����ϐ�
        string[] elementText = { "��", "�X", "��", "��", "��", "��", "��" };

        for (int i = 0; i < (int)ElementType.enNum; i++)
        {
            m_enemyDataBase.enemyDataList[m_selectNumber].EnemyElement[i] =
                 (ElementResistance)EditorGUILayout.Popup(
                     elementText[i],
                     (int)m_enemyDataBase.enemyDataList[m_selectNumber].EnemyElement[i],
                     new string[] { "�ϐ�", "��_", "--" }
                     );
        }

        // ���ȏ�ݒ肳�ꂽ�ꍇ�͌x����\������
        if (m_enemyDataBase.enemyDataList[m_selectNumber].EnemyElement.Length > (int)ElementType.enNum)
        {
            EditorGUILayout.HelpBox("�x���F�����̎�ނ���`��葽���ݒ肳��Ă��܂��I", MessageType.Warning);
        }
    }

    /// <summary>
    /// �g�p�\�X�L��
    /// </summary>
    private void DrawSkill()
    {
        for (int skillNumber = 0; skillNumber < m_enemyDataBase.enemyDataList[m_selectNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < m_skillDataBase.skillDataList.Count; dataNumber++)
            {
                // ���ʔԍ��������Ȃ�f�[�^������������
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
    /// �s���p�^�[��
    /// </summary>
    private void DrawMove()
    {
        for (int moveNumber = 0; moveNumber < m_enemyDataBase.enemyDataList[m_selectNumber].enemyMoveList.Count; moveNumber++)
        {
            for (int dataNumber = 0; dataNumber < m_enemyMoveDataBase.enemyMoveDataList.Count; dataNumber++)
            {
                // ���ʔԍ��������Ȃ�f�[�^������������
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
    /// ���O�ꗗ�̍쐬
    /// </summary>
    private static void ResetNameList()
    {
        m_nameList.Clear();

        // ���O����͂���
        foreach (var enemy in m_enemyDataBase.enemyDataList)
        {
            m_nameList.Add(enemy.EnemyName);
        }
    }

    /// <summary>
    /// �����̏���
    /// </summary>
    private void Search()
    {
        if (m_searchText == "")
        {
            return;
        }

        // ������
        int startNumber = m_selectNumber;
        startNumber = Mathf.Max(startNumber, 0);

        for(int i = startNumber; i < m_nameList.Count; i++)
        {
            if (m_nameList[i].Contains(m_searchText))
            {
                // �q�b�g������I��
                m_selectNumber = i;
                GUI.FocusControl("");
                Repaint();
                return;
            }
            // �q�b�g���Ȃ��ꍇ��-1
            m_selectNumber = -1;
        }
    }

    /// <summary>
    /// �f�[�^�̒ǉ�����
    /// </summary>
    private void AddData()
    {
        var newEnamyData = new EnemyData();

        // �ǉ�
        m_enemyDataBase.enemyDataList.Add(newEnamyData);
    }

    /// <summary>
    /// �f�[�^�̍폜����
    /// </summary>
    private void DeleteData()
    {
        if (m_selectNumber == -1)
        {
            return;
        }

        // �I���ʒu�̃f�[�^���폜
        m_enemyDataBase.enemyDataList.Remove(m_enemyDataBase.enemyDataList[m_selectNumber]);
        // ����
        m_selectNumber -= 1;
        m_selectNumber = Mathf.Max(m_selectNumber, 0);
    }
}
