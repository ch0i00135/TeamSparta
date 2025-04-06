using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 2f;        // 이동 속도

    private bool isStopped;             // 멈춘 상태인지

    [Header("Jump")]
    public float jumpHeight = 2f;       // 점프 높이
    public float jumpSpeed = 5f;        // 점프 속도

    private LayerMask zombieLayer;      // 좀비를 감지할 레이어
    private bool isJumping = false;     // 현재 점프 중인지
    private Vector3 jumpTargetPosition; // 점프 목표 위치

    [Header("Attack")]
    public float attackRange = 0.5f;    // 공격 범위
    public LayerMask boxLayer;          // 박스를 감지할 레이어
    public float attackCooldown = 1f;   // 공격 쿨타임

    private bool isAttacking = false;   // 현재 공격 중인지
    private float attackTimer = 0f;     // 공격 타이머
        
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        zombieLayer = 1 << gameObject.layer;
    }

    private void Update()
    {
        if (isAttacking)
        {
            HandleAttackCooldown();
        }
        else if (isJumping)
        {
            Jump();
        }
        else if (!isStopped)
        {
            MoveLeft();      
            CheckAndAttack();
            CheckAndJump();
        }
    }

    private void MoveLeft()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }

    private void CheckAndAttack()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0.7f, 0), Vector2.up, attackRange, boxLayer);

        if (hit.collider != null)
        {
            StartAttack();
        }
    }

    private void HandleAttackCooldown()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            isAttacking = false;
            attackTimer = 0f;
        }
    }

    private void CheckAndJump()
    {
        // 앞에 있는 좀비 감지
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0.7f, 0), Vector2.up, attackRange, zombieLayer);

        if (hit.collider != null)
        {            
            bool jumpChance = UnityEngine.Random.Range(0f, 1f) < 0.85f; // 확률로 점프

            if (jumpChance)
            {
                // 해당 좀비 위에 이미 다른 좀비가 있는지 확인
                Collider2D aboveZombie = Physics2D.OverlapCircle(hit.collider.transform.position + Vector3.up * 1.2f, 0.2f, zombieLayer);

                if (aboveZombie == null) // 위에 다른 좀비가 없으면 점프
                {
                    StartJump(hit.collider.transform.position);
                }
            }            
            else
            {
                isStopped = true;
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsDead", true);
            }
        }
    }

    private void StartJump(Vector3 targetPosition)
    {
        isJumping = true;

        // 목표 위치는 감지된 좀비의 위쪽으로 설정
        jumpTargetPosition = new Vector3(targetPosition.x, targetPosition.y + jumpHeight, targetPosition.z);
    }

    private void Jump()
    {
        // 현재 위치를 목표 위치로 이동
        transform.position = Vector3.MoveTowards(transform.position, jumpTargetPosition, jumpSpeed * Time.deltaTime);

        // 목표 위치에 도달했는지 확인
        if (Vector3.Distance(transform.position, jumpTargetPosition) < 0.01f)
        {
            isJumping = false; // 점프 종료
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        isStopped = true;
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsAttacking", true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(-0.5f, 0.7f, 0), transform.position + new Vector3(-0.5f, (0.7f + attackRange), 0));

        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.2f, 0.2f);
    }
}
