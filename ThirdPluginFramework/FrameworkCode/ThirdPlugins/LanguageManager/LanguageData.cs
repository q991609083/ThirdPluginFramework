using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LanguageSetting
{
    public class LanguageData : MonoBehaviour
    {
        public static Dictionary<string, List<string>> textTranslateData = new Dictionary<string, List<string>>
        {
            {"狠心放弃", new List<string>(){ "No，thanks" } },
            {"再开一个", new List<string>(){ "One more" } },
            {"再开三个", new List<string>(){ "Three more" } },
            {"限量", new List<string>(){ "Rare " } },
            {"超跑", new List<string>(){ "Sports Car" } },
            {"超跑：", new List<string>(){ "Sports Car: " } },
            {"车灯：", new List<string>(){ "Lamp: " } },
            {"车轮：", new List<string>(){ "Wheel: " } },
            {"车漆：", new List<string>(){ "Painting: " } },
            {"忍痛放弃", new List<string>(){ "No，thanks" } },
            {"点击永久", new List<string>(){ "Get it" } },
            {"长按开始游戏", new List<string>(){ "Hold to start" } },
            {"排行", new List<string>(){ "Rank" } },
            {"圈数", new List<string>(){ "Lap:" } },
            {"通关条件", new List<string>(){ "Target" } },
            {"点击开始游戏", new List<string>(){ "Tap to start" } },
            {"精英挑战", new List<string>(){ "Challenge" } },
            {"商店", new List<string>(){ "Shop" } },
            {" 商店", new List<string>(){ " Shop" } },
            {"氮气", new List<string>(){ "N2O" } },
            {"时速", new List<string>(){ "Speed" } },
            {"加速度", new List<string>(){ "Acc" } },
            {"免费", new List<string>(){ "Free" } },
            {"突破上限", new List<string>(){ "Promote" } },
            {"免费抽奖", new List<string>(){ "Free" } },
            {"闯关失败", new List<string>(){ "YOU LOSE" } },
            {"点击继续", new List<string>(){ "Continue" } },
            {"补领50金", new List<string>(){ "Claim $50" } },
            {"跳过此关", new List<string>(){ "Skip" } },
            {"闯关成功", new List<string>(){ "YOU WIN" } },
            {"双倍领取", new List<string>(){ "Claim X2" } },
            {"我来试试", new List<string>(){ "Try it" } },
            {"下次再试", new List<string>(){ "No，thanks" } },
            {"使用中", new List<string>(){ "Using" } },
            {"车辆", new List<string>(){ "Car" } },
            {"车轮", new List<string>(){ "Wheel" } },
            {"轮胎", new List<string>(){ "Wheel" } },
            {"车漆", new List<string>(){ "Painting" } },
            {"喷漆", new List<string>(){ "Painting" } },
            {"车灯", new List<string>(){ "Lamp" } },
            {"灯光", new List<string>(){ "Lamp" } },
            {"已获得", new List<string>(){ "got it" } },
            {"签到皮肤", new List<string>(){ "Sign Skin" } },
            {"今日已领取奖励！", new List<string>(){ "Today has been signed!" } },
            {"宝箱是空的，再来一次吧！", new List<string>(){ "Empty Box,Try Again!" } },
            {"恭喜获得新皮肤", new List<string>(){ "Congratulate to get new skin" } },
            {"获得限定皮肤试用1次", new List<string>(){ "Get 1 skin trial" } },
            {"已达上限", new List<string>(){ "Reach the limit" } },
            {"恭喜奖励，金钱 +10", new List<string>(){ "Congratulate to get $10" } },
            {"恭喜奖励，金钱 +5", new List<string>(){ "Congratulate to get $5" } },
            {"恭喜奖励，金钱 +50", new List<string>(){ "Congratulate to get $50" } },
            {"恭喜奖励，金钱 +25", new List<string>(){ "Congratulate to get $25" } },
            {"宝箱已打开！", new List<string>(){ "It's opened!" } },
            {"暂不能打开！", new List<string>(){ "Can't open now" } },
            {"宝箱已全部打开", new List<string>(){ "The boxs are all opened" } },
            {"已获取", new List<string>(){ "Got it" } },
            {"奖励领取成功，金钱+20", new List<string>(){ "Money adds by $20" } },
            {"兑换失败", new List<string>(){ "Exchange failed" } },
            {"已获得此皮肤", new List<string>(){ "Get it repeatedly" } },
            {"金钱不够了", new List<string>(){ "No money" } },
            {"完成观看广告后，才可获得奖励", new List<string>(){ "Get a reward after reading the AD" } },
            {"暂无合适的广告，请稍后再试", new List<string>(){ "No ADs" } },
            {"已领取", new List<string>(){ "Got" } },



            {"点击任意位置继续", new List<string>(){ "Click anywhere to continue" } },

            

            {"第[x]天", new List<string>(){ "Day [x]" } },
            {"第[x]关", new List<string>(){ "Level [x]" } },
        };

        public static List<string> imageTranslateData = new List<string>()
        {
            "13131313",
            "16478413",
            "Icon02",
            "Icon06qiandao",
            "nextGet",
            "qian",
            "shao",
            "yi",
            "zms",
            "zma",

        };
    }
}

