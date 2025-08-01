using Mono.Cecil;
using System;
using System.Collections.Generic;
using UnityEngine;


public class EnemyPistol : Enemy
{

    private const string PLAYER_TAG = "Player";
    [Header("default Weapon Info")]
    [SerializeField] public float weaponDamage = 1f;
    [SerializeField] public float bulletSpeed = 10f;
    [SerializeField] public Bullet bulletPrefab;
    private float timer = 0f;
              
    private float shootingRate;    
    public float shootingRange;
    public float shotDamage;
    public float shotRate;
    public float nextShotTime = 0f;
    
    public float rotationSpeed = 5f;
    public Transform firePoint;
    public Transform player;
    

    /// <summary>
    /// This script is based off for the Shooter and Machinegun Enemy
    /// 
    /// just has editable variables to make them act diffrently
    /// </summary>


    protected override void Start()
    {
        base.Start();
        health = new Health(30, 0);

        
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

        weapon = new Weapon("EnemyWeapon", shotDamage, bulletSpeed);

    }
    protected override void Update()
    {
        

            if (player == null) return;

       // attack range and rate
        if (Vector2.Distance(transform.position, target.position) < shootingRange)
        {
            if (Time.time >= nextShotTime)
            {
                Attack(shootingRate);
                nextShotTime = Time.time + shotRate; // Reset the timer
            }

        }
        

        // Rotates to the player
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        }
    }

    protected void FixedUpdate()
    {
        if(player == null)
        {
            
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        
        if (distance > shootingRange)
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

    public override void Shoot()
    {
        
    }

    public override void Attack(float shootingRate)
    {

      
            weapon.Shoot(bulletPrefab, "Player", this);
        
        


    }

    

}

