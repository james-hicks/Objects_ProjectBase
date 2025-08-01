using UnityEngine;

public class EnemySniper : EnemyPistol
{
    protected override void Start()
    {
        base.Start();
        health = new Health(30, 0);
    }
}
