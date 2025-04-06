using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 2f;        // �̵� �ӵ�

    private bool isStopped;             // ���� ��������

    [Header("Jump")]
    public float jumpHeight = 2f;       // ���� ����
    public float jumpSpeed = 5f;        // ���� �ӵ�

    private LayerMask zombieLayer;      // ���� ������ ���̾�
    private bool isJumping = false;     // ���� ���� ������
    private Vector3 jumpTargetPosition; // ���� ��ǥ ��ġ

    [Header("Attack")]
    public float attackRange = 0.5f;    // ���� ����
    public LayerMask boxLayer;          // �ڽ��� ������ ���̾�
    public float attackCooldown = 1f;   // ���� ��Ÿ��

    private bool isAttacking = false;   // ���� ���� ������
    private float attackTimer = 0f;     // ���� Ÿ�̸�
        
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
        // �տ� �ִ� ���� ����
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0.7f, 0), Vector2.up, attackRange, zombieLayer);

        if (hit.collider != null)
        {            
            bool jumpChance = UnityEngine.Random.Range(0f, 1f) < 0.85f; // Ȯ���� ����

            if (jumpChance)
            {
                // �ش� ���� ���� �̹� �ٸ� ���� �ִ��� Ȯ��
                Collider2D aboveZombie = Physics2D.OverlapCircle(hit.collider.transform.position + Vector3.up * 1.2f, 0.2f, zombieLayer);

                if (aboveZombie == null) // ���� �ٸ� ���� ������ ����
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

        // ��ǥ ��ġ�� ������ ������ �������� ����
        jumpTargetPosition = new Vector3(targetPosition.x, targetPosition.y + jumpHeight, targetPosition.z);
    }

    private void Jump()
    {
        // ���� ��ġ�� ��ǥ ��ġ�� �̵�
        transform.position = Vector3.MoveTowards(transform.position, jumpTargetPosition, jumpSpeed * Time.deltaTime);

        // ��ǥ ��ġ�� �����ߴ��� Ȯ��
        if (Vector3.Distance(transform.position, jumpTargetPosition) < 0.01f)
        {
            isJumping = false; // ���� ����
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
