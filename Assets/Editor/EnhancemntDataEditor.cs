using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class EnhancementDataEditor : EditorWindow
{
    // �Ώۂ̃f�[�^�x�[�X
    private static EnhancementDataBase m_enhancementDataBase;
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
    [MenuItem("Window/EnhancementDataBase")]
    private static void Open()
    {
        // �ǂݍ���
        m_enhancementDataBase = AssetDatabase.LoadAssetAtPath<EnhancementDataBase>("Assets/Data/EnhancementData.asset");
        // ���O��ύX
        GetWindow<EnhancementDataEditor>("�����f�[�^�x�[�X");
        // �ύX��ʒm
        EditorUtility.SetDirty(m_enhancementDataBase);

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
            m_enhancementDataBase.enhancementDataList[m_selectNumber].ID = m_selectNumber;
            GUILayout.Label($"ID:{m_enhancementDataBase.enhancementDataList[m_selectNumber].ID}   Name:{m_nameList[m_selectNumber]}");

            // ��
            EditorGUILayout.Space();

            // �ݒ藓��\��
            // ���O
            m_enhancementDataBase.enhancementDataList[m_selectNumber].EnhancementName =
                EditorGUILayout.TextField(
                    "���O",
                    m_enhancementDataBase.enhancementDataList[m_selectNumber].EnhancementName
                    );
            // �摜
            m_enhancementDataBase.enhancementDataList[m_selectNumber].EnhancementSprite =
                EditorGUILayout.ObjectField(
                    "�摜",
                    m_enhancementDataBase.enhancementDataList[m_selectNumber].EnhancementSprite,
                    typeof(Sprite), true) as Sprite;
            // �����l
            m_enhancementDataBase.enhancementDataList[m_selectNumber].EnhancementStatus =
                (EnhancementStatus)EditorGUILayout.Popup(
                    "�����l",
                    (int)m_enhancementDataBase.enhancementDataList[m_selectNumber].EnhancementStatus,
                    new string[] { "HP", "SP", "ATK", "DEF", "SPD", "LUCK" }
                    );
            // EP
            m_enhancementDataBase.enhancementDataList[m_selectNumber].EnhancementPoint =
               EditorGUILayout.IntField(
                   "�K�vEP",
                   m_enhancementDataBase.enhancementDataList[m_selectNumber].EnhancementPoint
                   );
        }
        EditorGUILayout.EndVertical();
        // �ۑ�
        Undo.RegisterCompleteObjectUndo(m_enhancementDataBase, "EnhancementDataBase");
    }

    /// <summary>
    /// ���O�ꗗ�̍쐬
    /// </summary>
    private static void ResetNameList()
    {
        m_nameList.Clear();

        // ���O����͂���
        foreach (var enhancement in m_enhancementDataBase.enhancementDataList)
        {
            m_nameList.Add(enhancement.EnhancementName);
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
        var newEnhancementData = new EnhancementData();

        // �ǉ�
        m_enhancementDataBase.enhancementDataList.Add(newEnhancementData);
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
        m_enhancementDataBase.enhancementDataList.Remove(m_enhancementDataBase.enhancementDataList[m_selectNumber]);
        // ����
        m_selectNumber -= 1;
        m_selectNumber = Mathf.Max(m_selectNumber, 0);
    }
}
