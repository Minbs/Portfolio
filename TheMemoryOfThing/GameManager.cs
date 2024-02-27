using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using System.IO;

public enum GameState
{
	None,
	Comment,
	Question,
	GuessSuccess,
	GameOver,
	GameEnd
}

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public GameState gameState = GameState.None;
	private DialogueSpreadSheet spreadsheet;

	public TextMeshProUGUI[] BTN_Choice;
	public TextMeshProUGUI PlayerTXT, GuestTXT;

	private List<CommentData> totalCommentDatas = new();
	private List<CommentData> currentCommentList = new();

	#region 랜덤 질문 생성 변수
	private List<QuestionData> questionDatas = new List<QuestionData>();
	private List<QuestionData> randomQuestionDatas = new List<QuestionData>();
	private List<int> questionSet = new();
	#endregion

	[field: SerializeField] public GameObject ChooseButton { get; set; }

	private List<AnswerData> answerDatas = new List<AnswerData>();
	public Image GuestIMG;
	public GameObject ChangeThingEffect;

	[field: SerializeField] public List<GuestData> GuestsData = new();
	[field: SerializeField]  public List<string> StageGuestName = new List<string>() { "Guitar", "Fire", "Bedding", "Laptop"};
	[field: SerializeField]  public List<string> StageGuessName = new List<string>() { "기타", "소화기", "이불", "노트북" };
	public List<float> StagesTimerData = new();

	public int currentStageCount = 0;
	public float currentStageTimer = 0;

	[field : SerializeField] public Slider typingSpeedSlider { get; set; }
	private void Awake()
	{
		if (!instance)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	private void Start()
	{
		spreadsheet = FindObjectOfType<DialogueSpreadSheet>();
	}

	private void Update()
	{
		commentTypingSpeed = 5 + typingSpeedSlider.value * 20;

		switch (gameState)
		{
			case GameState.None:
				break;
			case GameState.Comment:
				OnCommentState();
				OnOption();
				break;
			case GameState.Question:
				OnQuestionState();
				OnOption();
                break;
			case GameState.GuessSuccess:
				OnGuessSuccessState();
				OnOption();
                break;
			case GameState.GameOver:
				OnGameOverState();
				break;
			case GameState.GameEnd:
				OnGameEndState();
				break;
 		}

	}

	public void ChangeGameStateToCommentState() => gameState = GameState.Comment;

	public void Init()
	{
		questionDatas.Clear();
		answerDatas.Clear();
		totalCommentDatas.Clear();
		currentCommentList.Clear();
		randomQuestionDatas.Clear();
		questionSet.Clear();

		// 질문 데이터 생성
		foreach (string key in spreadsheet.QuestionDictionary.Keys)
		{
			QuestionData questionData = new QuestionData();
			questionData.guest = key.Split("_")[1];
			questionData.set = int.Parse(key.Split('_')[2].Substring(3));
			questionData.index = int.Parse(key.Split('_')[3]);
			spreadsheet.QuestionDictionary.TryGetValue(key, out questionData.Data);
			questionDatas.Add(questionData);
		}

		foreach (var key in spreadsheet.AnswerDictionary.Keys)
		{
			AnswerData answerData = new AnswerData();
			answerData.guest = key.Split("_")[1];
			answerData.index = int.Parse(key.Split('_')[2]);
			answerData.expression = key.Split('_')[3];
			spreadsheet.AnswerDictionary.TryGetValue(key, out answerData.Data);
			answerDatas.Add(answerData);
		}

		foreach (var key in spreadsheet.CommentDictionary.Keys)
		{
			CommentData comment = new CommentData();
			comment.guest = key.Split("_")[1];
			comment.index = int.Parse(key.Split('_')[2]);
			comment.speaker = key.Split('_')[3];
			spreadsheet.CommentDictionary.TryGetValue(key, out comment.Data);
			totalCommentDatas.Add(comment);
		}

		int length = 10;
		for (int i = 0; i < length; i++)
		{
			questionSet.Add(i);
		}

		commentTimer = 0;
		gameState = GameState.None;
		currentStageTimer = StagesTimerData[currentStageCount];
		SaveStageCount(currentStageCount);
		GuestIMG.sprite = GuestsData[currentStageCount].GetExpressionSprite("Normal");
		Rect rect = new Rect(GuestsData[currentStageCount].GetExpressionSprite("Normal").rect);
		GuestIMG.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, rect.height);
		GuestIMG.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
	}

	private const string filePath = "save.txt";
	public void SaveStageCount(int count)
	{
		StreamWriter sw = new StreamWriter(filePath);
		sw.Write(count);
		sw.Flush();
		sw.Close();
	}

	public int LoadStageCount() 
	{
		StreamReader sr = new StreamReader(filePath);

		string input = "";
		while (true) 
		{
			input = sr.ReadLine();

			if (input == null) { break; }
			else
			{
				sr.Close();
				return int.Parse(input);
			}
		}

		return -999;
	}

	/// <summary>
	/// 무작위 질문 3개를 생성하는 함수입니다.
	/// </summary>
	public void GenerateRamdomQuestion()
	{
		if (questionSet.Count <= 0)
		{
			return;
		}

		randomQuestionDatas.Clear();


		var random = new System.Random();
		int index = random.Next(questionSet.Count);

		List<QuestionData> questions =
			(from question in questionDatas
			 where question.set == questionSet[index]
			 && question.guest == StageGuestName[currentStageCount]
			 select question).ToList();

		questionSet.Remove(questionSet[index]);
		randomQuestionDatas = questions;

		int count = 0;
		foreach (var a in randomQuestionDatas)
		{
			BTN_Choice[count].text = a.Data;
			count++;
		}
	}
	//----------------------------------------------------------------------------------------
	#region 버튼함수
	public void BTNChoose(int index)
	{
		BTNRelay(index);
	}

	void BTNRelay(int a)
	{
		string akey = spreadsheet.QuestionDictionary.FirstOrDefault(x => x.Value == BTN_Choice[a].text).Key;
		string bkey = spreadsheet.QnADictionary[akey];
		string bvalue = spreadsheet.AnswerDictionary[bkey];

		GuestTXT.text = bvalue;
		GuestIMG.sprite = GuestsData[currentStageCount].GetExpressionSprite(answerDatas.Find(i => i.Data == bvalue).expression);

		DialogueHistory.instance.WriteHistoryText(BTN_Choice[a].text);
		DialogueHistory.instance.WriteHistoryText(" -> " + bvalue);
		GenerateRamdomQuestion();
	}
	#endregion
	//----------------------------------------------------------------------------------------

	public void PlayerTextStart()
	{
		currentCommentList.Clear();

		Debug.Log(StageGuessName[currentStageCount]);
		currentCommentList = (from comment in totalCommentDatas
							  where comment.guest == StageGuestName[currentStageCount]
							  && comment.index >= 0
							  && comment.index < 100
							  select comment).ToList();
	}

	private float commentTimer = 0;
	private float commentTypingSpeed = 15;

	public void SetCommentTypingSpeed(float value)
	{
		commentTypingSpeed = 5 + value * 20;
	}

	public void OnCommentState()
	{
		if (currentCommentList.Count > 0)
		{
			if(commentTimer < currentCommentList[0].Data.Length - 1)
			{
				commentTimer += Time.deltaTime * commentTypingSpeed;
			}
			else
			{
				commentTimer = currentCommentList[0].Data.Length;
			}


			if (currentCommentList[0].speaker == "Player")
			{
				if (PlayerTXT.text != currentCommentList[0].Data.Substring(0, (int)commentTimer))
				{
					Debug.Log((int)commentTimer);
					if (commentTimer >= 1 && currentCommentList[0].Data[(int)commentTimer - 1] == '<')
					{
						while (currentCommentList[0].Data[(int)commentTimer - 1] != '>')
						{
							commentTimer++;
						}
					}
					PlayerTXT.text = currentCommentList[0].Data.Substring(0, (int)commentTimer);
					SoundController.Instance.PlaySoundEffect("TextTyping");
				}
			}
			else
			{
				if (GuestTXT.text != currentCommentList[0].Data.Substring(0, (int)commentTimer))
				{
					if (commentTimer >= 1 &&  currentCommentList[0].Data[(int)commentTimer - 1] == '<')
					{
						while (currentCommentList[0].Data[(int)commentTimer - 1] != '>')
						{
							commentTimer++;
						}
					}
					GuestTXT.text = currentCommentList[0].Data.Substring(0, (int)commentTimer);
					SoundController.Instance.PlaySoundEffect("TextTyping");
				}
			}

			if (Input.GetMouseButtonDown(0) && !Canvas_Option.activeSelf)
			{
				if(commentTimer < currentCommentList[0].Data.Length)
				{
					commentTimer = currentCommentList[0].Data.Length;
				}
				else
				{
					commentTimer = 0;
					currentCommentList.RemoveAt(0);
				}
			}
		}
		else
		{
			PlayerTXT.text = "";
			GuestTXT.text = "";
			ChooseButton.SetActive(true);
			GenerateRamdomQuestion();
			gameState = GameState.Question;

		}
	}

	public GameObject Canvas_Counceling, Canvas_AnswerWindow, Canvas_Option;
	public Slider Slider;
	public TMP_InputField guessText;

	public void OnQuestionState()
	{
		currentStageTimer -= Time.deltaTime;



		if(currentStageTimer <= 0)
		{
			gameState = GameState.GameOver;
			currentCommentList = (from comment in totalCommentDatas
								  where comment.guest == StageGuestName[currentStageCount]
								  && comment.index == -2
								  select comment).ToList();
			commentTimer = 0;
			Canvas_AnswerWindow.gameObject.SetActive(false);
			ChooseButton.SetActive(false);
		}
		else
		{
			if (questionSet.Count == 0)
			{
				Canvas_AnswerWindow.gameObject.SetActive(true);
				return;
			}

			if (Input.GetKeyDown(KeyCode.Tab) && !Canvas_Option.activeSelf)
			{
				if (Canvas_AnswerWindow.gameObject.activeSelf == false)
				{
					Canvas_AnswerWindow.gameObject.SetActive(true);
				}
				else
				{
					Canvas_AnswerWindow.gameObject.SetActive(false);
				}
			}
		}
	}

	public void OnGameOverState()
	{
		if (commentTimer < currentCommentList[0].Data.Length - 1)
		{
			commentTimer += Time.deltaTime * commentTypingSpeed;
		}
		else
		{
			commentTimer = currentCommentList[0].Data.Length;
		}

		if (GuestTXT.text != currentCommentList[0].Data.Substring(0, (int)commentTimer))
		{
			if (commentTimer >= 1 && currentCommentList[0].Data[(int)commentTimer - 1] == '<')
			{
				while (currentCommentList[0].Data[(int)commentTimer - 1] != '>')
				{
					commentTimer++;
				}
			}
			GuestTXT.text = currentCommentList[0].Data.Substring(0, (int)commentTimer);
			SoundController.Instance.PlaySoundEffect("TextTyping");
		}

		if (Input.GetMouseButtonDown(0) && !Canvas_Option.activeSelf)
		{
			if (commentTimer < currentCommentList[0].Data.Length)
			{
				commentTimer = currentCommentList[0].Data.Length;
			}
			else
			{
				commentTimer = 0;
				currentCommentList.RemoveAt(0);
				StartCoroutine(GameOverFade());
				gameState = GameState.None;
			}
		}
	}

	public IEnumerator GameOverFade()
	{
		FadeInOut.instance.FadeAnimator.Play("GameOverFade");
		yield return null;
		while (FadeInOut.instance.FadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
		{
			if(Canvas_MainMenu.activeSelf == false && FadeInOut.instance.FadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
			{
				Canvas_MainMenu.SetActive(true);
				Canvas_Counseling.SetActive(false);
				GuestFade.Instance.GuestAnimator.SetTrigger("GoDefault");
				GuestIMG.color = new Color(GuestIMG.color.r, GuestIMG.color.g, GuestIMG.color.b, 0);
			}

			yield return null;
		}


		SaveStageCount(currentStageCount);
		currentStageCount = 0;
		Init();
	}

	public void OnGameEndState()
	{
		;
	}

	public void OnOption()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (Canvas_Option.gameObject.activeSelf == false)
			{
				Canvas_Option.gameObject.SetActive(true);
			}
			else
			{
				Canvas_Option.gameObject.SetActive(false);
			}
		}
	}

	public void GuessButtonDown()
	{
		Canvas_AnswerWindow.gameObject.SetActive(false);
		string input = guessText.text.ToString();

		guessText.text = "";

		foreach (var item in StageGuessName)
		{
			Debug.Log(item);
		}

		Debug.Log(StageGuessName[currentStageCount]);
		if (input == StageGuessName[currentStageCount])
		{
			currentCommentList = (from comment in totalCommentDatas
								  where comment.guest == StageGuestName[currentStageCount]
								  && comment.index >= 100
								  select comment).ToList();

			gameState = GameState.GuessSuccess;
			ChooseButton.SetActive(false);
			PlayerTXT.text = $"당신은 '{input}'이에요";
			GuestIMG.sprite = GuestsData[currentStageCount].GetOriginalObjectSprite();
            Rect rect = new Rect(GuestsData[currentStageCount].GetOriginalObjectSprite().rect);
            GuestIMG.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, rect.height);
			GuestIMG.GetComponent<RectTransform>().localScale = new Vector2(0.7f, 0.7f);

            ChangeThingEffect.GetComponent<Animator>().Play("ChangeThings", ChangeThingEffect.GetComponent<Animator>().GetLayerIndex("Base Layer"), 0);
		}
		else
		{
			OnGuessFailure(input);
		}
	}

	public void OnGuessSuccessState()
	{
		if (currentCommentList.Count > 0)
		{
			if (commentTimer < currentCommentList[0].Data.Length - 1)
			{
				commentTimer += Time.deltaTime * commentTypingSpeed;
			}
			else
			{
				commentTimer = currentCommentList[0].Data.Length;
			}


			if (currentCommentList[0].speaker == "Player")
			{
				if (PlayerTXT.text != currentCommentList[0].Data.Substring(0, (int)commentTimer))
				{
					Debug.Log((int)commentTimer);
					if (commentTimer >= 1 && currentCommentList[0].Data[(int)commentTimer - 1] == '<')
					{
						while (currentCommentList[0].Data[(int)commentTimer - 1] != '>')
						{
							commentTimer++;
						}
					}
					PlayerTXT.text = currentCommentList[0].Data.Substring(0, (int)commentTimer);
					SoundController.Instance.PlaySoundEffect("TextTyping");
				}
			}
			else
			{
				if (GuestTXT.text != currentCommentList[0].Data.Substring(0, (int)commentTimer))
				{
					if (commentTimer >= 1 && currentCommentList[0].Data[(int)commentTimer - 1] == '<')
					{
						while (currentCommentList[0].Data[(int)commentTimer - 1] != '>')
						{
							commentTimer++;
						}
					}
					GuestTXT.text = currentCommentList[0].Data.Substring(0, (int)commentTimer);
					SoundController.Instance.PlaySoundEffect("TextTyping");
				}
			}

			if (Input.GetMouseButtonDown(0) && !Canvas_Option.activeSelf)
			{

				if (commentTimer < currentCommentList[0].Data.Length)
				{
					commentTimer = currentCommentList[0].Data.Length;
				}
				else
				{
					commentTimer = 0;
					currentCommentList.RemoveAt(0);
				}
			}
		}
		else
		{
			PlayerTXT.text = "";
			GuestTXT.text = "";
			Debug.Log("스테이지 종료");
            StartCoroutine(EnterNextLevel());
		}
	}

	public TextMeshProUGUI EndText;
	public GameObject Canvas_MainMenu;
	public GameObject Canvas_Counseling;
	public IEnumerator EndFade()
	{
		FadeInOut.instance.FadeAnimator.Play("GameEndFadeIn");
		yield return null;
		while (FadeInOut.instance.FadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
		{
			yield return null;
		}

		Canvas_MainMenu.SetActive(true);
		Canvas_Counseling.SetActive(false);

		FadeInOut.instance.FadeAnimator.Play("GameEndFadeOut");
		while (FadeInOut.instance.FadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
		{
			EndText.alpha = FadeInOut.instance.FadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			yield return null;
		}

		currentStageCount = 0;
		SaveStageCount(currentStageCount);
		Init();
	}


	public IEnumerator EnterNextLevel()
	{
		GuestFade.Instance.GuestAnimator.SetTrigger("GoDefault");
        

        DialogueHistory.instance.Clear();
		FadeInOut.instance.FadeAnimator.Play("FadeOut");

		int length = 10;
		for (int i = 0; i < length; i++)
		{
			questionSet.Add(i);
		}

		currentStageCount++;
		Debug.Log(currentStageCount);
		if(currentStageCount >= StageGuestName.Count)
		{
			Debug.Log("스테이지 끝");
			gameState = GameState.GameEnd;
			StartCoroutine(EndFade());
			yield break;
		}
		else
		{
			Debug.Log("다음 스테이지");
			SaveStageCount(currentStageCount);
			currentStageTimer = StagesTimerData[currentStageCount];
			GuestIMG.sprite = GuestsData[currentStageCount].GetExpressionSprite("Normal");
            Rect rect = new Rect(GuestsData[currentStageCount].GetExpressionSprite("Normal").rect);
			GuestIMG.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, rect.height);
            GuestIMG.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
            PlayerTextStart();
		}
	}


	public void OnGuessFailure(string input)
	{
		currentStageTimer -= StagesTimerData[currentStageCount] / 10;
		GuestTXT.text = totalCommentDatas.Find(i => i.guest == StageGuestName[currentStageCount] && i.index == -1).Data.ToString();
		GuestTXT.text = GuestTXT.text.Replace("{KEY_INPUT_NAME}", input);
	}
}

