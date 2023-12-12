using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �퓬���̃G�l�~�[�̃X�e�[�^�X
/// </summary>
public struct EnemyBattleStatus
{
    public int HP;                              // ���݂�HP
    public int ATK;                             // ���݂�ATK
    public int DEF;                             // ���݂�DEF
    public int SPD;                             // ���݂�SPD
    public ActorHPState HPState;                // HP�̏��
    public ActorAbnormalState AbnormalState;    // ��Ԉُ�
    public ActionType ActionType;               // ���̍s��
}

public class EnemyMove : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private EnemyDataBase EnemyData;
    [SerializeField]
    private SkillDataBase SkillData;

    private const int HPMIN_VALUE = 0;              // HP�̍ŏ��l
    private const int RAND_MAX = 100;               // �����̍ő�l

    private List<PlayerMove> m_playerMove;
    private SaveDataManager m_saveDataManager;
    private BattleSystem m_battleSystem;
    private BuffCalculation m_buffCalculation;      // �o�t�̌v�Z
    private DrawCommandText m_drawCommandText;      // �R�}���h�̕\��
    private EnemyBattleStatus m_enemyBattleStatus;  // �퓬���̃X�e�[�^�X
    private int m_myNumber = 0;                     // ���g�̔ԍ�

    public int MyNumber
    {
        get => m_myNumber;
        set => m_myNumber = value;
    }

    public EnemyBattleStatus EnemyStatus
    {
        get => m_enemyBattleStatus;
    }

    public ActorHPState ActorHPState
    {
        get => m_enemyBattleStatus.HPState;
        set => m_enemyBattleStatus.HPState = value;
    }

    /// <summary>
    /// �G�l�~�[�𔭌����Ă��邩�擾����
    /// </summary>
    /// <param name="enemyNumber">�G�l�~�[�̔ԍ�</param>
    /// <returns>true�Ȃ甭�����Ă���</returns>
    public bool GetTrueEnemyRegister(int enemyNumber)
    {
        return m_saveDataManager.SaveData.saveData.EnemyRegister[enemyNumber];
    }

    /// <summary>
    /// �G�l�~�[�𔭌������t���O��true�ɂ���
    /// </summary>
    /// <param name="enemyNumber">�G�l�~�[�̔ԍ�</param>
    public void SetTrueEnemyRegister(int enemyNumber)
    {
        m_saveDataManager.SaveData.saveData.EnemyRegister[enemyNumber] = true;
        m_saveDataManager.Save();
    }

    /// <summary>
    /// �����̑ϐ��x�𔭌����Ă��邩�ǂ����擾����
    /// </summary>
    /// <param name="elementNumber">�����ԍ�</param>
    public bool GetTrueElementRegister(int elementNumber)
    {
        return m_saveDataManager.SaveData.saveData.ElementRegister[m_myNumber].Elements[elementNumber];
    }

    /// <summary>
    /// �����̑ϐ��x�𔭌������t���O��true�ɂ���
    /// </summary>
    /// <param name="elementNumber">�����ԍ�</param>
    public void SetTrueElementRegister(int elementNumber)
    {
        m_saveDataManager.SaveData.saveData.ElementRegister[m_myNumber].Elements[elementNumber] = true;
        m_saveDataManager.Save();
    }

    private void Awake()
    {
        m_saveDataManager = GameManager.Instance.SaveData;
        m_battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
        m_buffCalculation = this.gameObject.GetComponent<BuffCalculation>();
        m_drawCommandText = this.gameObject.GetComponent<DrawCommandText>();


        // �l��������
        m_enemyBattleStatus.HP = EnemyData.enemyDataList[m_myNumber].HP;
        m_enemyBattleStatus.ATK = EnemyData.enemyDataList[m_myNumber].ATK;
        m_enemyBattleStatus.DEF = EnemyData.enemyDataList[m_myNumber].DEF;
        m_enemyBattleStatus.SPD = EnemyData.enemyDataList[m_myNumber].SPD;
        ActorHPState = ActorHPState.enMaxHP;
        m_enemyBattleStatus.ActionType = ActionType.enNull;

        for(int skillNumber = 0; skillNumber < EnemyData.enemyDataList[m_myNumber].skillDataList.Count; skillNumber++)
        {
            for(int dataNumber = 0; dataNumber < SkillData.skillDataList.Count; dataNumber++)
            {
                // ���ʔԍ��������Ȃ�f�[�^������������
                if (EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillNumber != SkillData.skillDataList[dataNumber].SkillNumber)
                {
                    continue;
                }

                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillName = SkillData.skillDataList[dataNumber].SkillName;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillSprite = SkillData.skillDataList[dataNumber].SkillSprite;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].POW = SkillData.skillDataList[dataNumber].POW;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillElement = SkillData.skillDataList[dataNumber].SkillElement;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].EnhancementPoint = SkillData.skillDataList[dataNumber].EnhancementPoint;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillNecessary = SkillData.skillDataList[dataNumber].SkillNecessary;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].Type = SkillData.skillDataList[dataNumber].Type;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].BuffType = SkillData.skillDataList[dataNumber].BuffType;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillType = SkillData.skillDataList[dataNumber].SkillType;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].EffectRange = SkillData.skillDataList[dataNumber].EffectRange;
            }
        }
    }

    private void Start()
    {
        // playerMove��l�����p��
        PlayerMove[] playerMove = FindObjectsOfType<PlayerMove>();
        m_playerMove = new List<PlayerMove>(playerMove);
        // �\�[�g
        m_playerMove.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));
    }

    private void FixedUpdate()
    {
        RotationSprite();
        IsStateEnDie();

        for (int i = 0; i < (int)BuffStatus.enNum; i++)
        {
            if (m_buffCalculation.GetEffectEndFlag((BuffStatus)i) == false)
            {
                continue;
            }
            // ���ʎ��Ԃ��I�����Ă���Ȃ�X�e�[�^�X��߂�
            ResetEnemyBuffStatus((BuffStatus)i);
            m_buffCalculation.SetEffectEndFlag((BuffStatus)i, false);
        }
    }

    /// <summary>
    /// HP���񕜂��鏈��
    /// </summary>
    /// <param name="recoverValue">�񕜗�</param>
    public void RecoverHP(int recoverValue)
    {
        m_enemyBattleStatus.HP += recoverValue;

        // ���ʂ�葽���񕜂���Ȃ�␳
        if (m_enemyBattleStatus.HP >= EnemyData.enemyDataList[m_myNumber].HP)
        {
            // �S��
            m_enemyBattleStatus.HP = EnemyData.enemyDataList[m_myNumber].HP;
        }
        else if (m_enemyBattleStatus.HP >= EnemyData.enemyDataList[m_myNumber].HP / 4)
        {
            // �̗͂�1/4�ȉ��Ȃ��Ԃ�ύX
            ActorHPState = ActorHPState.enMaxHP;
        }
    }

    /// <summary>
    /// HP�����炷����
    /// </summary>
    /// <param name="decrementValue">�_���[�W��</param>
    public void DecrementHP(int decrementValue)
    {
        m_enemyBattleStatus.HP -= decrementValue;

        // ���ʂ������Ȃ�␳
        if (m_enemyBattleStatus.HP <= HPMIN_VALUE)
        {
            // ���S����
            m_enemyBattleStatus.HP = HPMIN_VALUE;
            ActorHPState = ActorHPState.enDie;
        }
        else if (m_enemyBattleStatus.HP <= EnemyData.enemyDataList[m_myNumber].HP / 4)
        {
            // �̗͂�1/4�ȉ��Ȃ��Ԃ�ύX
            ActorHPState = ActorHPState.enFewHP;
        }
    }

    /// <summary>
    /// �h�䏈��
    /// </summary>
    /// <returns></returns>
    public int Guard()
    {
        int defensePower = m_battleSystem.Guard(m_enemyBattleStatus.DEF);
        return defensePower;
    }

    /// <summary>
    /// �o�t�E�f�o�t�����������Ƃ��̃X�e�[�^�X��ύX����
    /// </summary>
    /// <param name="buffType">�ύX����X�e�[�^�X</param>
    /// <param name="statusFloatingValue">�ύX����l</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="isBuff">true�Ȃ�o�t�Bfalse�Ȃ�f�o�t</param>
    public void SetEnmeyBuffStatus(BuffType buffType, int statusFloatingValue, int skillNumber, bool isBuff)
    {
        int effectTime = EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].EffectTime;

        if (skillNumber < 0)
        {
            // �����}�C�i�X���w�肳��Ă�������ʎ��Ԃ�1
            effectTime = 1;
        }

        switch (buffType)
        {
            case BuffType.enATK:
                if (isBuff == true)
                {
                    m_buffCalculation.CalcBuff(BuffStatus.enBuff_ATK, statusFloatingValue, m_enemyBattleStatus.ATK, effectTime);
                    m_drawCommandText.SetStatusText(BuffStatus.enBuff_ATK);
                    break;
                }
                m_buffCalculation.CalcDebuff(BuffStatus.enDeBuff_ATK, statusFloatingValue, m_enemyBattleStatus.ATK, effectTime);
                m_drawCommandText.SetStatusText(BuffStatus.enDeBuff_ATK);
                break;
            case BuffType.enDEF:
                if (isBuff == true)
                {
                    m_buffCalculation.CalcBuff(BuffStatus.enBuff_DEF, statusFloatingValue, m_enemyBattleStatus.DEF, effectTime);
                    m_drawCommandText.SetStatusText(BuffStatus.enBuff_DEF);
                    break;
                }
                m_buffCalculation.CalcDebuff(BuffStatus.enDeBuff_DEF, statusFloatingValue, m_enemyBattleStatus.DEF, effectTime);
                m_drawCommandText.SetStatusText(BuffStatus.enDeBuff_DEF);
                break;
            case BuffType.enSPD:
                if (isBuff == true)
                {
                    m_buffCalculation.CalcBuff(BuffStatus.enBuff_SPD, statusFloatingValue, m_enemyBattleStatus.SPD, effectTime);
                    m_drawCommandText.SetStatusText(BuffStatus.enBuff_SPD);
                    break;
                }
                m_buffCalculation.CalcDebuff(BuffStatus.enDeBuff_SPD, statusFloatingValue, m_enemyBattleStatus.SPD, effectTime);
                m_drawCommandText.SetStatusText(BuffStatus.enDeBuff_SPD);
                break;
        }
    }

    /// <summary>
    /// �摜����]������
    /// </summary>
    private void RotationSprite()
    {
        Vector3 lookAtCamera = Camera.main.transform.position;
        lookAtCamera.y = transform.parent.gameObject.transform.position.y;
        transform.parent.gameObject.transform.LookAt(lookAtCamera);
    }

    /// <summary>
    /// ���g�̏�Ԃ��Ђ񎀂Ȃ�
    /// </summary>
    private void IsStateEnDie()
    {
        if(m_enemyBattleStatus.HPState!= ActorHPState.enDie)
        {
            return;
        }

        this.gameObject.SetActive(false);   // ���g���\���ɂ���
        this.tag = "DieEnemy";              // �^�O��ύX����
    }

    /// <summary>
    /// �^�[�Q�b�g�v���C���[��I�����鏈��
    /// </summary>
    /// <returns>�v���C���[�̔ԍ�</returns>
    public int SelectTargetPlayer()
    {
        // �����𐶐�
        int attackNumber = m_battleSystem.GetRandomValue(0, 2);
        // �^�[�Q�b�g���Ђ񎀂łȂ��Ȃ炱���ŏI��
        if(m_playerMove[attackNumber].PlayerStatus.HPState != ActorHPState.enDie)
        {
            return attackNumber;
        }

        for(int i = 0; i < m_playerMove.Count; i++)
        {
            if(m_playerMove[attackNumber].PlayerStatus.HPState == ActorHPState.enDie)
            {
                continue;
            }

            attackNumber = i;   // �Ђ񎀂łȂ��v���C���[���^�[�Q�b�g�ɂ���
            break;
        }

        return attackNumber;
    }

    /// <summary>
    /// �^�[�Q�b�g�G�l�~�[��I�����鏈��
    /// </summary>
    /// <param name="maxNumber">�G�l�~�[�̍ő吔</param>
    /// <returns></returns>
    public int SelectTargetEnemy(int maxNumber)
    {
        // 0�`�G�l�~�[�̍ő吔�ŗ����𐶐�
        int rand = m_battleSystem.GetRandomValue(0, maxNumber, false);
        return rand;
    }

    /// <summary>
    /// �Ђ񎀏�Ԃ̃G�l�~�[�̒����烉���_����1�̃^�[�Q�b�g�ɂ��鏈��
    /// </summary>
    /// <returns>�����_���ɑI�������I�u�W�F�N�g</returns>
    public GameObject SelectTargetDieEnemy()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("DieEnemy");

        // 0�`�G�l�~�[�̍ő吔�ŗ����𐶐�
        int rand = m_battleSystem.GetRandomValue(0, enemys.Length, false);

        return enemys[rand].gameObject;
    }

    /// <summary>
    /// �s����I�����鏈��
    /// </summary>
    /// <returns>�s���ԍ�</returns>
    public ActionType SelectAttackType()
    {
        int rand = m_battleSystem.GetRandomValue(0, RAND_MAX);

        switch (m_enemyBattleStatus.HPState)
        {
            // HP���ʏ�Ȃ�
            case ActorHPState.enMaxHP:
                if (rand <= 50)
                {
                    m_enemyBattleStatus.ActionType = ActionType.enAttack;
                }
                if (rand <= 90 && rand > 50)
                {
                    m_enemyBattleStatus.ActionType = ActionType.enSkillAttack;
                }
                if (rand <= 98 && rand > 90)
                {
                    m_enemyBattleStatus.ActionType = ActionType.enGuard;
                }
                if (rand <= RAND_MAX && rand > 98)
                {
                    m_enemyBattleStatus.ActionType = ActionType.enNull;
                }
                break;
            // HP��1/4�Ȃ�
            case ActorHPState.enFewHP:
                if (rand <= 40)
                {
                    m_enemyBattleStatus.ActionType = ActionType.enAttack;
                }
                if (rand <= 70 && rand > 41)
                {
                    m_enemyBattleStatus.ActionType = ActionType.enSkillAttack;
                }
                if (rand <= 90 && rand > 71)
                {
                    m_enemyBattleStatus.ActionType = ActionType.enGuard;
                }
                if (rand <= 98 && rand > 91)
                {
                    m_enemyBattleStatus.ActionType = ActionType.enEscape;
                }
                if (rand <= RAND_MAX && rand > 98)
                {
                    m_enemyBattleStatus.ActionType = ActionType.enNull;
                }
                break;
        }

        return m_enemyBattleStatus.ActionType;
    }

    /// <summary>
    /// �g�p����X�L�������肷�鏈��
    /// </summary>
    /// <returns>�X�L���̔ԍ�</returns>
    public int SelectSkill()
    {
        // 0�`�f�[�^�����X�L���̍ő吔�܂łŗ����𐶐�����
        int skillNumber = m_battleSystem.GetRandomValue(0, EnemyData.enemyDataList[m_myNumber].skillDataList.Count, false);

        return skillNumber;
    }

    /// <summary>
    /// �v���C���[�̃X�e�[�^�X�����ɖ߂�
    /// </summary>
    private void ResetEnemyBuffStatus(BuffStatus buffStatus)
    {
        switch (buffStatus)
        {
            case BuffStatus.enBuff_ATK:
                m_buffCalculation.ResetStatus(BuffStatus.enBuff_ATK, m_enemyBattleStatus.ATK, true);
                break;
            case BuffStatus.enBuff_DEF:
                m_buffCalculation.ResetStatus(BuffStatus.enBuff_DEF, m_enemyBattleStatus.DEF, true);
                break;
            case BuffStatus.enBuff_SPD:
                m_buffCalculation.ResetStatus(BuffStatus.enBuff_SPD, m_enemyBattleStatus.SPD, true);
                break;
            case BuffStatus.enDeBuff_ATK:
                m_buffCalculation.ResetStatus(BuffStatus.enDeBuff_ATK, m_enemyBattleStatus.ATK, false);
                break;
            case BuffStatus.enDeBuff_DEF:
                m_buffCalculation.ResetStatus(BuffStatus.enDeBuff_DEF, m_enemyBattleStatus.DEF, false);
                break;
            case BuffStatus.enDeBuff_SPD:
                m_buffCalculation.ResetStatus(BuffStatus.enDeBuff_SPD, m_enemyBattleStatus.SPD, false);
                break;
        }
    }
}
