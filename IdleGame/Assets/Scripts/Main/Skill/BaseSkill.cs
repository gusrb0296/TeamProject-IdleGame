using System.Collections;
using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    protected float _currentDurateTime;
    protected float _currentCoolDown;

    private bool _canUse = true;

    [SerializeField] private float effectDurateTime;
    [SerializeField] private float coolDown;

    Coroutine _skillDurateTimeCoroutine;
    Coroutine _coolDownCoroutine;

    protected virtual void ApplySkillEffect()
    {
        Debug.LogWarning("이 스킬의 'ApplySkillEffect' 메서드가 구현 및 오버라이드되지 않았습니다.");
    }

    protected virtual void RemoveSkillEffect()
    {
        Debug.LogWarning("이 스킬의 'RemoveSkillEffect' 메서드가 구현 및 오버라이드되지 않았습니다.");
    }

    public void UseSkill()
    {
        if (!_canUse)
        {
            return;
        }
        _canUse = false;
        StartCoroutine(CountDurateTime());
    }

    private IEnumerator CountDurateTime()
    {
        if (_skillDurateTimeCoroutine == null)
        {
            gameObject.GetComponent<BaseSkill>().ApplySkillEffect();
            Debug.LogWarning("스킬 유지 시작");
            _currentDurateTime = effectDurateTime;
            while (_currentDurateTime >= 0)
            {
                yield return null;
                _currentDurateTime -= Time.deltaTime;
            }
            Debug.LogWarning("스킬 유지 종료");
            _skillDurateTimeCoroutine = null;

            StartCoroutine(CountSkillCooldown());
        }
    }


    private IEnumerator CountSkillCooldown()
    {
        if (_coolDownCoroutine == null)
        {
            gameObject.GetComponent<BaseSkill>().RemoveSkillEffect();
            Debug.LogWarning("스킬 쿨타임 시작");
            _currentCoolDown = coolDown;
            while (_currentCoolDown >= 0)
            {
                yield return null;
                _currentCoolDown -= Time.deltaTime;
            }
            _canUse = true;
            _coolDownCoroutine = null;
            Debug.LogWarning("스킬 쿨타임 종료");
        }
    }
}
