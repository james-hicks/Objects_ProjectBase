using UnityEngine;

public class EnemyMachiner : EnemyPistol
{
    protected override void Start()
    {
        base.Start();
        health = new Health(20, 0);  
        

    }
}
