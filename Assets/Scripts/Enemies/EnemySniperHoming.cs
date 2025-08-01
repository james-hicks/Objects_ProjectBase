using UnityEngine;

public class EnemySniperHoming : EnemySniper
{
    protected override void Start()
    {
        base.Start();
        health = new Health(30, 0);
    }
}
