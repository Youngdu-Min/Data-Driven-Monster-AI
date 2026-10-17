using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    private float _currentHealth;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _recoveryValue;
    [SerializeField] private DamageablePart[] _damageParts;
    [SerializeField] private float _invinibleTime = 0.5f;
    [SerializeField] private bool _isInitDead;
    private bool _temporaryInvinible = false;
    private float _recentDamagedTime;
    private Coroutine _recoveryCoroutine;

    [SerializeField] private AudioClip _recoverySfx;
    [SerializeField] private AudioClip _spawnSfx;
    [SerializeField] private AudioClip _deadSfx;

    public Action onHit;
    public Action onSpawn;
    public Action onInvinibleEnd;
    public Action onDeath;
    public Action onRecovery;
    public Action onRecoveryFinish;


    private void Reset()
    {
        _damageParts = GetComponentsInChildren<DamageablePart>();
    }

    [ContextMenu("Set DamagePart Self")]
    public void SetDamagePartSelf()
    {
        if (!GetComponent<DamageablePart>())
            gameObject.AddComponent<DamageablePart>();
        Reset();
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = _maxHealth;
        for (int i = 0; i < _damageParts.Length; i++)
        {
            _damageParts[i].Init(this);
        }

        if (_isInitDead)
            DOVirtual.DelayedCall(Time.deltaTime, () => TriggerDeath());
    }

    public void SetInvincible(bool invincible)
        => _temporaryInvinible = invincible;

    public void Spawn()
    {
        print($"Spawn {gameObject}");
        onSpawn?.Invoke();

        /*
        if (_spawnSfx)
            Managers.Sound.Play(_spawnSfx);
        */

        _currentHealth = _maxHealth;
        _recentDamagedTime = 0;
    }

    public void StartHealthRecovery(float coolTime, int? repeatCount = null)
    {
        if (_recoveryCoroutine != null)
            StopCoroutine(_recoveryCoroutine);

        _recoveryCoroutine = StartCoroutine(RecoverHealth(coolTime, repeatCount));
    }

    public void StopHealthRecovery()
    {
        if (_recoveryCoroutine != null)
        {
            StopCoroutine(_recoveryCoroutine);
            _recoveryCoroutine = null;
        }
    }

    private IEnumerator RecoverHealth(float coolTime, int? repeatCount = null)
    {
        yield return new WaitForSeconds(coolTime);

        _currentHealth = Mathf.Min(_currentHealth + _recoveryValue, _maxHealth);

        /*
        if (_recoverySfx)
            Managers.Sound.Play(_recoverySfx);
        */

        onRecovery?.Invoke();

        if (_currentHealth == _maxHealth)
        {
            onRecoveryFinish?.Invoke();
            yield break;
        }

        if (repeatCount == null)
            _recoveryCoroutine = StartCoroutine(RecoverHealth(coolTime, repeatCount));
        else if (repeatCount.Value - 1 > 0)
            _recoveryCoroutine = StartCoroutine(RecoverHealth(coolTime, repeatCount.Value - 1));
    }

    public void SetRecoverValue(float value)
        => _recoveryValue = value;

    public bool Damage(float damage)
    {
        if (_temporaryInvinible)
            return false;
        if (_recentDamagedTime + _invinibleTime > Time.time)
            return false;
        _recentDamagedTime = Time.time;

        print($"{gameObject} Damaged {damage}");
        _currentHealth -= damage;
        onHit?.Invoke();

        if (IsDead())
            Death();
        else
            DOVirtual.DelayedCall(_invinibleTime, () => onInvinibleEnd.Invoke());

        return true;
    }

    void Death()
    {
        print($"{gameObject} is Dead");

        /*
        if (_deadSfx)
            Managers.Sound.Play(_deadSfx);
        */

        onDeath?.Invoke();
        //gameObject.SetActive(false);
    }

    public void TriggerDeath()
    {
        _currentHealth -= 9999;
        Death();
    }

    public bool IsDead()
        => _currentHealth < 0;
}
