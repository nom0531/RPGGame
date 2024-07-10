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
    [SerializeField, Header("エフェクト")]
    private GameObject Effect;
    [SerializeField, Header("ダメージ")]
    private GameObject Canvas;
    [SerializeField, Tooltip("SE")]
    private GameObject SECanvas;
    [SerializeField, Tooltip("生成する範囲")]
    private Vector3 RangeA, RangeB;

    private SoundManager m_soundManager;
    private AllOutAttackSystem m_allOutAttackSystem;
#if UNITY_EDITOR
    private TurnManager m_turnManager;
#endif
    private BattleManager m_battleManager;
    private BattleSystem m_battleSystem;
    private SetImage m_setImage;
    private GameObject m_effectObject = null;   // 生成したオブジェクト

    private OperatingPlayer m_operatingPlayer;


    private void Start()
    {
#if UNITY_EDITOR
        m_turnManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<TurnManager>();
#endif
        m_soundManager = GameManager.Instance.SoundManager;
        m_allOutAttackSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<AllOutAttackSystem>();
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
        m_setImage = GetComponent<SetImage>();
        m_operatingPlayer = m_battleManager.OperatingPlayer;    // 操作中のキャラクターを更新
    }

    /// <summary>
    /// キャラクターのボイスを再生する
    /// </summary>
    public void PlayCV()
    {
        m_operatingPlayer = m_battleManager.OperatingPlayer;    // 操作中のキャラクターを更新
        // 再生するボイスを設定する
        var number = (int)m_operatingPlayer;
        var rand = Random.Range(0, CharacterVoices[number].Voice.Count);
        var gameObject = Instantiate(CVObject);
        var audioSouse = gameObject.GetComponent<AudioSource>();
        // 音楽の再生を開始する
        gameObject.GetComponent<DestroySEObject>().PlayFlag = true;
        audioSouse.volume = GameManager.Instance.SoundManager.SEVolume * 1.2f;  // 音量を調整
        audioSouse.PlayOneShot(CharacterVoices[number].Voice[rand]);
    }

    /// <summary>
    /// 一枚絵の画像を設定する
    /// </summary>
    public void SetAllOutAttackImages()
    {
        m_operatingPlayer = m_battleManager.OperatingPlayer;    // 操作中のキャラクターを更新
        // 表示する画像を設定
        m_setImage.SetCharacter((int)m_operatingPlayer);
        // 全てのエネミーが死亡していないなら
        if(m_allOutAttackSystem.AllEnemyDieFlag == false)
        {
            m_setImage.SetTextImage(0);
            return;
        }
        m_setImage.SetTextImage(1);
    }

    /// <summary>
    /// サウンドエフェクトを再生する
    /// </summary>
    public void PlaySE_EventAttack()
    {
        m_soundManager.PlaySE(SENumber.enEventAttack, 0.1f);
    }

    /// <summary>
    /// サウンドエフェクトを再生する
    /// </summary>
    public void PlaySE_EventAttack2()
    {
        m_soundManager.PlaySE(SENumber.enEventAttack2, 0.1f);
    }

    /// <summary>
    /// サウンドエフェクトを再生する
    /// </summary>
    public void PlaySE_Explosion()
    {
        m_soundManager.PlaySE(SENumber.enExplosion, 0.5f);
    }

    /// <summary>
    /// エフェクトを生成する処理
    /// </summary>
    public void CreateEffect()
    {
        m_effectObject = Instantiate(Effect);
    }

    /// <summary>
    /// エフェクトを削除する処理
    /// </summary>
    public void DestroyEffect()
    {
        Destroy(m_effectObject.gameObject);
    }
    
    /// <summary>
    /// 座標をランダムに設定する
    /// </summary>
    private Vector3 SetRandPosition()
    {
        float x = Random.Range(RangeA.x, RangeB.x);
        float y = Random.Range(RangeA.y, RangeB.y);
        return new Vector3(x, y, 0.0f);
    }

    /// <summary>
    /// 見せかけのダメージを表示する
    /// </summary>
    public void DrawDamage()
    {
        var position = SetRandPosition();
        // ダメージテキストを生成
        var canvas = Instantiate(Canvas);
        canvas.transform.GetChild(0).position = position;
        // ダメージ量を設定
        var drawDamage = canvas.GetComponent<DrawDamage>();
        drawDamage.Damage = Random.Range(5, 50).ToString();
        drawDamage.Draw();
    }

    /// <summary>
    /// オノマトペを表示する
    /// </summary>
    public void DrawDamageSE()
    {
        var position = SetRandPosition();
        var canvas = Instantiate(SECanvas);
        canvas.transform.GetChild(0).position = position;
    }

    /// <summary>
    /// エネミーにダメージを与える
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
