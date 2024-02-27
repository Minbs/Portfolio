using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class DialogueSpreadSheet : MonoBehaviour
{
    public Dictionary<string, string> CommentDictionary = new Dictionary<string, string>();
    public Dictionary<string, string> QuestionDictionary = new Dictionary<string, string>();
    public Dictionary<string, string> AnswerDictionary = new Dictionary<string, string>();
    public Dictionary<string, string> QnADictionary = new Dictionary<string, string>();

    const string QuestionSheetData = "https://docs.google.com/spreadsheets/d/1zOlEWw_NEak1vjNgrcpti2WYr0hQc6AsdeWKA81Y5rI/export?format=tsv";
    const string CommentSheetData = "https://docs.google.com/spreadsheets/d/1zOlEWw_NEak1vjNgrcpti2WYr0hQc6AsdeWKA81Y5rI/export?format=tsv&gid=974100280";

	public static bool dataDownloaded = false;
    public TextMeshProUGUI PlayerDialogue, GuestDialogue;
    
    IEnumerator Start()
    {
		UnityWebRequest www2 = UnityWebRequest.Get(CommentSheetData);
		yield return www2.SendWebRequest();
		string data2 = www2.downloadHandler.text;
		LoadCommentData(data2);

		UnityWebRequest www = UnityWebRequest.Get(QuestionSheetData);
        yield return www.SendWebRequest();
        string data = www.downloadHandler.text;
        DataApplyToDIC(data);
	}

    private void DataApplyToDIC(string data)
    {
		string[] row = data.Split('\n');
        int rowSize = row.Length;

        for (int i = 1; i < rowSize; i++)
        {
            string[] cell = row[i].Split('\t');

            if (cell[0] == "")
                continue;

            QuestionDictionary.Add(cell[0], cell[1]);

            if (cell[2] != "" && cell[3] != "")
            {
                AnswerDictionary.Add(cell[2], cell[3]);
            }

            QnADictionary.Add(cell[0], cell[2]);
        }

        Debug.Log("1 로드 끝");
    }

    private void LoadCommentData(string data)
    {
		string[] row = data.Split('\n');
		int rowSize = row.Length;

		for (int i = 1; i < rowSize; i++)
		{
			string[] cell = row[i].Split('\t');

			if (cell[0] != "" && cell[1] != "")
			{
				CommentDictionary.Add(cell[0], cell[1]);
			}
		}

		Debug.Log("2 로드 끝");
	}
}
