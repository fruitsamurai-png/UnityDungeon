/*******************************************************************************
File:      PortalLogic.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/5/2018
Course:    CS186
Section:   Z

Description:
    This component handles the collision logic for portals to reset the level
    when colliding with the player.

*******************************************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalLogic : MonoBehaviour
{	
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hero = collision.GetComponent<HeroStats>();
        if (hero != null)
        {
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }
}
