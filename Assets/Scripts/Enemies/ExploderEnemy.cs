using UnityEngine;

public class ExploderEnemy : MeleeEnemy
{
    public float explosionRadius;

    public virtual void Explode()
    {
        Debug.Log("EXPLODE!");
        base.Die();
    }


    public override void Die()
    {
        Explode();
    }

    public override void Attack(float interval)
    {
        Die();
    }
}
