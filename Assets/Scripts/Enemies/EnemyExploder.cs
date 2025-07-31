using UnityEngine;

public class EnemyExploder : EnemyMelee
{
    public float blastRadious;
    private const string PLAYER_TAG = "Player";

   

    protected override void Start()
    {
        base.Start();
        health = new Health(20, 0);

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
        if (target == null) { return; }

        if (Vector2.Distance(transform.position, target.position) < attackrange)
        {
            Attack(attackTime);
            Destroy(gameObject);
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);


        }

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
        if (Vector2.Distance(transform.position, target.position) < attackrange)
        {
            target.GetComponent<IDamageable>().GetDamage(meleeDamage);
        }

    }    

}