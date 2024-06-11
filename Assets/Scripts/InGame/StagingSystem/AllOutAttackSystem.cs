using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllOutAttackSystem : MonoBehaviour
{
    [SerializeField, Header("���̃G�t�F�N�g")]
    private GameObject SmokeEffect;
    [SerializeField, Header("�{�^��")]
    private GameObject Button;
    [SerializeField, Header("�\������ꖇ�G"),Tooltip("�\������Canvas")]
    private Canvas Canvas;
    [SerializeField, Tooltip("�ꖇ�G")]
    private Sprite[] Sprite;

    /// <summary>
    /// �G�l�~�[�S�̂̏��
    /// </summary>
    private enum EnemyState
    {
        enNotDeath, // ����ł��Ȃ�
        enDeath     // ����ł���
    }

    private BattleSystem m_battleSystem;
    private StagingSystem m_stagingSystem;
    private EnemyState m_enemyState;
    private bool m_canStart = false;        // true�Ȃ�J�n�ł���
    private bool m_isStartAllOut = false;   // true�Ȃ瑍�U�����J�n����
    private int m_spriteNumber = 0;         // �\������ꖇ�G�̔ԍ�

    public bool CanStartFlag
    {
        get => m_canStart;
        set => m_canStart = value;
    }

    public bool StartAllOutFlag
    {
        get => m_isStartAllOut;
        set => m_isStartAllOut = value;
    }

    private void Start()
    {
        m_battleSystem = GetComponent<BattleSystem>();
        m_stagingSystem = GetComponent<StagingSystem>();
    }

    private void Update()
    {
        aaaTask();
    }

    /// <summary>
    /// ���U�����ł��邩�ǂ����̔�����s���^�X�N
    /// </summary>
    private void aaaTask()
    {
        // ���U�����ł���Ȃ�c
        if(m_canStart == true)
        {
            Button.SetActive(true);     // �{�^�����A�N�e�B�u�ɂ���
        }
    }

    /// <summary>
    /// ���U�����J�n���鏈��
    /// </summary>
    public void StartAllOutAttack()
    {
        AllOutAttack();
    }

    /// <summary>
    /// ���U���̏���
    /// </summary>
    private void AllOutAttack()
    {
        //SmokeEffect.SetActive(true);
        int damage = m_battleSystem.AllOutAttack();     // �_���[�W�ʂ��v�Z
        // �G�l�~�[�̏�Ԃŏ����𕪊�
        switch (m_enemyState)
        {
            case EnemyState.enNotDeath:
                NotDeath();
                break;
            case EnemyState.enDeath:
                Death(m_spriteNumber);
                break;
        }
        EndAllOutAttack();
    }

    /// <summary>
    /// ����ł��Ȃ����̏���
    /// </summary>
    private void NotDeath()
    {
        Debug.Log("���U���I��!�G�l�~�[�͑S�ł��Ȃ�����");
    }

    /// <summary>
    /// ���񂾂Ƃ��̏���
    /// </summary>
    private void Death(int number)
    {
        Debug.Log("���U���I��!�G�l�~�[�͑S��");
        Canvas.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Sprite[number];
    }

    /// <summary>
    /// ���U�����I�����鏈��
    /// </summary>
    private void EndAllOutAttack()
    {
        Button.SetActive(false);     // �{�^�����A�N�e�B�u�ɂ���
        StartAllOutFlag = false;
    }
}
