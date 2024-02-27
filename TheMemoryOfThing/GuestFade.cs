using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestFade : MonoBehaviour
{
    public static GuestFade Instance;

    public Animator GuestAnimator;

    public void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    public void StartGuestFade()
    {
        GuestAnimator.SetTrigger("GuestFadeIn");
		GameManager.instance.gameState = GameState.Comment;
	}
}
