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
  [SerializeField] 
  KeyCode AimKey = KeyCode.Mouse1;
  [SerializeField]
  KeyCode FireKey = KeyCode.Mouse0;
  CinemachineImpulseSource impulseSource;

  Animator animator; 
  Gun gun; 

  public struct AttackInput
  {
    public bool IsHodingAim;
    public bool HasPressShoot; 
  }

  public PlayerAttack(Animator animator, CinemachineImpulseSource impulseSource)
  {
    this.Gun = new Gun();
    this.animator = animator;
    this.impulseSource = impulseSource;
  }

  public void Update()
  {
    var input = this.GetInput();
    this.Gun.Update();
    if (input.HasPressShoot && !this.Gun.IsHot) {
      this.Gun.Fire();
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

  void OnFired()
  {
    this.PlayShootingSound();
    this.UpdateAnimator();
    this.GenerateCameraEffect();
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
