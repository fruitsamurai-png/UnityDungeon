/*******************************************************************************
File:      CollectibleLogic.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/5/2018
Course:    CS186
Section:   Z

Description:
    This component is shared among all collectible objects and determines what
    kind of collectible it is.

*******************************************************************************/
using UnityEngine;

public enum CollectibleTypes { HealthBoost, SpeedBoost, ShotBoost, SilverKey, GoldKey, Heart }

public class CollectibleLogic : MonoBehaviour
{
    public CollectibleTypes Type;
}
