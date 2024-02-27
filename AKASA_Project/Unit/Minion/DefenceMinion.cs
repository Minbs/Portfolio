using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Spine.Unity;
using Spine;
using Event = Spine.Event;
using UnityEditor;
using System.Diagnostics;

public enum MinionClass
{
	Buster,
	Guardian,
	Chaser,
	Rescue
}

public enum AttackType
{
	Bullet,
	Melee,
	MeleeRange,
	SingleHeal,
	HealRange,
	HitScan,
	HitScanRange
}

public enum SkillType
{
	Attack,
	Defence,
	Buff
}

public class DefenceMinion : Unit
{
	[Header("MinionStat")]

	public float cost;
	public float sellCost;
	public GameObject UImanager;
	public float skillTimer { get; set; }
	public float skillCoolTime;

	public Sprite bulletSprite;

	public float healAmountRate { get; set; }

	public GameObject shootPivot;

	public SkillType skillType;
	public MinionClass minionClass;


	private void Awake()
	{
		base.Awake();
	}
	public void resetUnitCard()
	{
		UImanager.GetComponent<Unit_Select_UI>().Display_Unit_Button(this.GetComponent<DefenceMinion>().Unitname);
	}
	private void OnDestroy()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.minionsList.Remove(gameObject);
		}
	}

	protected override void Start()
	{
		base.Start();
		transform.GetChild(0).GetComponent<SkeletonAnimation>().state.Event += AnimationSatateOnEvent;
		healAmountRate = 100;
		skillTimer = skillCoolTime;
	}

	public void AnimationSatateOnEvent(TrackEntry trackEntry, Event e)
	{
		if (e.Data.Name == "shoot" && (transform.GetChild(0).GetComponent<SkeletonAnimation>().AnimationName == skinName + "/skill"
			|| transform.GetChild(0).GetComponent<SkeletonAnimation>().AnimationName == skinName + "/skill1"
			|| transform.GetChild(0).GetComponent<SkeletonAnimation>().AnimationName == skinName + "/skill2"
			|| transform.GetChild(0).GetComponent<SkeletonAnimation>().AnimationName == skinName + "/skill3"))
		{
			SkillManager.Instance.MinionSkillEvent(Unitname);
		}


		if (e.Data.Name == "charge")
		{
			EffectManager.Instance.InstantiateAttackEffect("verity_charge", transform.position);
		}


		if (target == null)
		{
			return;
		}



		if (e.Data.Name == "shoot" && transform.GetChild(0).GetComponent<SkeletonAnimation>().AnimationName == skinName + "/attack")
		{
			switch (attackType)
			{
				case AttackType.Bullet:
					BulletAttack();
					break;
				case AttackType.Melee:
					MeleeAttack();
					break;
				case AttackType.MeleeRange:
					MeleeRangeAttack();
					break;
				case AttackType.SingleHeal:
					SingleHeal();
					break;
				case AttackType.HitScan:
					HitScanAttack();
					break;
				case AttackType.HitScanRange:
					HitScanRangeAttack();
					break;
			}
		}
	}

	private void OnMouseOver()
	{
		if (GameManager.Instance.state == State.WAIT && GameManager.Instance.deployState == DeployState.NONE)
		{
			if (Input.GetMouseButtonUp(1))
			{
				BattleUIManager.Instance.SetMinionUpgradeUI(gameObject);
			}
		}
	}

	private void OnMouseUp()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit raycastHit))
		{

			if (raycastHit.collider.transform.tag == "UI")
			{
				Debug.Log(raycastHit.collider.transform.tag);
				return;
			}
		}

		if (GameManager.Instance.state == State.WAIT && GameManager.Instance.deployState == DeployState.NONE)
		{
			GameManager.Instance.minionChangePos(gameObject);
		}
	}

	#region BaseAttack
	public void HitScanAttack()
	{
		if (target == null)
		{
			return;
		}

		target.GetComponent<Unit>().Deal(currentAtk);
	}

	public void HitScanRangeAttack()
	{
		Collider[] targets = Physics.OverlapSphere(target.transform.position, attackRange2);

		foreach (var e in targets)
		{
			if (!e.transform.tag.Equals("Enemy")) continue;
			Debug.Log(e.transform.parent.name);
			e.transform.parent.GetComponent<Unit>().Deal(currentAtk);

		}

		EffectManager.Instance.InstantiateAttackEffect("hwaseon_hit", target.transform.position);
	}

	public void SingleHeal()
	{
		Vector3 pos = transform.position;
		GameObject bulletObject = ObjectPool.Instance.PopFromPool("Bullet");
		bulletObject.GetComponent<SpriteRenderer>().sprite = bulletSprite;
		bulletObject.transform.position = pos;
		bulletObject.GetComponent<Bullet>().Init(-(int)(currentAtk * (healAmountRate / 100)), target);

		bulletObject.SetActive(true);
	}

	public void MeleeAttack()
	{
		target.GetComponent<Unit>().Deal(currentAtk);
	}

	public void MeleeRangeAttack()
	{
		Debug.Log(attackRange2);
		Vector3 box = new Vector3(attackRangeDistance, 1, attackRange2);
		Vector3 center = transform.position;
		center.x = transform.position.x + attackRangeDistance / 2;
		Collider[] targets = Physics.OverlapBox(center, box, Quaternion.identity);

		foreach (var e in targets)
		{
			if (!e.transform.tag.Equals("Enemy")) continue;
			Debug.Log(e.transform.parent.name);
			e.transform.parent.GetComponent<Unit>().Deal(currentAtk);

		}
	}

	public void BulletAttack()
	{
		Vector3 pos = shootPivot.transform.position;
		GameObject bulletObject = ObjectPool.Instance.PopFromPool("Bullet");
		bulletObject.GetComponent<SpriteRenderer>().sprite = bulletSprite;
		bulletObject.transform.position = pos;
		bulletObject.GetComponent<Bullet>().Init(currentAtk, target);

		bulletObject.SetActive(true);
	}
	#endregion

	// Update is called once per frame
	protected override void Update()
	{
		skillTimer += Time.deltaTime * GameManager.Instance.gameSpeed;

		base.Update();
	}
}

