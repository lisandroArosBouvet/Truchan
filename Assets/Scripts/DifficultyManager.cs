using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    // Instancia estática accesible desde cualquier lugar
    public static DifficultyManager Instance { get; private set; }

    [Range(0.1f, 1f)]
    public float currentDifficulty = .5f;

    private const float maxModifierDifficult = .25f;
    private const float minModifierDifficult = -.35f;
    private float waveFullTime;
    private float startPointWave;

    public void SetWaveFullTime(float t)
    {
        waveFullTime = t;
        startPointWave = Time.time;
        Debug.Log($"All time: {waveFullTime}");
    }

    public void EvaluateResultOnWave()
    {
        float remainingTimeWave = Time.time - startPointWave;
        float playerPerformance = remainingTimeWave / waveFullTime;
        float remaping = Mathf.Lerp(minModifierDifficult, maxModifierDifficult, playerPerformance);
        currentDifficulty += remaping;
        currentDifficulty = Mathf.Clamp(currentDifficulty, .1f, 1f);
        Debug.Log($"reming: {remainingTimeWave} / currenDifficult: {currentDifficulty}");
    }

    private void Awake()
    {
        // Verificar si ya existe otra instancia
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruir duplicados
            return;
        }

        // Asignar la instancia actual y evitar que se destruya al cambiar de escena
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
