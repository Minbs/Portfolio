using System.Collections;
using CT.Common.Gameplay;
using TMPro;
using UnityEngine;

namespace CTC.GUI.Gameplay.Common.PlayerNameTag
{
	public class Item_UserNameTag : MonoBehaviour
	{
		public RectTransform PlayerIconTransform;
		public TextMeshProUGUI PlayerNameText;
		public float TextPadding; // 아이콘과의 거리
		private RectTransform _rectTransform;

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		public void Initialize(string playerName, Faction team)
		{
			Color nameColor = Color.white;

			switch (team)
			{
				case Faction.Red:
					nameColor = Color.red;
					break;

				case Faction.Blue:
					nameColor = Color.blue;
					break;

				case Faction.Green:
					nameColor = Color.green;
					break;
			}

			PlayerNameText.color = nameColor;
			PlayerNameText.text = playerName;
			StartCoroutine(SetIconImagePosition());
		}

		IEnumerator SetIconImagePosition()
		{
			yield return null;

			float IconBoundWidth = PlayerIconTransform.sizeDelta.x;

			// 이미지 텍스트 전체 길이
			float TotalWidth = IconBoundWidth + TextPadding + PlayerNameText.rectTransform.sizeDelta.x;
			Vector3 nextPosition = Vector3.zero;
			nextPosition.y = PlayerIconTransform.localPosition.y;
			// 플레이어 아이콘 위치 계산
			nextPosition.x = -TotalWidth * 0.5f + IconBoundWidth * 0.5f;
			PlayerIconTransform.transform.localPosition = nextPosition;

			// 플레이어 텍스트 위치 계산
			nextPosition.x = nextPosition.x + IconBoundWidth * 0.5f + TextPadding + PlayerNameText.rectTransform.sizeDelta.x * 0.5f;
			PlayerNameText.rectTransform.localPosition = nextPosition;
		}
	}
}
