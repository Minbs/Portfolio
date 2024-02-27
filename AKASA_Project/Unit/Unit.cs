using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;
public enum Direction
{
    LEFT,
    RIGHT
}



public class Unit : MonoBehaviour
{
    public GameObject CostUimanager;
    public GameObject GameDataManager;
    public string poolItemName;
    public string Unitname;
    public int Level = 1;
    private Stat ParsingStat;
    public UnityEvent UnitDisplay;
    public bool OneTimeSummon = false;

    public Tile onTile { get; set; }
    [Header("UnitStat")]
    public AttackType attackType;

    public float atk;
    public float currentAtk; //{ get; set; }
    public float def;

    public float attackRangeDistance; // 유닛 공격 범위
    public float attackRange2;

    public float cognitiveRangeDistance; // 유닛 인지 범위
    public float attackSpeed;  //{ get; set; }


    public float damageRedution = 0;
    public float healTakeAmount = 0;

    // 중독 상태용 변수
    private bool isPoisoned = false;
    private float poisonTimer = 0;

    private float rewardCost;

    public bool isNonDamage = false;

    public Direction direction { get; set; }

    public SpineAnimation spineAnimation { get; set; }
    public SkeletonAnimation skeletonAnimation { get; set; }

    public GameObject target { get; set; }
    public GameObject healthBar;
    private float healthBarCount = 0;

    public SkeletonDataAsset skeletonData { get; set; }

    public string skinName { get; set; }

    public Color initSkeletonColor { get; set; } // 최초 스파인 색상

    public float normalizedTime { get; set; }  //스파인 애니메이션 진행도 0~1

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        CostUimanager = GameObject.FindGameObjectWithTag("WaveUI");

        if (Unitname == "Enemy1" || Unitname == "Enemy2"
            || Unitname == "EnemyTank" || Unitname == "EnemyHealer"
            || Unitname == "EnemyBoss")
        {
            Level = GameManager.Instance.currentWave;
            GameDataManager.gameObject.GetComponent<CSV_Player_Status>().StartParsing(this.Unitname);
            ParsingStat = GameDataManager.gameObject.GetComponent<CSV_Player_Status>().Call_Stat_CSV(Unitname, Level);
            maxHp = ParsingStat.HP;
            maxHpStat = maxHp;
            atk = ParsingStat.Atk;
            def = ParsingStat.Def;
            attackRangeDistance = ParsingStat.AtkRange1;
            attackRange2 = ParsingStat.AtkRange2;
            cognitiveRangeDistance = ParsingStat.CognitiveRange;
            attackSpeed = ParsingStat.AtkSpeed;
            rewardCost = ParsingStat.RewardCost;
        }
        else
        {
            GameDataManager.gameObject.GetComponent<CSV_Player_Status>().StartParsing(this.Unitname);
            ParsingStat = GameDataManager.gameObject.GetComponent<CSV_Player_Status>().Call_Stat_CSV(Unitname,Level);

            maxHp = ParsingStat.HP;
            maxHpStat = maxHp;
            atk = ParsingStat.Atk;
            def = ParsingStat.Def;
            attackRangeDistance = ParsingStat.AtkRange1;
            attackRange2 = ParsingStat.AtkRange2;
            cognitiveRangeDistance = ParsingStat.CognitiveRange;
            attackSpeed = ParsingStat.AtkSpeed;
        }




        if (!GetComponent<UnitStateMachine>())
        {
            gameObject.AddComponent<UnitStateMachine>();
        }

        if (transform.GetChild(0).GetComponent<SpineAnimation>() == null) transform.GetChild(0).gameObject.AddComponent<SpineAnimation>();

        spineAnimation = transform.GetChild(0).GetComponent<SpineAnimation>();
        skeletonAnimation = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        skeletonData = transform.GetChild(0).GetComponent<SkeletonAnimation>().skeletonDataAsset;
        transform.GetChild(0).GetComponent<SkeletonAnimation>().Initialize(true);
        skinName = transform.GetChild(0).GetComponent<SkeletonAnimation>().initialSkinName;
        initSkeletonColor = transform.GetChild(0).GetComponent<SkeletonAnimation>().skeleton.GetColor();

        transform.GetChild(0).GetComponent<MeshRenderer>().sortingLayerName = "Character2";
        transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -1;

   
    }

    public void Init()
    {
        currentAtk = atk;
        currentHp = maxHp;

        attackSpeed = 1;
        damageRedution = 0;
        transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
        transform.GetComponent<NavMeshAgent>().enabled = true;
    }

    protected virtual void Update()
    {
        Spine.TrackEntry trackEntry = new Spine.TrackEntry();
        trackEntry = spineAnimation.skeletonAnimation.AnimationState.Tracks.ElementAt(0);
        normalizedTime = trackEntry.AnimationLast / trackEntry.AnimationEnd;    
    }

    /// <summary>
    /// 데미지 부여 damage가 음수일 때 회복
    /// </summary>
    /// <param name="damage"></param>
    public void Deal(float damage)
    {
        float damageSum = 0;

        if (damage < 0) //heal
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(ChangeUnitColor(Color.green, 0.2f));

            damageSum = damage + (float)damage * ((float)healTakeAmount / 100);
           // Debug.Log(damageSum);
        }
        else
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(ChangeUnitColor(Color.red, 0.2f));

            //데미지 = (공격력 - 방어력) * N/100

            if(!isNonDamage)
            {

            damageSum = (float)((float)damage - def) * (float)(100 - damageRedution) / 100;
            damageSum = Mathf.Max(damageSum, 0.5f);
            }
        }

        currentHp -= (float)damageSum;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        healthBarCount = 0;
        StartCoroutine( UpdateHealthbar());
    }

    /// <summary>
    /// 체력바 UI 이미지 갱신
    /// </summary>
    /// 
    public IEnumerator UpdateHealthbar()
    {
        healthBar.transform.GetChild(1).GetComponent<Image>().fillAmount = currentHp / maxHp;

        if (healthBar.activeSelf)
            yield break;

        healthBar.SetActive(true);
     

        while (healthBarCount < 3)
        {
            healthBarCount += Time.deltaTime;
            yield return null;
        }


        healthBar.SetActive(false);

    }

    public void SetDirection(Direction direction)
    {
        Vector3 scale = Vector3.one;
        this.direction = direction;

        if (direction == Direction.LEFT)
        {
            scale.x = -1;
            transform.GetChild(0).localScale = new Vector3(Mathf.Abs(transform.GetChild(0).localScale.x) * scale.x, transform.GetChild(0).localScale.y, transform.GetChild(0).localScale.z);
        }
        else if (direction == Direction.RIGHT)
        {
            scale.x = 1;
            transform.GetChild(0).localScale = new Vector3(Mathf.Abs(transform.GetChild(0).localScale.x) * scale.x, transform.GetChild(0).localScale.y, transform.GetChild(0).localScale.z);
        }
    }

    public void SetAimUnitColor(bool Active)
    {
        int id = Shader.PropertyToID("_Black");

        MaterialPropertyBlock block = new MaterialPropertyBlock();

        if(Active)
            block.SetColor(id, new Color32(37, 37, 37, 1));
        else
            block.SetColor(id, new Color32(0, 0, 0, 1));

        transform.GetChild(0).GetComponent<MeshRenderer>().SetPropertyBlock(block);
    }

    public void GetPoisoned(float damage,float duration)
    {
        if (!isPoisoned)
        {
            StartCoroutine(Poisoned(damage, duration));
        }
        else
            poisonTimer = 0;
    }

    public IEnumerator Poisoned(float damage, float duration)
    {
        isPoisoned = true;

        while(poisonTimer <= duration)
        {

          poisonTimer += (1 + Time.deltaTime) * GameManager.Instance.gameSpeed;


          currentHp -= damage;
            healthBarCount = 0;
          StartCoroutine(ChangeUnitColor(new Color(1, 0, 1), 0.2f));
            StartCoroutine(UpdateHealthbar());

            Debug.Log(poisonTimer);
          yield return new WaitForSeconds(1);

        }

        isPoisoned = false;
    }

    public IEnumerator ChangeUnitColor(Color color, float duration)
    {
        if (gameObject != null)
            StopCoroutine("ChangeUnitColor");

        float timer = 0f;

        transform.GetChild(0).GetComponent<SkeletonAnimation>().skeleton.SetColor(color);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;

            if (gameObject != null)
                StopCoroutine("ChangeUnitColor");
        }


        transform.GetChild(0).GetComponent<SkeletonAnimation>().skeleton.SetColor(initSkeletonColor);
    }

    public IEnumerator Die()
    {
        if (!isAnimationPlaying("/die"))
        {
            spineAnimation.PlayAnimation(skinName + "/die", false, GameManager.Instance.gameSpeed);
        }

        if (skeletonAnimation.AnimationName == skinName + "/die" && normalizedTime >= 1)
        {
            if (GetComponent<Minion>())
            {
                gameObject.SetActive(false);
            }
            else if(GetComponent<Enemy>())
            {
                ObjectPool.Instance.PushToPool(poolItemName, gameObject);
                GameManager.Instance.enemiesList.Remove(gameObject);

                GameManager.Instance.cost += rewardCost;
                BattleUIManager.Instance.costText.text = GameManager.Instance.cost.ToString();

                CostUimanager.GetComponent<Wave_UI_Script>().CostUpUI(((int)rewardCost),"temp");

            }
        }

        yield return null;
    }

    /// <summary>
    /// 버프 디버프 등으로 인한 스탯 변경
    /// </summary>
    public IEnumerator ChangeStat(GameObject target, string stat, float value = 0, float duration = 0)
    {
        float Timer = 0;

        if (stat == "ats")
            attackSpeed += value;
        else if (stat == "def")
            def += value;
        else if (stat == "atk")
            currentAtk += value;
        else if (stat == "non")
            isNonDamage = true;

        while (Timer <= duration)
        {
            Timer += Time.deltaTime * GameManager.Instance.gameSpeed;
            yield return null;
        }


        if (stat == "ats")
            attackSpeed -= value;
        else if (stat == "def")
            def -= value;
        else if (stat == "atk")
            currentAtk -= value;
        else if (stat == "non")
            isNonDamage = false;
    }

    /// <summary>
    /// 유닛 타일 정 중앙에 고정
    /// </summary>
    public void SetPositionOnTile()
    {
        transform.position = onTile.gameObject.transform.position + GameManager.Instance.minionSetPosition;
    }

    public void SetUnitStat(Stat stat)
    {
      maxHp = stat.HP;
      currentHp = maxHp;
      atk = stat.Atk;
      currentAtk = atk;
      def = stat.Def;
      attackSpeed = stat.AtkSpeed;
      attackRangeDistance = stat.AtkRange1;
      attackRange2 = stat.AtkRange2;

      if (GetComponent<DefenceMinion>())
      {
        GetComponent<DefenceMinion>().cost = stat.BuyCost;
        GetComponent<DefenceMinion>().sellCost = stat.CellCost;
      }
    }
    /// <summary>
    /// 스파인 애니메이션 종료 확인 함수 </summary> <param name="animationName"> 스파인 애니메이션 이름</param>
    /// </summary>
    public bool isAnimationPlaying(string animationName) =>  skeletonAnimation.AnimationName == skinName + animationName && normalizedTime< 1;  // 실행중인 애니메이션의 이름이 animationName과 다르거나 끝나지 않았을 때
}