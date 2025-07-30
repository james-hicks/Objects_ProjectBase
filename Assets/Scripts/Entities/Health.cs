using UnityEngine;

public class Health
{
    private float maxHealth;
    private float currentHealth;
    private float healthRegenRate;


    /// <summary>
    /// Creates a new Health instance, with the variables of MaxHealth and a base Regeneration Rate.
    /// </summary>
    /// <param name="_maxHealth"> The maximum health of the object</param>
    /// <param name="_healthRegenRate"></param>
    /// <param name="_currentHealth"> If left empty, will default to MaxHealth.</param>
    public Health(float _maxHealth, float _healthRegenRate, float _currentHealth = -1)
    {
        maxHealth = _maxHealth;

        if(_currentHealth < 0)
        {
            currentHealth = _maxHealth;
        }
        else
        {
            currentHealth = _currentHealth;
        }

        healthRegenRate = _healthRegenRate;
    }

    /// <summary>
    /// Base Health instance Creation.
    /// </summary>
    public Health()
    {
    }
    public void AddHealth(float value)
    {
        currentHealth += value;
        if(currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void DeductHealth(float value)
    {
        currentHealth = Mathf.Max(0, currentHealth - value);
    }

    public void RegenHealth()
    {
        AddHealth(healthRegenRate * Time.deltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns> The current health of the object</returns>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
