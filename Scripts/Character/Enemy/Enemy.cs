using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy : MonoBehaviour
{
    enum SearchResult   // 적이 플레이어를 추적한 결과
    {
        Fail = 0,       // 추적 실패
        Suspect,        // 추적 의심
        Succeed         // 추적 성공
    }

    public Transform patrolRoute;

    EnemyState state = EnemyState.Idle;

    NavMeshAgent agent;
    Animator anim;
    
    // Idle 용 -------------------------------------------------------------------

    float waitTime_Idle = 2.0f;
    float timeCount_Idle = 2.0f;

    // Patrol 용 -----------------------------------------------------------------

    int index = 0, childCount = 0;

    // Suspect 용 ----------------------------------------------------------------
        
    public static float sightRange_Suspect = 10.0f;   // 적이 플레이어가 있는지 의심하는 거리(의심 거리) static으로 선언해서 모든 Enemy가 같은 값을 공유하도록 함
    public float waitTime_Suspect = 5.0f;
    float timeCount_Suspect = 5.0f;

    // Chase 용 ------------------------------------------------------------------

    public static float sightRange_Chase = 5.0f;      // 적이 플레이어를 확실히 보고 쫓아오는 거리(추적 거리)
    float sightAngle = 120.0f;
    Vector3 targetPosition = new();
    IEnumerator repeatChase;
    WaitForSeconds oneSecond = new WaitForSeconds(1.0f);

    // Attack 용 -----------------------------------------------------------------

    // Enemy가 공격하는 속도
    float attackSpeed = 2.0f;
    // 공격 속도 카운트용
    float attackCoolTime = 2.0f;
    // 공격 시 Player에 가해지는 데미지
    [SerializeField]
    float attackPower = 30.0f;

    Player attackTarget;

    // Die 용 --------------------------------------------------------------------

    bool isDead = false;

    // Unity 이벤트 함수 ------------------------------------------------------------------

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        childCount = patrolRoute.childCount;
        if (patrolRoute)
        {
            //agent.SetDestination(patrolRoute.GetChild(index).position);
        }
    }

    private void Update()
    {
        switch (state)
        {
            case EnemyState.Idle:
                IdleUpdate();
                break;
            case EnemyState.Patrol:
                PatrolUpdate();
                break;
            case EnemyState.Suspect:
                SuspectUpdate();
                break;
            case EnemyState.Chase:
                ChaseUpdate();
                break;
            case EnemyState.Attack:
                AttackUpdate();
                break;
            case EnemyState.Dead:
            default:
                break;
        }
    }

    // 업데이트 함수 ---------------------------------------------------------------------------------

    /// <summary>
    /// 대기 상태
    /// </summary>
    void IdleUpdate()
    {
        if (SearchPlayer() == SearchResult.Succeed)  // 추적 결과 성공
        {
            ChangeState(EnemyState.Chase);          // 추적 상태로 전환
            return;
        }
        if (SearchPlayer() == SearchResult.Suspect) // 추적 결과 의심
        {
            ChangeState(EnemyState.Suspect);        // 의심 상태로 전환
            return;
        }        
        timeCount_Idle -= Time.deltaTime;            
        if (timeCount_Idle < 0)                      // 대기 상태 끝나면
        {
            ChangeState(EnemyState.Patrol);         // 순찰 상태로 전환
            return;
        }
    }

    /// <summary>
    /// 순찰 상태
    /// </summary>
    void PatrolUpdate()
    {
        if (SearchPlayer() == SearchResult.Succeed)  // 추적 결과 성공
        {
            ChangeState(EnemyState.Chase);          // 추적 상태로 전환
            return;
        }
        if (SearchPlayer() == SearchResult.Suspect) // 추적 결과 의심
        {
            ChangeState(EnemyState.Suspect);        // 의심 상태로 전환
            return;
        }        
        if (agent.remainingDistance <= agent.stoppingDistance)  // 순찰지점 도착
        {
            index = ++index % childCount;
            agent.SetDestination(patrolRoute.GetChild(index).position);
            ChangeState(EnemyState.Idle);                       // 대기 상태로 전환
            return;
        }
    }

    /// <summary>
    /// 의심 상태
    /// </summary>
    void SuspectUpdate()    // 의심하다가 일정 시간 지나면 순찰상태로 전환
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(targetPosition - transform.position), 0.01f);

        if (SearchPlayer() == SearchResult.Succeed)     // 추적 결과 성공
        {
            ChangeState(EnemyState.Chase);              // 추적 상태로 전환
            return;
        }      
        
        if (SearchPlayer() == SearchResult.Suspect)     // 의심 상태가 풀리지 않고 계속되면 count 상승
        {
            timeCount_Suspect += Time.deltaTime;
        }        
        else if(SearchPlayer() == SearchResult.Fail)    // 의심 상태가 풀리면 count 감소
        {
            timeCount_Suspect -= Time.deltaTime;
        }

        if (timeCount_Suspect > waitTime_Suspect * 2.0f)    // 의심 상태가 일정 시간 이상 지속되면
        {
            ChangeState(EnemyState.Chase);          // 순찰 상태로 전환
            return;
        }

        if (timeCount_Suspect < 0)                  // 의심 상태 끝나면
        {
            ChangeState(EnemyState.Patrol);         // 순찰 상태로 전환
            return;
        }
    }

    /// <summary>
    /// 추적 상태
    /// </summary>
    void ChaseUpdate()
    {
        if (SearchPlayer() == SearchResult.Fail)    // 추적 실패
        {
            ChangeState(EnemyState.Suspect);
            return;
        }
        agent.SetDestination(targetPosition);
    }

    /// <summary>
    /// 공격 상태
    /// </summary>
    void AttackUpdate()
    {        
        attackCoolTime -= Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(attackTarget.transform.position - transform.position), 0.1f);
        if (attackCoolTime < 0.0f)
        {
            //Debug.Log("Enemy Attack");
            AudioManager.Inst.PlayClipAtPoint("Zombie_08_Attack", transform.position);
            anim.SetTrigger("Attack");
            Attack(attackTarget);
            attackCoolTime = attackSpeed;
        }       
    }

    // 이벤트 함수 ---------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.Inst.MainPlayer.gameObject)
        {
            attackTarget = other.GetComponent<Player>();
            ChangeState(EnemyState.Attack);
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameManager.Inst.MainPlayer.gameObject)
        {
            ChangeState(EnemyState.Chase);
            return;
        }
    }

    // 일반 함수 ---------------------------------------------------------------------------------


    /// <summary>
    /// 플레이어를 찾는 함수
    /// </summary>
    /// <returns>못 찾으면 SearchResult.Fail, 가까이서 서 있거나 근처에서 달리면 Suspect, 완전히 찾으면 Succeed 리턴</returns>
    SearchResult SearchPlayer()
    {
        SearchResult result = SearchResult.Fail;
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange_Suspect, LayerMask.GetMask("Player"));

        if (colliders.Length > 0)    // Player를 발견
        {
            Vector3 pos = colliders[0].transform.position;
            Player player = colliders[0].gameObject.GetComponent<Player>(); // 첫 번째 콜라이더에서 Player 받아오기 -> 잘 되는지 확인 필요
            if (player == null) // 알 수 없는 이유로 Player가 null이 될 경우를 대비
            {
                return SearchResult.Fail;
            }
            //Debug.Log($"BlockByWall : {BlockByWall(pos)}");
            if (!BlockByWall(pos))  // 벽에 가리지 않았을 때
            {
                //Debug.Log($"Player State : {state}");
                switch (player.State)   // Player의 현재 상태에 따라 인식 범위가 달라짐
                {
                    case PlayerState.Sit:   // Player가 앉아 있는 경우
                        {
                            if (InSightAngle(pos))   // 시야각 안에 있는 경우
                            {
                                if ((pos - transform.position).sqrMagnitude < sightRange_Chase * sightRange_Chase)   // Player가 추적 범위 안에 있는 경우
                                {
                                    result = SearchResult.Succeed;  // Player 추적 성공
                                }
                                else                                // Player가 추적 범위 밖, 의심 범위 안에 있는 경우
                                {
                                    result = SearchResult.Suspect;  // Player 추적 의심
                                }
                            }
                        }
                        break;
                    case PlayerState.Stand: // Player가 서 있거나 걷는 경우
                        {
                            if (InSightAngle(pos))   // 시야각 안에 있는 경우
                            {
                                result = SearchResult.Succeed;       // Player 추적 성공                           
                            }
                            else    // 시야각 안에 없는 경우
                            {
                                if ((pos - transform.position).sqrMagnitude < sightRange_Chase * sightRange_Chase)   // Player가 추적 범위 안에 있는 경우
                                {
                                    result = SearchResult.Suspect;  // Player 추적 의심
                                }
                            }
                        }
                        break;
                    case PlayerState.Run:   // Player가 달리고 있는 경우
                        {
                            result = SearchResult.Suspect;  // Player 추적 의심
                            // Player가 시야범위 안에 있거나 추적 범위 안에 있는 경우
                            if (InSightAngle(pos) || (pos - transform.position).sqrMagnitude < sightRange_Chase * sightRange_Chase)
                            {
                                result = SearchResult.Succeed;       // Player 추적 성공
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //Debug.Log($"Result : {result}");
            if (result != SearchResult.Fail)    // 추적에 실패하지 않았으면
            {
                targetPosition = pos;   // 타겟 포지션 설정
            }
        }
        return result;
    }

    void ChangeState(EnemyState newState)
    {
        if (isDead) return;
        //Debug.Log(newState);
        // 이전 상태를 나가면서 해야 할 일들
        switch (state)
        {
            case EnemyState.Idle:
                agent.isStopped = true;
                break;
            case EnemyState.Patrol:
                agent.isStopped = true;
                break;
            case EnemyState.Chase:
                agent.isStopped = true;
                StopCoroutine(repeatChase);
                anim.SetBool("isRun", false);
                break;
            case EnemyState.Suspect:
                agent.isStopped = true;
                break;
            case EnemyState.Attack:
                agent.isStopped = true;
                attackTarget = null;
                break;
            case EnemyState.Dead:
                isDead = false;
                break;
            default:
                break;
        }

        // 새 상태로 들어가면서 해야 할 일들
        switch (newState)
        {
            case EnemyState.Idle:
                agent.isStopped = true;
                timeCount_Idle = waitTime_Idle;
                anim.SetBool("isMove", false);
                break;
            case EnemyState.Patrol:
                agent.isStopped = false;
                agent.SetDestination(patrolRoute.GetChild(index).position);
                anim.SetBool("isMove", true);
                int random_sfx = Random.Range(1, 6);
                AudioManager.Inst.PlayClipAtPoint($"Zombie_0{random_sfx}", transform.position);
                break;
            case EnemyState.Chase:
                agent.isStopped = false;
                agent.SetDestination(targetPosition);
                repeatChase = RepeatChase();
                StartCoroutine(repeatChase);
                anim.SetBool("isMove", true);
                anim.SetBool("isRun", true);
                break;
            case EnemyState.Suspect:
                agent.isStopped = true;
                timeCount_Suspect = waitTime_Suspect;
                anim.SetBool("isMove", false);
                break;
            case EnemyState.Attack:
                agent.isStopped = true;
                attackCoolTime = 0.3f;          // 첫 공격은 빠르게 맞음(0.3초)
                anim.SetBool("isMove", false);
                break;
            case EnemyState.Dead:
                DiePresent();
                break;
            default:
                break;
        }

        state = newState;
        //anim.SetInteger("EnemyState", (int)state);
    }

    /// <summary>
    /// 플레이어가 시야각도(sightAngle) 안에 있으면 true를 리턴
    /// </summary>
    /// <returns></returns>
    bool InSightAngle(Vector3 targetPosition)
    {
        // 두 백터의 사이각
        float angle = Vector3.Angle(transform.forward, targetPosition - transform.position);
        // 몬스터의 시야범위 각도사이에 있는지 없는지
        return (sightAngle * 0.5f) > angle;
    }

    /// <summary>
    /// 벽에 대상이 숨어서 안보이는지 확인하는 함수
    /// </summary>
    /// <param name="targetPosition">확인할 대상의 위치</param>
    /// <returns>true면 벽에 가려져 있는 것. false면 가려져 있지않다.</returns>
    bool BlockByWall(Vector3 targetPosition)
    {
        bool result = true;
        Ray ray = new(transform.position, targetPosition - transform.position); // 레이 만들기(시작점, 방향)
        ray.origin += Vector3.up * 0.5f;    // 몬스터의 눈높이로 레이 시작점을 높임
        if (Physics.Raycast(ray, out RaycastHit hit, sightRange_Suspect))
        {
            if (hit.collider.CompareTag("Player"))     // 레이에 무언가가 걸렸는데 "Player"태그를 가지고 있으면
            {
                result = false; // 바로 보인 것이니 벽이 가리고 있지 않다.
            }
        }
        return result;  // true면 벽이 가렸거나 아무것도 충돌하지 않았거나
    }

    void Attack(Player target)
    {
        if(target != null)
        {            
            // Player가 죽으면 공격 끝내고 대기상태로 전환
            if(target.TakeDamage(attackPower))
            {
                Debug.Log("Kill");
                //target = null;
                //ChangeState(EnemyState.Idle);
                StartCoroutine(PlayerKill(target));
                attackSpeed = 1.0f;
            }
        }
    }

    /// <summary>
    /// Enemy가 Player를 죽였을 때 실행되는 코루틴
    /// </summary>
    /// <param name="target">플레이어</param>
    /// <returns></returns>
    IEnumerator PlayerKill(Player target)
    {
        float playTime = 0.0f;  // 누적시간
        agent.isStopped = true; // NavMesh 비활성화
        while (playTime < 4.0f)
        {
            playTime += Time.deltaTime;
            // 플레이어가 Enemy 쪽을 돌아보도록 함
            target.transform.rotation = Quaternion.Lerp(target.transform.rotation,  
                Quaternion.LookRotation(new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(target.transform.position.x, 0, 
                target.transform.position.z)), playTime);
            // Enemy가 플레이어 쪽을 돌아보도록 함
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(new Vector3(target.transform.position.x, 0, target.transform.position.z) - new Vector3(transform.position.x, 0,
                transform.position.z)), playTime);
            // Enemy가 플레이어 앞쪽에 서도록 함
            transform.position = Vector3.Lerp(transform.position, target.transform.position + target.transform.forward * 1.6f - Vector3.up * 0.8f // 높이 조정
                , playTime);
            yield return null;
        }
        yield return null;
    }

    void DiePresent()
    {
        anim.SetTrigger("Die");
        /*
        OnDead?.Invoke();
        gameObject.layer = LayerMask.NameToLayer("Default");
        anim.SetBool("Dead", true);
        anim.SetTrigger("Die");
        isDead = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        HP = 0;
        ItemDrop();
        StartCoroutine(DeadEffect());
        */
    }

    // 코루틴 ---------------------------------------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator RepeatChase()
    {
        while (true)
        {
            yield return oneSecond;
            agent.SetDestination(targetPosition);
        }
    }

    // 테스트용 함수

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, transform.up, sightRange_Chase);

        Handles.color = Color.black;
        Handles.DrawWireDisc(transform.position, transform.up, sightRange_Suspect);

        Handles.color = Color.green;
        if (state == EnemyState.Chase || state == EnemyState.Attack)
        {
            Handles.color = Color.red;  // 추적이나 공격 중일 때만 빨간색
        }
        else if (state == EnemyState.Suspect)
        {
            Handles.color = new Color(1, 0.7f, 0); // 의심 중일 땐 주황색
        }
        //Handles.DrawWireDisc(transform.position, transform.up, closeSightRange); // 근접 시야 범위

        Vector3 forward = transform.forward * sightRange_Chase;
        Quaternion q1 = Quaternion.Euler(0.5f * sightAngle * transform.up);
        Quaternion q2 = Quaternion.Euler(-0.5f * sightAngle * transform.up);
        Handles.DrawLine(transform.position, transform.position + q1 * forward);    // 시야각 오른쪽 끝
        Handles.DrawLine(transform.position, transform.position + q2 * forward);    // 시야각 왼쪽 끝

        Handles.DrawWireArc(transform.position, transform.up, q2 * transform.forward, sightAngle, sightRange_Chase, 5.0f);// 전체 시야범위
    }
#endif


}

