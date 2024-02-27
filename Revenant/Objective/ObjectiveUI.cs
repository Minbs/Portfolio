using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ObjectiveUI : MonoBehaviour
{
	// Visible Member Variables
	//public float p_LerpSpeed = 1f;
	public float p_LerpDuration = 1f;
	public RectTransform m_UIRect;
	public TextMeshProUGUI m_TitleTxt;
	public TextMeshProUGUI[] m_ObjectiveTextArr = new TextMeshProUGUI[5];
	public Image[] m_ProgressImgArr = new Image[5];
	public Image[] m_ClearEffectArr = new Image[5];
	public Material[] materials = new Material[4];


	// Member Variables
	private int m_Length = 0;
	private Objective m_CurObjective = null;
	private float m_XLerpGap = 1500f;

	public bool m_IsLerping { get; private set; } = false;
	private Vector2 m_OriginRectPos;
	private Vector2 m_OuterRectPos;
	private Coroutine m_LerpCoroutine;
	private bool isClearEffectOn = false;

	// Constructors
	private void Awake()
	{
		m_OriginRectPos = m_UIRect.anchoredPosition;
		m_OuterRectPos = m_OriginRectPos;
		m_OuterRectPos.x += m_XLerpGap;

		if (m_ObjectiveTextArr.Length != m_ProgressImgArr.Length)
		{
			Debug.Log("ERR : ObjectiveUI에서 ARR 개수 다름");
		}

		ResetObjectiveUI();
		m_Length = 0;
	}


	// Functions

	public void LerpUI(bool _isPush, Action _action = null)
	{
		m_IsLerping = true;
		StopCoroutine("LerpUIEnumerator");
		m_LerpCoroutine = StartCoroutine(LerpUIEnumerator(_isPush, _action));
	}

	/* 구버전
    private IEnumerator LerpUIEnumerator(bool _isPush, Action _action)
    {
        float timer = 0f;

        while(isClearEffectOn)
        {
            yield return null;
        }

        while (true)
        {
			timer += Time.deltaTime * p_LerpSpeed;
            if (_isPush)
            {
                m_UIRect.anchoredPosition =
                    Vector2.Lerp(m_OriginRectPos, m_OuterRectPos, timer);

                if (Vector2.Distance(m_UIRect.anchoredPosition, m_OuterRectPos) < 0.1f)
                    break;
            }
            else
            {
                m_UIRect.anchoredPosition =
                    Vector2.Lerp(m_OuterRectPos, m_OriginRectPos, timer);

                if (Vector2.Distance(m_UIRect.anchoredPosition, m_OriginRectPos) < 0.1f)
                    break;
            }
            yield return null;
        }

        _action?.Invoke();
        m_IsLerping = false;
        yield break;
    }
    */

	private IEnumerator LerpUIEnumerator(bool _isPush, Action _action)
	{
		float timer = 0f;

		while (isClearEffectOn)
		{
			yield return null;
		}

		while (timer < p_LerpDuration)
		{
			timer += Time.deltaTime;
			if (_isPush)
			{
				{
					Color InitColor = m_TitleTxt.color;
					Color FinalColor = m_TitleTxt.color;
					InitColor.a = 1;
					FinalColor.a = 0;

					m_TitleTxt.color = Color.Lerp(InitColor, FinalColor, timer * 2 / p_LerpDuration);
				}

				foreach (var text in m_ObjectiveTextArr)
				{
					Color InitColor = text.color;
					Color FinalColor = text.color;
					InitColor.a = 1;
					FinalColor.a = 0;

					text.color = Color.Lerp(InitColor, FinalColor, timer * 2 / p_LerpDuration);
				}

				foreach (var image in m_ProgressImgArr)
				{
					Color InitColor = image.color;
					Color FinalColor = image.color;
					InitColor.a = 1;
					FinalColor.a = 0;

					image.color = Color.Lerp(InitColor, FinalColor, timer * 2 / p_LerpDuration);
				}

				foreach (var material in materials)
				{
					material.SetFloat("_TutoUiTime", Mathf.Lerp(2, 0, timer / p_LerpDuration));
				}
			}
			else
			{
				{
					Color InitColor = m_TitleTxt.color;
					Color FinalColor = m_TitleTxt.color;
					InitColor.a = -0.5f;
					FinalColor.a = 1;
					m_TitleTxt.color = Color.Lerp(InitColor, FinalColor, timer / p_LerpDuration);
				}


				foreach (var texts in m_ObjectiveTextArr)
				{
					Color InitColor = texts.color;
					Color FinalColor = texts.color;
					InitColor.a = -0.5f;
					FinalColor.a = 1;

					texts.color = Color.Lerp(InitColor, FinalColor, timer / p_LerpDuration);
				}
				foreach (var images in m_ProgressImgArr)
				{
					Color InitColor = images.color;
					Color FinalColor = images.color;
					InitColor.a = -0.5f;
					FinalColor.a = 1;

					images.color = Color.Lerp(InitColor, FinalColor, timer / p_LerpDuration);
				}
				foreach (var material in materials)
				{
					material.SetFloat("_TutoUiTime", Mathf.Lerp(0, 2, timer / p_LerpDuration));
				}
				foreach (var effect in m_ClearEffectArr)
				{
					effect.material.SetFloat("_TutoUiTime", Mathf.Lerp(0, 2, timer / p_LerpDuration));
				}
			}
			yield return null;
		}

		_action?.Invoke();
		m_IsLerping = false;
		yield break;
	}

	/// <summary>
	/// 현재 Objective 멤버 변수의 특정 Text의 폰트스타일을 변경합니다.
	/// </summary>
	/// <param name="_idx"></param>
	/// <param name="_isSuccess"></param>
	public void SetObjectiveFontStyle(int _idx, bool _isSuccess)
	{
		if (ReferenceEquals(m_CurObjective, null) || _idx < 0 || _idx >= m_Length)
			return;

		m_ObjectiveTextArr[_idx].fontStyle = _isSuccess ? FontStyles.Strikethrough : FontStyles.Normal;
		m_ObjectiveTextArr[_idx].color = _isSuccess ? Color.gray : new Color32(255, 255, 255, 255);

		if (_isSuccess)
			StartCoroutine(StartProceedSuccessEffect(_idx));
	}

	IEnumerator StartProceedSuccessEffect(int _idx)
	{
		var mat = Instantiate(m_ClearEffectArr[_idx].material);
		isClearEffectOn = true;
		float timer = 2;
		yield return null;

		while (timer > 0)
		{
			timer -= Time.deltaTime;
			mat.SetFloat("_TutoUiTime", timer);
			m_ClearEffectArr[_idx].material = mat;
			yield return null;
		}

		isClearEffectOn = false;
		yield break;
	}

	/// <summary>
	/// Objective를 받아서 UI를 표기합니다.
	/// </summary>
	/// <param name="_objective"></param>
	/// <returns></returns>
	public int InputObjectiveToUI(Objective _objective)
	{
		if (!ReferenceEquals(m_CurObjective, null))
			return 0;

		m_CurObjective = _objective;
		m_TitleTxt.text = m_CurObjective.m_TitleTxt;

		for (int i = 0; i < m_CurObjective.m_ObjectiveTxtArr.Length; i++)
		{
			m_ObjectiveTextArr[i].text = m_CurObjective.m_ObjectiveTxtArr[i];
		}

		foreach (var obj in m_ObjectiveTextArr)
		{
			if (obj.text == "")
			{
				obj.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				obj.transform.parent.gameObject.SetActive(true);
			}
		}

		m_Length = m_CurObjective.m_ObjectiveTxtArr.Length;

		return 1;
	}

	/// <summary>
	/// 지정한 Idx의 Circle Img의 Fillamount를 설정합니다.
	/// </summary>
	/// <param name="_idx"></param>
	/// <param name="_value"></param>
	public void SetObjectiveProgress(int _idx, float _value)
	{
		if (ReferenceEquals(m_CurObjective, null) || _idx < 0 || _idx >= m_Length)
			return;

		m_ProgressImgArr[_idx].fillAmount = _value;
	}

	/// <summary>
	/// 지정한 Idx의 Circle Img의 Fillamount를 Get합니다.
	/// </summary>
	/// <param name="_idx"></param>
	/// <returns></returns>
	public float GetObjectiveProgress(int _idx)
	{
		if (ReferenceEquals(m_CurObjective, null) || _idx < 0 || _idx >= m_Length)
			return -1f;

		return m_ProgressImgArr[_idx].fillAmount;
	}


	/// <summary>
	/// ObjectiveUI를 초기화합니다.
	/// </summary>
	public void ResetObjectiveUI()
	{
		if (m_IsLerping)
		{
			m_IsLerping = false;
			StopCoroutine(m_LerpCoroutine);
		}

		//m_UIRect.anchoredPosition = m_OuterRectPos;

		m_TitleTxt.text = "";
		for (int i = 0; i < m_ObjectiveTextArr.Length; i++)
		{
			m_ObjectiveTextArr[i].text = "";
			m_ObjectiveTextArr[i].fontStyle = FontStyles.Normal;
		}

		for (int i = 0; i < m_ObjectiveTextArr.Length; i++)
		{
			m_ProgressImgArr[i].fillAmount = 0f;
		}

		m_Length = 0;
		m_CurObjective = null;


		Color InitColor = m_TitleTxt.color;
		InitColor.a = 0;
		m_TitleTxt.color = InitColor;



		foreach (var texts in m_ObjectiveTextArr)
		{
			texts.color = InitColor;
		}
		foreach (var images in m_ProgressImgArr)
		{
			images.color = InitColor;
		}
		foreach (var material in materials)
		{
			material.SetFloat("_TutoUiTime", 0);
		}
	}
}