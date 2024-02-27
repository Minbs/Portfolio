using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueHistory : MonoBehaviour
{
	public static DialogueHistory instance;
	public GameObject HistoryText;
	[field: SerializeField] public ScrollRect scrollBarRect { get; set; }

	public void Awake()
	{
		if(!instance)
		{
			instance = this;
		}
		else
		{
			return;
		}
	}

	public void WriteHistoryText(string _text)
	{
		GameObject historyText = Instantiate(HistoryText);
		historyText.gameObject.transform.SetParent(transform, false);
		historyText.GetComponent<TextMeshProUGUI>().text = _text;
		StartCoroutine(ScrollToBottom());
	}
	
	private IEnumerator ScrollToBottom()
	{
		yield return new WaitForEndOfFrame();
		scrollBarRect.verticalNormalizedPosition = 0f;
	}

	public void Clear()
	{
        if (transform.childCount <= 0)
            return;

        for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}
	}
}
