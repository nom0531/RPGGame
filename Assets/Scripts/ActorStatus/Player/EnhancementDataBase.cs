using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����Ώ�
/// </summary>
public enum EnhancementStatus
{
    enHP,
    enSP,
    enATK,
    enDEF,
    enSPD,
    enLUCK
}

/// <summary>
/// �v���C���[�̋����̍\����
/// </summary>
[System.Serializable]
public class EnhancementData
{
    [SerializeField, Header("���")]
    public string EnhancementName;                                  // ���O
    public int ID;                                                  // ���ʔԍ�
    public Sprite EnhancementSprite;                                // �摜
    public EnhancementStatus EnhancementStatus;                     // ��������Ώ�
    public int AddValue;                                            // �����l
    [SerializeField, Header("�K�v�����|�C���g")]
    public int EnhancementPoint;                                    // �K�v�����|�C���g
}

[CreateAssetMenu(fileName = "EnhancementDataBase", menuName = "CreateEnhancementDataBase")]
public class EnhancementDataBase : ScriptableObject
{
    public List<EnhancementData> enhancementDataList = new List<EnhancementData>();
}
