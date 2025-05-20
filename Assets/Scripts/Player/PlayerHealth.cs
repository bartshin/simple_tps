using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
  public PlayerStat stat;

  // Start is called before the first frame update
  void Start()
  {
#if UNITY_EDITOR
    if (stat.Hp == null) {
      this.stat = PlayerStat.GetDummy();
    }
#endif
  }

  // Update is called once per frame
  void Update()
  {

  }

  public int TakeDamage(int attackDamage)
  {
    if (attackDamage < 0) {
      throw (new ArgumentException($"{nameof(attackDamage)} cannot be lesss than zero"));
    }
    var (currentHp, maxHp) = this.stat.Hp.Value;
    var takenDamage = Math.Min(currentHp, attackDamage);
    this.stat.Hp.Value = (currentHp - takenDamage, maxHp);
    if (currentHp > 0 && this.stat.Hp.Value.current <= 0) {
      this.Die();
    }
    return (takenDamage);
  }

  public int TakeDamage(int attackDamage, Transform attacker) 
  {
    return (this.TakeDamage(attackDamage));
  }

  void Die()
  {
    Debug.Log("Player dead");
  }

}
