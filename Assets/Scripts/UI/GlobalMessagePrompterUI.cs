using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMessagePrompterUI : CursorTooltipUI
{
    public static GlobalMessagePrompterUI Instance;

    private void Awake()
    {
        Instance = this;
    }
}
