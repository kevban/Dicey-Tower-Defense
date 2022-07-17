using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardCard : MonoBehaviour
{

    public TextMeshProUGUI rewardLabel;
    int rewardType = 0;
    public GameManager gameManager;
    public GameObject rewardCanvas;
    public TextMeshProUGUI rewardTitle;
    public Image rewardImage;
    public Image border;
    public void UpdateReward(int rewardType) { 
        rewardLabel.text = rewardType.ToString();
        this.rewardType = rewardType;
        switch (rewardType) {
            case 0:
                rewardTitle.text = "Attack Up";
                rewardImage.sprite = Resources.Load<Sprite>("swordup1");
                border.color = Color.green;
                rewardLabel.text = "Increase attack by 20%";
                break;
            case 1:
                rewardTitle.text = "Projectile Up";
                rewardImage.sprite = Resources.Load<Sprite>("projectileup1");
                border.color = Color.green;
                rewardLabel.text = "Increase projectiles by 1";
                break;
            case 2:
                rewardTitle.text = "Range Up";
                rewardImage.sprite = Resources.Load<Sprite>("rangeup2");
                border.color = Color.green;
                rewardLabel.text = "Increase range by 10%";
                break;
            case 3:
                rewardTitle.text = "Pierce";
                rewardImage.sprite = Resources.Load<Sprite>("pierce");
                border.color = Color.yellow;
                rewardLabel.text = "Attacks may pierce through all enemies";
                break;
            case 4:
                rewardTitle.text = "Crit";
                rewardImage.sprite = Resources.Load<Sprite>("crit");
                border.color = Color.yellow;
                rewardLabel.text = "Attacks may crit for double damage";
                break;
            case 5:
                rewardTitle.text = "Fireball";
                rewardImage.sprite = Resources.Load<Sprite>("fireball");
                border.color = Color.yellow;
                rewardLabel.text = "Attacks may launch fireballs to surrounding enemies";
                break;
            case 6:
                rewardTitle.text = "Spread Shot";
                rewardImage.sprite = Resources.Load<Sprite>("chain");
                border.color = Color.yellow;
                rewardLabel.text = "Attacks may spread to other enemies";
                break;
            case 7:
                rewardTitle.text = "Projectile Up+";
                rewardImage.sprite = Resources.Load<Sprite>("projectileup2");
                border.color = Color.yellow;
                rewardLabel.text = "Increase projectiles by 3";
                break;
            case 8:
                rewardTitle.text = "Attack Up+";
                rewardImage.sprite = Resources.Load<Sprite>("swordup2");
                border.color = Color.yellow;
                rewardLabel.text = "Increase attack by 60%";
                break;
            case 9:
                rewardTitle.text = "Fireball";
                rewardImage.sprite = Resources.Load<Sprite>("fireballup1");
                border.color = Color.yellow;
                rewardLabel.text = "Increase fireball chance";
                break;
            case 10:
                rewardTitle.text = "Fireball";
                rewardImage.sprite = Resources.Load<Sprite>("fireballup2");
                border.color = Color.green;
                rewardLabel.text = "Increase fireball damage";
                break;
            case 11:
                rewardTitle.text = "Fireball";
                rewardImage.sprite = Resources.Load<Sprite>("fireballup3");
                border.color = Color.green;
                rewardLabel.text = "Increase fireball area";
                break;
            case 12:
                rewardTitle.text = "Crit";
                rewardImage.sprite = Resources.Load<Sprite>("critup1");
                border.color = Color.yellow;
                rewardLabel.text = "Increase crit chance";
                break;
            case 13:
                rewardTitle.text = "Crit";
                rewardImage.sprite = Resources.Load<Sprite>("critup2");
                border.color = Color.green;
                rewardLabel.text = "Increase crit damange";
                break;
            case 14:
                rewardTitle.text = "Spread Shot";
                rewardImage.sprite = Resources.Load<Sprite>("chainup1");
                border.color = Color.yellow;
                rewardLabel.text = "Increase chain chance";
                break;
            case 15:
                rewardTitle.text = "Spread Shot";
                rewardImage.sprite = Resources.Load<Sprite>("chainup2");
                border.color = Color.green;
                rewardLabel.text = "Increase spread taget by 1";
                break;
            case 16:
                rewardTitle.text = "Spread Shot";
                rewardImage.sprite = Resources.Load<Sprite>("chainup3");
                border.color = Color.green;
                rewardLabel.text = "Increase spread damage";
                break;
            case 17:
                rewardTitle.text = "Spread Shot";
                rewardImage.sprite = Resources.Load<Sprite>("pierceup1");
                border.color = Color.yellow;
                rewardLabel.text = "Increase pierce chance";
                break;
            case 18:
                rewardTitle.text = "Spread Shot";
                rewardImage.sprite = Resources.Load<Sprite>("pierceup2");
                border.color = Color.green;
                rewardLabel.text = "Increase pierce damage";
                break;
            default:
                break;
        }
            
                
    }

    public void ChooseReward() {
        switch (rewardType) { 
            case 0:
                gameManager.attack += 1;
                break;
            case 1:
                gameManager.extraAttacks += 1;
                break;
            case 2:
                gameManager.range += 0.5f;
                break;
            case 3:
                gameManager.pierceChance = 5;
                gameManager.bigRewards.Remove(3);
                gameManager.bigRewards.Add(17);
                gameManager.bigRewards.Add(18);
                break;
            case 4:
                gameManager.critChance = 5;
                gameManager.bigRewards.Remove(4);
                gameManager.bigRewards.Add(12);
                gameManager.normalRewards.Add(13);
                break;
            case 5:
                gameManager.fireballChance = 5;
                gameManager.bigRewards.Remove(5);
                gameManager.bigRewards.Add(9);
                gameManager.normalRewards.Add(10);
                gameManager.normalRewards.Add(11);
                break;
            case 6:
                gameManager.chainChance = 5;
                gameManager.bigRewards.Remove(6);
                gameManager.bigRewards.Add(14);
                gameManager.normalRewards.Add(15);
                gameManager.normalRewards.Add(16);
                break;
            case 7:
                gameManager.extraAttacks += 3;
                break;
            case 8:
                gameManager.attack += 3;
                break;
            case 9:
                gameManager.fireballChance += 5;
                break;
            case 10:
                gameManager.fireballDmg += 0.1f;
                break;
            case 11:
                gameManager.fireballArea += 0.1f;
                break;
            case 12:
                gameManager.critChance += 5;
                break;
            case 13:
                gameManager.critDmg += 0.5f;
                break;
            case 14:
                gameManager.chainChance += 5;
                break;
            case 15:
                gameManager.chainTargets += 2;
                break;
            case 16:
                gameManager.chainDmg += 0.1f;
                break;
            case 17:
                gameManager.pierceChance += 5;
                break;
            case 18:
                gameManager.pierceDmg += 0.1f;
                break;
            default:
                break;
        }
        rewardCanvas.SetActive(false);
        gameManager.inMenu = false;
        Time.timeScale = 1;
    }
}
