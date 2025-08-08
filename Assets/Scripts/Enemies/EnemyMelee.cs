using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class EnemyMelee : Enemy
{
    public Transform player;
    private const string PLAYER_TAG = "Player";

    public float attackrange;
    public float attackTime;
    public float meleeDamage;
    public float rotationSpeed = 5f;
    private float nextAttackTime = 0f;
    public float attackRate;
    private float timer = 0;

    Transform playerTransform;

    private bool isTracking = true;
    public float moveSpeed = 5f;
    public GameObject playerPrefab;

    

    protected override void Start()
    {
        base.Start();
        health = new Health(20, 0);

        // find player
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag(PLAYER_TAG);
            if (playerGO != null)
            {
                player = playerGO.transform;
            }
        }
    }





    protected override void Update()
    {
        if (target == null) return;


        if (Vector2.Distance(transform.position, target.position) < attackrange)
        {
            Attack(attackTime);
        }


        if (player != null)
        {
            if (player != null)
            {
                Vector3 direction = player.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            }
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackrange)
        {
            // Move towards the player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
        else
        {
            // Stop moving
            rb.linearVelocity = Vector2.zero;
        }
    }
    public override void Attack(float interval)
    {
        // attack rate
        if (timer <= interval)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            target.GetComponent<IDamageable>().GetDamage(meleeDamage);
        }

    }
    
}
