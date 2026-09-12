using System;
using UnityEngine;

public class HoldButton : TruchanButton
{
  [SerializeField] float timeToHold = 1.2f;
  bool isButtonPressed = false;
  float heldTimer = 0;
  [SerializeField] Sprite twentyPercentDamageSprite;
  [SerializeField] Sprite fortyPercentDamageSprite;
  [SerializeField] Sprite sixtyPercentDamageSprite;
  [SerializeField] Sprite eightyPercentDamageSprite;
  [SerializeField] SpriteRenderer outline;
  bool buttonDestroyed = false;
  private void Awake()
  {
    if (spriteRenderer == null)
      spriteRenderer = GetComponent<SpriteRenderer>();
    if (outline != null)
      outline.enabled = false;
  }
  protected override void Update()
  {
    if (buttonDestroyed)
      return;
    if (isButtonPressed)
    {
      if (Input.GetKeyUp(this.keyboardButton))
      {
        duration += heldTimer;
        isButtonPressed = false;
        heldTimer = 0;
        return;
      }
      heldTimer += Time.deltaTime;
      if (heldTimer >= timeToHold)
      {
        buttonDestroyed = true;
        base.OnPressed(DifficultyManager.Instance.actionsForHold);
      }
      UpdateSprite();
      if (outline != null)
        outline.enabled = true;
    }
    else
    {
      base.Update();
      if (outline != null)
        outline.enabled = false;
    }
  }

  private void UpdateSprite()
  {
    var wornAmount = Mathf.InverseLerp(0, timeToHold, heldTimer);
    if (wornAmount >= 0.8)
    {
      spriteRenderer.sprite = eightyPercentDamageSprite;
    }
    else if (wornAmount >= 0.6)
    {
      spriteRenderer.sprite = sixtyPercentDamageSprite;
    }
    else if (wornAmount >= 0.4)
    {
      spriteRenderer.sprite = fortyPercentDamageSprite;
    }
    else if (wornAmount >= 0.2)
    {
      spriteRenderer.sprite = twentyPercentDamageSprite;
    }
  }

  public override void OnPressed(float actionValue)
  {
    isButtonPressed = true;
  }
}
