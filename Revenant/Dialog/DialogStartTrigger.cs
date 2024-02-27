using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogStartTrigger : MonoBehaviour
{
    [field: SerializeField] public DialogSequence p_DialogSequence;
    [field: SerializeField] public SpriteRenderer PressUI;


	private void OnTriggerStay2D(Collider2D collision)
    {
        if(!p_DialogSequence.isDialogStart && Input.GetKeyDown(KeyCode.F))
        {
            p_DialogSequence.isDialogStart = true;
            PressUI.enabled = false;
            gameObject.SetActive(false);
		}
    }
}
