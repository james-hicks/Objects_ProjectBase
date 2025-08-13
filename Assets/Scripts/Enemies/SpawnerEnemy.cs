using UnityEngine;
using UnityEngine.Rendering;
public class SpawnerEnemy : Enemy
{

    public float EnemyRange;

    public Animator anim;

    protected override void Start()
    {
        base.Start();
        health = new Health(1, 0);
        anim = GetComponent<Animator>();

    }

    private void FixedUpdate()
    {

        Move(target.position);
    }


    public override void Move(Vector2 direction)
    {
        direction.x -= transform.position.x;
        direction.y -= transform.position.y;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        if ((Vector2.Distance(transform.position, target.position) <= EnemyRange))
        {
            rb.linearVelocity = direction * speed * 3 * Time.deltaTime;
            anim.SetTrigger("IsDashing");
        }
        else
        {
            rb.linearVelocity = direction * speed * 1 * Time.deltaTime;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        GameManager.GetInstance().CreateEnemy();
        GameManager.GetInstance().CreateEnemy();
        Die();
    }

    public override void Die()
    {
        Destroy(gameObject);
    }





}

