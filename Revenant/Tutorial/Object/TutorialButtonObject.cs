using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButtonObject : TutorialObject
{
	private Collider2D[] m_Colliders;

	protected override void Start()
	{
		base.Start();
		m_Colliders = GetComponentsInChildren<Collider2D>();
		foreach (var collider in m_Colliders)
		{
			collider.enabled = false;
		}
		Initialize();
	}

	public override void Initialize()
	{
		action += NextAnimation;
	}

	private void OnTriggerStay2D(Collider2D collision)
    {
		if (collision.tag == "@Player")
		{
			if (Input.GetKey(KeyCode.F))
			{
				action?.Invoke();
				transform.GetChild(0).gameObject.SetActive(false);
			}
		}
	}

	public void ActiveCollider()
	{
		foreach (var collider in m_Colliders)
		{
			collider.enabled = true;
		}

		m_animator.StopPlayback();
		m_animator.enabled = false;
		transform.GetChild(0).gameObject.SetActive(true);
	}
}
