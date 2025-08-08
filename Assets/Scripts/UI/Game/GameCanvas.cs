using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : UtopiaCanvas
{
    public static bool IsGamePaused => Instance.latestUIType != typeof(GameHUD);

    
}
