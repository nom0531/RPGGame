using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class DrawStatusValue : MonoBehaviour
{
    /// <summary>
    /// HP�ESP�o�[�̌�������
    /// </summary>
    private enum DecreaseProcess
    {
        enStart,    // �J�n
        enEnd,      // �I��
    }

    /// <summary>
    /// �����������I�������A�N�^
    /// </summary>
    private enum DecreaseState
    {
        enNone,
        enAttcker,
        enBuffer,
        enHealer,
    }

    [SerializeField, Header("�Q�ƃf�[�^")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private EnemyDataBase EnemyData;
    [SerializeField]
    private StateAbnormalDataBase StateAbnormalData;
    [SerializeField, Header("HP")]
    private GameObject[] Data_HPText;
    [SerializeField, Header("HP�Q�[�W")]
    private GameObject[] Data_HPBarGreen;
    [SerializeField]
    private GameObject[] Data_HPBarRed;
    [SerializeField, Header("SP")]
    private GameObject[] Data_SPText;
    [SerializeField, Header("SP�Q�[�W")]
    private GameObject[] Data_SPBarYellow;
    [SerializeField]
    private GameObject[] Data_SPBarRed;
    [SerializeField, Header("��������"),Tooltip("�������x")]
    private float DecreaseSpped = 1.0f;
    [SerializeField, Header("�o�t")]
    private GameObject[] Content;
    [SerializeField, Header("��Ԉُ�")]
    private GameObject[] Data_StatusAbnormalImage;
    [SerializeField, Header("���O��\������e�L�X�g")]
    private GameObject Data_Name;

    private List<PlayerMove> m_playerMove;
    private DecreaseProcess m_decreaseProcess = DecreaseProcess.enEnd;    // �o�[�̌��������̃X�e�[�g
    private DecreaseState m_decreaseState = DecreaseState.enNone;         // �������������l�I��������

    public int EnemyName
    {
        set
        {
            Data_Name.GetComponent<TextMeshProUGUI>().text = EnemyData.enemyDataList[value].EnemyName;
        }
    }

    public int PlayerName
    {
        set
        {
            Data_Name.GetComponent<TextMeshProUGUI>().text = PlayerData.playerDataList[value].PlayerName;
        }
    }

    private void Awake()
    {
        // playerMove��l�����p��
        PlayerMove[] playerMove = FindObjectsOfType<PlayerMove>();
        m_playerMove = new List<PlayerMove>(playerMove);
        m_playerMove.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));    // �ԍ����Ƀ\�[�g
    }

    private void Update()
    {
        // �J�n���Ȃ��Ȃ�X�L�b�v
        if(m_decreaseProcess == DecreaseProcess.enEnd)
        {
            return;
        }
        Decrease();
    }

    /// <summary>
    /// �e�L�X�g��ݒ肷��
    /// </summary>
    public void SetStatusText()
    {
        for(int i = 0; i < PlayerData.playerDataList.Count; i++)
        {
            Data_HPText[i].GetComponent<TextMeshProUGUI>().text =
                m_playerMove[i].PlayerStatus.HP.ToString();
            Data_SPText[i].GetComponent<TextMeshProUGUI>().text =
                m_playerMove[i].PlayerStatus.SP.ToString();
        }
    }

    /// <summary>
    /// �X�e�[�^�X��ݒ肷��
    /// </summary>
    public void SetStatus()
    {
        for (int i = 0; i < PlayerData.playerDataList.Count; i++)
        {
            SetFillAmount(i);
            DrawStatusAbnormalImage(i);
            DrawBuffStatusImage(i);
        }
    }

    /// <summary>
    /// �o�[�̊�����ݒ肷��
    /// </summary>
    async private void SetFillAmount(int number)
    {
        Data_HPBarGreen[number].GetComponent<Image>().fillAmount =
        CalculateRate(m_playerMove[number].PlayerStatus.HP, PlayerData.playerDataList[number].HP);
        Data_SPBarYellow[number].GetComponent<Image>().fillAmount =
        CalculateRate(m_playerMove[number].PlayerStatus.SP, PlayerData.playerDataList[number].SP);
        m_decreaseProcess = DecreaseProcess.enStart;
    }

    /// <summary>
    /// HP�ESP�̊������v�Z����
    /// </summary>
    /// <param name="nowValue">���ݒl</param>
    /// <param name="maxValue">�ő�l</param>
    private float CalculateRate(int nowValue, int maxValue)
    {
        var rate = 0.0f;
        return rate = (float)nowValue / (float)maxValue;
    }

    /// <summary>
    /// HP�ESP�̌�������
    /// </summary>
    private void Decrease()
    {
        // �����������J�n���Ȃ��Ȃ���s���Ȃ�
        if(m_decreaseProcess == DecreaseProcess.enEnd)
        {
            return;
        }
        // �v�Z
        for (int i = 0; i < PlayerData.playerDataList.Count; i++)
        {
            Data_HPBarRed[i].GetComponent<Image>().fillAmount = 
                SetRate(Data_HPBarRed[i].GetComponent<Image>().fillAmount,
                Data_HPBarGreen[i].GetComponent<Image>().fillAmount);
            Data_SPBarRed[i].GetComponent<Image>().fillAmount = 
                SetRate(Data_SPBarRed[i].GetComponent<Image>().fillAmount,
                Data_SPBarYellow[i].GetComponent<Image>().fillAmount);
            m_decreaseState = (DecreaseState)i+1;
            if (m_decreaseState == DecreaseState.enHealer)
            {
                m_decreaseProcess = DecreaseProcess.enEnd;  // �������I��
            }
        }
    }

    /// <summary>
    /// �l��ݒ�
    /// </summary>
    /// <param name="rate">���g�̊���</param>
    /// <param name="nowRate">���݂̊���</param>
    /// <returns>�v�Z��̎��g�̊���</returns>
    private float SetRate(float rate, float nowRate)
    {
        // ���Ɋ���������������ȉ��Ȃ���s���Ȃ�
        if (rate <= nowRate)
        {
            return rate;
        }
        // �l�����炷
        rate -= Time.deltaTime * DecreaseSpped;
        if (rate <= nowRate)
        {
            rate = nowRate;                             // �␳
        }
        return rate;
    }

    /// <summary>
    /// ��Ԉُ��UI��`�悷��
    /// </summary>
    private void DrawStatusAbnormalImage(int number)
    {
        // ��Ԉُ킪�Ȃ�(enNormal��)�ꍇ�͕\�����Ȃ�
        if (m_playerMove[number].ActorAbnormalState == ActorAbnormalState.enNormal)
        {
            Data_StatusAbnormalImage[number].SetActive(false);
            return;
        }
        // ���݂̏�Ԉُ�ƃf�[�^�̏�Ԉُ킪�����Ȃ�
        for(int i = 0; i < StateAbnormalData.stateAbnormalList.Count; i++)
        {
            if(m_playerMove[number].ActorAbnormalState != StateAbnormalData.stateAbnormalList[i].ActorAbnormalState)
            {
                continue;
            }
            // �ԍ���ݒ�
            var stateNumber = (int)StateAbnormalData.stateAbnormalList[i].ID;
            // �X�v���C�g��ݒ肷��
            Data_StatusAbnormalImage[number].GetComponent<Image>().sprite =
                StateAbnormalData.stateAbnormalList[stateNumber].StateImage;
            // �\������
            Data_StatusAbnormalImage[number].SetActive(true);
            break;
        }
    }

    /// <summary>
    /// �o�t�E�f�o�t��UI��`�悷��
    /// </summary>
    private void DrawBuffStatusImage(int number)
    {
        if(m_playerMove[number].NextActionType == ActionType.enGuard)
        {
            return;
        }
        if(m_playerMove[number].ActorAbnormalState != ActorAbnormalState.enNormal)
        {
            return;
        }

        var buffCalculation = m_playerMove[number].GetComponent<BuffCalculation>();
        if(buffCalculation.GetBuffFlag(BuffStatus.enBuff_ATK) == true)
        {
            if(buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_ATK) == true)
            {
                return;
            }
            SetImage(number, 0, 1, true);
        }
        else
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_ATK) == true)
            {
                return;
            }
            SetImage(number, 0, 1, false);
        }
        if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_DEF) == true)
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_DEF) == true)
            {
                return;
            }
            SetImage(number, 1, 2, true);
        }
        else
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_DEF) == true)
            {
                return;
            }
            SetImage(number, 1, 2, false);
        }
        if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_SPD) == true)
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_SPD) == true)
            {
                return;
            }
            SetImage(number, 2, 3, true);
        }
        else
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_SPD) == true)
            {
                return;
            }
            SetImage(number, 2, 3, false);
        }
        if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_ATK) == true)
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_ATK) == true)
            {
                return;
            }
            SetImage(number, 0, 4, true);
        }
        else
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_ATK) == true)
            {
                return;
            }
            SetImage(number, 0, 4, false);
        }
        if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_DEF) == true)
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_DEF) == true)
            {
                return;
            }
            SetImage(number, 1, 5, true);
        }
        else
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_DEF) == true)
            {
                return;
            }
            SetImage(number, 1, 5, false);
        }
        if(buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_SPD) == true)
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_SPD) == true)
            {
                return;
            }
            SetImage(number, 2, 6, true);
        }
        else
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_DEF) == true)
            {
                return;
            }
            SetImage(number, 2, 6, false);
        }
    }

    /// <summary>
    /// �摜��ݒ肷��
    /// </summary>
    /// <param name="contentNumber">�R���e���c�̔ԍ�</param>
    /// <param name="childNumber">�q�I�u�W�F�N�g�̔ԍ�</param>
    /// <param name="listNumber">���X�g�̔ԍ�</param>
    /// <param name="flag">true�Ȃ�`�悷��Bfalse�Ȃ�`�悵�Ȃ�</param>
    private void SetImage(int contentNumber, int childNumber, int listNumber, bool flag)
    {
        Content[contentNumber].transform.GetChild(childNumber).GetComponent<Image>().sprite = 
            StateAbnormalData.stateAbnormalList[listNumber].StateImage;
        Content[contentNumber].transform.GetChild(childNumber).gameObject.SetActive(flag);
    }
}
