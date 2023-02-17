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
    public AIState currentState;
    public LayerMask detectionLayerMask;
    public float idleWanderTimer;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        idleWanderTimer = 0f;
    }

    private void HandleStateMachine()
    {
        if (currentState != null)
        {
            AIState nextState = currentState.Tick(this, this.animationManager);

            if (nextState != null)
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
        if (isDead) { agent.enabled = false; return; }
        if (isInteracting) { agent.enabled = false; }
        float delta = Time.fixedDeltaTime;
        
        // update the animator
        animationManager.UpdateAnimator(delta, agent.velocity.magnitude, agent.velocity);
    }

    public void LookForEnemy()
    {
        Debug.DrawRay(sight.transform.position, sightVector, Color.red, 1f);

        // see if any characters are within sight range
        Collider2D[] targets = Physics2D.OverlapCircleAll(sight.transform.position, sightRange);
        foreach (Collider2D target in targets)
        {

            // first check to see if the target is an enemy
            if(target.gameObject.GetComponent<CharacterManager>() != null && target.gameObject.GetComponent<CharacterManager>().teamTag != teamTag && target.gameObject.GetComponent<CharacterManager>().teamTag != "NPC")
            {                
                // then see if we are looking in that direction
                Vector2 vectorToTarget = target.transform.position - sight.transform.position;
                if (Mathf.Abs(Vector2.Angle(sightVector, vectorToTarget)) <= sightAngle)
                {
                    // finally see if we hit them with a raycast
                    Debug.DrawRay(sight.transform.position, vectorToTarget, Color.red, 1f);
                    RaycastHit2D hit = Physics2D.Raycast(sight.transform.position, (target.transform.position - sight.transform.position), sightRange, detectionLayerMask);
                    if (hit) 
                    { 
                        canSee = true;
                        // set the new target
                        this.target = hit.collider.gameObject.transform.GetChild(1);
                    }
                    else { canSee = false; }
                }
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
