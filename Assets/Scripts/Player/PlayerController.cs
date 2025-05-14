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
    public float RotationSpeed { get; set; } 
  }

  public Vector3 Position => this.Avatar.transform.position;
  public Quaternion AimDirection => this.Aim.rotation;

  PlayerMovement movement;
  public Transform Aim => this.aim;
  [SerializeField]
  Transform aim;

  public Transform Avatar => this.avatar;
  [SerializeField]
  Transform avatar;

  [SerializeField]
  PlayerStat stat = new PlayerStat { 
    Acceleration = 5f, 
    MaxSpeed = 20f,
    RotationSpeed = 30f
  };

  public ObservableValue<bool> IsAiming { get; private set; } = new (false);


  void Awake()
  {
    this.movement = this.CreateMovement();
  }

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    this.UpdateAiming();
    this.UpdateMovement();
  }

  void OnDestory()
  {
    this.IsAiming.DestorySelf();
  }

  void UpdateAiming()
  {
    this.IsAiming.Value = Input.GetKey(KeyCode.Mouse1);
  }

  PlayerMovement CreateMovement()
  {
    if (this.Aim == null) { 
      this.aim = this.transform.Find("Aim");
    }
    if (this.Avatar == null) {
      this.avatar = this.transform.Find("Avatar");
    }
    var rigidbody = this.GetComponent<Rigidbody>();

    return (new PlayerMovement(
          rigidbody: rigidbody,
          avatar: this.Avatar,
          aim: this.Aim
          ));
  }

  void UpdateMovement()
  {
    var input = this.movement.GetInput();
    if (input.aimingInput != Vector2.zero) {
      var aimRotationAngles = this.movement.CalcAimRotationAngles(input.aimingInput);
      this.movement.RotateAim(aimRotationAngles);
    }
    var movingDirection = this.movement.CalcMovingDirection(input.movingInput);
    if (this.IsAiming.Value) {
      this.movement.AvatarLookDirection(new Vector3(
            this.aim.transform.localPosition.x,
            0,
            this.aim.transform.localPosition.z
            ));   
    }
    this.movement.AddVelocity(
        direction: movingDirection,
        acceleration: this.stat.Acceleration,
        maxSpeed: this.stat.MaxSpeed);
    if (input.movingInput != Vector2.zero) {
      this.movement.AvatarLookDirection(new Vector3(
            input.movingInput.x,
            0, input.movingInput.y
            ));
    }
  }
}
