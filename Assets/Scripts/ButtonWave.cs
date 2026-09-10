using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ButtonWave
{
    [Range(0, 8)]
    public int button_press = 0;
    [Range(0, 8)]
    public int button_hold = 0;
    [Range(0, 8)]
    public int button_quick = 0;
}
