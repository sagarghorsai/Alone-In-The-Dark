using UnityEngine;

public class HauntingBehavior : StateMachineBehaviour
{
    private EnemyAI enemyAI;
    private float calmDownTimer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get the reference to EnemyAI and initialize calm down timer
        enemyAI = animator.GetComponent<EnemyAI>();
        calmDownTimer = 0f;

        if (enemyAI != null)
        {
            enemyAI.navMeshAgent.speed = enemyAI.hauntSpeed;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyAI == null) return;

        // Follow the player
        enemyAI.navMeshAgent.destination = enemyAI.playerObj.transform.position;

        // Check if loudness drops below the calm-down threshold
        if (enemyAI.loudness < enemyAI.calmDownThreshold)
        {
            calmDownTimer += Time.deltaTime;
            if (calmDownTimer >= enemyAI.calmDownDuration)
            {
                animator.SetBool("IsHaunting", false);
                animator.SetBool("IsPatrolling", true);
            }
        }
        else
        {
            calmDownTimer = 0f; // Reset the calm down timer if loudness is above threshold
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset calm down timer
        calmDownTimer = 0f;
    } 
}
