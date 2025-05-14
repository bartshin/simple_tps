using System;
using System.Collections.Generic;
using UnityEngine;
using Architecture;

public class PlayerMovement 
{

  public Rigidbody rb { get; private set;  }
  public Transform avatar { get; private set; }
  public Transform aim { get; private set; }

  float rotationLerpThreshold = 5f;
  float rotationLerpSpeed = 10f;

  public PlayerMovement(
      Rigidbody rigidbody,
      Transform avatar,
      Transform aim
      )
  {
    this.rb = rigidbody;
    this.avatar = avatar;
    this.aim = aim;
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
    if (angles.x != 0) {
      this.aim.RotateAround(
          this.avatar.position,
          this.avatar.up,
          angles.x
          );
    }
    if (angles.y != 0) {
      var rotationAngles = this.aim.rotation.eulerAngles;
      rotationAngles.x += -angles.y;
      if (rotationAngles.x > 180) {
        rotationAngles.x -= 360;
      }
      rotationAngles.x = Mathf.Clamp(
          rotationAngles.x,
          InputSettings.Shared.MinVerticalPOV,
          InputSettings.Shared.MaxVerticalPOV
          );
      this.aim.rotation = Quaternion.Euler(
          rotationAngles.x,
          rotationAngles.y,
          rotationAngles.z
          );
    }
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
    var dir = (this.aim.forward * input.y) + (this.aim.right * input.x);
    dir.y = 0;
    return (dir.normalized);
  }

  public void AddVelocity(Vector3 direction, float acceleration, float maxSpeed)
  {
    this.rb.velocity += direction * Time.deltaTime * acceleration;
    var speed = this.rb.velocity.magnitude;
    if (this.rb.velocity.magnitude > maxSpeed) {
      this.rb.velocity *= speed / maxSpeed;
    } 
  }

  public (Vector2 movingInput, Vector2 aimingInput) GetInput()
  {
    switch (InputSettings.Shared.Control)
    {
      case InputSettings.ControlMode.KeyboardWithMouse:
        return (this.GetKeyboardMovingInput(), this.GetMouseAimInput()); 
        default: throw (new NotImplementedException());
    }
  }

  Vector2 GetKeyboardMovingInput()
  {
    var input = new Vector2(
        Input.GetAxisRaw("Horizontal"),
        Input.GetAxisRaw("Vertical")
        );
    if (input.magnitude > 1) {
      return (input.normalized);
    }
    return (input);
  }

  Vector2 GetMouseAimInput()
  {
    return (new Vector2(
        Input.GetAxis("Mouse X"),
        Input.GetAxis("Mouse Y")
        ));
  }
}
