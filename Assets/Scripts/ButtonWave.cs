using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ButtonWave
{
  public float timeToPressAllButtons = 0;
  public float timeUntilNextWave = 0;
  public int buttonDamage = 10;
    [Range(0, 8)]
    public int button_press = 0;
    [Range(0, 8)]
    public int button_hold = 0;
    [Range(0, 8)]
    public int button_quick = 0;
}
