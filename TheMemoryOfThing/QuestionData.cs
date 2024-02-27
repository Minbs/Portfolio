using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestionData
{
	public string guest;
	public int set;
	public int index;
	public string expression;
	public string Data;

	public QuestionData() { }

	QuestionData(string _guest, int _set, int _index, string _expression)
	{
		guest = _guest;
		set = _set;
		index = _index;
		expression = _expression;
	}

	/// <summary>
	/// 질문의 종류가 같다면 true 반환
	/// </summary>
	public static bool operator == (QuestionData left, QuestionData right)
	{
		if (left.guest == right.guest
			&& left.set == right.set
			&& left.index == right.index
			&& left.expression == right.expression)
			return true;

		return false;
	}

	/// <summary>
	/// 질문의 종류가 다르다면 true 반환
	/// </summary>
	public static bool operator !=(QuestionData left, QuestionData right)
	{
		if (left.guest != right.guest
			|| left.set != right.set
			|| left.index != right.index
			|| left.expression != right.expression)
			return true;

		return false;
	}

	/// <summary>
	/// 출력함
	/// </summary>
	public override string ToString()
	{
		return "손님 이름 : " + guest.ToString() + "\n" + "세트 번호 : " + set.ToString() + "\n" + "인덱스 숫자 : " + index.ToString() + "\n" + "데이터 : " + Data.ToString();
	}
}
