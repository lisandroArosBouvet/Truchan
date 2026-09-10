using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WaveSpawner : MonoBehaviour
{
  [SerializeField] List<BoxCollider2D> buttonSpawnZones;
  [SerializeField] List<RootButton> possibleButtons;
  [SerializeField] public UnityEvent OnWaveFinished;
  [SerializeField] public UnityEvent OnWaveCollectionFinished;
  List<TruchanButton> currentWaveButtons = new();
  [SerializeField] AudioManager audioManager;

    public float pressTime = 2f;
    public float holdTime = 3f;
    public float quickTime = 4f;
    public float baseTimePressAllButtons = 5f;
    public float timeUntilNextWave = 2;
    public int buttonDamage = 10;
    private void Awake()
  {
  }

  public IEnumerator SpawnWaveCollection(ButtonWaveCollection waveCollection)
  {
    for (int i = 0; i < waveCollection.waves.Count; i++)
    {
      var wave = waveCollection.waves[i];    
      SpawnButtonWave(wave);
      yield return new WaitUntil(() => currentWaveButtons.Any() == false);
      if (i != waveCollection.waves.Count - 1)
      {
                DifficultyManager.Instance.EvaluateResultOnWave();
        OnWaveFinished?.Invoke();
        yield return new WaitForSeconds(timeUntilNextWave);
      }
    }
    OnWaveCollectionFinished?.Invoke();
  }

  private void SpawnButtonWave(ButtonWave buttonWave)
  {
        List<RootButton> spawnedButtons = new(possibleButtons);
        spawnedButtons = spawnedButtons.OrderBy(x => Random.value).ToList();
        int length = possibleButtons.Count,
            pressCount = buttonWave.button_press,
            holdCount = buttonWave.button_hold,
            quickCount = buttonWave.button_quick;

        float timeToPressAllButtons =
            baseTimePressAllButtons +
            pressCount * pressTime +
            holdCount * holdTime +
            quickCount * quickTime;
        timeToPressAllButtons *= DifficultyManager.Instance.currentDifficulty;
        DifficultyManager.Instance.SetWaveFullTime(timeToPressAllButtons);
        for (int i = 0; i < length; i++)
        {
            TruchanButton b;
            if (pressCount > 0)
            {
                pressCount--;
                b = spawnedButtons[0].press;
            }
            else if (holdCount > 0)
            {
                holdCount--;
                b = spawnedButtons[0].hold;
            }
            else if (quickCount > 0)
            {
                quickCount--;
                b = spawnedButtons[0].quick;
            }
            else
                break;
            spawnedButtons.RemoveAt(0);
            SpawnButton(b, timeToPressAllButtons);
        }
    }
    private void SpawnButton(TruchanButton buttonPrefab, float timeToDissapear)
  {
    var spawnedButton = Instantiate(buttonPrefab);
    spawnedButton.duration = timeToDissapear;
    spawnedButton.damage = buttonDamage;
    int iterations = 0;
    while (iterations < 15 && IsButtonOverlappingOtherButtons(spawnedButton))
    {
      spawnedButton.transform.position = GetRandomPositionInBounds(buttonSpawnZones[Random.Range(0, buttonSpawnZones.Count)].bounds);
      iterations++;
    }
    if (iterations == 15)
      Debug.LogWarning("Iterations exceeded when spawning button");
    currentWaveButtons.Add(spawnedButton);
    spawnedButton.OnButtonDestroyed.AddListener(OnButtonDestroyed);
  }

  private void OnButtonDestroyed(TruchanButton obj)
  {
    currentWaveButtons.Remove(obj);
    audioManager.PlaySFX(obj.buttonDestroyedSound);
 
  }

  private bool IsButtonOverlappingOtherButtons(TruchanButton button)
  {
    var sprite = button.GetComponent<SpriteRenderer>();
    var buttonsInScene = Object.FindObjectsByType<TruchanButton>(FindObjectsSortMode.InstanceID).Where(x => !x.Equals(button));
    foreach (var buttonInScene in buttonsInScene)
    {
      var buttonInSceneSprite = buttonInScene.GetComponent<SpriteRenderer>();
      if (sprite.bounds.Intersects(buttonInSceneSprite.bounds))
      {
        return true;
      }
    }
    return false;
  }


  private Vector3 GetRandomPositionInBounds(Bounds bounds)
  {
    return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
  }
}
