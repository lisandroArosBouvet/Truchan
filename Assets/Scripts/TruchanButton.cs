using System;
using UnityEngine;
using UnityEngine.Events;

public class TruchanButton : MonoBehaviour
{
  [SerializeField] public KeyCode keyboardButton;
  [HideInInspector] public float duration;
  float durationTimer;
  [HideInInspector] public int damage;
  public Animator anim;
  public ParticleSystem pressParticle;
  public UnityEvent<TruchanButton> OnButtonDestroyed;
  [SerializeField] public AudioClip buttonDestroyedSound;
  public SpriteRenderer spriteRenderer;
  bool damageDealt = false;
    bool isPressed = false;
  private void Awake()
  {
    durationTimer = duration;
    if (anim == null)
      anim = GetComponent<Animator>();
    if (spriteRenderer == null)
      spriteRenderer = GetComponent<SpriteRenderer>();
  }

  protected virtual void Update()
  {
        if (isPressed)            return;
    durationTimer += Time.deltaTime;
    spriteRenderer.color = Color.Lerp(Color.white, Color.darkRed, Mathf.InverseLerp(0, duration, durationTimer));
    if (durationTimer > duration && damageDealt == false)
    {
      Utilities.DealDamageToPlayer(damage, true);
      damageDealt = true;
      Destroy(gameObject);
      OnButtonDestroyed?.Invoke(this);
    }
  }
  public virtual void OnPressed(float actionValue)
  {
        anim.SetTrigger("pressDissapear");
        pressParticle.transform.SetParent(null);
        pressParticle.Play();
        Destroy(pressParticle, 2.5f);
        Destroy(gameObject, .5f);
        OnButtonDestroyed?.Invoke(this);
        isPressed = true;
        DifficultyManager.Instance.PressCorrectButton(actionValue);
  }

  private void OnDestroy()
  {
  }
}
