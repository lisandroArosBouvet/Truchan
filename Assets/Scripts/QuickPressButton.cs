using UnityEngine;
using System.Collections.Generic;
using System;
public class QuickPressButton : TruchanButton
{
  [SerializeField] int pressAmount = 5;
  [SerializeField] Sprite twentyPercentDamageSprite;
  [SerializeField] Sprite fortyPercentDamageSprite;
  [SerializeField] Sprite sixtyPercentDamageSprite;
  [SerializeField] Sprite eightyPercentDamageSprite;
  [SerializeField] AudioClip damageClip;
  int timesPressed = 0;

  private void Awake()
  {
    if (spriteRenderer == null)
      spriteRenderer = GetComponent<SpriteRenderer>();
  }
  public override void OnPressed(float actionValue)
  {
    timesPressed++;
    if (timesPressed >= pressAmount)
    {
      base.OnPressed(DifficultyManager.Instance.actionsForQuick);
    }
    else
    {
      pressParticle.Play();
      UpdateSprite();
      var damageAmount = Mathf.InverseLerp(0, pressAmount, timesPressed);
      var pitchAmount = Mathf.Lerp(1, 2, damageAmount);
      Utilities.PlaySFX(damageClip, pitchAmount);
    }
  }

  private void UpdateSprite()
  {
    var damageAmount = Mathf.InverseLerp(0, pressAmount, timesPressed);
    if (damageAmount >= 0.8)
    {
      spriteRenderer.sprite = eightyPercentDamageSprite;
    }
    else if (damageAmount >= 0.6)
    {
      spriteRenderer.sprite = sixtyPercentDamageSprite;
    }
    else if (damageAmount >= 0.4)
    {
      spriteRenderer.sprite = fortyPercentDamageSprite;
    }
    else if (damageAmount >= 0.2)
    {
      spriteRenderer.sprite = twentyPercentDamageSprite;
    }
  }
}
