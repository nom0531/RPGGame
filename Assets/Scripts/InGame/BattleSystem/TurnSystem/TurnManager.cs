using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// �^�[�����񂷑�
/// </summary>
public enum TurnStatus
{
    enPlayer,   // �v���C���[
    enEnemy,    // �G�l�~�[
}

public class TurnManager : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject ResultObject;
    [SerializeField, Header("�^�[���J�n���̐�s��")]
    private TurnStatus m_turnStatus = TurnStatus.enPlayer;
    [SerializeField, Header("���U���g�f�[�^"), Tooltip("UI��\������܂ł̑ҋ@����")]
    private float WaitTime = 1.0f;

    private BattleManager m_battleManager;
    private LockOnManager m_lockOnManager;
    private GameManager m_gameManager;
    private DrawBattleResult m_drawBattleResult;                // ���U���g���o
    private int m_turnSum = 1;                                  // �����^�[����

    public int TurnSum
    {
        get => m_turnSum;
    }

    public TurnStatus TurnStatus
    {
        get => m_turnStatus;
        set => m_turnStatus = value;
    }

    private void Start()
    {
        m_battleManager = GetComponent<BattleManager>();
        m_lockOnManager = GetComponent<LockOnManager>();
        m_drawBattleResult = ResultObject.GetComponent<DrawBattleResult>();
        m_gameManager = GameManager.Instance;

        // �^�X�N��ݒ肷��
        GameClearTask().Forget();
        GameOverTask().Forget();
    }

    private void Update()
    {
        if(m_battleManager.GameState != GameState.enPlay)
        {
            return;
        }
        IsGameClear();
        IsGameOver();
    }

    /// <summary>
    /// �^�[�����I�����Ă��邩���肷��
    /// </summary>
    public void IsTurnEnd()
    {
        // �S���̍s�����I�����Ă��Ȃ��Ȃ���s���Ȃ�
        for (int playerNumber = 0; playerNumber < m_battleManager.PlayerMoveList.Count; playerNumber++)
        {
            if (m_battleManager.PlayerMoveList[playerNumber].ActionEndFlag == false)
            {
                return;
            }
        }
        for (int enemyNumber = 0; enemyNumber < m_battleManager.EnemyMoveList.Count; enemyNumber++)
        {
            if (m_battleManager.EnemyMoveList[enemyNumber].ActionEndFlag == false)
            {
                return;
            }
        }
        TurnEnd();
    }

    /// <summary>
    /// ��̃X�e�[�^�X�����Z�b�g���āA���̃^�[���Ɉڍs����
    /// </summary>
    private void TurnEnd()
    {
        // ���̑���L�����N�^�[������A�J�������Đݒ肷��
        m_battleManager.InitValue();
        m_lockOnManager.ResetCinemachine();
        m_battleManager.ResetGameStatus();
        // �^�[����n��
        m_turnStatus = TurnStatus.enPlayer;
        m_turnSum++;
    }

    /// <summary>
    /// �Q�[���N���A���擾����
    /// </summary>
    private void IsGameClear()
    {
        var sumEP = 0;  // �l��EP�̑���
        for (int i = 0; i < m_battleManager.EnemyMoveList.Count; i++)
        {
            // ���肪1�̂ł��������Ă���Ȃ�Q�[���N���A�ł͂Ȃ�
            if (m_battleManager.EnemyMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                return;
            }
            // �������Ă��Ȃ��Ȃ�EP�����Z
            sumEP += m_battleManager.EnemyDataBase.enemyDataList[m_battleManager.EnemyMoveList[i].MyNumber].EnhancementPoint;
        }
        m_drawBattleResult.EP = sumEP;
        m_battleManager.GameState = GameState.enBattleWin;
        // �A�j���[�V�������Đ�
        for (int i = 0; i < m_battleManager.PlayerMoveList.Count; i++)
        {
            m_battleManager.PlayerMoveList[i].PlayerAnimation.PlayAnimation(AnimationState.enWin);
        }
    }

    /// <summary>
    /// �Q�[���I�[�o�[���擾����
    /// </summary>
    private void IsGameOver()
    {
        for (int i = 0; i < m_battleManager.PlayerMoveList.Count; i++)
        {
            // 1�̂ł��������Ă���Ȃ�Q�[���I�[�o�[�ł͂Ȃ�
            if (m_battleManager.PlayerMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                return;
            }
        }
        m_battleManager.GameState = GameState.enBattleLose;
        // �A�j���[�V�������Đ�
        for (int i = 0; i < m_battleManager.PlayerMoveList.Count; i++)
        {
            m_battleManager.PlayerMoveList[i].PlayerAnimation.PlayAnimation(AnimationState.enLose);
        }
    }

    /// <summary>
    /// �Q�[���N���A���o�̃^�X�N
    /// </summary>
    async UniTask GameClearTask()
    {
        // ���o���I�������Ȃ�ȉ��̏��������s����
        await UniTask.WaitUntil(() => m_battleManager.GameState == GameState.enBattleWin);
        await UniTask.Delay(TimeSpan.FromSeconds(WaitTime));
        m_drawBattleResult.GameClearStaging();
    }

    /// <summary>
    /// �Q�[���I�[�o�[���o�̃^�X�N
    /// </summary>
    async UniTask GameOverTask()
    {
        // ���o���I�������Ȃ�ȉ��̏��������s����
        await UniTask.WaitUntil(() => m_battleManager.GameState == GameState.enBattleLose);
        await UniTask.Delay(TimeSpan.FromSeconds(WaitTime));
        m_drawBattleResult.GameOverStaging();
    }
}
