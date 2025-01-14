using UnityEngine;

public class IdleBehavior : StateMachineBehaviour
{
    private float idleTimer;
    private EnemyAI enemyAI;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Initialize the idle timer and get reference to EnemyAI
        idleTimer = 0f;
        enemyAI = animator.GetComponent<EnemyAI>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyAI == null) return;

        // Increment the idle timer
        idleTimer += Time.deltaTime;

        // Check if the idle duration has been met
        if (idleTimer >= enemyAI.idleDuration)
        {
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsPatrolling", true);
        }

        // Check for loudness and haunt condition
        if (enemyAI.loudness >= enemyAI.hauntThreshold)
        {
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsHaunting", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset the idle timer
        idleTimer = 0f;
    }
}
