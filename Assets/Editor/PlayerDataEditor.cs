using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class PlayerDataEditor : EditorWindow
{
    // �Ώۂ̃f�[�^�x�[�X
    static PlayerDataBase m_playerDataBase;
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
    [MenuItem("Window/PlayerDataBase")]
    static void Open()
    {
        // �ǂݍ���
        m_playerDataBase = AssetDatabase.LoadAssetAtPath<PlayerDataBase>("Assets/Data/PlayerData.asset");
        // ���O��ύX
        GetWindow<PlayerDataEditor>("�v���C���[�f�[�^�x�[�X");

        ResetNameList();

        // �ύX��ʒm
        EditorUtility.SetDirty(m_playerDataBase);
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
            GUILayout.Label("ID:" + m_selectNumber + "   Name:" + m_nameList[m_selectNumber]);

            // ��
            EditorGUILayout.Space();

            // �ݒ藓��\��
            // ���O
            m_playerDataBase.playerDataList[m_selectNumber].PlayerName =
                EditorGUILayout.TextField(
                    "���O",
                    m_playerDataBase.playerDataList[m_selectNumber].PlayerName
                    );
            // �摜
            m_playerDataBase.playerDataList[m_selectNumber].PlayerSprite =
                EditorGUILayout.ObjectField(
                    "�摜",
                    m_playerDataBase.playerDataList[m_selectNumber].PlayerSprite,
                    typeof(Sprite), true) as Sprite;
            // ���g�̃��[��
            m_playerDataBase.playerDataList[m_selectNumber].PlayerRoll = 
                (PlayerRoll)EditorGUILayout.Popup(
                    "���[��",
                    (int)m_playerDataBase.playerDataList[m_selectNumber].PlayerRoll,
                    new string[] { "�A�^�b�J�[", "�o�b�t�@�[", "�q�[���[" }
                    );

            EditorGUILayout.Space();
            ElementViewUpdate();
            EditorGUILayout.Space();

            // �X�e�[�^�X��
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

            // �l���ُ�ȏꍇ�͌x����\������
            if (m_playerDataBase.playerDataList[m_selectNumber].HP <= 0)
            {
                EditorGUILayout.HelpBox("�x���F�����̗͂�0�ȉ��ł��I", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();

        // �ۑ�
        Undo.RegisterCompleteObjectUndo(m_playerDataBase, "EnemyDataBase");
    }

    /// <summary>
    /// �����ϐ��̃r���[�̍X�V����
    /// </summary>
    void ElementViewUpdate()
    {
        {
            // �����ϐ�
            string[] elementText = { "��", "�X", "��", "��", "��", "��", "��" };

            for (int i = 0; i < (int)ElementType.enNum; i++)
            {
                m_playerDataBase.playerDataList[m_selectNumber].PlayerElement[i] =
                     (ElementResistance)EditorGUILayout.Popup(
                         elementText[i],
                         (int)m_playerDataBase.playerDataList[m_selectNumber].PlayerElement[i],
                         new string[] { "�ϐ�", "��_", "--" }
                         );
            }

            // ���ȏ�ݒ肳�ꂽ�ꍇ�͌x����\������
            if (m_playerDataBase.playerDataList[m_selectNumber].PlayerElement.Length > (int)ElementType.enNum)
            {
                EditorGUILayout.HelpBox("�x���F�����̎�ނ���`��葽���ݒ肳��Ă��܂��I", MessageType.Warning);
            }
        }
    }

    /// <summary>
    /// ���O�ꗗ�̍쐬
    /// </summary>
    static void ResetNameList()
    {
        m_nameList.Clear();

        // ���O����͂���
        foreach (PlayerData player in m_playerDataBase.playerDataList)
        {
            m_nameList.Add(player.PlayerName);
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
        PlayerData newPlayerData = new PlayerData();

        // �ǉ�
        m_playerDataBase.playerDataList.Add(newPlayerData);
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
        m_playerDataBase.playerDataList.Remove(m_playerDataBase.playerDataList[m_selectNumber]);
        // ����
        m_selectNumber -= 1;
        m_selectNumber = Mathf.Max(m_selectNumber, 0);
    }
}
