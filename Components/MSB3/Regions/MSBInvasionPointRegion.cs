﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3InvasionPointRegion : MSB3Region
{
    /// <summary>
    /// Not sure what this does.
    /// </summary>
    public int Priority;

    public void SetRegion(MSB3.Region.InvasionPoint region)
    {
        setBaseRegion(region);
        Priority = region.Priority;
    }
}
