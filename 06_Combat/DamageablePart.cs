using System;
using UnityEngine;

public class DamageablePart : MonoBehaviour
{
    [SerializeField] private Animator _partAnim = null;
    private Collider2D _partCollider;
    private Collider2D PartCollider => _partCollider ?? (_partCollider = GetComponent<Collider2D>());

    public Action onHit;
    private bool _temporaryInvinible;

    private static readonly int _anim_Idle = Animator.StringToHash("Idle");
    private static readonly int _anim_Hit = Animator.StringToHash("Hit");
    private static readonly int _anim_Dead = Animator.StringToHash("Dead");

    [SerializeField] private float _damageMultiplier = 1;
    Health _health;

    [SerializeField] private AudioClip _activeSfx;
    [SerializeField] private AudioClip _hitSfx;

    public void Init(Health parentHealth)
    {
        // _anim_Idle = Animator.StringToHash(_anim_State_Idle);
        // _anim_Hit = Animator.StringToHash(_anim_State_Hit);
        // _anim_Dead = Animator.StringToHash(_anim_State_Dead);

        _health = parentHealth;

        //_partAnim.CrossFade(_anim_Idle, 0);
        _health.onInvinibleEnd += () =>
        {
            if (!_temporaryInvinible)
                PlayAnimation(_anim_Idle);
        };
        _health.onDeath += () => PlayAnimation(_anim_Dead);


        Debug.Assert(PartCollider != null, "DamageablePart/_partCollider Null");
    }

    public (float calculDamage, bool damageReceived) SendDamage(float damage)
    {
        if (_temporaryInvinible || _health.IsDead())
            return (0, false);

        damage *= _damageMultiplier;
        bool isDamaged = _health.Damage(damage);

        PlayAnimation(_anim_Hit);

        if (isDamaged)
        {
            onHit?.Invoke();
            PlaySfx(_hitSfx);
        }

        return (damage, isDamaged);
    }

    public void SetInvincible(bool invincible)
    {
        Debug.Assert(PartCollider != null, "DamageablePart/_partCollider Null");
        print($"part invincible {gameObject} {invincible}");

        PlayAnimation(invincible ? _anim_Dead : _anim_Idle);
        if (!invincible)
            PlaySfx(_activeSfx);
        _temporaryInvinible = invincible;

        if (PartCollider)
            PartCollider.enabled = !invincible;
    }

    private void PlaySfx(AudioClip audioClip)
    {
        if (!audioClip)
            return;
        // Managers.Sound.Play(audioClip);
    }

    private void PlayAnimation(int animHash, int transitionDuration = 0)
    {
        Debug.Assert(_partAnim != null, "DamageablePart/_partAnim Null");
        if (!_partAnim)
            return;

        print($"PlayAnimation {gameObject} {animHash}");
        _partAnim.CrossFade(animHash, transitionDuration);
    }

    public void ChangeAnimator(Animator anim)
    {
        if (!anim)
            return;
        _partAnim = anim;
    }
}
