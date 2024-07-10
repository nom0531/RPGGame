using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Voices
{
    public List<AudioClip> Voice = new List<AudioClip>();
}

public class AllOutAttackEvent : MonoBehaviour
{
    [SerializeField, Header("CV")]
    private List<Voices> CharacterVoices = new List<Voices>();
    [SerializeField]
    private GameObject CVObject;
    [SerializeField, Header("SE")]
    private GameObject SEObject;
    [SerializeField, Header("�G�t�F�N�g")]
    private GameObject Effect;
    [SerializeField, Header("�_���[�W")]
    private GameObject Canvas;
    [SerializeField, Tooltip("SE")]
    private GameObject SECanvas;
    [SerializeField, Tooltip("��������͈�")]
    private Vector3 RangeA, RangeB;
    [SerializeField, Header("�摜")]
    private Sprite[] CharacterImage;

    private SoundManager m_soundManager;
    private AllOutAttackSystem m_allOutAttackSystem;
#if UNITY_EDITOR
    private TurnManager m_turnManager;
#endif
    private BattleManager m_battleManager;
    private BattleSystem m_battleSystem;
    private SetImage m_setImage;
    private GameObject m_effectObject = null;   // ���������I�u�W�F�N�g
    private GameObject m_dummyObject;           // �C�x���g�p�̃_�~�[�I�u�W�F�N�g

    private OperatingPlayer m_operatingPlayer;
    private Vector3 m_dummyPosition = Vector3.zero;


    private void Start()
    {
#if UNITY_EDITOR
        m_turnManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<TurnManager>();
#endif
        m_soundManager = GameManager.Instance.SoundManager;
        m_allOutAttackSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<AllOutAttackSystem>();
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
        m_dummyObject = transform.GetChild(1).gameObject;
        m_dummyPosition = m_dummyObject.transform.position;
        m_setImage = GetComponent<SetImage>();
        m_operatingPlayer = m_battleManager.OperatingPlayer;    // ���쒆�̃L�����N�^�[���X�V
    }

    /// <summary>
    /// �L�����N�^�[�̃{�C�X���Đ�����
    /// </summary>
    public void PlayCV()
    {
        m_operatingPlayer = m_battleManager.OperatingPlayer;    // ���쒆�̃L�����N�^�[���X�V
        // �Đ�����{�C�X��ݒ肷��
        var number = (int)m_operatingPlayer;
        var rand = Random.Range(0, CharacterVoices[number].Voice.Count);
        var gameObject = Instantiate(CVObject);
        var audioSouse = gameObject.GetComponent<AudioSource>();
        // ���y�̍Đ����J�n����
        gameObject.GetComponent<DestroySEObject>().PlayFlag = true;
        audioSouse.volume = GameManager.Instance.SoundManager.SEVolume * 1.2f;  // ���ʂ𒲐�
        audioSouse.PlayOneShot(CharacterVoices[number].Voice[rand]);
    }

    /// <summary>
    /// �ꖇ�G�̉摜��ݒ肷��
    /// </summary>
    public void SetAllOutAttackImages()
    {
        m_operatingPlayer = m_battleManager.OperatingPlayer;    // ���쒆�̃L�����N�^�[���X�V
        // �\������摜��ݒ�
        m_setImage.SetCharacter((int)m_operatingPlayer);
        // �S�ẴG�l�~�[�����S���Ă��Ȃ��Ȃ�
        if(m_allOutAttackSystem.AllEnemyDieFlag == false)
        {
            m_setImage.SetTextImage(0);
            return;
        }
        m_setImage.SetTextImage(1);
    }

    /// <summary>
    /// �A�j���[�V�������Đ�����
    /// </summary>
    public void PlayAnimation()
    {
        m_operatingPlayer = m_battleManager.OperatingPlayer;                                            // ���쒆�̃L�����N�^�[���X�V
        m_dummyObject.GetComponent<SpriteRenderer>().sprite = CharacterImage[(int)m_operatingPlayer];   // �摜���X�V
        m_dummyObject.transform.position = m_dummyPosition;                                             // ��ʓ��ɉf��Ȃ��ʒu�ɒ��߂���
        var animator = m_dummyObject.GetComponent<Animator>();
        string animationTrigger = "";
        switch (m_operatingPlayer)
        {
            case OperatingPlayer.enAttacker:
                animationTrigger = "PlayerName_Unity";
                break;
            case OperatingPlayer.enBuffer:
                animationTrigger = "PlayerName_Toko";
                break;
            case OperatingPlayer.enHealer:
                animationTrigger = "PlayerName_Yuko";
                break;
        }
        animator.SetTrigger(animationTrigger);
    }

    /// <summary>
    /// �T�E���h�G�t�F�N�g���Đ�����
    /// </summary>
    public void PlaySE_EventAttack()
    {
        m_soundManager.PlaySE(SENumber.enEventAttack, 0.1f);
    }

    /// <summary>
    /// �T�E���h�G�t�F�N�g���Đ�����
    /// </summary>
    public void PlaySE_EventAttack2()
    {
        m_soundManager.PlaySE(SENumber.enEventAttack2, 0.1f);
    }

    /// <summary>
    /// �T�E���h�G�t�F�N�g���Đ�����
    /// </summary>
    public void PlaySE_Explosion()
    {
        m_soundManager.PlaySE(SENumber.enExplosion, 0.5f);
    }

    /// <summary>
    /// �G�t�F�N�g�𐶐����鏈��
    /// </summary>
    public void CreateEffect()
    {
        m_effectObject = Instantiate(Effect);
    }

    /// <summary>
    /// �G�t�F�N�g���폜���鏈��
    /// </summary>
    public void DestroyEffect()
    {
        Destroy(m_effectObject.gameObject);
    }
    
    /// <summary>
    /// ���W�������_���ɐݒ肷��
    /// </summary>
    private Vector3 SetRandPosition()
    {
        float x = Random.Range(RangeA.x, RangeB.x);
        float y = Random.Range(RangeA.y, RangeB.y);
        return new Vector3(x, y, 0.0f);
    }

    /// <summary>
    /// ���������̃_���[�W��\������
    /// </summary>
    public void DrawDamage()
    {
        var position = SetRandPosition();
        // �_���[�W�e�L�X�g�𐶐�
        var canvas = Instantiate(Canvas);
        canvas.transform.GetChild(0).position = position;
        // �_���[�W�ʂ�ݒ�
        var drawDamage = canvas.GetComponent<DrawDamage>();
        drawDamage.Damage = Random.Range(5, 50).ToString();
        drawDamage.Draw();
    }

    /// <summary>
    /// �I�m�}�g�y��\������
    /// </summary>
    public void DrawDamageSE()
    {
        var position = SetRandPosition();
        var canvas = Instantiate(SECanvas);
        canvas.transform.GetChild(0).position = position;
    }

    /// <summary>
    /// �G�l�~�[�Ƀ_���[�W��^����
    /// </summary>
    public void AddDamage()
    {
#if UNITY_EDITOR
        m_turnManager.AllOutAttackFlag = true;
#endif
        for (int i = 0; i<m_battleManager.EnemyMoveList.Count; i++)
        {
            m_battleManager.DamageEnemy(i, m_battleSystem.AllOutAttack());
        }
    }
}
