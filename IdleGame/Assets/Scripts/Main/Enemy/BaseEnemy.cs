using System.Collections;
using TMPro;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IDamageable
{
    #region Fields

    private EnemyBlueprint _enemyBlueprint;

    private string _enemyName;
    private Coroutine _attackCoroutine;

    //체력
    private long _maxHp;
    private long _currentHP;

    //공격
    private long _damage;
    private float _attackSpeed;
    private float _range;

    //속도
    private float _moveSpeed;

    public long _rewards;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;

    public int TestWeight;

    #endregion

    #region Init

    public void SetEnemy(EnemyBlueprint blueprint)
    {
        _enemyBlueprint = blueprint;
        _enemyName = blueprint.EnemyName;
        _spriteRenderer.sprite = blueprint.EnemySprite;

        _maxHp = blueprint.HP;


        _damage = blueprint.Damage;
        _attackSpeed = blueprint.AttackSpeed;
        _range = blueprint.Range;

        _moveSpeed = blueprint.MoveSpeed;
        _rewards = blueprint.Rewards;

        gameObject.name = _enemyName;
        SetStatWeight(TestWeight);
        ResetHealth();
    }

    public void SetStatWeight(int Weight)
    {
        long _weight = (long)(Weight - 1);
        _maxHp = _maxHp + _maxHp * _weight;
        _damage = _damage + _damage * _weight;
        ResetHealth();
    }

    public void SetGoldWeight(int Weight)
    {
        long _weight = (long)(Weight - 1);
        _rewards = _rewards + _rewards * _weight;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    //추후 오브젝트 풀링 시 초기화 할 수 있게 메서드
    private void ResetHealth()
    {
        _currentHP = _maxHp;
    }
    #endregion

    #region Unity Flow

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        EvaluateState();
    }

    #endregion

    #region StateMethod
    //플레이어 방향으로 이동
    private void EvaluateState()
    {
        if (_range < Vector2.Distance(Manager.Game.Player.gameObject.transform.position, transform.position))
        {
            _rigidbody.velocity = new Vector2(
                Manager.Game.Player.gameObject.transform.position.x - transform.position.x,
                0f
                ).normalized * _moveSpeed;
        }
        else
        {
            _rigidbody.velocity = Vector2.zero;

            _attackCoroutine ??= StartCoroutine(AttackRoutine());
            //if (_attackCoroutine == null)
            //{
            //    _attackCoroutine = StartCoroutine(AttackRoutine());
            //}
        }
    }
    #endregion

    #region Attack Method
    //발사체를 생성 및 초기화
    private void CreateProjectail()
    {
        //Resources 폴더에서 EnemyProjectileFrame(발사체 틀)을 생성하고 go로 할당받음
        var go = Manager.Resource.InstantiatePrefab("EnemyProjectileFrame", gameObject.transform);

        //발사체 초기화를 위해 정보를 넘겨줌
        go.GetComponent<EnemyProjectileHandler>().ProjectileVFX = _enemyBlueprint.ProjectailVFX;
        go.GetComponent<EnemyProjectileHandler>().Damage = _damage;
    }

    //발사체 생성 코루틴
    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / _attackSpeed);
            CreateProjectail();
        }
    }

    public void TakeDamage(long Damage, DamageType damageTypeValue)
    {        
        AmountDamage(Damage);
        FloatingDamage(new Vector3(0, 0.05f, 0), Damage, damageTypeValue);
    }

    public void FloatingDamage(Vector3 position, long Damage, DamageType damageTypeValue)
    {
        GameObject DamageHUD = Manager.Resource.InstantiatePrefab("Canvas_FloatingDamage");
        DamageHUD.GetComponentInChildren<TextMeshProUGUI>().color = damageTypeValue == DamageType.Critical ? Color.red : Color.white;
        DamageHUD.transform.position = this.gameObject.transform.position + position;
        DamageHUD.GetComponentInChildren<UIFloatingText>().SetDamage(Damage);
    }

    private void AmountDamage(long Damage)
    {
        if (_currentHP - Damage <= 0)
        {
            _currentHP = 0;
            Manager.Stage.GetEnemyList().Remove(gameObject.GetComponent<BaseEnemy>());
            Die();
        }
        else
        {
            _currentHP -= Damage;
            
        }
    }

    private void Die()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        Manager.Game.Player.RewardGold(_rewards);

        if(Manager.Quest.CurrentQuest.questType == QuestType.DefeatEnemy)
        {
            Manager.Quest.QuestCurrentValueUp();
            UISceneMain uiSceneMain = Manager.UI.CurrentScene as UISceneMain;
            uiSceneMain.UpdateQuestObjective();
        }

        Destroy(gameObject);
    }
    #endregion
}
