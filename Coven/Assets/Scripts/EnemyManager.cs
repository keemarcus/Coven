using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : CharacterManager
{
    public NavMeshAgent agent;
    public Transform target;
    public Transform sight;
    public bool canSee;
    public float sightRange = 10f;
    public float sightAngle = 30f;
    public float minAttackRange;
    public float maxAttackRange;
    public AIState currentState;
    public LayerMask detectionLayerMask;
    public float idleWanderTimer;
    public bool selectedForRevive;
    PlayerManager playerManager;
    public Shield shield;

    public SpellSlot spell;
    public GameObject spellOriginOffset;

    public Collider2D meleeAttackCollider;
    public int meleeDamage;
    public string meleeDamageType;

    AudioSource audioSource;
    public AudioClip meleeHitSound;


    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        idleWanderTimer = 0f;
        selectedForRevive = false;

        playerManager = FindObjectOfType<PlayerManager>();
        shield = this.GetComponentInChildren<Shield>();
        alreadyCast = false;

        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    public void Cast()
    {
        if (spell == null || spellOriginOffset == null || alreadyCast || this.aimDirection == Vector2.zero) { return; }
        aimDirection.Normalize();
        spell.Cast(spellOriginOffset.transform.position, this.gameObject, audioSource);
        alreadyCast = true;
    }

    public void MeleeAttack(CharacterManager targetHit)
    {
        if (meleeHitSound != null && audioSource != null)
        {
            audioSource.clip = meleeHitSound;
            audioSource.Play();
        }
        targetHit.TakeDamage(this.meleeDamage, this.meleeDamageType);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterManager targetHit = collision.gameObject.GetComponent<CharacterManager>();
        if(targetHit != null && targetHit.teamTag != this.teamTag)
        {
            MeleeAttack(targetHit);
        }
    }

    private void HandleStateMachine()
    {
        if (isInteracting) { return; }
        if (currentState != null)
        {
            AIState nextState = currentState.Tick(this, this.animationManager);

            if (nextState != null && nextState != currentState)
            {
                SwitchToNextState(nextState);
            }
        }
    }

    private void SwitchToNextState(AIState newState)
    {
        currentState = newState;
    }

    private void FixedUpdate()
    {
        HandleStateMachine();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerManager != null)
        {
            if (playerManager.transform.position.y > this.transform.position.y && !this.isDead)
            {
                this.GetComponent<SpriteRenderer>().sortingOrder = 6;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().sortingOrder = 4;
            }

            if (playerManager.activeSpell.name.Contains("Raise Dead") && this.selectedForRevive)
            {
                this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        

        if (isDead) { agent.enabled = false; return; }
        if (isInteracting) { agent.enabled = false; }
        float delta = Time.fixedDeltaTime;
        
        // update the animator
        animationManager.UpdateAnimator(delta, agent.velocity.magnitude, agent.velocity);
    }

    public void LookForEnemy()
    {
        this.target = null;
        
        Debug.DrawRay(sight.transform.position, sightVector, Color.red, 1f);

        // see if any characters are within sight range
        Collider2D[] targets = Physics2D.OverlapCircleAll(sight.transform.position, sightRange);
        foreach (Collider2D target in targets)
        {

            // first check to see if the target is an enemy
            if(target.gameObject.GetComponent<CharacterManager>() != null && target.gameObject.GetComponent<CharacterManager>().teamTag != teamTag && target.gameObject.GetComponent<CharacterManager>().teamTag != "NPC" && !target.gameObject.GetComponentInChildren<CharacterManager>().isDead)
            {                
                // then see if we are looking in that direction
                Vector2 vectorToTarget = target.transform.position - sight.transform.position;
                if (Mathf.Abs(Vector2.Angle(sightVector, vectorToTarget)) <= sightAngle)
                {
                    // finally see if we hit them with a raycast
                    Debug.DrawRay(sight.transform.position, vectorToTarget, Color.red, 1f);
                    RaycastHit2D hit = Physics2D.Raycast(sight.transform.position, (target.transform.position - sight.transform.position), sightRange, detectionLayerMask);
                    if (hit && hit.collider.gameObject.GetComponentInChildren<CharacterManager>()) 
                    { 
                        //canSee = true;
                        // set the new target
                        this.target = hit.collider.gameObject.transform.GetChild(1);
                    }
                    //else { canSee = false; }
                }
            }
        }

        if (this.target == null)
        {
            this.canSee = false;
        }
        else
        {
            this.canSee = true;
        }
    }

    public void RaiseFromDead(string newTeamTag)
    {
        if(this.gameObject.GetComponentInChildren<Shield>() != null)
        {
            this.gameObject.GetComponent<GoblinPursueTargetState>().blockState = null;
        }
        this.isDead = false;
        this.agent.enabled = true;
        this.characterStats.CurrentHP = this.characterStats.MaxHP;
        this.teamTag = newTeamTag;
        this.selectedForRevive = false;
        this.animationManager.animator.SetBool("Dead", false);
        this.target = null;
        this.canSee = false;

        // reenable all the colliders for this character
        foreach (Collider2D collider in this.gameObject.GetComponents<Collider2D>())
        {
            if(collider != meleeAttackCollider)
            {
                collider.enabled = true;
            } 
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitCircle * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }

        Debug.DrawLine(this.transform.position, finalPosition, Color.red, 10f);
        return finalPosition;
    }
}
