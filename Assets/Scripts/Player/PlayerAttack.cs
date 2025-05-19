using System;
using UnityEngine;
using Architecture;
using Cinemachine;

public class PlayerAttack 
{
  public Gun Gun { 
    get => this.gun; 
    private set {
      if (this.gun != null) {
        this.gun.OnFire -= this.OnFired;
      }
      this.gun = value;
      if (value != null) {
        value.OnFire += this.OnFired;
      }
    } 
  }

  public ObservableValue<bool> IsAiming { get; private set; } = new (false);
  public Action OnShooting;
  [SerializeField] 
  KeyCode AimKey = KeyCode.Mouse1;
  [SerializeField]
  KeyCode FireKey = KeyCode.Mouse0;
  CinemachineImpulseSource impulseSource;
  int targetLayer;
  Transform attackOrigin;
  Transform aim;

  Animator animator; 
  Gun gun; 

  (GameObject gameObject, IDamagable damagable) lastTarget;

  public struct AttackInput
  {
    public bool IsHodingAim;
    public bool HasPressShoot; 
  }

  public PlayerAttack(
      Transform attackOrigin,
      Transform aim,
      Animator animator,
      CinemachineImpulseSource impulseSource)
  {
    this.attackOrigin = attackOrigin;
    this.aim = aim;
    this.Gun = new Gun();
    this.animator = animator;
    this.impulseSource = impulseSource;
    this.targetLayer = 1 << (LayerMask.NameToLayer("Monster"));
  }

  public void Update()
  {
    var input = this.GetInput();
    this.Gun.Update();
    if (input.HasPressShoot && !this.Gun.IsHot) {
      this.FireGun();
    }
    var isAiming = input.IsHodingAim;
    if (this.IsAiming.Value != isAiming) {
      this.animator.SetBool("IsAiming", isAiming);
    }
    this.IsAiming.Value = isAiming;
  }

  public AttackInput GetInput()
  {
    return (
      new AttackInput {
      IsHodingAim = Input.GetKey(this.AimKey),
      HasPressShoot = Input.GetKey(this.FireKey)
    });
  }

  void FireGun()
  {
    this.Gun.Fire();
    var target = this.FindTarget();
    if (target != null) {
      target.TakeDamage(this.Gun.Damage, this.attackOrigin);
    }
    if (this.OnShooting != null) {
      this.OnShooting.Invoke();
    }
  }

  void OnFired()
  {
    this.PlayShootingSound();
    this.UpdateAnimator();
    this.GenerateCameraEffect();
  }

  IDamagable FindTarget()
  {
    Debug.DrawRay(
        this.attackOrigin.position,
        this.aim.forward * 10f,
        Color.red,
        0.5f
        );
    if (Physics.Raycast(
        this.attackOrigin.position,
        this.aim.forward,
        out RaycastHit hitInfo,
        this.Gun.Range,
        this.targetLayer
        )) {
      if (this.lastTarget.gameObject != null &&
          hitInfo.transform.gameObject == this.lastTarget.gameObject) {
        return (this.lastTarget.damagable);
      }
      var target = IDamagable.FindIDamagableFrom(hitInfo.transform.gameObject);

      if (target != null) {
        this.lastTarget = (hitInfo.transform.gameObject, target);
      }
      return (target);
    }
    else {
      return (null);
    }
  }

  void PlayShootingSound()
  {
    var sfx = AudioManager.Shared.GetSfxController();
    sfx.PlaySound(this.Gun.FireSound);
  }

  void UpdateAnimator()
  {
    this.animator.SetTrigger("FireGun");
  }

  void GenerateCameraEffect()
  {
    this.impulseSource.GenerateImpulse();
  }
}
