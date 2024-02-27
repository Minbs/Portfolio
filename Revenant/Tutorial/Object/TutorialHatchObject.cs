using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialHatchObject : TutorialObject
{
	private Collider2D[] m_Colliders;
	public List<GameObject> ActiveOffPrefabs = new();
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

	public override void Initialize()
	{
		action += NextAnimation;

		var bulletUI = FindObjectOfType<LeftBullet_WUI>().gameObject;
		ActiveOffPrefabs.Add(bulletUI);
	}

	public void ActiveCollider()
	{
		foreach (var collider in m_Colliders)
		{
			collider.enabled = true;
		}

		transform.GetChild(0).gameObject.SetActive(true);
		m_animator.StopPlayback();
		m_animator.enabled = false;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if(Input.GetKey(KeyCode.F) && collision.tag == "@Player" && m_animator.enabled == false)
		{
			m_animator.enabled = true;
			transform.GetChild(0).gameObject.SetActive(false);
			action?.Invoke();
			collision.gameObject.SetActive(false);

			foreach(var obj in ActiveOffPrefabs)
			{
				obj.SetActive(false);
			}
			
		}
	}

	public void LoadNextScene()
	{
		GameMgr.GetInstance().RequestLoadScene(4, true);
	}
}
