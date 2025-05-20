using System;
using System.Collections.Generic;
using UnityEngine;
using Architecture;
using Cinemachine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
  public PlayerStat stat;
  public ObservableValue<bool> IsAlive;
  public Action OnDied;
  [SerializeField]
  AudioClip hitSound;
  [SerializeField]
  AudioClip dieSound;
  [SerializeField]
  CinemachineImpulseSource impulseSource;

  const float HIT_SOUND_VOLUME = 0.5f;
  const float DIE_SOUND_VOLUME = 0.8f;

  // Start is called before the first frame update
  void Start()
  {
#if UNITY_EDITOR
    if (stat.Hp == null) {
      this.stat = PlayerStat.GetDummy();
    }
#endif
    this.IsAlive = new (this.stat.Hp.Value.current > 0);
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
    this.PlayHitSound();
    if (currentHp > 0 && this.stat.Hp.Value.current <= 0) {
      this.Die();
    }
    this.impulseSource.GenerateImpulse();
    return (takenDamage);
  }

  public int TakeDamage(int attackDamage, Transform attacker) 
  {
    return (this.TakeDamage(attackDamage));
  }

  void PlayHitSound()
  {
    var sfx = AudioManager.Shared.GetSfxController();
    sfx.transform.position = this.transform.position;
    sfx.SetVolume(PlayerHealth.HIT_SOUND_VOLUME);
    sfx.PlaySound(this.hitSound);
  }

  void Die()
  {
    this.IsAlive.Value = false;
    this.PlayDieSound();
    if (this.OnDied != null) {
      this.OnDied.Invoke();
    }
  }

  void PlayDieSound()
  {
    var sfx = AudioManager.Shared.GetSfxController();
    sfx.transform.position = this.transform.position;
    sfx.SetVolume(PlayerHealth.DIE_SOUND_VOLUME);
    sfx.PlaySound(this.dieSound);
  }
}
