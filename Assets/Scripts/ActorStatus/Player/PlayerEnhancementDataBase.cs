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
public class PlayerEnhancementData
{
    [SerializeField, Header("���")]
    public string EnhancementName;                                  // ���O
    public Sprite EnhancementSprite;                                // �摜
    public EnhancementStatus EnhancementStatus;                     // ��������Ώ�
    public int AddValue;                                            // �����l
    [SerializeField, Header("�K�v�����|�C���g")]
    public int EnhancementPoint;                                    // �K�v�����|�C���g
}

[CreateAssetMenu(fileName = "PlayerEnhancementDataBase", menuName = "CreatePlayerEnhancementDataBase")]
public class PlayerEnhancementDataBase : ScriptableObject
{
    public List<PlayerEnhancementData> playerEnhancementDataList = new List<PlayerEnhancementData>();
}
