using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCManager : CharacterManager
{
    public NavMeshAgent agent;
    public NPCState currentState;
    public float idleWanderTimer;
    public bool isTalking;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        idleWanderTimer = 0f;
        isTalking = false;
    }

    private void HandleStateMachine()
    {
        if (currentState != null)
        {
            NPCState nextState = currentState.Tick(this, this.animationManager);

            if (nextState != null)
            {
                SwitchToNextState(nextState);
            }
        }
    }

    private void SwitchToNextState(NPCState newState)
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
