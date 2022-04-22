using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    private Animator animator;

    public State state = State.PATROL;

    private Transform playerTr;
    private Transform enemyTr;

    public float attackDist = 5.0f;
    public float traceDist = 10.0f;
    public bool isDie;
    private WaitForSeconds ws;

    private MoveAgent moveAgent;
    private EnemyFire enemyFire;

    private EnemyFOV enemyFOV;

    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");



    private void Awake()
    {
        animator = GetComponent<Animator>();
        var player = GameObject.FindGameObjectWithTag("PLAYER");
        moveAgent = GetComponent<MoveAgent>();
        enemyFire = GetComponent<EnemyFire>();
        enemyFOV = GetComponent<EnemyFOV>();
        if (player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }

        enemyTr = GetComponent<Transform>();

        ws = new WaitForSeconds(0.3f);
        animator.SetFloat(hashOffset, UnityEngine.Random.Range(0.0f, 1.0f));
        animator.SetFloat(hashWalkSpeed, UnityEngine.Random.Range(1.0f, 1.2f));
        

    }
    private void OnDisable()
    {
        Damage.OnPlayerDie -= this.OnPlayerDie;
    }

    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());

        Damage.OnPlayerDie += this.OnPlayerDie;
    }



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    IEnumerator CheckState()
    {
        yield return new WaitForSeconds(1.0f);
        while(isDie==false)
        {
            if(state ==State.DIE)
            {
                yield break;
            }
            float dist = Vector3.Distance(playerTr.position, enemyTr.position);

            if(dist<=attackDist)
            {
                if (enemyFOV.isViewPlayer())
                { 
                    state = State.ATTACK; 
                }
                else
                {
                    state = State.TRACE;
                }
                
            }
            //else if(dist <=traceDist)
            else if (enemyFOV.isTracePlayer())
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }



            yield return ws;

        }
    }

    IEnumerator Action()
    {
        while(isDie==false)
        {
            yield return ws;

            switch(state)
            {
                case State.PATROL:
                    enemyFire.isFire = false;
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    enemyFire.isFire = false;
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);

                    if(enemyFire.isFire==false)
                        enemyFire.isFire = true;
                    break;
                case State.DIE:
                    isDie = true;
                    enemyFire.isFire = false;
                    moveAgent.Stop();
                    this.gameObject.tag = "Untagged";

                    animator.SetInteger(hashDieIdx, UnityEngine.Random.Range(0, 3));
                    animator.SetTrigger(hashDie);
                    break;
            }
            
        }
    }

    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        StopAllCoroutines();

        animator.SetTrigger(hashPlayerDie);
    }
}
