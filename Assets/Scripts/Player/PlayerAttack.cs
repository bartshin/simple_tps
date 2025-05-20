using System;
using Architecture;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack 
{
  const float FIRE_SOUND_VOLUME = 0.65f;
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
  InputAction aimInput;
  InputAction shootInput;
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
      CinemachineImpulseSource impulseSource,
      PlayerInput input
      )
  {
    this.attackOrigin = attackOrigin;
    this.aim = aim;
    this.Gun = new Gun();
    this.animator = animator;
    this.impulseSource = impulseSource;
    this.targetLayer = ~(( 1 << LayerMask.NameToLayer("Player")) 
        | (1 << LayerMask.NameToLayer("UI")));
    this.aimInput = input.actions["Aim"];
    this.shootInput = input.actions["Shoot"];
    this.aimInput.Enable();
    this.shootInput.Enable();
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
      IsHodingAim = this.aimInput.IsPressed(),
      HasPressShoot = this.shootInput.IsPressed() 
    });
  }

  void FireGun()
  {
    this.Gun.Fire();
    var (hitObject, point, normal) = this.GetHitInfo();
    if (hitObject != null) {
      this.SpawnParticle(point, normal);
      var target = this.FindTargetFrom(hitObject);
      if (target != null) {
        target.TakeDamage(this.Gun.Damage, this.attackOrigin);
      }
    }
    if (this.OnShooting != null) {
      this.OnShooting.Invoke();
    }
  }

  void SpawnParticle(Vector3 position, Vector3 normal)
  {
    var particle = this.Gun.GetParticle();
    particle.transform.position = position; 
    particle.transform.LookAt(normal);
  }

  void OnFired()
  {
    this.PlayShootingSound();
    this.UpdateAnimator();
    this.GenerateCameraEffect();
  }

  (GameObject hitObject, Vector3 point, Vector3 normal) GetHitInfo()
  {
    var dir = (this.aim.position - this.attackOrigin.position).normalized;
#if UNITY_EDITOR
    Debug.DrawRay(
        this.attackOrigin.position,
        dir * this.Gun.Range,
        Color.red,
        0.5f
        );
#endif
    if (Physics.Raycast(
        this.attackOrigin.position,
        dir,
        out RaycastHit hitInfo,
        this.Gun.Range,
        this.targetLayer
        )) {
      return (hitInfo.collider.gameObject, hitInfo.point, hitInfo.normal);
    }
    return (null, Vector3.zero, Vector3.zero);
  }

  IDamagable FindTargetFrom(GameObject gameObject)
  {
    if (this.lastTarget.gameObject != null &&
        gameObject == this.lastTarget.gameObject) {
      return (this.lastTarget.damagable);
    }
    var target = IDamagable.FindIDamagableFrom(gameObject);
    if (target != null) {
      this.lastTarget = (gameObject, target);
    }
    return (target);
  }

  void PlayShootingSound()
  {
    var sfx = AudioManager.Shared.GetSfxController();
    sfx.SetVolume(PlayerAttack.FIRE_SOUND_VOLUME);
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
