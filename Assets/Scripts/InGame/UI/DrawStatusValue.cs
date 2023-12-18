using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DrawStatusValue : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private EnemyDataBase EnemyData;
    [SerializeField]
    private StateAbnormalDataBase StateAbnormalData;
    [SerializeField, Header("HP")]
    private GameObject[] Data_HPText;
    [SerializeField]
    private GameObject[] Data_HPBar;
    [SerializeField, Header("SP")]
    private GameObject[] Data_SPText;
    [SerializeField]
    private GameObject[] Data_SPBar;
    [SerializeField, Header("�o�t")]
    private GameObject[] Content;
    [SerializeField, Header("��Ԉُ�")]
    private GameObject[] Data_StatusAbnormalImage;
    [SerializeField, Header("���O��\������e�L�X�g")]
    private GameObject Data_Name;

    private List<PlayerMove> m_playerMove;

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
            Data_HPBar[i].GetComponent<Image>().fillAmount =
                CalculateRate(
                    m_playerMove[i].PlayerStatus.HP,
                    PlayerData.playerDataList[i].HP
                    );

            Data_SPBar[i].GetComponent<Image>().fillAmount =
                CalculateRate(
                    m_playerMove[i].PlayerStatus.SP,
                    PlayerData.playerDataList[i].SP
                    );

            DrawStatusAbnormalImage(i);
            DrawBuffStatusImage(i);
        }
    }

    /// <summary>
    /// HP�ESP�̊������v�Z����
    /// </summary>
    /// <param name="nowValue">���݂̒l</param>
    /// <param name="maxValue">�ő�l</param>
    private float CalculateRate(int nowValue, int maxValue)
    {
        float rate = 0.0f;
        return rate = (float)nowValue / (float)maxValue;
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
            var stateNumber = (int)StateAbnormalData.stateAbnormalList[i].StateNumber;
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

        // �o�t�E�f�o�t���������Ă���Ȃ�
        for(int i = 0; i < Content[number].gameObject.transform.childCount; i++)
        {
            for (int j = 0; j < (int)BuffStatus.enNum; j++)
            {
                if (buffCalculation.GetBuffFlag((BuffStatus)j) == false)
                {
                    Content[number].transform.GetChild(i).gameObject.SetActive(false);
                    continue;
                }
                // �ԍ���ݒ�
                var stateNumber = j + 1;
                // �X�v���C�g��ݒ�
                Content[number].gameObject.transform.GetChild(i).GetComponent<Image>().sprite =
                    StateAbnormalData.stateAbnormalList[stateNumber].StateImage;
                Content[number].transform.GetChild(i).gameObject.SetActive(true);
                break;
            }
        }
    }
}
