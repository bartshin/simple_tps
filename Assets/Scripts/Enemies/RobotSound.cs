using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSound : MonoBehaviour
{
  Monster monsterController;
  [SerializeField]
  AudioClip moveSound;
  [SerializeField]
  AudioClip attackSound;
  [SerializeField]
  AudioClip hitSound;
  [SerializeField]
  AudioClip destroyedSound;
  const float MOVE_SOUND_VOLOUME = 0.1f;
  const float ATTACK_SOUND_VOLOUME = 0.8f;
  const float HIT_SOUND_VOLOUME = 0.8f;
  const float DESTROYED_SOUND_VOLOUME = 0.3f;
  SfxController moveSfx;

  void Awake()
  {
    this.monsterController = this.GetComponent<Monster>();
  }

  void Start()
  {
    this.moveSfx = AudioManager.Shared.GetSfxController();
    this.moveSfx.SetSound(this.moveSound);
    this.moveSfx.SetLoop(true);
    this.moveSfx.transform.parent = this.transform;
    this.moveSfx.transform.position = Vector3.zero;
    this.moveSfx.SetVolume(RobotSound.MOVE_SOUND_VOLOUME);
    this.SetEvents();
  }

  void OnEnable()
  {
    if (this.monsterController.Status != null) {
      this.SetEvents();
    }
  }

  void OnDisable()
  {
    this.monsterController.OnTakeDamage -= this.OnTakeDamage;
    this.monsterController.OnDied -= this.OnDied;
    this.monsterController.OnAttack -= this.OnAttack;
    this.monsterController.Status.IsMoving.OnChanged -= this.OnChangedIsMoving;

  }

  void SetEvents()
  {
    this.monsterController.OnTakeDamage += this.OnTakeDamage;
    this.monsterController.OnDied += this.OnDied;
    this.monsterController.OnAttack += this.OnAttack;
    this.monsterController.Status.IsMoving.OnChanged += this.OnChangedIsMoving;
  }

  void OnChangedIsMoving(bool isMoving)
  {
    if (isMoving) {
      this.moveSfx.PlayBack();
    }
    else {
      this.moveSfx.Pause();
    }
  }

  void OnAttack()
  {
    var sfx = AudioManager.Shared.GetSfxController();
    sfx.transform.position = this.transform.position;
    sfx.SetVolume(RobotSound.ATTACK_SOUND_VOLOUME);
    sfx.PlaySound(this.attackSound);
  }

  void OnTakeDamage()
  {
    var sfx = AudioManager.Shared.GetSfxController();
    sfx.transform.position = this.transform.position;
    sfx.SetVolume(RobotSound.HIT_SOUND_VOLOUME);
    sfx.PlaySound(this.hitSound);
  }

  void OnDied()
  {
    this.moveSfx.transform.parent = AudioManager.Shared.transform;
    var sfx = AudioManager.Shared.GetSfxController();
    sfx.transform.position = this.transform.position;
    sfx.SetVolume(RobotSound.DESTROYED_SOUND_VOLOUME);
    sfx.PlaySound(this.destroyedSound);
  }
}
