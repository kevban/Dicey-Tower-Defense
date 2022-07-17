using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public bool building = true;
    public Tilemap tilemap;
    public Tilemap buildableTiles;
    public Tile greenTile;
    public AnimatedTile towerTile;
    // player stats
    public int attack = 5;
    public float range = 5f;
    public int extraAttacks = 0;
    public int exp = 0;
    public int level = 0;
    // crit
    public int critChance = 0;
    public float critDmg = 2f;
    // fireball
    public int fireballChance = 0;
    public float fireballArea = 0;
    public float fireballDmg = 0.5f;
    // chain
    public int chainChance = 0;
    public int chainTargets = 3;
    public float chainDmg = 0.5f;
    // pierce
    public int pierceChance = 0;
    public float pierceDmg = 0.2f;

    public List<int> levelCurve = new List<int>();
    public int bigRewardCounter = 0;
    public List<int> normalRewards = new List<int>();
    public List<int> bigRewards = new List<int>();
    public int newTowerCounter = 3;
    public int difficulty = 1;


    public bool coroutineAllowed = false;
    public bool endOfLevel = false;
    public bool win = false;

    public List<GameObject> dices;
    public List<GameObject> wayPoints;

    public GameObject enemyPrefab; //bat
    public GameObject enemyPrefab2; //slime
    public GameObject enemyPrefab3; //spider
    public GameObject enemyPrefab4; //phantom
    public GameObject enemyPrefab5; //spinner
    public GameObject enemyPrefab6; //boss
    public GameObject bonusRewardCardPrefab;
    public GameObject explosionPrefab;
    public GameObject projectilePrefab;
    public GameObject towerPrefab;
    public List<GameObject> spawnArea;
    public GameObject rewardCanvas;
    public GameObject bonusRewardCanvas;
    public RewardCard rewardCard1;
    public RewardCard rewardCard2;
    public RewardCard rewardCard3;
    public GameObject rewardHolder;
    public GameObject continueButton;
    public GameObject tutorialCanvas;
    public GameObject winCanvas;

    public TextMeshProUGUI diceTotalLabel;
    public TextMeshProUGUI goldLabel;
    public TextMeshProUGUI bonusEXPLabel;
    public TextMeshProUGUI comboLabel;
    public TextMeshProUGUI towerLabel;
    public TextMeshProUGUI tutorialLabel;
    public TextMeshProUGUI winLabel;

    //spanwed enemies
    public List<Enemy> enemyList = new List<Enemy>();

    // variables
    public bool enemyTurn = false;
    public int diceTotal = 0;
    public int round = 0;
    public int expGain = 0;
    public int bonusCard = 0;
    public List<Tower> towerList = new List<Tower>();
    public int tutorial = 0;
    float curpitch = 1f;

    public bool inMenu = false;

    public bool muted = false;

    public AudioManager audioManager;



    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 1000; i++) {
            levelCurve.Add(i * 50 + 100);
        }
        normalRewards.Add(0);
        normalRewards.Add(1);
        normalRewards.Add(2);
        bigRewards.Add(3);
        bigRewards.Add(4);
        bigRewards.Add(5);
        bigRewards.Add(6);
        bigRewards.Add(7);
        bigRewards.Add(8);
        if (tutorial == 0) {
            NextTutorial();
        }

        audioManager.Play("bgm1");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && building && !inMenu)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int location;
            location = tilemap.WorldToCell(mousePos);
            location.z = 0;
            if (buildableTiles.GetTile(location) == towerTile)
            {
                buildableTiles.gameObject.SetActive(false);
                buildableTiles.SetTile(location, greenTile);
                Vector3 worldLoc = tilemap.CellToWorld(location);
                GameObject newTower = Instantiate(towerPrefab, new Vector3 (worldLoc.x + 0.5f, worldLoc.y + 0.6f, 0), Quaternion.identity);
                Tower tower = newTower.GetComponent<Tower>();
                towerList.Add(tower);
                buildableTiles.color = Color.white;
                building = false;

                if (tutorial == 1)
                {
                    NextTutorial();
                }
                else if (tutorial == 4) {
                    NextTutorial();
                }
                towerLabel.text = "8 turns until new Hero";

            }
        }
    }

    public void DiceRoll() {
        if (coroutineAllowed) {
            StartCoroutine("RollTheDice");
        }


    }

    private IEnumerator RollTheDice()
    {
        curpitch = 1;
        coroutineAllowed = false;
        SpawnEnemy();
        enemyList.RemoveAll(e => e == null);
        foreach (Enemy enemy in enemyList)
        {
            if (enemy == null)
            {
                print("it is null");
            }
            else if (enemy.isDead)
            {
                Destroy(enemy.gameObject);
            }
        }
        int[] randomDiceSide = new int[5];
        for (int k = 0; k <= 20; k++)
        {
            for (int i = 0; i < 5; i++) {
                randomDiceSide[i] = Random.Range(0, 6);
                dices[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("dice_" + (randomDiceSide[i] + 1).ToString());
            }


            yield return new WaitForSeconds(0.05f);
        }

        foreach (int num in randomDiceSide) {
            diceTotal += num + 1;
        }
        diceTotalLabel.text = diceTotal.ToString();
        foreach (Tower tower in towerList)
        {
            tower.Fire(diceTotal + extraAttacks);
        }
        yield return new WaitForSeconds(2f);
        enemyTurn = true;
        yield return new WaitForSeconds(2f);
        int combo = CheckCombo(randomDiceSide);
        StartCoroutine(AccumEXP(combo));
        enemyTurn = false;
        diceTotal = 0;
        if (combo < 4) {
            coroutineAllowed = true;
        }
        newTowerCounter--;
        towerLabel.text = newTowerCounter.ToString() + " turns until new Hero";
        if (newTowerCounter <= 0) {
            towerLabel.text = "Place a new Hero";
            AddTower();
            newTowerCounter = 8;
        }
        
    }

    private IEnumerator AccumEXP(int combo) {
        if (combo > 0) {
            if (tutorial == 3)
            {
                NextTutorial();
            }
            bonusRewardCanvas.SetActive(true);
            StartCoroutine(PlayComboSound("combo", combo));
        }
        switch (combo) {
            case 1:
                comboLabel.text = "One Pair";
                expGain = 15 + 5 * (int)(round / 10);
                break;
            case 2:
                comboLabel.text = "Two Pairs";
                expGain = 30 + 10 * (int)(round / 10);
                break;
            case 3:
                comboLabel.text = "Three of a Kind";
                expGain = 60 + 20 * (int)(round / 10);
                break;
            case 4:
                comboLabel.text = "Full House!";
                expGain = 120 + 30 * (int)(round / 10);
                bonusCard = 1;
                break;
            case 5:
                comboLabel.text = "Straight!";
                expGain = 180 + 40 * (int)(round / 10);
                bonusCard = 2;
                break;
            case 6:
                comboLabel.text = "Four of a Kind!";
                expGain = 240 + 60 * (int)(round / 10);
                bonusCard = 3;
                break;
            case 7:
                comboLabel.text = "!FIVE OF A KIND!";
                expGain = 300 + 100 * (int)(round / 10);
                bonusCard = 5;
                break;
        }
        if (bonusCard == 0)
        {
            while (int.Parse(bonusEXPLabel.text) < expGain)
            {
                bonusEXPLabel.text = (int.Parse(bonusEXPLabel.text) + 1).ToString();
                audioManager.Play("tick");
                yield return new WaitForSeconds(1.5f / expGain);
            }
            print("started");
            Invoke("AddBonusEXP", 1.5f);
        }
        else {
            inMenu = true;
            for (int i = 0; i < bonusCard; i++) {
                Invoke("AddBonusCard", i * 2f + 4f);
            }
            Invoke("FadeToBlack", 3f);
            while (int.Parse(bonusEXPLabel.text) < expGain)
            {
                audioManager.Play("tick");
                bonusEXPLabel.text = (int.Parse(bonusEXPLabel.text) + 1).ToString();
                yield return new WaitForSeconds((bonusCard * 2f + 4f) / expGain);
            }
            continueButton.SetActive(true);

        }
    }

    private IEnumerator PlayComboSound(string track, int combo) {
        for (int i = 0; i < combo; i++) {
            audioManager.PlayPitch(track, 1 + i * 0.4f);
            yield return new WaitForSeconds(0.5f);
        }
    }


    public void FadeToBlack() { 
        Animator animator = bonusRewardCanvas.GetComponent<Animator>();
        animator.Play("fadeToBlack");
    }

    public void AddBonusEXP() {
        if (bonusCard > 0) {
            coroutineAllowed = true;
        }
        UpdateExp(expGain);
        expGain = 0;
        bonusCard = 0;
        bonusRewardCanvas.GetComponent<Animator>().Play("idle");
        bonusRewardCanvas.SetActive(false);
        continueButton.SetActive(false);
        foreach (Transform child in rewardHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        bonusEXPLabel.text = "0";
        inMenu = false;
    }


    public void AddBonusCard() {
        int random = Random.Range(0, normalRewards.Count);
        audioManager.PlayPitch("bonuscard", 1 + curpitch);
        curpitch += 0.4f;
        GameObject newReward = Instantiate(bonusRewardCardPrefab);
        newReward.transform.SetParent(rewardHolder.transform, false);
        newReward.GetComponent<BonusRewardCard>().UpdateReward(normalRewards[random]);
    }

    public int CheckCombo(int[] dicesides) {
        int count = 0;
        int threeOfKind = 0;
        int pair = 0;
        int straight = 0;
        for (int i = 0; i < 6; i++) {
            foreach (int k in dicesides) {
                if (i == k) {
                    count++;
                }
            }
            if (count == 5)
            {
                return 7;
            }
            else if (count == 4)
            {
                return 6;
            }
            else if (count == 3)
            {
                threeOfKind++;
            }
            else if (count == 2)
            {
                pair++;
            }
            else if (count == 1)
            {
                straight++;
            }
            else {
                if (i != 5){
                    straight = 0;
                }
                
            }
            count = 0;
        }
        if (pair >= 2)
        {
            return 2;
        }
        else if (pair == 1 && threeOfKind == 1)
        {
            return 4;
        }
        else if (pair == 1)
        {
            return 1;
        }
        else if (threeOfKind == 1)
        {
            return 3;
        }
        else if (straight >= 5) {
            return 5;
        }
        return 0;
    }


    public void CreateProjectile(Vector3 spawnPos, Enemy enemy) {
        GameObject newProjectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Projectile projectile = newProjectile.GetComponent<Projectile>();
        int randomNum = Random.Range(0, 100);
        projectile.attack = attack;
        projectile.enemy = enemy;
        if (randomNum < critChance)
        {
            projectile.GetComponent<SpriteRenderer>().color = Color.yellow;
            projectile.type = 1;
            projectile.attack = (int)(projectile.attack*critDmg);
            audioManager.Play("crit");
        }
        else if (randomNum < (critChance + fireballChance))
        {
            projectile.GetComponent<SpriteRenderer>().color = Color.red;
            projectile.type = 2;
            audioManager.Play("fireballstart");
        }
        else if (randomNum < (critChance + fireballChance + chainChance))
        {
            projectile.GetComponent<SpriteRenderer>().color = Color.blue;
            projectile.type = 3;
            projectile.target = chainTargets;
            audioManager.Play("chain");
        }
        else if (randomNum < (critChance + fireballChance + chainChance + pierceChance))
        {
            projectile.GetComponent<SpriteRenderer>().color = Color.green;
            projectile.type = 4;
            audioManager.Play("pierce");
        }
        else {
            projectile.GetComponent<SpriteRenderer>().color = Color.white;
            projectile.type = 0;
            audioManager.Play("fire");
        }
        
    }

    public void SpawnEnemy() {
        int level = (int) (round / 5);
        if (round % 5 == 0) {
            switch (level) { 
                case 0: // 20 bats
                    for (int i = 0; i < (20); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    break;
                case 1: // 10 bats 10 slimes
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab2, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    break;
                case 2: // 5 bats 15 slimes, 3 spiders
                    for (int i = 0; i < (5); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (15); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab2, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (3); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab3, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    break;
                case 3: // 10 slimes 5 spiders
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab2, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (5); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab3, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    break;
                case 4: // 10 slimes, 10 spiders
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab2, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab3, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    break;
                case 5: // 5 bats 5 slimes, 5 spiders, 1 phantom
                    for (int i = 0; i < (5); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (5); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab2, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (5); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab3, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (1); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab4, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    break;
                case 6: // 5 slimes 3 phantom
                    for (int i = 0; i < (5); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab2, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (5); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab4, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    break;
                case 7: // 5 slimes 5 phantom, 1 spinner
                    for (int i = 0; i < (5); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab2, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (5); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab4, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (1); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab5, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    break;
                case 8: // 10 phantom, 5 spinner
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab4, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (5); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab5, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    break;
                case 9: // 10 spiders 10 phantom, 10 spinner
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab4, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab4, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab5, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    break;
                case 10: // 10 slimes 10 spiders 10 phantom, 10 spinner, 1 boss
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab4, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab2, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab4, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (10); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab5, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    for (int i = 0; i < (1); i++)
                    {
                        int randSpawn = Random.Range(0, spawnArea.Count);
                        GameObject newEnemy = Instantiate(enemyPrefab6, new Vector3(spawnArea[randSpawn].transform.position.x + Random.Range(0f, 60f) / 10f, spawnArea[randSpawn].transform.position.y + Random.Range(-10f, 10f) / 10f, 0), Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().boss = true;
                        newEnemy.GetComponent<Enemy>().health2 *= difficulty;
                        enemyList.Add(newEnemy.GetComponent<Enemy>());
                    }
                    break;
                default:
                    break;

            }
            
        }
        round++;
    }

    public void UpdateExp(int amt) {

        if (tutorial == 2) { 
            NextTutorial();
        }
        exp += amt;
        if (exp >= levelCurve[level]) {
            List<int> randomInts = new List<int>();
            int random; 
            if (bigRewardCounter <= 0)
            {
                random = Random.Range(0, bigRewards.Count);
                while (randomInts.Count < 3)
                {
                    random = Random.Range(0, bigRewards.Count);
                    if (!randomInts.Contains(random))
                    {
                        randomInts.Add(random);
                    }
                }
                rewardCard1.UpdateReward(bigRewards[randomInts[0]]);
                rewardCard2.UpdateReward(bigRewards[randomInts[1]]);
                rewardCard3.UpdateReward(bigRewards[randomInts[2]]);
                bigRewardCounter = 5;
            }
            else {
                random = Random.Range(0, normalRewards.Count);
                while (randomInts.Count < 3)
                {
                    random = Random.Range(0, normalRewards.Count);
                    if (!randomInts.Contains(random))
                    {
                        randomInts.Add(random);
                    }

                }
                rewardCard1.UpdateReward(normalRewards[randomInts[0]]);
                rewardCard2.UpdateReward(normalRewards[randomInts[1]]);
                rewardCard3.UpdateReward(normalRewards[randomInts[2]]);
                bigRewardCounter--;
            }
            
            exp -= levelCurve[level];
            Time.timeScale = 0;
            inMenu = true;
            rewardCanvas.SetActive(true);
            level++;
        }
        goldLabel.text = "Exp: " + exp.ToString() + "/ " + levelCurve[level];
    }

    public void AddTower() {
        buildableTiles.gameObject.SetActive(true);
        building = true;
    }

    public void NextTutorial() {
        
        switch (tutorial) {
            case 0:
                tutorialLabel.text = "Click on highlighted tiles to place heroes";
                tutorial += 1;
                break;
            case 1:
                tutorialLabel.text = "Heroes will fire at enemies based on the number you roll.Click 'GO' to roll the dice";
                tutorial += 1;
                coroutineAllowed = true;
                break;
            case 2:
                tutorialLabel.text = "You earn exp when enemy is defeated. You can upgrade your heroes when you level up!";
                tutorial += 1;
                break;
            case 3:
                tutorialLabel.text = "You can earn bonus reward if the dice form a specific hand.";
                tutorial = 4;
                break;
            case 4:
                tutorialLabel.text = "If an enemy reach the end of the path, it is game over! GLHF.";
                tutorial = -1;
                break;
            default:
                break;

        }
        inMenu = true;
        Time.timeScale = 0;
        tutorialCanvas.SetActive(true);
    }

    public void CloseTutorial() {
        inMenu = false;
        Time.timeScale = 1;
        tutorialCanvas.SetActive(false);
    }

    public void GameOver() {
        inMenu = true;
        if (win)
        {
            winLabel.text = "You Win! Thanks for playing! Click play again to continue.";
        }
        else {
            winLabel.text = "Game Over!";
        }
        winCanvas.SetActive(true);
    }

    public void PlayAgain() {
        inMenu = false;
        if (win)
        {
            difficulty++;
            round = 0;
            winCanvas.SetActive(false);
        }
        else {
            SceneManager.LoadScene(0);
        }
        
    }

    public void Mute() {
        if (muted)
        {
            audioManager.Play("bgm1");
            muted = false;
        }
        else {
            
            audioManager.Stop("bgm1");
            muted = true;
        }
        
    }

}
