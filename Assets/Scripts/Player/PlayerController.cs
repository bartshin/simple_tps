using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Architecture;
using Cinemachine;

public class PlayerController : MonoBehaviour
{

  public Vector3 Position => this.Avatar.transform.position;
  public Quaternion AimDirection => this.AimContainer.rotation;
  public ObservableValue<bool> IsAiming => this.attack.IsAiming;
  public ObservableValue<bool> IsMovinig => this.movement.IsMoving;
  public ObservableValue<(int current, int max)> Hp => this.stat.Hp;
  public Action OnShooting 
  {
    get => this.attack.OnShooting;
    set {
      this.attack.OnShooting = value;
    }
  }

  PlayerMovement movement;
  PlayerAttack attack;
  public Transform AimContainer => this.aimContainer;

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
  GauageImageUI HpBarUI;

  [Header("Aim")]
  [SerializeField]
  Transform aimContainer;
  [SerializeField]
  Transform aimCameraPosition;
  [SerializeField]
  Transform aimEnd;

  PlayerStat stat = new PlayerStat { 
    Acceleration = 10f, 
    MaxSpeed = 20f,
    MovingSpeedWhenAiming = 2f,
    RotationSpeed = 30f,
    Hp = new ObservableValue<(int, int)>((100, 100))
  };

  void Awake()
  {
    var input = this.GetComponent<PlayerInput>();
    this.movement = this.CreateMovementController(input);
    this.attack = this.CreateAttackController(input);
    this.HpBarUI.WatchingIntValue = this.stat.Hp;
  }

  void Start()
  {
    var health = this.GetComponent<PlayerHealth>();
    health.stat = this.stat; 
  }

  // Update is called once per frame
  void Update()
  {
    var input = this.GetComponent<PlayerInput>();
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

  PlayerAttack CreateAttackController(PlayerInput input)
  {
    PlayerAttack attack = new PlayerAttack(
        attackOrigin: this.aimCameraPosition.transform,
        aim: this.aimEnd.transform,
        animator: this.animator,
        impulseSource: this.impulseSource,
        input: input
        );
    attack.IsAiming.OnChanged += this.OnAimingChanged;
    return (attack);
  }

  PlayerMovement CreateMovementController(PlayerInput input)
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
          aim: this.AimContainer,
          input: input
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
    this.UpdateAim(input.aimingInput);
    this.UpdatePosition(input.movingInput);
    this.OnMovementUpdated(input.movingInput);
  }

  void UpdateAim(Vector2 aimingInput)
  {
    if (this.attack.IsAiming.Value) {
      this.movement.AvatarLookDirection(new Vector3(
            this.AimContainer.forward.x,
            0,
            this.AimContainer.forward.z
            ));   
    }
    if (aimingInput != Vector2.zero) {
      var aimRotationAngles = this.movement.CalcAimRotationAngles(aimingInput);
      if (this.IsAiming.Value) {
        aimRotationAngles *= 0.3f;
      }
      this.movement.RotateAim(aimRotationAngles);
    }
  }

  void UpdatePosition(Vector2 movingInput)
  {
    var movingDirection = this.movement.CalcMovingDirection(movingInput);
    if (movingInput != Vector2.zero) {
      if (this.attack.IsAiming.Value) {
        this.movement.Move(
            direction: movingDirection,
            speed: this.stat.MovingSpeedWhenAiming
            );
      }
      else {
        if (this.movement.IsOppositeDirection(movingDirection)) {
          this.movement.Slowdown();
        }
        this.movement.AddVelocity(
            direction: movingDirection,
            acceleration: this.stat.Acceleration,
            maxSpeed: this.stat.MaxSpeed);
      }
    }
    if (movingInput == Vector2.zero){
      this.movement.Slowdown();
    }
  }

  void OnMovementUpdated(Vector2 movingInput)
  {
    var movingDirection = this.movement.CalcMovingDirection(movingInput);
    if (this.IsAiming.Value) {
      this.movement.OnUpdate(
          isMoving: movingInput != Vector2.zero,
          velocity: movingDirection);
    }
    else {
      this.movement.OnUpdate();
    }
  }
}
