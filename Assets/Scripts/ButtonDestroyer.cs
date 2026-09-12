using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ButtonDestroyer : MonoBehaviour
{
  List<KeyCode> acceptedKeys;
  [SerializeField] int wrongButtonPressedDamage = 10;
  [SerializeField] float invulnerabilityTime = 1f;
  [SerializeField] Image hands;
  [SerializeField] Sprite defaultSprite;
  [SerializeField] Sprite[] buttonDestroySprite;
  [SerializeField] Sprite damageReceivedSprite;
  [SerializeField] float damageReceivedSpriteDuration = 0.7f;
  [SerializeField] float buttonDestroyedSpriteDuration = 0.4f;
  [SerializeField] AudioClip wrongButtonPressedClip;
  float lastDamageTime = Mathf.NegativeInfinity;
  float spriteResetTime = Mathf.NegativeInfinity;
    [SerializeField] Animator anim;
  [SerializeField] AudioManager audioService;

  private void Awake()
  {
    var allKeys = (int[])Enum.GetValues(typeof(KeyCode));
    acceptedKeys = new();
    acceptedKeys.AddRange(allKeys.Where(key => key >= 97 && key <= 122).Cast<KeyCode>()); // Teclas de la A a la Z
    acceptedKeys.AddRange(allKeys.Where(key => key >= 273 && key <= 276).Cast<KeyCode>()); // Flechas direccionales
  }

  private void Update()
  {
    if (Time.time > spriteResetTime)
    {
      hands.sprite = defaultSprite;
    }
    if (Input.anyKeyDown)
    {
    var buttonsInScene = Utilities.GetAllButtons();
      foreach (KeyCode keyCode in acceptedKeys)
      {
        if (Input.GetKeyDown(keyCode))
        {
          var buttonsWithKey = buttonsInScene.Where(button => button.keyboardButton == keyCode);
          if (buttonsWithKey.Any())
          {
            foreach(var button in buttonsWithKey)
            {
              button.OnPressed(DifficultyManager.Instance.actionsForPress);

                            anim.SetTrigger("magic");
                            hands.sprite = buttonDestroySprite[Random.Range(0, buttonDestroySprite.Length)] ;
              spriteResetTime = Time.time + buttonDestroyedSpriteDuration;
            }
          }
          else if (Time.time - lastDamageTime > invulnerabilityTime)
          {
                        anim.SetTrigger("damage");
            Utilities.DealDamageToPlayer(wrongButtonPressedDamage);
            lastDamageTime = Time.time;
            hands.sprite = damageReceivedSprite;
            spriteResetTime = Time.time + damageReceivedSpriteDuration;
            audioService.PlaySFX(wrongButtonPressedClip);
          }          
        }
      }
    }
  }
}
