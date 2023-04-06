using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy : MonoBehaviour
{
    enum SearchResult   // ���� �÷��̾ ������ ���
    {
        Fail = 0,       // ���� ����
        Suspect,        // ���� �ǽ�
        Succeed         // ���� ����
    }

    public Transform patrolRoute;

    EnemyState state = EnemyState.Idle;

    NavMeshAgent agent;
    Animator anim;
    
    // Idle �� -------------------------------------------------------------------

    float waitTime_Idle = 2.0f;
    float timeCount_Idle = 2.0f;

    // Patrol �� -----------------------------------------------------------------

    int index = 0, childCount = 0;

    // Suspect �� ----------------------------------------------------------------
        
    public static float sightRange_Suspect = 10.0f;   // ���� �÷��̾ �ִ��� �ǽ��ϴ� �Ÿ�(�ǽ� �Ÿ�) static���� �����ؼ� ��� Enemy�� ���� ���� �����ϵ��� ��
    public float waitTime_Suspect = 5.0f;
    float timeCount_Suspect = 5.0f;

    // Chase �� ------------------------------------------------------------------

    public static float sightRange_Chase = 5.0f;      // ���� �÷��̾ Ȯ���� ���� �Ѿƿ��� �Ÿ�(���� �Ÿ�)
    float sightAngle = 120.0f;
    Vector3 targetPosition = new();
    IEnumerator repeatChase;
    WaitForSeconds oneSecond = new WaitForSeconds(1.0f);

    // Attack �� -----------------------------------------------------------------

    // Enemy�� �����ϴ� �ӵ�
    float attackSpeed = 2.0f;
    // ���� �ӵ� ī��Ʈ��
    float attackCoolTime = 2.0f;
    // ���� �� Player�� �������� ������
    [SerializeField]
    float attackPower = 30.0f;

    Player attackTarget;

    // Die �� --------------------------------------------------------------------

    bool isDead = false;

    // Unity �̺�Ʈ �Լ� ------------------------------------------------------------------

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

    // ������Ʈ �Լ� ---------------------------------------------------------------------------------

    /// <summary>
    /// ��� ����
    /// </summary>
    void IdleUpdate()
    {
        if (SearchPlayer() == SearchResult.Succeed)  // ���� ��� ����
        {
            ChangeState(EnemyState.Chase);          // ���� ���·� ��ȯ
            return;
        }
        if (SearchPlayer() == SearchResult.Suspect) // ���� ��� �ǽ�
        {
            ChangeState(EnemyState.Suspect);        // �ǽ� ���·� ��ȯ
            return;
        }        
        timeCount_Idle -= Time.deltaTime;            
        if (timeCount_Idle < 0)                      // ��� ���� ������
        {
            ChangeState(EnemyState.Patrol);         // ���� ���·� ��ȯ
            return;
        }
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    void PatrolUpdate()
    {
        if (SearchPlayer() == SearchResult.Succeed)  // ���� ��� ����
        {
            ChangeState(EnemyState.Chase);          // ���� ���·� ��ȯ
            return;
        }
        if (SearchPlayer() == SearchResult.Suspect) // ���� ��� �ǽ�
        {
            ChangeState(EnemyState.Suspect);        // �ǽ� ���·� ��ȯ
            return;
        }        
        if (agent.remainingDistance <= agent.stoppingDistance)  // �������� ����
        {
            index = ++index % childCount;
            agent.SetDestination(patrolRoute.GetChild(index).position);
            ChangeState(EnemyState.Idle);                       // ��� ���·� ��ȯ
            return;
        }
    }

    /// <summary>
    /// �ǽ� ����
    /// </summary>
    void SuspectUpdate()    // �ǽ��ϴٰ� ���� �ð� ������ �������·� ��ȯ
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(targetPosition - transform.position), 0.01f);

        if (SearchPlayer() == SearchResult.Succeed)     // ���� ��� ����
        {
            ChangeState(EnemyState.Chase);              // ���� ���·� ��ȯ
            return;
        }      
        
        if (SearchPlayer() == SearchResult.Suspect)     // �ǽ� ���°� Ǯ���� �ʰ� ��ӵǸ� count ���
        {
            timeCount_Suspect += Time.deltaTime;
        }        
        else if(SearchPlayer() == SearchResult.Fail)    // �ǽ� ���°� Ǯ���� count ����
        {
            timeCount_Suspect -= Time.deltaTime;
        }

        if (timeCount_Suspect > waitTime_Suspect * 2.0f)    // �ǽ� ���°� ���� �ð� �̻� ���ӵǸ�
        {
            ChangeState(EnemyState.Chase);          // ���� ���·� ��ȯ
            return;
        }

        if (timeCount_Suspect < 0)                  // �ǽ� ���� ������
        {
            ChangeState(EnemyState.Patrol);         // ���� ���·� ��ȯ
            return;
        }
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    void ChaseUpdate()
    {
        if (SearchPlayer() == SearchResult.Fail)    // ���� ����
        {
            ChangeState(EnemyState.Suspect);
            return;
        }
        agent.SetDestination(targetPosition);
    }

    /// <summary>
    /// ���� ����
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

    // �̺�Ʈ �Լ� ---------------------------------------------------------------------------------

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

    // �Ϲ� �Լ� ---------------------------------------------------------------------------------


    /// <summary>
    /// �÷��̾ ã�� �Լ�
    /// </summary>
    /// <returns>�� ã���� SearchResult.Fail, �����̼� �� �ְų� ��ó���� �޸��� Suspect, ������ ã���� Succeed ����</returns>
    SearchResult SearchPlayer()
    {
        SearchResult result = SearchResult.Fail;
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange_Suspect, LayerMask.GetMask("Player"));

        if (colliders.Length > 0)    // Player�� �߰�
        {
            Vector3 pos = colliders[0].transform.position;
            Player player = colliders[0].gameObject.GetComponent<Player>(); // ù ��° �ݶ��̴����� Player �޾ƿ��� -> �� �Ǵ��� Ȯ�� �ʿ�
            if (player == null) // �� �� ���� ������ Player�� null�� �� ��츦 ���
            {
                return SearchResult.Fail;
            }
            //Debug.Log($"BlockByWall : {BlockByWall(pos)}");
            if (!BlockByWall(pos))  // ���� ������ �ʾ��� ��
            {
                //Debug.Log($"Player State : {state}");
                switch (player.State)   // Player�� ���� ���¿� ���� �ν� ������ �޶���
                {
                    case PlayerState.Sit:   // Player�� �ɾ� �ִ� ���
                        {
                            if (InSightAngle(pos))   // �þ߰� �ȿ� �ִ� ���
                            {
                                if ((pos - transform.position).sqrMagnitude < sightRange_Chase * sightRange_Chase)   // Player�� ���� ���� �ȿ� �ִ� ���
                                {
                                    result = SearchResult.Succeed;  // Player ���� ����
                                }
                                else                                // Player�� ���� ���� ��, �ǽ� ���� �ȿ� �ִ� ���
                                {
                                    result = SearchResult.Suspect;  // Player ���� �ǽ�
                                }
                            }
                        }
                        break;
                    case PlayerState.Stand: // Player�� �� �ְų� �ȴ� ���
                        {
                            if (InSightAngle(pos))   // �þ߰� �ȿ� �ִ� ���
                            {
                                result = SearchResult.Succeed;       // Player ���� ����                           
                            }
                            else    // �þ߰� �ȿ� ���� ���
                            {
                                if ((pos - transform.position).sqrMagnitude < sightRange_Chase * sightRange_Chase)   // Player�� ���� ���� �ȿ� �ִ� ���
                                {
                                    result = SearchResult.Suspect;  // Player ���� �ǽ�
                                }
                            }
                        }
                        break;
                    case PlayerState.Run:   // Player�� �޸��� �ִ� ���
                        {
                            result = SearchResult.Suspect;  // Player ���� �ǽ�
                            // Player�� �þ߹��� �ȿ� �ְų� ���� ���� �ȿ� �ִ� ���
                            if (InSightAngle(pos) || (pos - transform.position).sqrMagnitude < sightRange_Chase * sightRange_Chase)
                            {
                                result = SearchResult.Succeed;       // Player ���� ����
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //Debug.Log($"Result : {result}");
            if (result != SearchResult.Fail)    // ������ �������� �ʾ�����
            {
                targetPosition = pos;   // Ÿ�� ������ ����
            }
        }
        return result;
    }

    void ChangeState(EnemyState newState)
    {
        if (isDead) return;
        //Debug.Log(newState);
        // ���� ���¸� �����鼭 �ؾ� �� �ϵ�
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

        // �� ���·� ���鼭 �ؾ� �� �ϵ�
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
                attackCoolTime = 0.3f;          // ù ������ ������ ����(0.3��)
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
    /// �÷��̾ �þ߰���(sightAngle) �ȿ� ������ true�� ����
    /// </summary>
    /// <returns></returns>
    bool InSightAngle(Vector3 targetPosition)
    {
        // �� ������ ���̰�
        float angle = Vector3.Angle(transform.forward, targetPosition - transform.position);
        // ������ �þ߹��� �������̿� �ִ��� ������
        return (sightAngle * 0.5f) > angle;
    }

    /// <summary>
    /// ���� ����� ��� �Ⱥ��̴��� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="targetPosition">Ȯ���� ����� ��ġ</param>
    /// <returns>true�� ���� ������ �ִ� ��. false�� ������ �����ʴ�.</returns>
    bool BlockByWall(Vector3 targetPosition)
    {
        bool result = true;
        Ray ray = new(transform.position, targetPosition - transform.position); // ���� �����(������, ����)
        ray.origin += Vector3.up * 0.5f;    // ������ �����̷� ���� �������� ����
        if (Physics.Raycast(ray, out RaycastHit hit, sightRange_Suspect))
        {
            if (hit.collider.CompareTag("Player"))     // ���̿� ���𰡰� �ɷȴµ� "Player"�±׸� ������ ������
            {
                result = false; // �ٷ� ���� ���̴� ���� ������ ���� �ʴ�.
            }
        }
        return result;  // true�� ���� ���Ȱų� �ƹ��͵� �浹���� �ʾҰų�
    }

    void Attack(Player target)
    {
        if(target != null)
        {            
            // Player�� ������ ���� ������ �����·� ��ȯ
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
    /// Enemy�� Player�� �׿��� �� ����Ǵ� �ڷ�ƾ
    /// </summary>
    /// <param name="target">�÷��̾�</param>
    /// <returns></returns>
    IEnumerator PlayerKill(Player target)
    {
        float playTime = 0.0f;  // �����ð�
        agent.isStopped = true; // NavMesh ��Ȱ��ȭ
        while (playTime < 4.0f)
        {
            playTime += Time.deltaTime;
            // �÷��̾ Enemy ���� ���ƺ����� ��
            target.transform.rotation = Quaternion.Lerp(target.transform.rotation,  
                Quaternion.LookRotation(new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(target.transform.position.x, 0, 
                target.transform.position.z)), playTime);
            // Enemy�� �÷��̾� ���� ���ƺ����� ��
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(new Vector3(target.transform.position.x, 0, target.transform.position.z) - new Vector3(transform.position.x, 0,
                transform.position.z)), playTime);
            // Enemy�� �÷��̾� ���ʿ� ������ ��
            transform.position = Vector3.Lerp(transform.position, target.transform.position + target.transform.forward * 1.6f - Vector3.up * 0.8f // ���� ����
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

    // �ڷ�ƾ ---------------------------------------------------------------------------------

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

    // �׽�Ʈ�� �Լ�

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
            Handles.color = Color.red;  // �����̳� ���� ���� ���� ������
        }
        else if (state == EnemyState.Suspect)
        {
            Handles.color = new Color(1, 0.7f, 0); // �ǽ� ���� �� ��Ȳ��
        }
        //Handles.DrawWireDisc(transform.position, transform.up, closeSightRange); // ���� �þ� ����

        Vector3 forward = transform.forward * sightRange_Chase;
        Quaternion q1 = Quaternion.Euler(0.5f * sightAngle * transform.up);
        Quaternion q2 = Quaternion.Euler(-0.5f * sightAngle * transform.up);
        Handles.DrawLine(transform.position, transform.position + q1 * forward);    // �þ߰� ������ ��
        Handles.DrawLine(transform.position, transform.position + q2 * forward);    // �þ߰� ���� ��

        Handles.DrawWireArc(transform.position, transform.up, q2 * transform.forward, sightAngle, sightRange_Chase, 5.0f);// ��ü �þ߹���
    }
#endif


}

