using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeMonster : Monster
{
  enum State
  {
    Idle,
    Roaming,
    ChasingPlayer,
    InCombat
  }

  public override int MAX_HP => this.maxHp;
  [SerializeField]
  NavMeshAgent navMeshAgent;
  [SerializeField]
  Rigidbody rb;
  [SerializeField]
  Animator animator;

  Coroutine currentRoutine;
  YieldInstruction idleWait = new WaitForSeconds(1f);
  PlayerHealth player;
  IDamagable target;
  float distToPlayer;

  [SerializeField]
  State state;

  [Header("Configs")]
  [SerializeField]
  int maxHp;
  [SerializeField]
  int attackDamage;
  [SerializeField]
  float detectDistance = 15f;
  [SerializeField]
  float roamingDistance = 10f;
  [SerializeField]
  float roamingAccuracy = 2f;
  [SerializeField]
  float stopVelocityThreshold = 0.5f;
  [SerializeField]
  float attackRange = 2f;
  float delayAfterAttack = 3f;
  float attackDelay = 1f;
  float remainingAttackDelay;
  float dyingDelay = 2f;

  protected override void Awake()
  {
    this.Status = MonsterStatus.GetDummy();
    base.Awake();
    if (this.navMeshAgent == null) {
      this.navMeshAgent = this.GetComponent<NavMeshAgent>();
    }
    if (this.rb == null) {
      this.rb = this.GetComponent<Rigidbody>();
    }
    this.state = State.Idle;
  }

  public override int TakeDamage(int attackDamage, Transform attacker)
  {
    if (this.state == State.Idle ||
        this.state == State.Roaming) {
      if (this.currentRoutine != null) {
        this.StopCoroutine(this.currentRoutine);
        this.currentRoutine = null;
      }
      this.state = State.ChasingPlayer;
    }
    return base.TakeDamage(attackDamage, attacker);
  }

  void OnEnable()
  {
    this.Status.MovingSpeed.OnChanged += this.OnChangedMovingSpeed;
    this.Status.IsMoving.OnChanged += this.OnChangedIsMoving;
    if (this.player != null) {
      this.player.OnDied += this.OnPlayerDied;
    }
  }

  void OnDisable()
  {
    this.Status.MovingSpeed.OnChanged -= this.OnChangedMovingSpeed;
    this.Status.IsMoving.OnChanged -= this.OnChangedIsMoving;
    this.player.OnDied -= this.OnPlayerDied;
  }

  // Start is called before the first frame update
  void Start()
  {
    var playerObj = GameObject.FindWithTag("Player");
    if (playerObj != null) {
      this.player = playerObj.GetComponent<PlayerHealth>();
      this.target = playerObj.GetComponent<PlayerHealth>() as IDamagable;
      this.player.OnDied += this.OnPlayerDied;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (this.currentRoutine == null && this.player != null &&
        this.player.IsAlive.Value) {
      this.navMeshAgent.isStopped = false;
      this.currentRoutine = this.StartCoroutine(
          this.ChooseRoutine(this.state));
    }
    if (this.navMeshAgent != null) {
      this.OnMove(this.navMeshAgent.velocity.magnitude);
    }
  }
  protected override void Die()
  {
    if (this.currentRoutine != null) {
      this.StopCoroutine(this.currentRoutine);
    }
    this.StartCoroutine(this.StartDying());
  }

  IEnumerator ChooseRoutine(State state)
  {
    switch (state)
    {
      case State.Idle:
        this.state = State.Roaming;
        return (this.StartRoaming());
      case State.Roaming:
        return (this.StartRoaming());
      case State.ChasingPlayer:
        return (this.StartChasingPlayer());
      case State.InCombat:
        return (this.BattlePlayer());
      default:
        throw (new NotImplementedException());
    }
  }

  IEnumerator StartRoaming()
  {
    this.distToPlayer = Vector3.Distance(
        this.player.transform.position,
        this.transform.position
        );

    Vector3? nextDest = null;
    while (this.distToPlayer > this.detectDistance || !this.player.IsAlive.Value) {
      yield return (idleWait);
      if (!nextDest.HasValue) {
        nextDest = this.GetRandomDestination();
        if (nextDest.HasValue) {
          this.navMeshAgent.SetDestination(nextDest.Value);
        }
      }
      else if (this.IsReachedToDest()) {
        nextDest = null;
      }
      this.distToPlayer = Vector3.Distance(
        this.player.transform.position,
        this.transform.position
        );
    }
    this.state = State.ChasingPlayer;    
    this.currentRoutine = null;
  }

  Vector3? GetRandomDestination()
  {
    Vector3 randomPoint = this.transform.position + UnityEngine.Random.insideUnitSphere * this.roamingDistance;
    if (NavMesh.SamplePosition(
          randomPoint, 
          out NavMeshHit hit,
          this.roamingAccuracy, 
          NavMesh.AllAreas)) {
      return (hit.position);
    }
    return (null);
  }

  bool IsReachedToDest()
  {
    if (this.navMeshAgent.hasPath) {
      return (false);
    }
    if (this.navMeshAgent.remainingDistance < this.navMeshAgent.stoppingDistance) {
      return (true);
    }
    return (this.navMeshAgent.velocity.sqrMagnitude < this.stopVelocityThreshold);
  }

  IEnumerator StartChasingPlayer()
  {
    while (this.distToPlayer > this.attackRange && this.player.IsAlive.Value) {
      this.navMeshAgent.SetDestination(this.player.transform.position);
      this.distToPlayer = Vector3.Distance(this.transform.position, this.player.transform.position);
      yield return (null); 
    }
    this.state = State.InCombat;
    this.currentRoutine = null; 
  }

  IEnumerator BattlePlayer()
  {
    this.remainingAttackDelay = 0f;
    while (this.distToPlayer < this.attackRange * 1.5 && this.player.IsAlive.Value) {
      if (this.remainingAttackDelay < this.delayAfterAttack) {
        yield return (this.Attack());
        this.remainingAttackDelay = this.delayAfterAttack;
      }
      this.remainingAttackDelay -= Time.deltaTime;
      this.navMeshAgent.SetDestination(this.player.transform.position);
      this.distToPlayer = Vector3.Distance(this.transform.position, this.player.transform.position);
      yield return (null);
    } 
    this.state = State.ChasingPlayer;
    this.currentRoutine = null;
  }

  YieldInstruction Attack()
  {
    this.animator.SetTrigger("Attack");
    this.navMeshAgent.isStopped = true;
    this.target.TakeDamage(this.attackDamage);
    if (this.OnAttack != null) {
      this.OnAttack.Invoke();
    }
    return new WaitForSeconds(this.attackDelay);
  }

  IEnumerator StartDying()
  {
    base.Die();
    this.Status.IsAttacking.Value = false;
    this.Status.IsMoving.Value = false;
    this.navMeshAgent.isStopped = true;
    this.rb.velocity = Vector3.zero;
    yield return (new WaitForSeconds(this.dyingDelay));
    Destroy(this.gameObject);
  }

  void RotateTo(Vector3 dir)
  {
    this.transform.LookAt(dir);
  }

  void OnChangedIsMoving(bool isMoving)
  {
    this.animator.SetBool("IsMoving", isMoving);
  }

  void OnChangedMovingSpeed(float movingSpeed)
  {
    this.animator.SetFloat("MovingSpeed", movingSpeed);
  }

  void OnPlayerDied()
  {
    this.navMeshAgent.isStopped = true;
    this.state = State.Roaming;
    if (this.currentRoutine != null) {
      this.StopCoroutine(this.currentRoutine);
      this.currentRoutine = null;
    }
  }
}
