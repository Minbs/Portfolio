using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHideObject : TutorialObject
{
	public GameObject p_DynaOutline;
	
	private Collider2D[] m_Colliders;
	// Start is called before the first frame update
	protected override void Start()
	{
		base.Start();
		m_Colliders = GetComponentsInChildren<Collider2D>();
		foreach (var collider in m_Colliders)
		{
			collider.enabled = false;
		}
	}

	private void OnEnable()
	{
		p_DynaOutline.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{

	}
	public override void Initialize()
	{
		p_DynaOutline.SetActive(true);
		action += NextAnimation;
	}

	public void ActiveCollider()
	{
		foreach (var collider in m_Colliders)
		{
			collider.enabled = true;
		}
	}
}
