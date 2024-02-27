using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CinemachineDialogText : MonoBehaviour
{
	[field: SerializeField] public TextMeshProUGUI TextUI { get; set; }
	private string textString;
	private float currentTextCount = 0;

	public bool isTypingEffect = false;
	public float TypingSpeed = 0.5f;

	public bool isShake = false;
	public float ShakeAmount = 2;
	public float ShakeDuration = 1;
	Vector2 startingPos = new();
	public bool isTextEnd { get; set; } = false;

	private RectTransform rectTransform;


	void Start()
	{
		TextUI = GetComponentInChildren<TextMeshProUGUI>();
		rectTransform = GetComponent<RectTransform>();
		textString = TextUI.text;
		if (isTypingEffect)
			TextUI.text = "";

		startingPos = rectTransform.anchoredPosition;
	}

	void FixedUpdate()
	{
		if(isShake && ShakeDuration > 0)
		{
			Vector3 pos = rectTransform.anchoredPosition;
			pos = startingPos + (Random.insideUnitCircle * (new Vector2(1, 1) * ShakeAmount));
			//Debug.Log(pos.z);
			rectTransform.anchoredPosition = pos;
			ShakeDuration -= Time.deltaTime;
		}

		if (isTypingEffect)
		{
			if (textString.Length > 0 && currentTextCount < textString.Length - 1)
			{
				currentTextCount += Time.deltaTime * TypingSpeed;
				if (textString[(int)currentTextCount] == '<')
				{
					while (textString[(int)currentTextCount] != '>')
					{
						currentTextCount++;
					}
				}
				TextUI.text = textString.Substring(0, (int)currentTextCount + 1);
			}
		}
		else
		{
			currentTextCount = textString.Length - 1;
		}
	}


}
