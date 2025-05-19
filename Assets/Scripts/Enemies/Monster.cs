using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Architecture;

public abstract class Monster : MonoBehaviour, IDamagable
{
  abstract public int MAX_HP { get; }
  public ObservableValue<(int current, int max)> Hp { get; protected set; }
  public Action OnDied;
  [SerializeField]
  GauageImageUI hpBar;

  virtual public int TakeDamage(int attackDamage)
  {
    var hp = this.Hp.Value;
    var takenDamage = Math.Min(attackDamage, hp.current);
    this.Hp.Value = (hp.current - takenDamage, hp.max);
    if (this.Hp.Value.current <= 0 &&
        hp.current > 0) {
      this.Die();
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

  virtual protected void Awake()
  {
    this.Hp = new ((MAX_HP, MAX_HP));
    if (this.hpBar != null) {
      this.hpBar.WatchingIntValue = this.Hp;
    }
  }
}
