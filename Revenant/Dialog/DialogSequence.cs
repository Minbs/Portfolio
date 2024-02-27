using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSequence : MonoBehaviour
{
    //public scene
    public SceneLoader_Signal p_SceneLoader;
    
    public bool isDialogStart = false;
    public int DialogCount = 0;
    public Vector2 PlayerDialogPosition;
    private DialogBox currentBox;
    [field: SerializeField] public GameObject Hologram;

    private GameObject m_Player;
    private void Start()
    {
        if (FindObjectOfType<Player>())
            m_Player = FindObjectOfType<Player>().gameObject;

        GameMgr.GetInstance().p_PlayerInputMgr.p_FireLock = true;
    }

    public void StartDialog() => isDialogStart = true;
    // Update is called once per frame
    void Update()
    {
        if (!isDialogStart)
            return;

        if(DialogCount < transform.childCount)
        { 
            if(DialogCount == 0)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                currentBox = transform.GetChild(0).GetComponent<DialogBox>();
                if(Hologram != null)
                    Hologram.SetActive(true);
            }

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
            {
                currentBox.SkipEvent?.Invoke();
                if (currentBox.isTextEnd)
                {
                    currentBox.gameObject.SetActive(false);
                    DialogCount++;
                    if(DialogCount < transform.childCount)
                    {
                        transform.GetChild(DialogCount).gameObject.SetActive(true);
                        currentBox = transform.GetChild(DialogCount).GetComponent<DialogBox>();
                        if(currentBox.isOnPlayerPosition && m_Player != null)
                        {
                            currentBox.GetComponent<RectTransform>().anchoredPosition = m_Player.transform.position + (Vector3)PlayerDialogPosition;
                        }
                    }
                    else // 대화 끝
                    {
                        if (Hologram != null)
                            Hologram.SetActive(false);

                        if (!ReferenceEquals(p_SceneLoader, null))
                        {
                            p_SceneLoader.LoadScene();
                        }
                    }
                }
            }
        }
    }
}