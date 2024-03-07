using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class LevelDataEditor : EditorWindow
{
    private const int MAX_TEXTNUM = 14;

    // �Ώۂ̃f�[�^�x�[�X
    private static LevelDataBase m_levelDataBase;
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
    [MenuItem("Window/LevelDataBase")]
    private static void Open()
    {
        // �ǂݍ���
        m_levelDataBase = AssetDatabase.LoadAssetAtPath<LevelDataBase>("Assets/Data/LevelData.asset");
        // ���O��ύX
        GetWindow<LevelDataEditor>("���x���f�[�^�x�[�X");
        // �ύX��ʒm
        EditorUtility.SetDirty(m_levelDataBase);

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
            GUILayout.Label($"ID:{m_selectNumber}   Name:{m_nameList[m_selectNumber]}");

            // ��
            EditorGUILayout.Space();

            // �ݒ藓��\��
            // ���O
            m_levelDataBase.levelDataList[m_selectNumber].LevelName =
                EditorGUILayout.TextField(
                    "���O",
                    m_levelDataBase.levelDataList[m_selectNumber].LevelName
                    );
            // ��Փx
            m_levelDataBase.levelDataList[m_selectNumber].LevelState = 
                (LevelState)EditorGUILayout.Popup(
                    "��Փx",
                    (int)m_levelDataBase.levelDataList[m_selectNumber].LevelState,
                    new string[] { "����������", "����������", "����������", "����������", "����������" }
                    );
            EditorGUILayout.Space();
            // ����
            GUILayout.Label("����");
            m_levelDataBase.levelDataList[m_selectNumber].LevelDetail =
                EditorGUILayout.TextArea(m_levelDataBase.levelDataList[m_selectNumber].LevelDetail);

            // �x����\��
            if (m_levelDataBase.levelDataList[m_selectNumber].LevelName.Length >= MAX_TEXTNUM)
            {
                EditorGUILayout.HelpBox("�x���F�N�G�X�g���̕��������������܂��I", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();
        // �ۑ�
        Undo.RegisterCompleteObjectUndo(m_levelDataBase, "LevelDataBase");
    }

    /// <summary>
    /// ���O�ꗗ�̍쐬
    /// </summary>
    private static void ResetNameList()
    {
        m_nameList.Clear();
        // ���O����͂���
        foreach (var level in m_levelDataBase.levelDataList)
        {
            m_nameList.Add(level.LevelName);
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
    private void AddData()
    {
        var newLevelData = new LevelData();
        // �ǉ�
        m_levelDataBase.levelDataList.Add(newLevelData);
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
        m_levelDataBase.levelDataList.Remove(m_levelDataBase.levelDataList[m_selectNumber]);
        // ����
        m_selectNumber -= 1;
        m_selectNumber = Mathf.Max(m_selectNumber, 0);
    }
}
