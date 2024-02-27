using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class Enemy : Unit
{
    [Header("보스 스킬 변수")]
    public float skillCoolTime;
    public float skillRangeWidth;
    public float skillRangeHeight;
    public float skillValue;

    public float skillTimer { get; set; }
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        skillTimer = 0;
        transform.GetChild(0).GetComponent<SkeletonAnimation>().state.Event += AnimationSatateOnEvent;
        Init();
    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null)
        GameManager.Instance.enemiesList.Remove(gameObject);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (currentHp <= 0)
        {
            StartCoroutine(Die());
            return;
        }

        if(GameManager.Instance.state.Equals(State.BATTLE))
        skillTimer += Time.deltaTime * GameManager.Instance.gameSpeed;
    }

    public void SkillRangeAttack()
    {
        Vector3 box = new Vector3(skillRangeWidth, 1, skillRangeHeight);
        Vector3 center = transform.position;
        center.x = transform.position.x + skillRangeWidth / 2;
        Collider[] targets = Physics.OverlapBox(center, box, Quaternion.identity);

        foreach (var e in targets)
        {
            if (!e.transform.parent.tag.Equals("Ally")) continue;

            e.transform.parent.GetComponent<Object>().Deal(currentAtk * skillValue / 100);

        }
    }

    void MeleeAttack()
    {
        target.GetComponent<Object>().Deal(currentAtk);
    }

    void HitScanAttack()
    {
        target.GetComponent<Object>().Deal(currentAtk);
    }

    public void MeleeRangeAttack()
    {
        Vector3 box = new Vector3(attackRangeDistance, 1, attackRange2);
        Vector3 center = transform.position;
        center.x = transform.position.x + attackRangeDistance / 2;
        Collider[] targets = Physics.OverlapBox(center, box, Quaternion.identity);

        foreach (var e in targets)
        {
            if (!e.transform.parent.tag.Equals("Ally")) continue;

            e.transform.parent.GetComponent<Object>().Deal(currentAtk);

        }
    }

    public void HealRangeAttack()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, attackRangeDistance);

        foreach (var e in targets)
        {
            if (!e.transform.tag.Equals("Enemy")
                || e.transform.parent .GetComponent<Object>().currentHp <= 0
                || e.transform.parent. GetComponent<Object>().currentHp >= e.transform.parent. GetComponent<Object>().maxHp) continue;



            e.transform.parent.GetComponent<Unit>().Deal(-currentAtk);

            EffectManager.Instance.InstantiateHomingEffect("heal", e.transform.parent.gameObject, 2);
        }
    }


    public void AnimationSatateOnEvent(TrackEntry trackEntry, Event e)
    {
        if (e.Data.Name == "shoot" && transform.GetChild(0).GetComponent<SkeletonAnimation>().AnimationName == skinName + "/skill")
        {
            Debug.Log("스킬");
            SkillRangeAttack();
        }


        if (target == null)
        {
            return;
        }

        if (e.Data.Name == "shoot" && transform.GetChild(0).GetComponent<SkeletonAnimation>().AnimationName == skinName + "/attack")
        {
            if(target != GameManager.Instance.turret)
            target.GetComponent<UnitStateMachine>().SetAttackTargetInRange(gameObject);

            switch(attackType)
            {
                case AttackType.Melee:
                    MeleeAttack();
                    break;
                case AttackType.HitScan:
                    HitScanAttack();
                    break;
                case AttackType.MeleeRange:
                    MeleeRangeAttack();
                    break;
                case AttackType.HealRange:
                    HealRangeAttack();
                    break;
            }
        }
    }
}
