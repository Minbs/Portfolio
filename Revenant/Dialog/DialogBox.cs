using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
	[field: SerializeField] public TextMeshProUGUI TextUI { get; set; }
	private string textString;
	private float currentTextCount = 0;

	public Action SkipEvent;
	public bool isTypingEffect = false;
	public float TypingSpeed = 0.5f;
	public bool isOnPlayerPosition = false;
	public bool isTextEnd { get; set; } = false;





	void Start()
	{
		TextUI = GetComponentInChildren<TextMeshProUGUI>();
		textString = TextUI.text;
		if (isTypingEffect)
			TextUI.text = "";
		SkipEvent += SkipText;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (isTypingEffect)
		{
			if (textString.Length > 0 && currentTextCount < textString.Length - 1)
			{
				currentTextCount += Time.deltaTime * TypingSpeed;
				if(textString[(int)currentTextCount] == '<')
				{
					while(textString[(int)currentTextCount] != '>')
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

	private void SkipText()
	{
		if (currentTextCount < textString.Length - 1)
		{
			currentTextCount = textString.Length - 1;
			TextUI.text = textString;
		}
		else
		{
			isTextEnd = true;
		}
	}
}
