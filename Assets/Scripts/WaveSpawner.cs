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
  bool finishedWaveCollection;
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
        OnWaveFinished?.Invoke();
        yield return new WaitForSeconds(wave.timeUntilNextWave);
      }
    }
    OnWaveCollectionFinished?.Invoke();
  }
  private void Update()
  {
  }

  private void SpawnButtonWave(ButtonWave buttonWave)
  {
        List<RootButton> spawnedButtons = new(possibleButtons);
        spawnedButtons.OrderBy(x => Random.value).ToList();
        int length = possibleButtons.Count,
            pressCount = buttonWave.button_press,
            holdCount = buttonWave.button_hold,
            quickCount = buttonWave.button_quick;
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
            SpawnButton(buttonWave, b);
        }
    }
    private void SpawnButton(ButtonWave wave, TruchanButton buttonPrefab)
  {
    var spawnedButton = Instantiate(buttonPrefab);
    spawnedButton.duration = wave.timeToPressAllButtons;
    spawnedButton.damage = wave.buttonDamage;
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

  private void OnButtonDestroyed()
  {

  }
}
