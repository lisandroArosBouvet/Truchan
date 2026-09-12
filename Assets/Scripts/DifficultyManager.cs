using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    // Instancia estática accesible desde cualquier lugar
    public static DifficultyManager Instance { get; private set; }

    private float[] timeForActions = { 2.4f,2,1.8f, 1.6f,1,1f, .9f, .7f, .45f,.35f };
    private int currentDifficultIndex = 5;

    private float initTime = 1f;
    public float actionsForPress { get; private set; } = 1;
    public float actionsForHold { get; private set; } = 2f;
    public float actionsForQuick { get; private set; } = 2.5f;

    private float totalTime;
    private float goalActionForTime;
    private float startTime;
    private const float PERCENT_TO_UP_DIFFICULT = .6f;
    private const float PERCENT_TO_DOWN_DIFFICULT = .8f;

    private List<float> inputTimers = new List<float>();

    internal float GetTimeForThisWave(int pressCount, int holdCount, int quickCount)
    {
        float actionTime = timeForActions[currentDifficultIndex];
        float waveTotalTime =
            (initTime +
            actionsForPress * pressCount +
            actionsForHold * holdCount +
            actionsForQuick * quickCount) *
            actionTime
            ;
        totalTime = waveTotalTime;
        goalActionForTime = totalTime/(actionsForPress * pressCount +actionsForHold * holdCount + actionsForQuick * quickCount);
        return waveTotalTime;
    }
    public void InitWave()
    {
        startTime = Time.time;
        inputTimers.Clear();
    }
    public void PressCorrectButton(float value)
    {
        inputTimers.Add(value);
    }

    public void EvaluateResultOnWave()
    {
        var allActions = inputTimers.Sum();
        var playerTimeToFinish = Time.time - startTime;
        var playerPerformance = playerTimeToFinish / allActions;

       Debug.Log($"TotalTime {totalTime} - Resultado promedio de inputs: Esperado {goalActionForTime}({totalTime}) / Player: {playerPerformance}({playerTimeToFinish}) /Percent: {playerPerformance/goalActionForTime}");
        if (playerPerformance < (goalActionForTime * PERCENT_TO_UP_DIFFICULT))
            ChangeDifficult(+1);
        else if (playerPerformance > goalActionForTime * PERCENT_TO_DOWN_DIFFICULT)
            ChangeDifficult(-1);

        inputTimers.Clear();
    }

    private void ChangeDifficult(int v)
    {
        currentDifficultIndex += v;
        currentDifficultIndex = Mathf.Clamp(currentDifficultIndex, 0,timeForActions.Length-1);
        Debug.Log("currentDifficultIndex: " + currentDifficultIndex);
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
