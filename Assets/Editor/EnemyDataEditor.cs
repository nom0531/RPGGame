using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class EnemyDataEditor : EditorWindow
{
    // �Ώۂ̃f�[�^�x�[�X
    static EnemyDataBase m_enemyDataBase;
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
    [MenuItem("Window/EnemyDataBase")]
    static void Open()
    {
        // �ǂݍ���
        m_enemyDataBase = AssetDatabase.LoadAssetAtPath<EnemyDataBase>("Assets/Data/EnemyData.asset");
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
            m_enemyDataBase.enemyDataList[m_selectNumber].EnemyNumber = m_selectNumber;
            GUILayout.Label("ID:" + m_enemyDataBase.enemyDataList[m_selectNumber].EnemyNumber + "   Name:" + m_nameList[m_selectNumber]);

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
            // �o���ݒ�
            m_enemyDataBase.enemyDataList[m_selectNumber].PopLocation =
                (LocationType)EditorGUILayout.Popup(
                    "�o�������",
                    (int)m_enemyDataBase.enemyDataList[m_selectNumber].PopLocation,
                    new string[] { "����", "�X", "�C", "�ΎR", "--" }
                    );
            m_enemyDataBase.enemyDataList[m_selectNumber].PopTime =
                (LocationTime)EditorGUILayout.Popup(
                    "�o�����鎞��",
                    (int)m_enemyDataBase.enemyDataList[m_selectNumber].PopTime,
                    new string[] { "��", "���v�O", "��", "--" }
                    );

            EditorGUILayout.Space();
            ElementViewUpdate();
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

            EditorGUILayout.Space();

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
    /// �����ϐ��̃r���[�̍X�V����
    /// </summary>
    void ElementViewUpdate()
    {
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
    }

    /// <summary>
    /// ���O�ꗗ�̍쐬
    /// </summary>
    static void ResetNameList()
    {
        m_nameList.Clear();

        // ���O����͂���
        foreach (EnemyData enemy in m_enemyDataBase.enemyDataList)
        {
            m_nameList.Add(enemy.EnemyName);
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
    void AddData()
    {
        EnemyData newEnamyData = new EnemyData();

        // �ǉ�
        m_enemyDataBase.enemyDataList.Add(newEnamyData);
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
        m_enemyDataBase.enemyDataList.Remove(m_enemyDataBase.enemyDataList[m_selectNumber]);
        // ����
        m_selectNumber -= 1;
        m_selectNumber = Mathf.Max(m_selectNumber, 0);
    }
}
