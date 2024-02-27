using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestData : MonoBehaviour
{
    public Sprite normalExpression;
	public Sprite thinkExpression;
	public Sprite smileExpression;
	public Sprite madExpression;
	public Sprite originalObjectSprite;

	public Sprite GetExpressionSprite(string expression)
	{
		if (expression == "Normal") return normalExpression;
		else if (expression == "Think") return thinkExpression;
		else if (expression == "Smile") return smileExpression;
		else if (expression == "Mad") return madExpression;
		else Debug.Log(expression); return null;
	}

	public Sprite GetOriginalObjectSprite()
	{
		return originalObjectSprite;
	}
}
