using System;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : PlayableObject
{
    public EnemyType enemyType;

    protected Transform target;
    [SerializeField] protected float speed;

    public Rigidbody2D rb;

    public int ScoreValue;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    protected virtual void Start()
    {
        //target = GameObject.FindWithTag("Player").transform;
        try
        {
            target = FindFirstObjectByType<Player>().GetComponent<Transform>();
        }catch(NullReferenceException e)
        {
            Debug.Log("There is no Alive player in the game, stopping all future spawning." + e);
            GameManager.GetInstance().DisableSpawning();
            Destroy(gameObject);
        }

        rb= GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {

    }

    private void FixedUpdate()
    {
        if (target == null) return;

        Move(target.position);
    }


    public override void Move(Vector2 direction, Vector2 target){ }

    public override void Move(float spd)
    {
        transform.Translate(Vector2.right * spd * Time.deltaTime);
    }

    public override void Move(Vector2 direction)
    {
        // Difference between our current position and target position
        direction.x -= transform.position.x;
        direction.y -= transform.position.y;


        // Rotate towards target by getting the angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,angle);

        // Move towards target direction, based on our speed
        rb.linearVelocity = direction.normalized * speed * Time.deltaTime;
    }

    public override void Shoot()
    {
        Debug.Log("Shooting");
    }

    public override void Attack(float interval)
    {
        Debug.Log("Attacking");
    }

    public override void Die()
    {
        GameManager.GetInstance().scoreManager.IncrementScore(ScoreValue);
        GameManager.GetInstance().NotifyDeath(this);
        audioManager.PlaySFX(audioManager.enemyKill);

       // FindFirstObjectByType<AudioManager>().Play("EnemyKill");

        Destroy(gameObject);
    }

    public void SetEnemyType(EnemyType enemyType)
    {
        this.enemyType = enemyType;
    }
}
