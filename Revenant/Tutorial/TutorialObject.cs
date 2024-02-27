using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObject : MonoBehaviour
{
	public Action action;
	public Animator m_animator;

	protected virtual void Start()
	{
		if(TryGetComponent(out Animator animator))
		{
			if(animator)
			{
				m_animator = animator;
			}
		}
	}

	/// <summary>
	/// 오브젝트가 속한 튜토리얼 레벨이 시작했을 때 실행
	/// </summary>
	public virtual void Initialize() 
	{
		NextAnimation();
	}

	public void NextAnimation()
	{
		m_animator.SetInteger("TutorialAnimationIndex", m_animator.GetInteger("TutorialAnimationIndex") + 1);
	}
}
