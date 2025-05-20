using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Architecture;

public class PlayerMovement 
{

  public Rigidbody rb { get; private set;  }
  public Animator animator { get; private set;  }
  public Transform avatar { get; private set; }
  public Transform aim { get; private set; }
  public ObservableValue<bool> IsMoving { get; private set; } = new (false);
  public Vector3 Velocity => this.rb.velocity;

  InputAction movingInput;
  InputAction aimInput;

  float rotationLerpThreshold = 5f;
  float rotationLerpSpeed = 10f;
  float slowdownLerpThreshold = 1f;
  float slowdownLerpSpeed = 2.5f;
  float movingThreshold = 0.1f;
  float aimDist;
  (float min, float max) aimSinValues;
  (float min, float max) aimCosValues;

  public PlayerMovement(
      Rigidbody rigidbody,
      Animator animator,
      Transform avatar,
      Transform aim,
      PlayerInput input
      )
  {
    this.rb = rigidbody;
    this.animator = animator;
    this.avatar = avatar;
    this.aim = aim;
    this.aimDist = aim.transform.position.z;
    this.aimSinValues = (
        MathF.Sin(InputSettings.Shared.MinVerticalPOV),  
        MathF.Sin(
        InputSettings.Shared.MaxVerticalPOV
        ));
    this.aimCosValues = (
        MathF.Cos(InputSettings.Shared.MinVerticalPOV),
        MathF.Cos(InputSettings.Shared.MaxVerticalPOV)
        );
    this.movingInput = input.actions["Move"];
    this.aimInput = input.actions["Rotate Aim"];
    this.movingInput.Enable();
    this.aimInput.Enable();
  }

  public void OnUpdate()
  {
    this.OnUpdate(this.rb.velocity.magnitude > this.movingThreshold, this.rb.velocity);
    if (IsMoving.Value) {
      this.AvatarLookDirectionLerp(new Vector3(
        this.Velocity.x,
        0, 
        this.Velocity.z
        ));
    }
  }

  public void OnUpdate(bool isMoving, Vector3 velocity)
  {
    if (this.IsMoving.Value != isMoving) {
      this.animator.SetBool("IsMoving", isMoving);
    }
    this.IsMoving.Value = isMoving;
    this.animator.SetFloat("VelocityX",
        Math.Abs(Vector3.Dot(this.avatar.right, velocity))
        );
    this.animator.SetFloat("VelocityZ",
        Math.Abs(Vector3.Dot(this.avatar.forward, velocity)));
  }

  public void AvatarLookDirection(Vector3 dir)
  {
    this.avatar.rotation = Quaternion.LookRotation(dir);
  }

  public void AvatarLookDirectionLerp(Vector3 dir)
  {
    var currentDir = this.avatar.forward;
    if (Vector3.Distance(dir, currentDir) > this.rotationLerpThreshold)  {
      this.avatar.rotation = Quaternion.Lerp(
          this.avatar.rotation,
          Quaternion.LookRotation(dir),
          this.rotationLerpSpeed * Time.deltaTime
          );
    }
    else {
      this.avatar.forward = dir;
    }
  }

  public bool IsOppositeDirection(Vector3 dirToMove)
  {
    if (this.rb.velocity.magnitude < this.movingThreshold) {
      return (false);
    }
    if (this.rb.velocity.x * dirToMove.x < 0 && 
        this.rb.velocity.z * dirToMove.z < 0) {
      return (true);
    }
    return (Vector2.Dot(
          new Vector2(this.rb.velocity.x, this.rb.velocity.z),
          new Vector3(dirToMove.x, dirToMove.z)
          ) < 0);
  }

  public Vector2 CalcAimRotationAngles(Vector2 input)
  {
    Vector2 angles;
    switch (InputSettings.Shared.Control)
    {
      case InputSettings.ControlMode.KeyboardWithMouse:
        angles = new Vector2(
          input.x * InputSettings.Shared.MouseSpeedForPOV * Time.deltaTime,
          input.y * InputSettings.Shared.MouseSpeedForPOV * Time.deltaTime
        );
        break;
      default: throw (new NotImplementedException());
    }
    return (angles);
  }

  public void RotateAim(Vector2 angles)
  {
    var currentRotation = this.aim.localRotation.eulerAngles;
    if (angles.x != 0) {
      currentRotation.y += angles.x;
    }
    if (angles.y != 0) {
      currentRotation.x -= angles.y;
      if (currentRotation.x > 180) {
        currentRotation.x -= 360;
      }
      currentRotation.x = Math.Clamp(
        currentRotation.x,
        InputSettings.Shared.MinVerticalPOV,
        InputSettings.Shared.MaxVerticalPOV
        );
    }
    this.aim.localRotation = Quaternion.Euler(currentRotation);
  }

  public Vector2 CalcAvatarRotateAngle(Vector2 input, float rotateSpeed)
  {
    return (new Vector2(0, input.x * rotateSpeed));
  }

  public void RotateAvatar(Vector2 angles)
  {
    var avatarAngles = this.avatar.rotation.eulerAngles;
    avatarAngles.x += angles.x;
    avatarAngles.y += angles.y;
    var currentRotation = this.avatar.rotation;
    if (Vector3.Distance(
          currentRotation.eulerAngles,
          avatarAngles
          ) > this.rotationLerpThreshold) {
      this.avatar.transform.rotation = Quaternion.Lerp(
          currentRotation,
          Quaternion.Euler(avatarAngles),
          this.rotationLerpSpeed * Time.deltaTime 
          );
    }
    else {
      this.avatar.transform.rotation = Quaternion.Euler(avatarAngles);
    }
  }

  public Vector3 CalcMovingDirection(Vector2 input)
  {
    var movingDir = this.aim.forward * input.y + 
      this.aim.right * input.x;
    movingDir.y = 0;
    return (movingDir);
  }

  public void AddVelocity(Vector3 direction, float acceleration, float maxSpeed)
  {
    this.rb.velocity += direction * Time.deltaTime * acceleration;
    var speed = this.rb.velocity.magnitude;
    if (this.rb.velocity.magnitude > maxSpeed) {
      this.rb.velocity *= maxSpeed / speed;
    } 
  }

  public void Move(Vector3 direction, float speed) 
  {
    this.rb.Move(
        this.rb.position + direction * Time.deltaTime * speed,
        this.rb.rotation
        );
  }

  public (Vector2 movingInput, Vector2 aimingInput) GetInput()
  {
    switch (InputSettings.Shared.Control)
    {
      case InputSettings.ControlMode.KeyboardWithMouse:
        var movingInput = this.movingInput.ReadValue<Vector2>();
        var aimInput = this.aimInput.ReadValue<Vector2>();
        return (movingInput.magnitude > 1 ? movingInput.normalized: movingInput, aimInput.magnitude > 1 ? aimInput.normalized: aimInput); 
        default: throw (new NotImplementedException());
    }
  }

  public void Stop()
  {
    this.rb.velocity = new Vector3(0, this.rb.velocity.y, 0);
  }

  public void Slowdown()
  {
    if (this.rb.velocity.magnitude > this.slowdownLerpThreshold) {
      this.rb.velocity = Vector3.Lerp(
          this.rb.velocity,
          Vector3.zero,
          this.slowdownLerpSpeed * Time.deltaTime
          );
    }
    else {
      this.rb.velocity = Vector3.zero;
    }
  }
}
