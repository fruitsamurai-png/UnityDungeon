/*******************************************************************************
File:      HealthBar.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/5/2018
Course:    CS186
Section:   Z

Description:
    This component controls all of the behavior for a health bar which is used
    by enemies an the player.

*******************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject HealthBarTickPrefab;
    public Transform HealthBarBackground;

    public Color FilledTickColor = Color.red;
    public Color MissingTickColor = Color.gray;

    public int MaxHealth
    {
        get { return _MaxHealth; }

        set
        {
            //Recreate all hearts icons
            for (int i = 0; i < HealthBarBackground.transform.childCount; ++i)
            {
                Destroy(HealthBarBackground.transform.GetChild(i).gameObject);
            }
            Ticks.Clear();

            for (int i = 0; i < value; ++i)
            {
                //Parent new icons to the panel
                var obj = Instantiate(HealthBarTickPrefab, HealthBarBackground.transform);
                Ticks.Add(obj.GetComponent<Image>());
            }

            _MaxHealth = value;
        }
    }
    private int _MaxHealth = 0;
    private List<Image> Ticks = new List<Image>();

    public int Health
    {
        get { return _Health; }

        set
        {
            //Ignore values out of bounds
            if (value > MaxHealth || value < 0)
                return;

            //Color health icons based on new health
            for (int i = Ticks.Count - 1; i >= 0; --i)
            {
                if (i < value)
                    Ticks[i].color = FilledTickColor;
                else
                    Ticks[i].color = MissingTickColor;
            }

            _Health = value;
        }

    }
    private int _Health;
}
