using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/*
 * ========================================
 * 生活系统测试UI (LifeSystemTestUI.cs)
 * ========================================
 * 
 * 功能说明：
 * 提供完整的生活系统测试界面，用于测试房产、汽车、宠物功能
 * 
 * Unity操作步骤：
 * 1. 在Canvas下创建一个Panel，命名为"LifeSystemTestPanel"
 * 2. 添加此脚本到Panel上
 * 3. 创建UI元素并连接：
 *    - 1个TextMeshPro Text（statusText）用于显示状态
 *    - 13个Button用于操作
 * 4. 将所有UI元素拖到脚本的对应字段
 * 5. 运行游戏，点击按钮测试各项功能
 * 
 * 测试流程：
 * 1. 点击"添加测试资金"获得10万虚拟币
 * 2. 点击"租普通公寓"测试租赁
 * 3. 点击"买数据滑板"测试购买汽车
 * 4. 点击"买数据犬"测试购买宠物
 * 5. 等待一个游戏周期观察自动扣租金和心情加成
 * 6. 点击"刷新显示"查看最新状态
 */

public class LifeSystemTestUI : MonoBehaviour
{
        [Header("UI引用")]
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("房产操作按钮")]
        [SerializeField] private Button rentCapsuleButton;      // 租胶囊公寓
        [SerializeField] private Button rentApartmentButton;    // 租普通公寓
        [SerializeField] private Button buyVillaButton;         // 买独栋别墅
        [SerializeField] private Button buyMansionButton;       // 买数据豪宅

        [Header("汽车操作按钮")]
        [SerializeField] private Button buySkateboardButton;    // 买数据滑板
        [SerializeField] private Button buySportsCarButton;     // 买光速跑车
        [SerializeField] private Button buyQuantumButton;       // 买量子飞行器

        [Header("宠物操作按钮")]
        [SerializeField] private Button buyDogButton;           // 买数据犬
        [SerializeField] private Button buyCatButton;           // 买赛博猫
        [SerializeField] private Button buyDragonButton;        // 买像素龙

        [Header("通用操作按钮")]
        [SerializeField] private Button refreshButton;          // 刷新显示
        [SerializeField] private Button addMoneyButton;         // 添加测试资金
        [SerializeField] private Button showAllButton;          // 显示所有可用项

        void Start()
        {
            InitializeUI();
            SubscribeEvents();
            RefreshDisplay();
        }

        /// <summary>
        /// 初始化UI按钮
        /// </summary>
        private void InitializeUI()
        {
            // 房产按钮
            rentCapsuleButton?.onClick.AddListener(() => RentHousing("rent_capsule"));
            rentApartmentButton?.onClick.AddListener(() => RentHousing("rent_apartment"));
            buyVillaButton?.onClick.AddListener(() => BuyHousing("buy_house"));
            buyMansionButton?.onClick.AddListener(() => BuyHousing("buy_mansion"));

            // 汽车按钮
            buySkateboardButton?.onClick.AddListener(() => BuyVehicle("car_basic"));
            buySportsCarButton?.onClick.AddListener(() => BuyVehicle("car_sport"));
            buyQuantumButton?.onClick.AddListener(() => BuyVehicle("car_luxury"));

            // 宠物按钮
            buyDogButton?.onClick.AddListener(() => BuyPet("pet_dog"));
            buyCatButton?.onClick.AddListener(() => BuyPet("pet_cat"));
            buyDragonButton?.onClick.AddListener(() => BuyPet("pet_dragon"));

            // 通用按钮
            refreshButton?.onClick.AddListener(RefreshDisplay);
            addMoneyButton?.onClick.AddListener(AddTestMoney);
            showAllButton?.onClick.AddListener(ShowAllAvailable);
        }

        /// <summary>
        /// 订阅生活系统事件
        /// </summary>
        private void SubscribeEvents()
        {
            if (LifeSystemManager.Instance != null)
            {
                LifeSystemManager.Instance.OnHousingChanged += OnHousingChanged;
                LifeSystemManager.Instance.OnVehicleBought += OnVehicleBought;
                LifeSystemManager.Instance.OnPetBought += OnPetBought;
                LifeSystemManager.Instance.OnRentPaid += OnRentPaid;
                LifeSystemManager.Instance.OnMoodBonusApplied += OnMoodBonusApplied;
            }
        }

        // ===== 按钮回调 =====

        /// <summary>
        /// 租赁房产
        /// </summary>
        private void RentHousing(string housingId)
        {
            if (LifeSystemManager.Instance.RentHousing(housingId))
            {
                ShowMessage($"成功租赁房产！", Color.green);
                RefreshDisplay();
            }
            else
            {
                ShowMessage($"租赁失败！检查等级和虚拟币", Color.red);
            }
        }

        /// <summary>
        /// 购买房产
        /// </summary>
        private void BuyHousing(string housingId)
        {
            if (LifeSystemManager.Instance.BuyHousing(housingId))
            {
                ShowMessage($"成功购买房产！", Color.green);
                RefreshDisplay();
            }
            else
            {
                ShowMessage($"购买失败！检查等级和虚拟币", Color.red);
            }
        }

        /// <summary>
        /// 购买汽车
        /// </summary>
        private void BuyVehicle(string vehicleId)
        {
            if (LifeSystemManager.Instance.BuyVehicle(vehicleId))
            {
                ShowMessage($"成功购买汽车！", Color.green);
                RefreshDisplay();
            }
            else
            {
                ShowMessage($"购买失败！检查等级和虚拟币", Color.red);
            }
        }

        /// <summary>
        /// 购买宠物
        /// </summary>
        private void BuyPet(string petId)
        {
            if (LifeSystemManager.Instance.BuyPet(petId))
            {
                ShowMessage($"成功购买宠物！", Color.green);
                RefreshDisplay();
            }
            else
            {
                ShowMessage($"购买失败！检查等级和虚拟币", Color.red);
            }
        }

        /// <summary>
        /// 添加测试资金
        /// </summary>
        private void AddTestMoney()
        {
            ResourceManager.Instance.AddVirtualCoin(100000, "测试");
            ShowMessage($"添加100000虚拟币", Color.cyan);
            RefreshDisplay();
        }

        /// <summary>
        /// 显示所有可用项
        /// </summary>
        private void ShowAllAvailable()
        {
            string info = "=== 所有可用项 ===\n\n";

            // 房产
            info += "【房产】\n";
            var housings = LifeSystemManager.Instance.GetAvailableHousings();
            foreach (var h in housings)
            {
                info += h.GetDescription() + "\n\n";
            }

            // 汽车
            info += "\n【汽车】\n";
            var vehicles = LifeSystemManager.Instance.GetAvailableVehicles();
            foreach (var v in vehicles)
            {
                info += v.GetDescription() + "\n\n";
            }

            // 宠物
            info += "\n【宠物】\n";
            var pets = LifeSystemManager.Instance.GetAvailablePets();
            foreach (var p in pets)
            {
                info += p.GetDescription() + "\n\n";
            }

            Debug.Log(info);
            ShowMessage("查看Console以查看所有可用项", Color.yellow);
        }

        // ===== 事件回调 =====

        private void OnHousingChanged(HousingData housing)
        {
            Debug.Log($"[UI] 房产变化: {housing.housingName}");
            RefreshDisplay();
        }

        private void OnVehicleBought(VehicleData vehicle)
        {
            Debug.Log($"[UI] 购买汽车: {vehicle.vehicleName}");
            RefreshDisplay();
        }

        private void OnPetBought(PetData pet)
        {
            Debug.Log($"[UI] 购买宠物: {pet.petName}");
            RefreshDisplay();
        }

        private void OnRentPaid(float amount)
        {
            Debug.Log($"[UI] 支付租金: {amount}币");
            ShowMessage($"支付租金: {amount}币", Color.yellow);
        }

        private void OnMoodBonusApplied(float amount)
        {
            Debug.Log($"[UI] 心情加成: +{amount}");
            ShowMessage($"心情加成: +{amount}", Color.green);
        }

        // ===== 显示更新 =====

        /// <summary>
        /// 刷新显示
        /// </summary>
        private void RefreshDisplay()
        {
            if (statusText == null) return;

            string display = "=== 生活系统状态 ===\n\n";

            // 玩家基本信息
            display += "【玩家信息】\n";
            display += $"等级: Lv.{ResourceManager.Instance.GetPlayerLevel()}\n";
            display += $"虚拟币: {ResourceManager.Instance.GetVirtualCoin():F0}\n";
            display += $"心情值: {ResourceManager.Instance.GetMoodValue()}\n\n";

            // 当前房产
            var housing = LifeSystemManager.Instance.GetCurrentHousing();
            display += "【当前房产】\n";
            if (housing != null)
            {
                display += $"名称: {housing.housingName}\n";
                display += $"类型: {(housing.housingType == HousingType.Rental ? "租赁" : "已购买")}\n";
                if (housing.housingType == HousingType.Rental)
                {
                    display += $"租金: {housing.rentPer5Min}币/5分钟\n";
                }
                display += $"心情加成: +{housing.moodBonusPer5Min}/5分钟\n";
                display += $"家具槽位: {housing.furnitureSlots}个\n";
            }
            else
            {
                display += "无\n";
            }
            display += "\n";

            // 拥有的汽车
            var vehicles = LifeSystemManager.Instance.GetOwnedVehicles();
            display += $"【拥有的汽车】({vehicles.Count}辆)\n";
            if (vehicles.Count > 0)
            {
                var activeVehicle = LifeSystemManager.Instance.GetActiveVehicle();
                foreach (var v in vehicles)
                {
                    string active = (activeVehicle != null && v.vehicleId == activeVehicle.vehicleId) ? " [使用中]" : "";
                    display += $"· {v.vehicleName} (速度{v.speedBonus}x){active}\n";
                }
            }
            else
            {
                display += "无\n";
            }
            display += "\n";

            // 拥有的宠物
            var pets = LifeSystemManager.Instance.GetOwnedPets();
            display += $"【拥有的宠物】({pets.Count}只)\n";
            if (pets.Count > 0)
            {
                float totalPetMood = 0;
                foreach (var p in pets)
                {
                    display += $"· {p.petName} (+{p.moodBonusPer5Min}/5分钟)\n";
                    totalPetMood += p.moodBonusPer5Min;
                }
                display += $"总心情加成: +{totalPetMood}/5分钟\n";
            }
            else
            {
                display += "无\n";
            }
            display += "\n";

            // 统计信息
            var stats = LifeSystemManager.Instance.GetStatistics();
            display += "【统计数据】\n";
            display += $"累计租金: {stats.totalRent:F0}币\n";
            display += $"累计心情: +{stats.totalMood:F0}\n\n";

            // 周期信息
            display += "【每周期收支】\n";
            float expense = 0;

            // 房产租金
            if (housing != null && housing.housingType == HousingType.Rental)
            {
                expense += housing.rentPer5Min;
                display += $"房产租金: -{housing.rentPer5Min}币\n";
            }

            // 心情加成
            float moodBonus = 0;
            if (housing != null)
            {
                moodBonus += housing.moodBonusPer5Min;
            }
            foreach (var p in pets)
            {
                moodBonus += p.moodBonusPer5Min;
            }
            if (moodBonus > 0)
            {
                display += $"心情加成: +{moodBonus}\n";
            }

            display += $"\n净支出: {expense:F0}币/5分钟 ({expense * 12:F0}币/小时)";

            statusText.text = display;
        }

        /// <summary>
        /// 显示临时消息
        /// </summary>
        private void ShowMessage(string message, Color color)
        {
            Debug.Log($"[LifeSystemTestUI] {message}");
            // TODO: 可以添加临时提示UI
        }

        void OnDestroy()
        {
            // 取消订阅事件
            if (LifeSystemManager.Instance != null)
            {
                LifeSystemManager.Instance.OnHousingChanged -= OnHousingChanged;
                LifeSystemManager.Instance.OnVehicleBought -= OnVehicleBought;
                LifeSystemManager.Instance.OnPetBought -= OnPetBought;
                LifeSystemManager.Instance.OnRentPaid -= OnRentPaid;
                LifeSystemManager.Instance.OnMoodBonusApplied -= OnMoodBonusApplied;
            }
        }
    
}
