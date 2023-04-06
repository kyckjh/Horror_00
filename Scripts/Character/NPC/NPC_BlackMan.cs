using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_BlackMan : MonoBehaviour
{
    Animator anim;

    [SerializeField]
    Transform[] destinations;

    public float walkSpeed = 3.0f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    IEnumerator FirstMove()
    {
        anim.SetTrigger("Walk");
        while (Vector3.Distance(transform.position, destinations[0].position) > 0.1f)
        {
            Vector3 direction = (destinations[0].position - transform.position).normalized;
            transform.position += direction * Time.deltaTime * walkSpeed;

            yield return null;
        }
        anim.SetTrigger("Sit"); // 앉는 애니메이션 돌입
        while (Vector3.Distance(transform.position, destinations[1].position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, destinations[1].position, Time.deltaTime * 10.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, destinations[1].rotation, Time.deltaTime * 10.0f);

            yield return null;
        }
        yield return new WaitForSeconds(12.0f);    // 12초간 재생
        anim.SetTrigger("Walk");
        yield return new WaitForSeconds(0.5f);
        while (Vector3.Distance(transform.position, destinations[0].position) > 0.1f)
        {
            Vector3 direction = (destinations[0].position - transform.position).normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation, destinations[2].rotation, Time.deltaTime * 5.0f);
            transform.position += direction * Time.deltaTime * walkSpeed;

            yield return null;
        }
        while (Vector3.Distance(transform.position, destinations[2].position) > 0.1f)
        {
            Vector3 direction = (destinations[2].position - transform.position).normalized;
            transform.position += direction * Time.deltaTime * walkSpeed;
            transform.rotation = Quaternion.Lerp(transform.rotation, destinations[2].rotation, Time.deltaTime * 5.0f);

            yield return null;
        }
        while (Vector3.Distance(transform.position, destinations[3].position) > 0.1f)
        {
            Vector3 direction = (destinations[3].position - transform.position).normalized;
            transform.position += direction * Time.deltaTime * walkSpeed;
            transform.rotation = Quaternion.Lerp(transform.rotation, destinations[3].rotation, Time.deltaTime * 10.0f);

            yield return null;
        }
        anim.SetTrigger("Idle");
        Destroy(this.gameObject);
        yield return null;
    }

    public void FirstMoveStart()
    {
        StartCoroutine(FirstMove());
    }
}
