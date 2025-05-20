using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Architecture;

public abstract class Monster : MonoBehaviour, IDamagable
{
  abstract public int MAX_HP { get; }
  public MonsterStatus Status 
  {
    get => this.status;
    protected set {
      this.status = value;
    }
  }
  MonsterStatus status;
  public Action OnDied;
  public Action OnAttack;
  public Action OnTakeDamage;
  [SerializeField]
  GauageImageUI hpBar;
  protected const float MOVING_SPEED_THRESHOLD = 0.3f;

  virtual public int TakeDamage(int attackDamage)
  {
    var hp = this.Status.Hp.Value;
    var takenDamage = Math.Min(attackDamage, hp.current);
    this.Status.Hp.Value = (hp.current - takenDamage, hp.max);
    if (this.Status.Hp.Value.current <= 0 &&
        hp.current > 0) {
      this.Die();
    }
    if (this.OnTakeDamage != null) {
      this.OnTakeDamage.Invoke();
    }
    return (takenDamage);
  }

  virtual public int TakeDamage(int attackDamage, Transform attacker)
  {
    return (this.TakeDamage(attackDamage));
  }

  virtual protected void Die()
  {
    if (this.OnDied != null) {
      this.OnDied.Invoke();
    }
  }

  virtual protected void OnMove(float speed)
  {
    this.status.IsMoving.Value = speed > MOVING_SPEED_THRESHOLD;
    this.status.MovingSpeed.Value = speed;
  }

  virtual protected void Awake()
  {
    if (this.status == null) {
      this.status = MonsterStatus.GetDummy();
    }
    if (this.hpBar != null) {
      this.hpBar.WatchingIntValue = this.Status.Hp;
    }
  }
}
