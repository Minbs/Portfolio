using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Spine.Unity;
enum SkilRangeType
{
    Circle,
    Rectangle
}

enum SkillAimType
{
    Single,
    Circle,
    Auto
}

public class SkillManager : Singleton<SkillManager>
{
    Ray ray;
    RaycastHit hit;

    public float skillAimTimeSpeed;

    private GameObject skillUnit; // 스킬을 사용중인 유닛

    public bool isSkillActing = false;

    // 스킬 UI 변수
    public GameObject skillBackgroundImage;
    public GameObject skillCircleRangeUI;

    public GameObject skillAimCircleRangeUI;


    public GameObject poisonMist;
    public GameObject healDrone;
    public GameObject verityShot;

    private Vector3 skillHitpos;

    private bool isSkillAimEnd = false;

    private List<GameObject> skillTargets = new List<GameObject>();

    public GameObject skillCutScene;

    [Header("페이 스킬")]
    public float paySkillValue;
    public float paySkillDuration;

    [Header("소피아 스킬")]
    public float sophiaSkillValue;
    public float sophiaSkillDuration;

    [Header("화선 스킬")]
    public float hwaseonSkillValue;
    public float hwaseonSkillDuration;
    public float hwaseonSkillAimRange;

    [Header("베리티 스킬")]
    public float veritySkillValue;
    public float veritySkillAimRange;

    [Header("파르도 스킬")]
    public float pardoSkillValue;
    public float pardoSkillAimRange;
    public float pardoSkillRange;
    public float pardoSkillDuration;

    [Header("어셔 스킬")]
    public float asherSkillAimRange;
    public float asherSkillRange;
    public float asherSkillDuration;

    [Header("보그 스킬")]
    public float vogueSkillValue;
    public float vogueSkillAimRange;
    public float vogueSkillRange;

    [Header("레이스 스킬")]
    public float wraithSkillValue;
    public float wraithSkillRangeWidth;
    public float wraithSkillRangeHeight;

    [Header("이자벨라 스킬")]
    public float isabellaSkillValue;
    public float isabellaSkillRange;

    [Header("지포 스킬")]
    public float zippoSkillValue;
    public float zippoSkillRangeWidth;
    public float zippoSkillRangeHeight;

    [Header("코우엔 스킬")]
    public float kuenSkillValue;
    public float kuenSkillRangeWidth;
    public float kuenSkillRangeHeight;

    [Header("에레메디움 스킬")]
    public float eremediumSkillValue;
    public float eremediumSkillDuration;

    void Start()
    {
        skillUnit = null;
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    public void UseCharacterSkill(int index)
    {
        if (index > GameManager.Instance.minionsList.Count - 1 || skillBackgroundImage.activeSelf
            || GameManager.Instance.state != State.BATTLE)
            return;

        string minionName;

        skillUnit = GameManager.Instance.minionsList[index];

          if (skillUnit.GetComponent<DefenceMinion>().skillTimer < skillUnit.GetComponent<DefenceMinion>().skillCoolTime
            || skillUnit.GetComponent<Unit>().currentHp <= 0)
           {
              return;
           }
       
        minionName = skillUnit.GetComponent<DefenceMinion>().Unitname;
        isSkillActing = true;
        skillBackgroundImage.SetActive(true);
        skillTargets.Clear();

        Debug.Log(minionName + "Skill");
        StartCoroutine(minionName + "Skill");
    }

    private List<GameObject> AimSkillTargetsInRange(SkilRangeType skilRangeType, SkillAimType skillAimType, string unitType = "Minion", float range = 0, float range2 = 0)
    {
        List<GameObject> targetsList = null;
        GameManager.Instance.SetGameSpeed(skillAimTimeSpeed);

        List<GameObject> returnTargets = new List<GameObject>();

        if (unitType.Equals("Minion"))
        {
            targetsList = GameManager.Instance.minionsList;
        }
        else if (unitType.Equals("Enemy"))
        {
            targetsList = GameManager.Instance.enemiesList;
        }

        foreach (var target in targetsList)
        {
            target.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -1;
            target.GetComponent<Unit>().SetAimUnitColor(false);
        }

        if (skilRangeType.Equals(SkilRangeType.Circle))
        {
            skillCircleRangeUI.SetActive(true);
            skillCircleRangeUI.transform.localScale = new Vector3(2, 2, 2) * range;
            skillCircleRangeUI.transform.position = skillUnit.transform.position;


            Collider[] colliders = Physics.OverlapSphere(skillUnit.transform.position, range);


                foreach (var target in targetsList)
                {
                    foreach (var col in colliders)
                    {
                        if (!col.transform.parent.GetComponent<Unit>())
                            continue;

                        if (col.transform.parent.gameObject.Equals(target)
                            && col.transform.parent.GetComponent<Object>().currentHp > 0)
                        {
                            col.GetComponent<MeshRenderer>().sortingOrder = 1;
                        }
                    }
                }
        }
        else if (skilRangeType.Equals(SkilRangeType.Rectangle))
        {
            Vector3 center = skillUnit.transform.position;
            center.x =  skillUnit.transform.position.x + range / 2;

            Collider[] colliders = Physics.OverlapBox(center, box, skillUnit.transform.rotation);


            foreach (var target in targetsList)
            {
                foreach (var col in colliders)
                {
                    if (!col.transform.parent.GetComponent<Unit>())
                        continue;

                    if (col.transform.parent.gameObject.Equals(target)
                        && col.transform.parent.GetComponent<Object>().currentHp > 0)
                    {
                        col.GetComponent<MeshRenderer>().sortingOrder = 1;
                    }
                }
            }
        }


        if (skillAimType.Equals(SkillAimType.Single))
        {
            skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -1;
            skillUnit.GetComponent<Unit>().SetAimUnitColor(false);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Object")))
            {
                if (Input.GetMouseButtonDown(0)
                    && hit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder.Equals(1))
                {
                    foreach (var target in targetsList)
                    {
                        target.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -1;
                        target.GetComponent<Unit>().SetAimUnitColor(false);
                    }

                    Debug.Log(hit.transform.name);
                    returnTargets.Add(hit.transform.gameObject);
                    skillCircleRangeUI.SetActive(false);


                }
            }
        }
        else if (skillAimType.Equals(SkillAimType.Circle))
        {
            skillAimCircleRangeUI.SetActive(true);
            skillAimCircleRangeUI.transform.localScale = new Vector3(2, 2, 2) * range2;


            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default")))
            {
                Vector3 pos1, pos2;
                pos1 = skillUnit.transform.position;
                pos2 = hit.point;

                pos1.y = skillCircleRangeUI.transform.position.y;
                pos2.y = skillCircleRangeUI.transform.position.y;

                var hitPosDir = (pos2 - pos1).normalized;
                float distance = Vector3.Distance(pos2, pos1);
                distance = Mathf.Min(distance, range);

                var hitPos = pos1 + hitPosDir * distance;
                skillAimCircleRangeUI.transform.position = hitPos;


                    Collider[] colliders = Physics.OverlapSphere(skillAimCircleRangeUI.transform.position, range2);
                    foreach (var col in colliders)
                    {
                    if (col.transform.parent.GetComponent<Unit>()
                        && col.GetComponent<MeshRenderer>().sortingOrder.Equals(1))
                    {
                        col.transform.parent.GetComponent<Unit>().SetAimUnitColor(true);

                        if (Input.GetMouseButtonDown(0))
                        {
                            returnTargets.Add(col.transform.parent.gameObject);
                            Debug.Log(col.transform.parent.gameObject);
                       
                        }
                    }
                    }

                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("끝");
                    isSkillAimEnd = true;
                    skillHitpos = skillAimCircleRangeUI.transform.position;
                    skillCircleRangeUI.SetActive(false);
                    skillAimCircleRangeUI.SetActive(false);
                }

                }

        }
        else if (skillAimType.Equals(SkillAimType.Auto))
        {
            foreach(var target in targetsList)
            {
                if (target.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder.Equals(1))
                {
                    returnTargets.Add(target);
                }
            }
        }


        return returnTargets;
        }

        IEnumerator PaySkill()
        {
            GameManager.Instance.SetGameSpeed(0);
            skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;
            skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().SkillPerformState);

        GameManager.Instance.SetGameSpeed(0);

        #region skillCutScene
        skillCutScene.SetActive(true);
        skillCutScene.GetComponent<Animator>().Play(skillUnit.GetComponent<Unit>().Unitname + "Skill", -1, 0f);
        yield return null;
        while (skillCutScene.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            yield return null;
        }

        skillCutScene.SetActive(false);
        yield return null;
        #endregion
        skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill", false, 1);
            yield return null;

            while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
            {
                yield return null;
            }

        StartCoroutine(skillUnit.GetComponent<Unit>().ChangeStat(skillUnit, "def", skillUnit.GetComponent<DefenceMinion>().def * (paySkillValue / 100), paySkillDuration));
        EffectManager.Instance.InstantiateHomingEffect("pay_effect", skillUnit, paySkillDuration);

        SkillEnd();
    }

        IEnumerator SophiaSkill()
        {

            GameManager.Instance.SetGameSpeed(0);
            skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;

        GameManager.Instance.SetGameSpeed(0);

        #region skillCutScene
        skillCutScene.SetActive(true);
        skillCutScene.GetComponent<Animator>().Play(skillUnit.GetComponent<Unit>().Unitname + "Skill", -1, 0f);
        yield return null;
        while (skillCutScene.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            yield return null;
        }

        skillCutScene.SetActive(false);
        yield return null;
        #endregion
        skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().SkillPerformState);
            skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill", false, 1);
            yield return null;

            while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
            {
                yield return null;
            }


        StartCoroutine(skillUnit.GetComponent<Unit>().ChangeStat(skillUnit, "ats", skillUnit.GetComponent<DefenceMinion>().def * (sophiaSkillValue / 100), sophiaSkillDuration));
        EffectManager.Instance.InstantiateHomingEffect("sophia_effect", skillUnit, sophiaSkillDuration);

        SkillEnd();

    }

        IEnumerator HwaseonSkill()
        {
            var targetsList = GameManager.Instance.minionsList;

            while (skillTargets.Count <= 0)
            {
                skillTargets = AimSkillTargetsInRange(SkilRangeType.Circle, SkillAimType.Single, "Minion", hwaseonSkillAimRange);
                yield return null;
            }

            foreach (var minion in targetsList)
            {
                minion.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -1;
            }

            foreach (var target in skillTargets)
            {
                target.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;
            }

            GameManager.Instance.SetGameSpeed(0);
            skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;

        GameManager.Instance.SetGameSpeed(0);

        #region skillCutScene
        skillCutScene.SetActive(true);
        skillCutScene.GetComponent<Animator>().Play(skillUnit.GetComponent<Unit>().Unitname + "Skill", -1, 0f);
        yield return null;
        while (skillCutScene.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            yield return null;
        }

        skillCutScene.SetActive(false);
        yield return null;
        #endregion
        skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().SkillPerformState);
            skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill", false, 1);

            Vector3 startPos = skillUnit.transform.position;

            while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
            {
                yield return null;
            }


            skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill2", false, 1);

            Vector3 offset;
            offset = skillTargets[0].transform.position;
            offset.x = skillTargets[0].transform.GetChild(0).localScale.x * 10 * 1.2f;

            skillUnit.transform.position = offset;
            yield return null;

            while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
            {
                yield return null;
            }



            skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill3", false, 1);
            yield return null;

            skillUnit.transform.position = startPos;

            while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
            {

                yield return null;
            }


        StartCoroutine(skillUnit.GetComponent<Unit>().ChangeStat(skillTargets[0], "atk", skillUnit.GetComponent<DefenceMinion>().currentAtk * (hwaseonSkillValue / 100), hwaseonSkillDuration));
        EffectManager.Instance.InstantiateHomingEffect("sophia_effect", skillTargets[0], hwaseonSkillDuration);
        SkillEnd();
    }

        IEnumerator VeritySkill()
        {
        var targetsList = GameManager.Instance.minionsList;

        while (skillTargets.Count <= 0)
        {
            skillTargets = AimSkillTargetsInRange(SkilRangeType.Circle, SkillAimType.Single, "Enemy", veritySkillAimRange);
            yield return null;
        }

        foreach (var minion in targetsList)
        {
            minion.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -1;
        }

        foreach (var target in skillTargets)
        {
            target.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;
        }

        GameManager.Instance.SetGameSpeed(0);


        #region skillCutScene
        skillCutScene.SetActive(true);
        skillCutScene.GetComponent<Animator>().Play(skillUnit.GetComponent<Unit>().Unitname + "Skill", -1, 0f);
        yield return null;
        while (skillCutScene.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {           
            yield return null;
        }

        skillCutScene.SetActive(false);
        yield return null;
        #endregion

        skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;
        skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().SkillPerformState);
        skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill", false, 1);
        yield return null;

   

        while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
        {
            yield return null;
        }

        SkillEnd();

    }

        IEnumerator PardoSkill()
        {
            var targetsList = GameManager.Instance.enemiesList;

            while (!isSkillAimEnd)
            {
                skillTargets = AimSkillTargetsInRange(SkilRangeType.Circle, SkillAimType.Circle, "Enemy", pardoSkillAimRange, pardoSkillRange);
                yield return null;
            }



            foreach (var minion in targetsList)
            {
                minion.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -1;
            }

            foreach (var target in skillTargets)
            {
                target.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;
                target.GetComponent<Unit>().SetAimUnitColor(false);
           }


        GameManager.Instance.SetGameSpeed(0);
            skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;

        #region skillCutScene
        skillCutScene.SetActive(true);
        skillCutScene.GetComponent<Animator>().Play(skillUnit.GetComponent<Unit>().Unitname + "Skill", -1, 0f);
        yield return null;
        while (skillCutScene.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            yield return null;
        }

        skillCutScene.SetActive(false);
        yield return null;
        #endregion


        skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().SkillPerformState);
            skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill", false, 1);
            yield return null;

        while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
        {
            yield return null;
        }


        GameObject skillObject = Instantiate(poisonMist);
        skillObject.transform.position = skillHitpos;
        skillObject.GetComponent<PoisonMist>().duration = pardoSkillDuration;
        skillObject.GetComponent<PoisonMist>().damage = skillUnit.GetComponent<Unit>().currentAtk * (pardoSkillValue / 100);
         
        Destroy(skillObject, pardoSkillDuration);

        SkillEnd();
    }

        IEnumerator AsherSkill()
    {
        var targetsList = GameManager.Instance.minionsList;

        while (skillTargets.Count <= 0)
        {
            skillTargets = AimSkillTargetsInRange(SkilRangeType.Circle, SkillAimType.Circle, "Minion", asherSkillAimRange, asherSkillRange);
            yield return null;
        }



        foreach (var minion in targetsList)
        {
            minion.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -1;
        }

        foreach (var target in skillTargets)
        {
            target.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;
            target.GetComponent<Unit>().SetAimUnitColor(false);
        }


        GameManager.Instance.SetGameSpeed(0);
        skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;

        #region skillCutScene
        skillCutScene.SetActive(true);
        skillCutScene.GetComponent<Animator>().Play(skillUnit.GetComponent<Unit>().Unitname + "Skill", -1, 0f);
        yield return null;
        while (skillCutScene.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            yield return null;
        }

        skillCutScene.SetActive(false);
        yield return null;
        #endregion
        skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().SkillPerformState);
        skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill", false, 1);
        yield return null;

        while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
        {
            yield return null;
        }


        foreach (var target in skillTargets)
        {
            target.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -1;
            EffectManager.Instance.InstantiateHomingEffect("asher_barrier", target, asherSkillDuration);
            StartCoroutine(skillUnit.GetComponent<Unit>().ChangeStat(skillUnit, "non", 0, asherSkillDuration));
        }

        SkillEnd();

    }

        IEnumerator VogueSkill()
        {
        var targetsList = GameManager.Instance.enemiesList;

        while (!isSkillAimEnd)
        {
            skillTargets = AimSkillTargetsInRange(SkilRangeType.Circle, SkillAimType.Circle, "Enemy", vogueSkillAimRange, vogueSkillRange);
            yield return null;
        }



        foreach (var minion in targetsList)
        {
            minion.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -1;
        }

        foreach (var target in skillTargets)
        {
            target.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;
            target.GetComponent<Unit>().SetAimUnitColor(false);
        }


        GameManager.Instance.SetGameSpeed(0);
        skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;

        #region skillCutScene
        skillCutScene.SetActive(true);
        skillCutScene.GetComponent<Animator>().Play(skillUnit.GetComponent<Unit>().Unitname + "Skill", -1, 0f);
        yield return null;
        while (skillCutScene.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            yield return null;
        }

        skillCutScene.SetActive(false);
        yield return null;
        #endregion

        skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().SkillPerformState);
        skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill", false, 1);
        yield return null;

        while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
        {
            yield return null;
        }

        skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill2", false, 1);
        Vector3 startPos = skillUnit.transform.position;
        yield return null;

        while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
        {
            skillUnit.transform.position = Vector3.Lerp(startPos, skillHitpos, skillUnit.GetComponent<Unit>().normalizedTime);
            yield return null;
        }



        skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill3", false, 1);
        yield return null;
        while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
        {
            yield return null;
        }


        SkillEnd();
    }

        IEnumerator WraithSkill()
        {


        skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;
        skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().SkillPerformState);

        skillTargets = AimSkillTargetsInRange(SkilRangeType.Rectangle, SkillAimType.Auto, "Enemy", wraithSkillRangeWidth, wraithSkillRangeHeight);
        GameManager.Instance.SetGameSpeed(0);

        #region skillCutScene
        skillCutScene.SetActive(true);
        skillCutScene.GetComponent<Animator>().Play(skillUnit.GetComponent<Unit>().Unitname + "Skill", -1, 0f);
        yield return null;
        while (skillCutScene.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            yield return null;
        }

        skillCutScene.SetActive(false);
        yield return null;
        #endregion

        skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill", false, 1);
        yield return null;


        while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
        {
            yield return null;
        }


        SkillEnd();

        yield return null;
        }

        IEnumerator IsabellaSkill()
        {


        skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;



        skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().SkillPerformState);


        skillTargets = AimSkillTargetsInRange(SkilRangeType.Rectangle, SkillAimType.Auto, "Enemy", isabellaSkillRange, 1);
        GameManager.Instance.SetGameSpeed(0);

        #region skillCutScene
        skillCutScene.SetActive(true);
        skillCutScene.GetComponent<Animator>().Play(skillUnit.GetComponent<Unit>().Unitname + "Skill", -1, 0f);
        yield return null;
        while (skillCutScene.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            yield return null;
        }

        skillCutScene.SetActive(false);
        yield return null;
        #endregion
        skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill", false, 1);
        yield return null;

        while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
        {
            yield return null;
        }

        foreach (var target in skillTargets)
        {
            target.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        SkillEnd();

        yield return null;
    }

        IEnumerator ZippoSkill()
        {

        skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;
        GameManager.Instance.SetGameSpeed(0);

        #region skillCutScene
        skillCutScene.SetActive(true);
        skillCutScene.GetComponent<Animator>().Play(skillUnit.GetComponent<Unit>().Unitname + "Skill", -1, 0f);
        yield return null;
        while (skillCutScene.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            yield return null;
        }

        skillCutScene.SetActive(false);
        yield return null;
        #endregion

        skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().SkillPerformState);
        skillTargets = AimSkillTargetsInRange(SkilRangeType.Rectangle, SkillAimType.Auto, "Enemy", zippoSkillRangeWidth, zippoSkillRangeHeight);
       
        skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill", false, 1);
        EffectManager.Instance.InstantiateAttackEffect("zippo_skill", skillUnit.transform.position);

        yield return null;

        while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
        {
            yield return null;
        }

        SkillEnd();

        yield return null;
    }

    IEnumerator KuenSkill()
    {
        skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;
        skillTargets = AimSkillTargetsInRange(SkilRangeType.Rectangle, SkillAimType.Auto, "Enemy", kuenSkillRangeWidth, kuenSkillRangeHeight);
        skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().SkillPerformState);
        GameManager.Instance.SetGameSpeed(0);

        #region skillCutScene
        skillCutScene.SetActive(true);
        skillCutScene.GetComponent<Animator>().Play(skillUnit.GetComponent<Unit>().Unitname + "Skill", -1, 0f);
        yield return null;
        while (skillCutScene.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            yield return null;
        }

        skillCutScene.SetActive(false);

        yield return null;
        #endregion

        skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill1", false, 1);


        yield return null;

        while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
        {
            yield return null;
        }

        EffectManager.Instance.InstantiateAttackEffect("kuen_effect", skillUnit.transform.position);

        Vector3 startPos = skillUnit.transform.position;
        skillUnit.transform.position = new Vector3(1000, 1000, 1000);
        foreach (var target in skillTargets)
        {
            target.GetComponent<Unit>().Deal(skillUnit.GetComponent<Unit>().currentAtk * kuenSkillValue / 100);
        }

        yield return new WaitForSeconds(0.4f);


        foreach (var target in skillTargets)
        {
            target.GetComponent<Unit>().Deal(skillUnit.GetComponent<Unit>().currentAtk * kuenSkillValue / 100);
        }

        yield return new WaitForSeconds(0.4f);

        skillUnit.transform.position = startPos;
        skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill2", false, 1);
        yield return null;

        while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
        {
            yield return null;
        }

        SkillEnd();

        yield return null;
    }
    IEnumerator EremediumSkill()
    {
        GameManager.Instance.SetGameSpeed(0);
        skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 1;

        #region skillCutScene
        skillCutScene.SetActive(true);
        skillCutScene.GetComponent<Animator>().Play(skillUnit.GetComponent<Unit>().Unitname + "Skill", -1, 0f);
        yield return null;
        while (skillCutScene.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            yield return null;
        }

        skillCutScene.SetActive(false);
        yield return null;
        #endregion

        skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().SkillPerformState);
        skillUnit.GetComponent<Unit>().spineAnimation.PlayAnimation(skillUnit.GetComponent<Unit>().skinName + "/skill", false, 1);
        yield return null;

        while (skillUnit.GetComponent<Unit>().normalizedTime < 1)
        {
            yield return null;
        }

        SkillEnd();
    }

    private void SkillEnd()
    {
        skillUnit.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().moveState);
        GameManager.Instance.SetGameSpeed(1);
        skillBackgroundImage.SetActive(false);
        skillUnit.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -1;

        foreach(var target in skillTargets)
        {
            target.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -1;
        }
        isSkillActing = false;
        isSkillAimEnd = false;
        skillUnit.GetComponent<DefenceMinion>().skillTimer = 0;
        Debug.Log("연출 끝");
    }

    IEnumerator WraithSkillEvent()
    {
        EffectManager.Instance.InstantiateAttackEffect("wraith_skill", skillUnit.transform.position);

        foreach(var target in skillTargets)
        {
            target.GetComponent<Unit>().Deal(skillUnit.GetComponent<Unit>().currentAtk * wraithSkillValue / 100);
        }

        yield return null;
    }
    IEnumerator VogueSkillEvent()
    {
        foreach (var target in skillTargets)
        {
            target.GetComponent<Unit>().Deal(skillUnit.GetComponent<Unit>().currentAtk * vogueSkillValue / 100);
        }

        yield return null;
    }
    IEnumerator IsabellaSkillEvent()
    {
        EffectManager.Instance.InstantiateAttackEffect("isabella_skill", skillUnit.transform.position);

        foreach (var target in skillTargets)
        {
            target.GetComponent<Unit>().Deal(skillUnit.GetComponent<Unit>().currentAtk * isabellaSkillValue / 100);
            target.GetComponent<Rigidbody>().AddExplosionForce(100, skillUnit.transform.position, 3, 0, ForceMode.Impulse);
            target.GetComponent<UnitStateMachine>().ChangeState(skillUnit.GetComponent<UnitStateMachine>().idleState);
        }

        yield return null;
    }

    IEnumerator ZippoSkillEvent()
    {
        foreach (var target in skillTargets)
        {
            target.GetComponent<Unit>().Deal(skillUnit.GetComponent<Unit>().currentAtk * zippoSkillValue / 100);
            EffectManager.Instance.InstantiateAttackEffect("zippo_skillHit", target.transform.position);
        }

        yield return null;
    }

    IEnumerator KuenSkillEvent()
    {
        foreach (var target in skillTargets)
        {
            target.GetComponent<Unit>().Deal(skillUnit.GetComponent<Unit>().currentAtk * 0.5f);
            EffectManager.Instance.InstantiateAttackEffect("kuen_effect", target.transform.position);
        }

        yield return null;
    }

    IEnumerator EremediumSkillEvent()
    {
        yield return null;

        GameObject skillObject = Instantiate(healDrone);
        skillObject.transform.position = skillUnit.transform.position;
        skillObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1;

        skillObject.GetComponent<HealDrone>().target = skillUnit;
        skillObject.GetComponent<HealDrone>().duration = eremediumSkillDuration;
        skillObject.GetComponent<HealDrone>().healAmount = skillUnit.GetComponent<Unit>().atk* eremediumSkillValue / 100;
        skillObject.GetComponent<HealDrone>().healRange = 2;


    }

    IEnumerator VeritySkillEvent()
    {
        yield return null;

        GameObject skillObject = Instantiate(verityShot);

        Vector3 startPos = skillUnit.GetComponent<DefenceMinion>().shootPivot.transform.position;
        startPos.y += 2.1f;
        
        skillObject.transform.position = startPos;


        skillObject.GetComponent<VerityShot>().target = skillTargets[0];
        skillObject.GetComponent<VerityShot>().speed = 30;
        skillObject.GetComponent<VerityShot>().damage = skillUnit.GetComponent<Unit>().currentAtk * (veritySkillValue / 100);


    }
}
