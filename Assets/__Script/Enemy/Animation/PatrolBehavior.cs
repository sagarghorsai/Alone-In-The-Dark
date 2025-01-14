using UnityEngine;

public class PatrolBehavior : StateMachineBehaviour
{
    private EnemyAI enemyAI;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get the reference to EnemyAI
        enemyAI = animator.GetComponent<EnemyAI>();

        if (enemyAI != null)
        {
            enemyAI.navMeshAgent.speed = enemyAI.patrolSpeed;
            enemyAI.SetRandomDestination();
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyAI == null) return;

        // Check if destination has been reached
        if (enemyAI.navMeshAgent.remainingDistance < 0.5f && !enemyAI.navMeshAgent.pathPending)
        {
            animator.SetBool("IsPatrolling", false);
            animator.SetBool("IsIdle", true);
        }

        // Check for haunt condition
        if (enemyAI.loudness >= enemyAI.hauntThreshold)
        {
            animator.SetBool("IsPatrolling", false);
            animator.SetBool("IsHaunting", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Nothing specific to reset, but you could add logic here if needed
    }
}
