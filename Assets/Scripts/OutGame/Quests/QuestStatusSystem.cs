using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestStatusSystem : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private LevelDataBase LevelData;
    [SerializeField]
    private EnemyDataBase EnemyData;
    [SerializeField, Header("�\���p�f�[�^")]
    private GameObject Data_QuestName;
    [SerializeField]
    private GameObject Data_QuestDetail,Data_QuestLevel;
    [SerializeField, Header("�N�G�X�g���X�g"), Tooltip("��������I�u�W�F�N�g")]
    private GameObject QuestContent;
    [SerializeField, Tooltip("��������{�^��")]
    private GameObject QuestButton;
    [SerializeField, Header("�G�l�~�[���X�g"), Tooltip("��������I�u�W�F�N�g")]
    private GameObject EnemyContent;
    [SerializeField, Tooltip("��������摜")]
    private GameObject EnemyImage;
    [SerializeField]
    private GameObject AnimatorObject;
    [SerializeField, Tooltip("���O")]
    private GameObject Data_EnemyName;
    [SerializeField, Tooltip("����")]
    private GameObject Data_Fire, Data_Ice, Data_Wind, Data_Thunder, Data_Light, Data_Dark;
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject QuestStartButton;

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
            if (m_gameManager.SaveDataManager.SaveData.saveData.ClearStage[QuestNumber] == true)
            {
                // Clear�̃e�L�X�g��\������
                questObject.transform.GetChild(1).gameObject.SetActive(true);
            }
            var questButton = questObject.GetComponent<QuestButton>();
            questButton.SetQuestStatus(
                QuestNumber,
                LevelData.levelDataList[QuestNumber].LevelName,
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
        //LevelData.levelDataList[number].enemyDataList.Clear();
        for (int dataNumber = 0; dataNumber < EnemyData.enemyDataList.Count; dataNumber++)
        {
            // ��Փx�̐ݒ�
            if (LevelData.levelDataList[number].LevelState < EnemyData.enemyDataList[dataNumber].LevelState)
            {
                continue;
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
        // �l���X�V
        Data_QuestName.GetComponent<TextMeshProUGUI>().text = $"�u{LevelData.levelDataList[number].LevelName}�v";
        Data_QuestDetail.GetComponent<TextMeshProUGUI>().text = LevelData.levelDataList[number].LevelDetail;
        SetLevel(number);
        SetEnemySprites(number);
    }

    /// <summary>
    /// ��Փx��ݒ肷��
    /// </summary>
    private void SetLevel(int number)
    {
        switch (LevelData.levelDataList[number].LevelState)
        {
            case LevelState.enOne:
                Data_QuestLevel.GetComponent<TextMeshProUGUI>().text = $"��Փx <sprite=4><sprite=5><sprite=5><sprite=5><sprite=5>";
                return;
            case LevelState.enTwo:
                Data_QuestLevel.GetComponent<TextMeshProUGUI>().text = $"��Փx <sprite=4><sprite=4><sprite=5><sprite=5><sprite=5>";
                return;
            case LevelState.enThree:
                Data_QuestLevel.GetComponent<TextMeshProUGUI>().text = $"��Փx <sprite=4><sprite=4><sprite=4><sprite=5><sprite=5>";
                return;
            case LevelState.enFour:
                Data_QuestLevel.GetComponent<TextMeshProUGUI>().text = $"��Փx <sprite=4><sprite=4><sprite=4><sprite=4><sprite=5>";
                return;
            case LevelState.enFive:
                Data_QuestLevel.GetComponent<TextMeshProUGUI>().text = $"��Փx <sprite=4><sprite=4><sprite=4><sprite=4><sprite=4>";
                return;
        }
    }

    /// <summary>
    /// �G�l�~�[�̉摜�𐶐����鏈��
    /// </summary>
    /// <param name="number">�N�G�X�g�̔ԍ�</param>
    private void SetEnemySprites(int number)
    {
        for (int enemyNumber = 0; enemyNumber < LevelData.levelDataList[number].enemyDataList.Count; enemyNumber++)
        {
            for (int dataNumber = 0; dataNumber < EnemyData.enemyDataList.Count; dataNumber++)
            {
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
                // �f�[�^��ݒ�
                var enemyData = enemyObject.GetComponent<DrawEnemyStatus>();
                enemyData.MyNumber = instrantiateNumber;
                enemyData.SetObject(Data_EnemyName, Data_Fire, Data_Ice, Data_Wind, Data_Thunder, Data_Light, Data_Dark);
                var uiAnimation = enemyObject.GetComponent<UIAnimation>();
                uiAnimation.AnimatorObject = AnimatorObject;

                if (m_gameManager.SaveDataManager.SaveData.saveData.EnemyRegisters[instrantiateNumber] == false)
                {
                    // �������Ă��Ȃ��Ȃ�J���[��ύX����
                    enemyObject.GetComponent<Image>().color = Color.gray;
                }
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
