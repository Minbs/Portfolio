using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDialog : MonoBehaviour
{
	[field: SerializeField] public TextMeshProUGUI TextUI { get; set; }
	private string textString;
	private float currentTextCount = 0;

	public Action SkipEvent;
	public float TypingSpeed = 0.5f;

	public bool isTextEnd { get; set; } = false;
	private Material material;
	
	void Start()
	{
		TextUI = GetComponentInChildren<TextMeshProUGUI>();
		material = GetComponent<Image>().material;
		TextUI.text = "";
		SkipEvent += SkipText;
	}

	// Update is called once per frame
	void Update()
	{
		transform.localScale = transform.parent.parent.localScale * 0.01f;

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
			TextUI.text += "<alpha=#00>";
			string transparencyText = textString.Substring((int)currentTextCount + 1, textString.Length - (int)currentTextCount - 1);
			transparencyText = transparencyText.Replace("<color=orange>", "");
			transparencyText = transparencyText.Replace("<color=red>", "");
			transparencyText = transparencyText.Replace("</color>", "");
			TextUI.text += transparencyText;
		}
		else
		{
			if(!isTextEnd)
			{
				isTextEnd = true;
				material.SetFloat("_TextEnd", 1);
				//DialogEndUI.SetBool("isTextEnd", isTextEnd);
			}
		}
	}

	private void SkipText()
	{
		if (currentTextCount < textString.Length - 1)
		{
			currentTextCount = textString.Length - 1;
			TextUI.text = textString;
		}
		isTextEnd = true;
		material.SetFloat("_TextEnd", 1);
		//DialogEndUI.SetBool("isTextEnd", isTextEnd);
	}

	public void SetDialogText(string text)
	{
		textString = text;
		currentTextCount = 0;
		isTextEnd = false;
		material.SetFloat("_TextEnd", 0);
		//DialogEndUI.SetBool("isTextEnd", isTextEnd);
		if (TextUI)
		TextUI.text = "";
	}

	public void SetDialogActive(bool value)
	{
		if(!material)
		{
			material = GetComponent<Image>().material;
		}

		transform.localScale = transform.parent.parent.localScale * 0.01f;
		gameObject.SetActive(value);
		float v = value == true ? 1 : 0;
		material.SetFloat("_TextEnd", v);
		//DialogEndUI.gameObject.SetActive(value);
	}
}
