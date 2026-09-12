using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
  [SerializeField] int maxHealth = 100;
  [SerializeField] Image healthBar;
  [SerializeField] Image portrait;
  [SerializeField] Sprite highHealthSprite;
  [SerializeField] Sprite lowHealthSprite;
  [SerializeField] Sprite damagedSprite;
  [SerializeField] Sprite deathSprite;
  [SerializeField] UnityEvent OnDeath;
  [SerializeField] UnityEvent OnDamageTaken;
  [SerializeField] float invulnerabilityTime = 1f;
  int currentHealth;
  float invulTimer = 0;

    [SerializeField] Public spectators;

  private void Awake()
  {
    invulTimer = invulnerabilityTime;
    currentHealth = maxHealth;
  }
  private void Update()
  {
    invulTimer += Time.deltaTime;
    UpdateHealthUI();
    }

    public void DealDamage(int damage, bool ignoreInvulnerability = false)
    {
        if (ignoreInvulnerability == false && invulTimer < invulnerabilityTime)
            return;
        currentHealth -= damage;
        currentHealth = Math.Max(currentHealth, 0);
        spectators.SetPublics(currentHealth, maxHealth);
        invulTimer = 0;
        if (currentHealth == 0)
            OnDeath?.Invoke();
        else if (invulTimer > invulnerabilityTime)
            OnDamageTaken?.Invoke();
    }

    private void UpdateHealthUI()
  {
    healthBar.fillAmount = Mathf.InverseLerp(0, maxHealth, currentHealth);
    if (invulTimer < invulnerabilityTime)
    {
      portrait.sprite = damagedSprite;
    }
    else if (currentHealth > maxHealth / 2)
    {
      portrait.sprite = highHealthSprite;
    }
    else if (currentHealth > 0)
    {
      portrait.sprite = lowHealthSprite;
    }
    else
    {
      portrait.sprite = deathSprite;
    }
  }

  public void RestoreAllHealth()
  {
    currentHealth = maxHealth;
        spectators.AllDissapear();
  }
}
