using System;
using System.Collections.Generic;
using UnityEngine;
using Architecture;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
  [Serializable]
  public struct PlayerStat
  {
    public float Acceleration { get; set; }
    public float MaxSpeed { get; set; }
    public float MovingSpeedWhenAiming { get; set; }
    public float RotationSpeed { get; set; } 
  }

  public Vector3 Position => this.Avatar.transform.position;
  public Quaternion AimDirection => this.Aim.rotation;
  public ObservableValue<bool> IsAiming => this.attack.IsAiming;
  public ObservableValue<bool> IsMovinig => this.movement.IsMoving;

  PlayerMovement movement;
  PlayerAttack attack;
  public Transform Aim => this.aimContainer;
  [SerializeField]
  Transform aimContainer;
  [SerializeField]
  Animator animator;
  [SerializeField]
  CinemachineImpulseSource impulseSource;
  [SerializeField]
  Rigidbody rb;

  public Transform Avatar => this.avatar;
  [SerializeField]
  Transform avatar;

  [SerializeField]
  PlayerStat stat = new PlayerStat { 
    Acceleration = 10f, 
    MaxSpeed = 20f,
    MovingSpeedWhenAiming = 2f,
    RotationSpeed = 30f
  };

  void Awake()
  {
    this.movement = this.CreateMovementController();
    this.attack = this.CreateAttackController();
  }

  // Update is called once per frame
  void Update()
  {
    this.UpdateAttack();
    this.UpdateMovement();
  }

  void OnDestory()
  {
    this.attack.IsAiming.DestorySelf();
    this.movement.IsMoving.DestorySelf();
  }

  void UpdateAttack()
  {
    this.attack.Update();
  }

  PlayerAttack CreateAttackController()
  {
    PlayerAttack attack = new PlayerAttack(
          animator: this.animator,
          impulseSource: this.impulseSource);
    attack.IsAiming.OnChanged += this.OnAimingChanged;
    return (attack);
  }

  PlayerMovement CreateMovementController()
  {
    if (this.aimContainer == null) { 
      this.aimContainer = this.transform.Find("Aim Container");
    }
    if (this.Avatar == null) {
      this.avatar = this.transform.Find("Avatar");
    }

    return (new PlayerMovement(
          rigidbody: this.rb,
          animator: this.animator,
          avatar: this.Avatar,
          aim: this.Aim
          ));
  }

  void OnAimingChanged(bool isAiming) 
  {
    if (isAiming) {
      this.movement.Stop();
    }
  }

  void UpdateMovement()
  {
    var input = this.movement.GetInput();
    if (input.aimingInput != Vector2.zero) {
      var aimRotationAngles = this.movement.CalcAimRotationAngles(input.aimingInput);
      this.movement.RotateAim(aimRotationAngles);
    }
    if (input.movingInput != Vector2.zero) {
      var movingDirection = this.movement.CalcMovingDirection(input.movingInput);
      if (!this.attack.IsAiming.Value) {
        if (this.movement.IsOppositeDirection(movingDirection)) {
          this.movement.Slowdown();
        }
        this.movement.AddVelocity(
            direction: movingDirection,
            acceleration: this.stat.Acceleration,
            maxSpeed: this.stat.MaxSpeed);
      }
      else {
        this.movement.Move(
            direction: movingDirection,
            speed: this.stat.MovingSpeedWhenAiming
            );
      }
    }
    else {
      this.movement.Slowdown();
    }
    if (this.attack.IsAiming.Value) {
      this.movement.AvatarLookDirection(new Vector3(
            this.aimContainer.forward.x,
            0,
            this.aimContainer.forward.z
            ));   
    }
    this.movement.OnUpdate();
    if (this.movement.IsMoving.Value && !this.attack.IsAiming.Value) {
      this.movement.AvatarLookDirection(new Vector3(
            this.movement.Velocity.x,
            0, 
            this.movement.Velocity.z
            ));
    }
  }
}
