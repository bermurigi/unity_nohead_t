using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover))] //필요한 컴포넌트를 자동적으로 넣어준다.
public class Priest_movement : MonoBehaviourPun
{
    //[SerializeField] //인스펙터 창에 표시
    private Animator Animator;
    private AgentLinkMover LinkMover;
    private GameObject target;
    public float updateSpeed = 0.1f;
    private const string IsWalking = "IsWalking";
    private const string Jump = "Jump";
    private const string Landed = "Landed";
    private NavMeshAgent agent;
    int count;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        LinkMover = GetComponent<AgentLinkMover>();
        LinkMover.OnLinkStart += HandleLinkStart; //C#에서 +=는 특정한 이벤트의 발생 가능
        LinkMover.OnLinkEnd += HandleLinkEnd;


    }

    private void Start()
    {
        agent.updateRotation = false;
        StartCoroutine(FollowTarget());
        //  LookAt lookAt = GetComponent<LookAt> ();

    }
    private void HandleLinkStart()
    {
        Animator.SetTrigger(Jump);
        Debug.Log("점프 작동됨!");
    }
    private void HandleLinkEnd()
    {
        Animator.SetTrigger(Landed);
        Debug.Log("착륙 작동됨!");
    }
    private void Update()
    {
        Animator.SetBool("IsWalking", agent.velocity.magnitude > 0.01f);
        if (agent.desiredVelocity.sqrMagnitude >= 0.1f * 0.1f)
        {
            Vector3 direction = agent.desiredVelocity;
            Quaternion targetAngle = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetAngle, Time.deltaTime * 8.0f);
        }
    }

    private IEnumerator FollowTarget()//0.1f의 시간 차이를 두고 지속적으로 플레이어 태그를 가진 대상 추적
    {
        WaitForSeconds wait = new WaitForSeconds(updateSpeed);

        while (enabled)
        {
            FindPlayerTarget();
            if (target != null)
            {
                agent.SetDestination(target.transform.position);
            }
            yield return wait;
        }
    }

    private void FindPlayerTarget() //플레이어 태그가 여럿일 때, 가장 가까운 플레이어 태그를 가진 대상을 추적.
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //float closestDistance = Mathf.Infinity;

        if (players.Length == 1)
        {
            target = players[0];
        }
        else if (players.Length == 2)
        {

            float distance = Vector3.Distance(transform.position, players[0].transform.position);
            float distance2 = Vector3.Distance(transform.position, players[1].transform.position);
            if (players[0].GetComponent<GhostMotionController>().soundOnOff == true && players[1].GetComponent<GhostMotionController>().FollowingOnOff == true)
            {
                if (distance < distance2)
                {
                    target = players[0];
                    //closestDistance = distance;
                }
                else
                {
                    target = players[1];
                }
            }
            else if (players[0].GetComponent<GhostMotionController>().soundOnOff == true)
            {
                target = players[0];
            }
            else
            {
                if (players[1].GetComponent<GhostMotionController>().FollowingOnOff == false)
                {
                    if (distance < distance2)
                    {
                        target = players[0];
                        //closestDistance = distance;
                    }
                    else
                    {
                        target = players[1];
                    }
                }
                else
                {
                    target = players[1];
                }
            }
        }
    }
}

