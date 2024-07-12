using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

/// <summary>
/// ���o�̃X�e�[�g
/// </summary>
public enum AllOutAttackState
{
    enAllOutAttackWaiting,  // ���o�̊J�n�҂�
    enAllOutAttackStart,    // ���o�J�n
    enAllOutAttackEnd,      // ���o�I��
}

public class AllOutAttackSystem : MonoBehaviour
{
    [SerializeField, Header("���U���f�[�^"),Tooltip("Timeline")]
    private PlayableDirector PlayableDirector;
    [SerializeField, Tooltip("�ꖇ�G��\������Canvas")]
    private Canvas Canvas;
    [SerializeField, Tooltip("�{�^��")]
    private GameObject Button;

    private BattleManager m_battleManager;
    private TurnManager m_turnManager;
    private AllOutAttackState m_allOutAttackState = AllOutAttackState.enAllOutAttackWaiting;
    private bool m_canStart = false;        // true�Ȃ�J�n�ł���
    private bool m_isAllEnemyDie = false;   // �S�ẴG�l�~�[���|���ꂽ���ǂ���

    public AllOutAttackState AllOutAttackState
    {
        get => m_allOutAttackState;
        set => m_allOutAttackState = value;
    }

    public bool AllEnemyDieFlag
    {
        get => m_isAllEnemyDie;
        set => m_isAllEnemyDie = value;
    }

    public bool CanStartFlag
    {
        get => m_canStart;
        set => m_canStart = value;
    }

    private void Start()
    {
        m_turnManager = GetComponent<TurnManager>();
        m_battleManager = GetComponent<BattleManager>();
    }

    private void Update()
    {
        CanAllOutAttack();
    }

    /// <summary>
    /// ���U�����ł��邩�ǂ����̔�����s���^�X�N
    /// </summary>
    private void CanAllOutAttack()
    {
        // ���U�����ł���Ȃ�
        if(m_canStart == true)
        {
            Button.GetComponent<Button>().interactable = true;
        }
        else
        {
            Button.GetComponent<Button>().interactable = false;
        }
    }

    /// <summary>
    /// �G�l�~�[�̎��S����
    /// </summary>
    public void IsAllEnemyDie()
    {
        // ��������
        for (int i = 0; i < m_battleManager.EnemyMoveList.Count; i++)
        {
            // ���肪1�̂ł��������Ă���Ȃ�return
            if (m_battleManager.EnemyMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                Debug.Log("�������Ă�G�l�~�[������");
                return;
            }
            m_battleManager.EnemyMoveList[i].gameObject.SetActive(false);
        }
        Debug.Log("�������Ă�G�l�~�[�͂��Ȃ���");
        AllEnemyDieFlag = true;
    }

    /// <summary>
    /// ���U�����J�n���鏈��
    /// </summary>
    public void StartAllOutAttack()
    {
        m_turnManager.AllOutAttackFlag = true;      // ���U���C�x���g���J�n�������Ƃ�������
        AllOutAttackState = AllOutAttackState.enAllOutAttackStart;
        PlayableDirector.Play();
    }

    /// <summary>
    /// ���U�����I�����鏈��
    /// </summary>
    public void EndAllOutAttack()
    {
        CanStartFlag = false;
        Button.GetComponent<AllOutAttackGauge>().ResetPoint();

        // �����G�l�~�[���S�Ď��S���Ă��Ȃ��Ȃ�
        if (AllEnemyDieFlag == false)
        {
            Canvas.GetComponent<Animator>().SetTrigger("NotActive");
            Canvas.gameObject.transform.GetChild(1).GetComponent<Animator>().SetTrigger("NotActive");
        }
        PlayableDirector.Stop();

        AllOutAttackState = AllOutAttackState.enAllOutAttackEnd;
    }
}