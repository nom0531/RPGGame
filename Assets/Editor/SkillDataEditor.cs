using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class SkillDataEditor : EditorWindow
{
    // �Ώۂ̃f�[�^�x�[�X
    static SkillDataBase m_skillDataBase;
    // ���O�ꗗ
    static List<string> m_nameList = new List<string>();
    // �X�N���[���ʒu
    Vector2 m_leftScrollPosition = Vector2.zero;
    // �I�𒆃i���o�[
    int m_selectNumber = -1;
    // ������
    SearchField m_searchField;
    string m_searchText = "";

    // �E�B���h�E���쐬
    [MenuItem("Window/SkillDataBase")]
    static void Open()
    {
        // �ǂݍ���
        m_skillDataBase = AssetDatabase.LoadAssetAtPath<SkillDataBase>("Assets/Data/SkillData.asset");
        // ���O��ύX
        GetWindow<SkillDataEditor>("�X�L���f�[�^�x�[�X");
        // �ύX��ʒm
        EditorUtility.SetDirty(m_skillDataBase);

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
    void LeftUpdate()
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
                for (int i = 0; i < m_nameList.Count; i++)
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
                    if (GUILayout.Button(i + ":" + m_nameList[i]))
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
            GUILayout.Label("���ڐ�:" + m_nameList.Count);
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// �r���[�E���̍X�V����
    /// </summary>
    void NameViewUpdate()
    {
        if (m_selectNumber < 0)
        {
            return;
        }

        // �E�����X�V
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            // ��b����\��
            m_skillDataBase.skillDataList[m_selectNumber].SkillNumber = m_selectNumber;
            GUILayout.Label("ID:" + m_skillDataBase.skillDataList[m_selectNumber].SkillNumber + "   Name:" + m_nameList[m_selectNumber]);

            // ��
            EditorGUILayout.Space();

            // �ݒ藓��\��
            // ���O
            m_skillDataBase.skillDataList[m_selectNumber].SkillName =
                EditorGUILayout.TextField(
                    "���O",
                    m_skillDataBase.skillDataList[m_selectNumber].SkillName
                    );
            // �摜
            m_skillDataBase.skillDataList[m_selectNumber].SkillSprite =
                EditorGUILayout.ObjectField(
                    "�摜",
                    m_skillDataBase.skillDataList[m_selectNumber].SkillSprite,
                    typeof(Sprite), true) as Sprite;
            // ����
            m_skillDataBase.skillDataList[m_selectNumber].SkillElement =
                 (ElementType)EditorGUILayout.Popup(
                     "����",
                     (int)m_skillDataBase.skillDataList[m_selectNumber].SkillElement,
                     new string[] { "��", "�X", "��", "��", "��", "��", "��", "--" });
            // �^�C�v
            m_skillDataBase.skillDataList[m_selectNumber].SkillType =
                 (SkillType)EditorGUILayout.Popup(
                     "�X�L���^�C�v",
                     (int)m_skillDataBase.skillDataList[m_selectNumber].SkillType,
                     new string[] { "�U��", "�o�t", "�f�o�t", "��", "����" });

            EditorGUILayout.Space();

            // �ǉ�����
            Draw();

            EditorGUILayout.Space();

            // �����f�[�^
            m_skillDataBase.skillDataList[m_selectNumber].Type =
                (NecessaryType)EditorGUILayout.Popup(
                    "�����f�[�^�̎��",
                    (int)m_skillDataBase.skillDataList[m_selectNumber].Type,
                    new string[] { "SP", "HP" });
            m_skillDataBase.skillDataList[m_selectNumber].SkillNecessary =
                EditorGUILayout.IntField(
                    "�����l",
                m_skillDataBase.skillDataList[m_selectNumber].SkillNecessary);
            // ��b�U���́E�񕜗�
            m_skillDataBase.skillDataList[m_selectNumber].POW =
                EditorGUILayout.IntField(
                    "��b�l(������)",
                    m_skillDataBase.skillDataList[m_selectNumber].POW);

            EditorGUILayout.Space();
            // �X�L���̌��ʔ͈�
            m_skillDataBase.skillDataList[m_selectNumber].EffectRange =
                (EffectRange)EditorGUILayout.Popup(
                    "���ʔ͈�",
                    (int)m_skillDataBase.skillDataList[m_selectNumber].EffectRange,
                    new string[] { "�P��", "�S��" });
            // �o�t�̃^�C�v
            if (m_skillDataBase.skillDataList[m_selectNumber].SkillType != SkillType.enBuff
                || m_skillDataBase.skillDataList[m_selectNumber].SkillType != SkillType.enDeBuff)
            {
                // �^�C�v����
                m_skillDataBase.skillDataList[m_selectNumber].BuffType = BuffType.enNull;
            }
            m_skillDataBase.skillDataList[m_selectNumber].BuffType =
                (BuffType)EditorGUILayout.Popup(
                    "�o�t�^�C�v",
                    (int)m_skillDataBase.skillDataList[m_selectNumber].BuffType,
                    new string[] { "ATK", "DEF", "SPD", "--" });

            EditorGUILayout.Space();

            // �X�L���J���ɕK�v��EP
            m_skillDataBase.skillDataList[m_selectNumber].EnhancementPoint =
                EditorGUILayout.IntField(
                    "�K�vEP",
                    m_skillDataBase.skillDataList[m_selectNumber].EnhancementPoint
                    );

            EditorGUILayout.Space();

            // �}�Ӑ���
            GUILayout.Label("�}�Ӑ���");
            m_skillDataBase.skillDataList[m_selectNumber].SkillDetail =
                EditorGUILayout.TextArea(m_skillDataBase.skillDataList[m_selectNumber].SkillDetail);
        }
        EditorGUILayout.EndVertical();

        // �ۑ�
        Undo.RegisterCompleteObjectUndo(m_skillDataBase, "SkillDataBase");
    }

    /// <summary>
    /// �ǉ����ʂ�\������
    /// </summary>
    private void Draw()
    {
        // ���O
        m_skillDataBase.skillDataList[m_selectNumber].StateAbnormalData.StateName =
                EditorGUILayout.TextField(
                    "�ǉ�����:",
                    m_skillDataBase.skillDataList[m_selectNumber].StateAbnormalData.StateName
                    );
        // �摜
        m_skillDataBase.skillDataList[m_selectNumber].StateAbnormalData.StateImage = 
            EditorGUILayout.ObjectField(
                    "�摜",
                    m_skillDataBase.skillDataList[m_selectNumber].SkillSprite,
                    typeof(Sprite), true) as Sprite;
        // �K�v�^�[����
        m_skillDataBase.skillDataList[m_selectNumber].StateAbnormalData.RequiredTime =
            EditorGUILayout.IntField(
                "�����܂ł̃^�[����",
                m_skillDataBase.skillDataList[m_selectNumber].StateAbnormalData.RequiredTime
                );
    }

    /// <summary>
    /// ���O�ꗗ�̍쐬
    /// </summary>
    static void ResetNameList()
    {
        m_nameList.Clear();

        // ���O����͂���
        foreach (SkillData skill in m_skillDataBase.skillDataList)
        {
            m_nameList.Add(skill.SkillName);
        }
    }

    /// <summary>
    /// �����̏���
    /// </summary>
    void Search()
    {
        if (m_searchText == "")
        {
            return;
        }

        // ������
        int startNumber = m_selectNumber;
        startNumber = Mathf.Max(startNumber, 0);

        for (int i = startNumber; i < m_nameList.Count; i++)
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
    void AddData()
    {
        SkillData newSkillData = new SkillData();

        // �ǉ�
        m_skillDataBase.skillDataList.Add(newSkillData);
    }

    /// <summary>
    /// �f�[�^�̍폜����
    /// </summary>
    void DeleteData()
    {
        if (m_selectNumber == -1)
        {
            return;
        }

        // �I���ʒu�̃f�[�^���폜
        m_skillDataBase.skillDataList.Remove(m_skillDataBase.skillDataList[m_selectNumber]);
        // ����
        m_selectNumber -= 1;
        m_selectNumber = Mathf.Max(m_selectNumber, 0);
    }
}
