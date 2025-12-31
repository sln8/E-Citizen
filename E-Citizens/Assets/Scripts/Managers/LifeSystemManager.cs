using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * ========================================
 * 生活系统管理器 (LifeSystemManager.cs)
 * ========================================
 * 
 * 功能说明：
 * 管理玩家的生活系统，包括房产、汽车、宠物
 * 
 * 主要功能：
 * 1. 管理玩家拥有的房产（租赁或购买）
 * 2. 管理玩家拥有的汽车
 * 3. 管理玩家拥有的宠物
 * 4. 处理租金自动扣除
 * 5. 处理心情值加成
 * 6. 与资源管理器集成
 * 
 * Unity操作步骤：
 * 1. 在Hierarchy中找到GameManager对象
 * 2. 添加LifeSystemManager组件
 * 3. 运行游戏，管理器会自动初始化
 * 
 * 使用示例：
 * // 租赁房产
 * LifeSystemManager.Instance.RentHousing("rent_apartment");
 * 
 * // 购买汽车
 * LifeSystemManager.Instance.BuyVehicle("car_sport");
 * 
 * // 购买宠物
 * LifeSystemManager.Instance.BuyPet("pet_cat");
 * 
 * 代码结构：
 * - 单例模式
 * - 事件系统（房产变化、汽车购买、宠物购买等）
 * - 与GameTimerManager集成（自动扣租金、心情加成）
 * - 数据持久化支持
 */

public class LifeSystemManager : MonoBehaviour
{
        // ===== 单例 =====
        private static LifeSystemManager instance;
        public static LifeSystemManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<LifeSystemManager>();
                    if (instance == null)
                    {
                        Debug.LogError("[LifeSystemManager] 场景中没有找到LifeSystemManager！");
                    }
                }
                return instance;
            }
        }

        // ===== 数据库：所有可用的选项 =====
        [Header("数据库")]
        [SerializeField] private List<HousingData> availableHousings = new List<HousingData>();
        [SerializeField] private List<VehicleData> availableVehicles = new List<VehicleData>();
        [SerializeField] private List<PetData> availablePets = new List<PetData>();

        // ===== 玩家当前拥有的物品 =====
        [Header("玩家拥有")]
        [SerializeField] private HousingData currentHousing;     // 当前房产
        [SerializeField] private List<VehicleData> ownedVehicles = new List<VehicleData>();
        [SerializeField] private List<PetData> ownedPets = new List<PetData>();
        [SerializeField] private VehicleData activeVehicle;      // 当前使用的汽车

        // ===== 统计数据 =====
        [Header("统计")]
        [SerializeField] private float totalRentPaid;           // 累计支付的租金
        [SerializeField] private float totalMoodFromLife;       // 从生活系统获得的总心情值

        // ===== 事件 =====
        public event Action<HousingData> OnHousingChanged;      // 房产变化
        public event Action<VehicleData> OnVehicleBought;       // 购买汽车
        public event Action<VehicleData> OnVehicleChanged;      // 切换汽车
        public event Action<PetData> OnPetBought;               // 购买宠物
        public event Action<float> OnRentPaid;                  // 支付租金
        public event Action<float> OnMoodBonusApplied;          // 应用心情加成

        // ===== 初始化 =====
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            InitializeLifeSystem();
        }

        /// <summary>
        /// 初始化生活系统
        /// </summary>
        private void InitializeLifeSystem()
        {
            Debug.Log("[LifeSystemManager] 初始化生活系统...");

            // 初始化可用选项数据库
            InitializeDatabase();

            // 订阅游戏周期事件
            if (GameTimerManager.Instance != null)
            {
                GameTimerManager.Instance.OnGameTickStart += OnGameTick;
                Debug.Log("[LifeSystemManager] 已订阅游戏周期事件");
            }

            // 如果玩家还没有房产，自动分配胶囊公寓
            if (currentHousing == null && availableHousings.Count > 0)
            {
                currentHousing = availableHousings[0].Clone(); // 默认胶囊公寓
                Debug.Log($"[LifeSystemManager] 自动分配初始房产: {currentHousing.housingName}");
            }

            Debug.Log("[LifeSystemManager] 生活系统初始化完成");
        }

        /// <summary>
        /// 初始化数据库（根据游戏设计文档）
        /// </summary>
        private void InitializeDatabase()
        {
            // 清空现有数据
            availableHousings.Clear();
            availableVehicles.Clear();
            availablePets.Clear();

            // ===== 初始化房产数据库 =====
            availableHousings.Add(HousingData.CreateRentalHousing(
                "rent_capsule", "胶囊公寓", 2, 0,
                "capsule_interior.png", "capsule_exterior.png", 2,
                HousingTier.Basic, 1));

            availableHousings.Add(HousingData.CreateRentalHousing(
                "rent_apartment", "普通公寓", 5, 1,
                "apartment_interior.png", "apartment_exterior.png", 5,
                HousingTier.Standard, 1));

            availableHousings.Add(HousingData.CreatePurchaseHousing(
                "buy_house", "独栋别墅", 50000, 3,
                "villa_interior.png", "villa_exterior.png", 15,
                HousingTier.Luxury, 0, 10));

            availableHousings.Add(HousingData.CreatePurchaseHousing(
                "buy_mansion", "数据豪宅", 500000, 10,
                "mansion_interior.png", "mansion_exterior.png", 30,
                HousingTier.Premium, 9.99f, 20));

            // ===== 初始化汽车数据库 =====
            availableVehicles.Add(VehicleData.CreateVehicle(
                "car_basic", "数据滑板", 500, 1.1f, 5,
                "skateboard.png", VehicleTier.Basic, 0, 1));

            availableVehicles.Add(VehicleData.CreateVehicle(
                "car_sport", "光速跑车", 10000, 1.5f, 20,
                "sportscar.png", VehicleTier.Sport, 0, 5));

            availableVehicles.Add(VehicleData.CreateVehicle(
                "car_luxury", "量子飞行器", 100000, 2.0f, 50,
                "quantum_vehicle.png", VehicleTier.Luxury, 4.99f, 15));

            // ===== 初始化宠物数据库 =====
            availablePets.Add(PetData.CreatePet(
                "pet_dog", "数据犬", 1000, 0.5f,
                "data_dog.json", PetTier.Common, 0, 1));

            availablePets.Add(PetData.CreatePet(
                "pet_cat", "赛博猫", 1500, 0.8f,
                "cyber_cat.json", PetTier.Common, 0, 1));

            availablePets.Add(PetData.CreatePet(
                "pet_dragon", "像素龙", 50000, 2.0f,
                "pixel_dragon.json", PetTier.Legendary, 2.99f, 10));

            Debug.Log($"[LifeSystemManager] 数据库初始化完成: {availableHousings.Count}个房产, " +
                     $"{availableVehicles.Count}个汽车, {availablePets.Count}个宠物");
        }

        // ===== 房产管理 =====

        /// <summary>
        /// 租赁房产
        /// </summary>
        /// <param name="housingId">房产ID</param>
        /// <returns>是否成功租赁</returns>
        public bool RentHousing(string housingId)
        {
            var housing = availableHousings.Find(h => h.housingId == housingId);
            if (housing == null)
            {
                Debug.LogError($"[LifeSystemManager] 房产不存在: {housingId}");
                return false;
            }

            if (housing.housingType != HousingType.Rental)
            {
                Debug.LogError($"[LifeSystemManager] {housing.housingName} 不是租赁类型");
                return false;
            }

            // 检查条件
            int playerLevel = ResourceManager.Instance.GetPlayerLevel();
            float virtualCoin = ResourceManager.Instance.GetVirtualCoin();

            if (!housing.CanAfford(playerLevel, virtualCoin))
            {
                Debug.LogWarning($"[LifeSystemManager] 无法租赁 {housing.housingName}: " +
                               $"等级{playerLevel}/{housing.requiredLevel}, " +
                               $"虚拟币{virtualCoin}/{housing.rentPer5Min}");
                return false;
            }

            // 如果已有房产，退租
            if (currentHousing != null && currentHousing.housingType == HousingType.Rental)
            {
                Debug.Log($"[LifeSystemManager] 退租原房产: {currentHousing.housingName}");
            }

            // 租赁新房产
            currentHousing = housing.Clone();
            Debug.Log($"[LifeSystemManager] 成功租赁: {currentHousing.housingName}, " +
                     $"租金{currentHousing.rentPer5Min}币/5分钟");

            OnHousingChanged?.Invoke(currentHousing);
            return true;
        }

        /// <summary>
        /// 购买房产
        /// </summary>
        /// <param name="housingId">房产ID</param>
        /// <returns>是否成功购买</returns>
        public bool BuyHousing(string housingId)
        {
            var housing = availableHousings.Find(h => h.housingId == housingId);
            if (housing == null)
            {
                Debug.LogError($"[LifeSystemManager] 房产不存在: {housingId}");
                return false;
            }

            if (housing.housingType != HousingType.Purchase)
            {
                Debug.LogError($"[LifeSystemManager] {housing.housingName} 不是购买类型");
                return false;
            }

            // 检查条件
            int playerLevel = ResourceManager.Instance.GetPlayerLevel();
            float virtualCoin = ResourceManager.Instance.GetVirtualCoin();

            if (!housing.CanAfford(playerLevel, virtualCoin, !housing.requireRealMoney))
            {
                Debug.LogWarning($"[LifeSystemManager] 无法购买 {housing.housingName}: " +
                               $"等级{playerLevel}/{housing.requiredLevel}, " +
                               $"虚拟币{virtualCoin}/{housing.purchasePrice}");
                return false;
            }

            // TODO: 检查真实货币支付

            // 扣除费用
            if (!ResourceManager.Instance.SpendVirtualCoin(housing.purchasePrice))
            {
                Debug.LogError($"[LifeSystemManager] 扣除虚拟币失败");
                return false;
            }

            // 购买房产
            currentHousing = housing.Clone();
            Debug.Log($"[LifeSystemManager] 成功购买: {currentHousing.housingName}, " +
                     $"花费{housing.purchasePrice}币");

            OnHousingChanged?.Invoke(currentHousing);
            return true;
        }

        /// <summary>
        /// 获取当前房产
        /// </summary>
        /// <returns>当前房产数据</returns>
        public HousingData GetCurrentHousing()
        {
            return currentHousing;
        }

        /// <summary>
        /// 获取所有可用房产
        /// </summary>
        /// <returns>房产列表</returns>
        public List<HousingData> GetAvailableHousings()
        {
            return new List<HousingData>(availableHousings);
        }

        // ===== 汽车管理 =====

        /// <summary>
        /// 购买汽车
        /// </summary>
        /// <param name="vehicleId">汽车ID</param>
        /// <returns>是否成功购买</returns>
        public bool BuyVehicle(string vehicleId)
        {
            var vehicle = availableVehicles.Find(v => v.vehicleId == vehicleId);
            if (vehicle == null)
            {
                Debug.LogError($"[LifeSystemManager] 汽车不存在: {vehicleId}");
                return false;
            }

            // 检查是否已拥有
            if (ownedVehicles.Exists(v => v.vehicleId == vehicleId))
            {
                Debug.LogWarning($"[LifeSystemManager] 已拥有 {vehicle.vehicleName}");
                return false;
            }

            // 检查条件
            int playerLevel = ResourceManager.Instance.GetPlayerLevel();
            float virtualCoin = ResourceManager.Instance.GetVirtualCoin();

            if (!vehicle.CanAfford(playerLevel, virtualCoin, !vehicle.requireRealMoney))
            {
                Debug.LogWarning($"[LifeSystemManager] 无法购买 {vehicle.vehicleName}: " +
                               $"等级{playerLevel}/{vehicle.requiredLevel}, " +
                               $"虚拟币{virtualCoin}/{vehicle.purchasePrice}");
                return false;
            }

            // TODO: 检查真实货币支付

            // 扣除费用
            if (!ResourceManager.Instance.SpendVirtualCoin(vehicle.purchasePrice))
            {
                Debug.LogError($"[LifeSystemManager] 扣除虚拟币失败");
                return false;
            }

            // 购买汽车
            var newVehicle = vehicle.Clone();
            ownedVehicles.Add(newVehicle);

            // 应用购买时的心情加成
            ResourceManager.Instance.ChangeMoodValue(Mathf.RoundToInt(vehicle.moodBonus), "购买汽车");
            totalMoodFromLife += vehicle.moodBonus;

            // 如果是第一辆车，自动设为当前使用
            if (activeVehicle == null)
            {
                activeVehicle = newVehicle;
            }

            Debug.Log($"[LifeSystemManager] 成功购买: {newVehicle.vehicleName}, " +
                     $"花费{vehicle.purchasePrice}币, 获得{vehicle.moodBonus}心情值");

            OnVehicleBought?.Invoke(newVehicle);
            return true;
        }

        /// <summary>
        /// 切换当前使用的汽车
        /// </summary>
        /// <param name="vehicleId">汽车ID</param>
        /// <returns>是否成功切换</returns>
        public bool SetActiveVehicle(string vehicleId)
        {
            var vehicle = ownedVehicles.Find(v => v.vehicleId == vehicleId);
            if (vehicle == null)
            {
                Debug.LogWarning($"[LifeSystemManager] 未拥有此汽车: {vehicleId}");
                return false;
            }

            activeVehicle = vehicle;
            Debug.Log($"[LifeSystemManager] 切换汽车: {activeVehicle.vehicleName}");

            OnVehicleChanged?.Invoke(activeVehicle);
            return true;
        }

        /// <summary>
        /// 获取当前使用的汽车
        /// </summary>
        /// <returns>当前汽车数据</returns>
        public VehicleData GetActiveVehicle()
        {
            return activeVehicle;
        }

        /// <summary>
        /// 获取拥有的所有汽车
        /// </summary>
        /// <returns>汽车列表</returns>
        public List<VehicleData> GetOwnedVehicles()
        {
            return new List<VehicleData>(ownedVehicles);
        }

        /// <summary>
        /// 获取所有可用汽车
        /// </summary>
        /// <returns>汽车列表</returns>
        public List<VehicleData> GetAvailableVehicles()
        {
            return new List<VehicleData>(availableVehicles);
        }

        // ===== 宠物管理 =====

        /// <summary>
        /// 购买宠物
        /// </summary>
        /// <param name="petId">宠物ID</param>
        /// <returns>是否成功购买</returns>
        public bool BuyPet(string petId)
        {
            var pet = availablePets.Find(p => p.petId == petId);
            if (pet == null)
            {
                Debug.LogError($"[LifeSystemManager] 宠物不存在: {petId}");
                return false;
            }

            // 检查是否已拥有
            if (ownedPets.Exists(p => p.petId == petId))
            {
                Debug.LogWarning($"[LifeSystemManager] 已拥有 {pet.petName}");
                return false;
            }

            // 检查条件
            int playerLevel = ResourceManager.Instance.GetPlayerLevel();
            float virtualCoin = ResourceManager.Instance.GetVirtualCoin();

            if (!pet.CanAfford(playerLevel, virtualCoin, !pet.requireRealMoney))
            {
                Debug.LogWarning($"[LifeSystemManager] 无法购买 {pet.petName}: " +
                               $"等级{playerLevel}/{pet.requiredLevel}, " +
                               $"虚拟币{virtualCoin}/{pet.purchasePrice}");
                return false;
            }

            // TODO: 检查真实货币支付

            // 扣除费用
            if (!ResourceManager.Instance.SpendVirtualCoin(pet.purchasePrice))
            {
                Debug.LogError($"[LifeSystemManager] 扣除虚拟币失败");
                return false;
            }

            // 购买宠物
            var newPet = pet.Clone();
            ownedPets.Add(newPet);

            Debug.Log($"[LifeSystemManager] 成功购买: {newPet.petName}, " +
                     $"花费{pet.purchasePrice}币, " +
                     $"每5分钟+{pet.moodBonusPer5Min}心情值");

            OnPetBought?.Invoke(newPet);
            return true;
        }

        /// <summary>
        /// 获取拥有的所有宠物
        /// </summary>
        /// <returns>宠物列表</returns>
        public List<PetData> GetOwnedPets()
        {
            return new List<PetData>(ownedPets);
        }

        /// <summary>
        /// 获取所有可用宠物
        /// </summary>
        /// <returns>宠物列表</returns>
        public List<PetData> GetAvailablePets()
        {
            return new List<PetData>(availablePets);
        }

        // ===== 游戏周期处理 =====

        /// <summary>
        /// 处理游戏周期事件（每5分钟调用一次）
        /// </summary>
        private void OnGameTick()
        {
            Debug.Log("[LifeSystemManager] 处理生活系统周期结算...");

            // 1. 扣除租金
            PayRent();

            // 2. 应用心情加成
            ApplyMoodBonus();

            Debug.Log("[LifeSystemManager] 生活系统周期结算完成");
        }

        /// <summary>
        /// 支付租金
        /// </summary>
        private void PayRent()
        {
            if (currentHousing == null)
            {
                return;
            }

            if (currentHousing.housingType != HousingType.Rental)
            {
                return; // 购买的房产不需要付租金
            }

            float rent = currentHousing.rentPer5Min;
            if (ResourceManager.Instance.CanAfford(rent))
            {
                if (ResourceManager.Instance.SpendVirtualCoin(rent))
                {
                    totalRentPaid += rent;
                    Debug.Log($"[LifeSystemManager] 支付租金: {rent}币 " +
                             $"(累计{totalRentPaid}币)");
                    OnRentPaid?.Invoke(rent);
                }
            }
            else
            {
                Debug.LogWarning($"[LifeSystemManager] 虚拟币不足，无法支付租金{rent}币！");
                // TODO: 处理无法支付租金的情况（可能被驱逐）
            }
        }

        /// <summary>
        /// 应用心情值加成
        /// </summary>
        private void ApplyMoodBonus()
        {
            float totalBonus = 0;

            // 房产加成
            if (currentHousing != null)
            {
                totalBonus += currentHousing.moodBonusPer5Min;
            }

            // 宠物加成
            foreach (var pet in ownedPets)
            {
                totalBonus += pet.moodBonusPer5Min;
            }

            if (totalBonus > 0)
            {
                ResourceManager.Instance.ChangeMoodValue(Mathf.RoundToInt(totalBonus), "生活系统");
                totalMoodFromLife += totalBonus;
                Debug.Log($"[LifeSystemManager] 应用心情加成: +{totalBonus} " +
                         $"(房产+{currentHousing?.moodBonusPer5Min ?? 0}, " +
                         $"宠物+{GetTotalPetMoodBonus()})");
                OnMoodBonusApplied?.Invoke(totalBonus);
            }
        }

        // ===== 辅助方法 =====

        /// <summary>
        /// 获取宠物的总心情加成
        /// </summary>
        /// <returns>总心情加成（每5分钟）</returns>
        private float GetTotalPetMoodBonus()
        {
            float total = 0;
            foreach (var pet in ownedPets)
            {
                total += pet.moodBonusPer5Min;
            }
            return total;
        }

        /// <summary>
        /// 获取当前的速度加成（用于娱乐系统）
        /// </summary>
        /// <returns>速度加成倍数</returns>
        public float GetCurrentSpeedBonus()
        {
            return activeVehicle != null ? activeVehicle.speedBonus : 1.0f;
        }

        /// <summary>
        /// 获取统计数据
        /// </summary>
        public (float totalRent, float totalMood) GetStatistics()
        {
            return (totalRentPaid, totalMoodFromLife);
        }

        // ===== 数据持久化 =====

        /// <summary>
        /// 保存数据（TODO: 实现Firebase持久化）
        /// </summary>
        public void SaveData()
        {
            // TODO: 保存到Firebase
            Debug.Log("[LifeSystemManager] 保存生活系统数据（暂未实现）");
        }

        /// <summary>
        /// 加载数据（TODO: 实现Firebase持久化）
        /// </summary>
        public void LoadData()
        {
            // TODO: 从Firebase加载
            Debug.Log("[LifeSystemManager] 加载生活系统数据（暂未实现）");
        }

        void OnDestroy()
        {
            // 取消订阅
            if (GameTimerManager.Instance != null)
            {
                GameTimerManager.Instance.OnGameTickStart -= OnGameTick;
            }
        }
    }
}
