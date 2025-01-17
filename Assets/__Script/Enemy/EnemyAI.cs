using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Patrol, Haunt }
    public State currentState;

    [Header("AI Settings")]
    public float patrolSpeed = 2f;
    public float hauntSpeed = 5f;
    public GameObject playerObj;
    public float patrolRadius = 10f;
    public float sightRange = 15f;

    private float idleTimer;
    public float idleDuration = 3f;

    public NavMeshAgent navMeshAgent;
    private Vector3 randomDestination;
    private Vector3 lastKnownPosition;

    [Header("Audio Detection")]
    public float detectionInterval = 0.1f;
    private float detectionTimer;
    public ScaleFromMicrophone scaleFromMicrophone;
    public float loudnessSensitivity = 100f;
    public float hauntThreshold = 0.2f;
    public float calmDownThreshold = 0.1f;
    public float calmDownDuration = 3f;
    public float maxDetectionRange = 20f;

    public float loudness;
    private float calmDownTimer;

    [Header("Scaling Effect")]
    public Vector3 minScale = new Vector3(1, 1, 1);
    public Vector3 maxScale = new Vector3(3, 3, 3);

    private Animator animator;
    private FPSController playerController;

    void Start()
    {
        currentState = State.Patrol;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerController = playerObj.GetComponent<FPSController>();

        navMeshAgent.speed = patrolSpeed;
        SetRandomDestination();
    }

    void Update()
    {
        // Update loudness check with interval
        detectionTimer += Time.deltaTime;
        if (detectionTimer >= detectionInterval)
        {
            UpdateLoudness();
            detectionTimer = 0f;
        }

        // State machine handling
        switch (currentState)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.Patrol:
                HandlePatrolState();
                break;
            case State.Haunt:
                HandleHauntState();
                break;
        }

        UpdateAnimator();
    }

    void UpdateLoudness()
    {
        if (scaleFromMicrophone == null || playerObj.transform == null) return;

        // Ignore loudness if the player is hiding
        if (playerController != null && playerController.isHiding)
        {
            loudness = 0f; // Treat as no sound
            return;
        }

        // Get base loudness
        float baseLoudness = scaleFromMicrophone.loudness * loudnessSensitivity;

        // Calculate distance factor
        float distanceToPlayer = Vector3.Distance(transform.position, playerObj.transform.position);
        float distanceFactor = 1f - Mathf.Clamp01(distanceToPlayer / maxDetectionRange);

        // Apply distance falloff
        loudness = baseLoudness * distanceFactor;

        if (loudness > hauntThreshold)
        {
            Debug.Log($"Sound detected! Loudness: {loudness:F2} at distance: {distanceToPlayer:F2}m");
        }
    }

    void HandleIdleState()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            idleTimer = 0f;
            currentState = State.Patrol;
            navMeshAgent.speed = patrolSpeed;
            SetRandomDestination();
        }

        CheckHauntCondition();
    }

    void HandlePatrolState()
    {
        if (navMeshAgent.remainingDistance < 0.5f && !navMeshAgent.pathPending)
        {
            currentState = State.Idle;
        }

        CheckHauntCondition();
    }

    void HandleHauntState()
    {
        if (IsPlayerVisible())
        {
            lastKnownPosition = playerObj.transform.position;
            navMeshAgent.destination = playerObj.transform.position;
        }
        else
        {
            navMeshAgent.destination = lastKnownPosition;

            if (navMeshAgent.remainingDistance < 0.5f && !navMeshAgent.pathPending)
            {
                StartCoroutine(TransitionState(State.Patrol, 2f)); // Smooth transition
            }
        }

        // Check calm down
        if (playerController.isHiding || loudness < calmDownThreshold)
        {
            calmDownTimer += Time.deltaTime;

            if (calmDownTimer >= calmDownDuration)
            {
                calmDownTimer = 0f;
                StartCoroutine(TransitionState(State.Patrol, 1f)); // Gradual transition
            }
        }
        else
        {
            calmDownTimer = 0f;
        }
    }

    void CheckHauntCondition()
    {
        bool playerInSight = IsPlayerVisible();
        bool soundTrigger = loudness >= hauntThreshold;

        // Ignore sound triggers if the player is hiding
        if (playerController != null && playerController.isHiding)
        {
            soundTrigger = false;
        }

        if (soundTrigger || playerInSight)
        {
            StopAllCoroutines(); // Stop any ongoing transitions
            StartCoroutine(TransitionState(State.Haunt, 0.5f)); // Smoothly enter haunt state

            if (soundTrigger && !playerInSight)
            {
                lastKnownPosition = playerObj.transform.position;
                Debug.Log("Following sound to player position!");
            }
            else if (playerInSight)
            {
                lastKnownPosition = playerObj.transform.position;
                Debug.Log("Player spotted visually!");
            }
        }
    }

    bool IsPlayerVisible()
    {
        if (playerObj.transform == null) return false;

        Vector3 directionToPlayer = playerObj.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > sightRange)
            return false;

        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, sightRange))
        {
            if (hit.transform == playerObj.transform && !playerController.isHiding)
            {
                Debug.DrawLine(transform.position, playerObj.transform.position, Color.red);
                return true;
            }
        }

        return false;
    }

    public void SetRandomDestination()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * patrolRadius;
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            randomDestination = hit.position;
            navMeshAgent.SetDestination(randomDestination);
        }
    }

    void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("IsIdle", currentState == State.Idle);
            animator.SetBool("IsPatrolling", currentState == State.Patrol);
            animator.SetBool("IsHaunting", currentState == State.Haunt);
        }
    }

    private System.Collections.IEnumerator TransitionState(State newState, float duration)
    {
        yield return new WaitForSeconds(duration);
        currentState = newState;
    }

    private void OnDrawGizmosSelected()
    {
        // Patrol radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        // Sight range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Sound detection range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxDetectionRange);

        // Line to player if visible
        if (playerObj.transform != null)
        {
            Gizmos.color = IsPlayerVisible() ? Color.red : Color.gray;
            Gizmos.DrawLine(transform.position, playerObj.transform.position);
        }

        // Last known position
        if (lastKnownPosition != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(lastKnownPosition, 0.3f);
        }
    }
}
