using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TutorialStairObject : TutorialObject
{
    private Collider2D[] m_Colliders;
	public Transform p_StairTopTransform; // 계단 오르기 진행도 확인용
	public Transform p_StairBottomTransform; // 계단 내려오기 진행도 확인용
	public Light2D p_StairLight;
	public float TurnLightDuration = 1;

	// Start is called before the first frame update
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

    public void ActiveCollider()
    {
		foreach (var collider in m_Colliders)
		{
			collider.enabled = true;
		}

		m_animator.StopPlayback();
		m_animator.enabled = false;
		StartCoroutine(TurnOnLight());
	}

	IEnumerator TurnOnLight()
	{
		float t = 0;

		while(t / TurnLightDuration >= 0)
		{
			t += Time.deltaTime;
			p_StairLight.intensity = Mathf.Lerp(0, 1,t / TurnLightDuration);
			yield return null;
		}

		p_StairLight.intensity = 1;
		yield return null;
	}
}
