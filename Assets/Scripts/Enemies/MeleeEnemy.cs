using UnityEngine;
using UnityEngine.Rendering;

public class MeleeEnemy : Enemy
{
    [Space]
    public float attackRange;
    [SerializeField] private float attackTime = 0;
    [SerializeField] private float meleeDamage = 1;
    [SerializeField] private int baseHealth = 3;

    private float timer = 0;

    protected override void Start()
    {
        base.Start();
        health = new Health(baseHealth, 0);
    }

    protected override void Update()
    {
        if (target == null) return;

        if (Vector2.Distance(transform.position, target.position) > attackRange / 2)
        {
            base.Update();
        }


        if(Vector2.Distance(transform.position, target.position) < attackRange)
        {
            Attack(attackTime);
        }
    }

    public override void Attack(float interval)
    {
        if(timer <= interval)
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
