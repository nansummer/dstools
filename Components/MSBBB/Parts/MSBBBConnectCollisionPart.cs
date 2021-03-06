﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Parts/Connect Collision")]
public class MSBBBConnectCollisionPart : MSBBBPart
{
    /// <summary>
    /// The name of the associated collision part.
    /// </summary>
    public string CollisionName;

    /// <summary>
    /// A map ID in format mXX_XX_XX_XX.
    /// </summary>
    public byte MapID1, MapID2, MapID3, MapID4;

    public override void SetPart(MSBBB.Part bpart)
    {
        var part = (MSBBB.Part.ConnectCollision)bpart;
        setBasePart(part);
        CollisionName = part.CollisionName;
        MapID1 = part.MapID1;
        MapID2 = part.MapID2;
        MapID3 = part.MapID3;
        MapID4 = part.MapID4;
    }

    public override MSBBB.Part Serialize(GameObject parent)
    {
        var part = new MSBBB.Part.ConnectCollision(parent.name);
        _Serialize(part, parent);
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.MapID1 = MapID1;
        part.MapID2 = MapID2;
        part.MapID3 = MapID3;
        part.MapID4 = MapID4;
        return part;
    }
}
