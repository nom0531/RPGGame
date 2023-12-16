using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ԉُ�̃��X�g
/// </summary>
[System.Serializable]
public class StateAbnormalData
{
    public string StateName;                        // ���O
    public Sprite StateImage;                       // �\������摜
    public int StateNumber;                         // ���ʔԍ�
    public int POW;                                 // �_���[�W�ʁE���ʔ����̊���
    public int EffectTime;                          // �^�[����
    public ActorAbnormalState ActorAbnormalState;   // ��Ԉُ�̎��
}

[CreateAssetMenu(fileName = "StateAbnormalDataBase", menuName = "CreateStateAbnormalDataBase")]
public class StateAbnormalDataBase : ScriptableObject
{
    public List<StateAbnormalData> stateAbnormalList = new List<StateAbnormalData>();
}
