using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialDroneObject : TutorialObject
{
    public GameObject player;
	public float p_OffsetX;
	public float p_OffsetY;
	public float p_FollowSpeed = 2;
	public TutorialDialog p_TutorialDialog;
	private Rigidbody2D rigidbody;

    protected override void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
		rigidbody = GetComponent<Rigidbody2D>();
		Vector3 startPosition = transform.position;
		startPosition.y = player.transform.position.y + p_OffsetY;
		transform.position = startPosition;
	}

    void FixedUpdate()
    {
		FollowCharacter();
	}

    /// <summary>
    /// 플레이어를 따라갑니다.
    /// </summary>
    public void FollowCharacter()
    {
        Vector2 targetPosition = player.transform.position;
		targetPosition.y = player.transform.position.y + p_OffsetY;

		if (player.transform.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
			targetPosition.x = player.transform.position.x + p_OffsetX;
		}
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
			targetPosition.x = player.transform.position.x - p_OffsetX;
		}

		if (Mathf.Abs(player.transform.position.x - transform.position.x) > p_OffsetX)
		{
			Vector2 moveVector = targetPosition - (Vector2)transform.position;
			moveVector.Normalize();
			rigidbody.AddForce(moveVector * p_FollowSpeed, ForceMode2D.Force);
		}
	}
}
