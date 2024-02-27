using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerData
{
	public string guest; // 말하는 사람
	public int index;
	public string expression; // 표정
	public string Data;

	public AnswerData() { }

	AnswerData(string _speaker, int _index, string _expression)
	{
		guest = _speaker;
		index = _index;
		expression = _expression;
	}

	/// <summary>
	/// 질문의 종류가 같다면 true 반환
	/// </summary>
	public static bool operator ==(AnswerData left, AnswerData right)
	{
		return left.guest == right.guest && 
			left.index == right.index && 
			left.expression == right.expression && 
			left.Data == right.Data;
	}

	/// <summary>
	/// 질문의 종류가 다르다면 true 반환
	/// </summary>
	public static bool operator !=(AnswerData left, AnswerData right)
	{
		return left.guest != right.guest || 
			left.index != right.index || 
			left.expression != right.expression || 
			left.Data != right.Data;
	}

	/// <summary>
	/// 출력함
	/// </summary>
	public override string ToString()
	{
		return $"말하는 사람 : {guest}\n인덱스 숫자 : {index}\n표정 : {expression}\n데이터 : {Data}";
	}
}