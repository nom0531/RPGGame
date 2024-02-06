using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestStatusSystem : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    LevelDataBase LevelData;
    [SerializeField]
    EnemyDataBase EnemyData;
    [SerializeField, Header("�\���p�f�[�^"),Tooltip("�N�G�X�g��")]
    GameObject Data_QuestName;
    [SerializeField,Tooltip("�N�G�X�g�̏ڍ�")]
    GameObject Data_QuestDetail;
    [SerializeField, Header("�N�G�X�g���X�g"), Tooltip("��������I�u�W�F�N�g")]
    GameObject QuestContent;
    [SerializeField, Tooltip("��������{�^��")]
    GameObject QuestButton;
    [SerializeField, Header("�G�l�~�[���X�g"), Tooltip("��������I�u�W�F�N�g")]
    GameObject EnemyContent;
    [SerializeField, Tooltip("��������摜")]
    GameObject EnemyImage;
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    GameObject QuestStartButton;

    private const int MAX_INSTANTIATE_SUM = 3;  // �G�l�~�[�̉摜�̍ő吔

    private GameManager m_gameManager;

    private void Start()
    {
        // �G�l�~�[�̐����f�[�^���폜����
        for(int i = 0; i < LevelData.levelDataList.Count; i++)
        {
            // �v�f��0�Ȃ���s���Ȃ�
            if (LevelData.levelDataList[i].enemyDataList.Count > 0)
            {
                // �폜
                LevelData.levelDataList[i].enemyDataList.RemoveRange(0, LevelData.levelDataList[i].enemyDataList.Count);
            }
        }

        m_gameManager = GameManager.Instance;
        Data_QuestName.SetActive(false);
        Data_QuestDetail.SetActive(false);
        QuestStartButton.GetComponent<Button>().interactable = false;

        for (int QuestNumber = 0; QuestNumber < LevelData.levelDataList.Count; QuestNumber++)
        {
            // �{�^���𐶐����Ďq�I�u�W�F�N�g�ɂ���
            var questObject = Instantiate(QuestButton);
            questObject.transform.SetParent(QuestContent.transform);
            // �T�C�Y�𒲐�
            questObject.transform.localScale = Vector3.one;
            questObject.transform.localPosition = Vector3.zero;
            // ���ɃN���A���Ă���Ȃ�
            if (m_gameManager.SaveData.SaveData.saveData.ClearStage[QuestNumber] == true)
            {
                // Clear�̃e�L�X�g��\������
                questObject.transform.GetChild(1).gameObject.SetActive(true);
            }
            var questButton = questObject.GetComponent<QuestButton>();
            questButton.SetQuestStatus(
                QuestNumber,
                LevelData.levelDataList[QuestNumber].LevelName,
                Color.black,
                this
                );

            SetLevelStatus(QuestNumber);
        }
    }

    /// <summary>
    /// �N�G�X�g�f�[�^�̏�����
    /// </summary>
    /// <param name="number">�N�G�X�g�̔ԍ�</param>
    private void SetLevelStatus(int number)
    {
        for (int dataNumber = 0; dataNumber < EnemyData.enemyDataList.Count; dataNumber++)
        {
            // ���̔���
            switch (LevelData.levelDataList[number].LocationType)
            {
                case LocationType.enHell:
                case LocationType.enForest:
                case LocationType.enSea:
                case LocationType.enVolcano:
                    // �S�Ă̊��ɑΉ����Ă���Ȃ珈�����΂�
                    if (EnemyData.enemyDataList[dataNumber].PopLocation == LocationType.enAllLocation)
                    {
                        break;
                    }
                    else if (LevelData.levelDataList[number].LocationType != EnemyData.enemyDataList[dataNumber].PopLocation)
                    {
                        continue;
                    }
                    break;
            }
            // ���Ԃ̔���
            switch (LevelData.levelDataList[number].LocationTime)
            {
                case LocationTime.enMorning:
                case LocationTime.enTwilight:
                case LocationTime.enEvening:
                    // �S�Ă̎��ԂɑΉ����Ă���Ȃ珈�����΂�
                    if (EnemyData.enemyDataList[dataNumber].PopTime == LocationTime.enAllTime)
                    {
                        break;
                    }
                    else if (LevelData.levelDataList[number].LocationTime != EnemyData.enemyDataList[dataNumber].PopTime)
                    {
                        continue;
                    }
                    break;
            }
            // ���Ă͂܂��Ă���Ȃ�f�[�^��ǉ�����
            var enemyData = new EnemyData();
            enemyData.ID = EnemyData.enemyDataList[dataNumber].ID;
            LevelData.levelDataList[number].enemyDataList.Add(enemyData);
        }
    }

    /// <summary>
    /// ���͂��ꂽ�f�[�^��\�����鏈��
    /// </summary>
    /// <param name="number">�N�G�X�g�̔ԍ�</param>
    public void DisplaySetSValue(int number)
    {
        m_gameManager.LevelNumber = number;
        QuestStartButton.GetComponent<Button>().interactable = true;
        DestroyEnemySprites();
        Data_QuestName.SetActive(true);
        Data_QuestDetail.SetActive(true);
        // �l���X�V
        Data_QuestName.GetComponent<TextMeshProUGUI>().text = $"�u{LevelData.levelDataList[number].LevelName}�v";
        Data_QuestDetail.GetComponent<TextMeshProUGUI>().text = LevelData.levelDataList[number].LevelDetail;

        SetEnemySprites(number);
    }

    /// <summary>
    /// �G�l�~�[�̉摜�𐶐����鏈��
    /// </summary>
    /// <param name="number">�N�G�X�g�̔ԍ�</param>
    private void SetEnemySprites(int number)
    {
        int InstantiateSum = 0; // �I�u�W�F�N�g�̐�����
        for (int enemyNumber = 0; enemyNumber < LevelData.levelDataList[number].enemyDataList.Count; enemyNumber++)
        {
            for (int dataNumber = 0; dataNumber < EnemyData.enemyDataList.Count; dataNumber++)
            {
                if(InstantiateSum >= MAX_INSTANTIATE_SUM)
                {
                    // ���������ő吔�𒴂��Ă���Ȃ�I������
                    break;
                }
                // �G�l�~�[�̉摜�𐶐�
                if (LevelData.levelDataList[number].enemyDataList[enemyNumber].ID != EnemyData.enemyDataList[dataNumber].ID)
                {
                    // �ԍ����قȂ�Ȃ玟�̃��[�v�Ɉڍs����
                    continue;
                }
                var instrantiateNumber = LevelData.levelDataList[number].enemyDataList[enemyNumber].ID;
                var enemyObject = Instantiate(EnemyImage);
                enemyObject.transform.SetParent(EnemyContent.transform);
                enemyObject.GetComponent<Image>().sprite = EnemyData.enemyDataList[instrantiateNumber].EnemySprite;
                // �T�C�Y�𒲐�
                enemyObject.transform.localScale = Vector3.one;
                enemyObject.transform.localPosition = Vector3.zero;

                if (m_gameManager.SaveData.SaveData.saveData.EnemyRegisters[enemyNumber] != true)
                {
                    // �������Ă��Ȃ��Ȃ�J���[��ύX����
                    enemyObject.GetComponent<Image>().color = Color.black;
                }
                InstantiateSum++;
                break;
            }
        }
    }

    /// <summary>
    /// PopEnemy�^�O�̕t�����I�u�W�F�N�g��S�č폜���鏈��
    /// </summary>
    private void DestroyEnemySprites()
    {
        var popEnemys = GameObject.FindGameObjectsWithTag("PopEnemy");

        foreach (var sprite in popEnemys)
        {
            Destroy(sprite);
        }
    }
}
