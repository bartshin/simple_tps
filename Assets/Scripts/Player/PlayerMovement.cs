using System;
using System.Collections.Generic;
using UnityEngine;
using Architecture;

public class PlayerMovement 
{

  Rigidbody rb;
  Transform avatar;
  Transform aim;
  PlayerStat stat;
  ObservableValue<bool> isAiming;

  public PlayerMovement(
      Rigidbody rigidbody,
      Transform avatar,
      Transform aim,
      ObservableValue<bool> isAiming, 
      PlayerStat stat
      )
  {
    this.rb = rigidbody;
    this.avatar = avatar;
    this.aim = aim;
    this.isAiming = isAiming;
    this.stat = stat;
  }

  public void Update()
  {
    var input = this.GetInput();
    
    this.Rotate(input.movingInput, input.aimingInput);
    this.Move(input.movingInput);
  }

  void Rotate(Vector2 movingInput, Vector2 aimingInput)
  {
    if (this.isAiming.Value) {

    }
    else {
      Debug.Log(aimingInput.y);
      var lookRotation = this.aim.eulerAngles;
      //lookRotation.x = lookRotation.x + aimingInput.y * InputSettings.Shared.MouseSpeedForPOV;
      lookRotation.x = Mathf.Clamp(
          lookRotation.x + aimingInput.y * InputSettings.Shared.MouseSpeedForPOV,
          InputSettings.Shared.MinVerticalPOV,
          InputSettings.Shared.MaxVerticalPOV
          );
      lookRotation.y = 
          lookRotation.y + aimingInput.x * InputSettings.Shared.MouseSpeedForPOV;
      this.aim.rotation = Quaternion.Euler(lookRotation);
    }
  }

  void Move(Vector2 movingInput)
  {
    var currentForward = this.avatar.forward;
    this.rb.velocity += 
      (currentForward * movingInput.y) 
      * Time.deltaTime * this.stat.MoveAcceleration;
    var speed = this.rb.velocity.magnitude;
    if (this.rb.velocity.magnitude > this.stat.MaxSpeed) {
      this.rb.velocity *= speed / this.stat.MaxSpeed;
    } 
  }

  (Vector2 movingInput, Vector2 aimingInput) GetInput()
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
