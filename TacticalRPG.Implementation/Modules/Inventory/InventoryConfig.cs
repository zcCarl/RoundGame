using System;
using TacticalRPG.Core.Modules.Config;
using TacticalRPG.Core.Modules.Inventory;
using TacticalRPG.Implementation.Modules.Config;

namespace TacticalRPG.Implementation.Modules.Inventory
{
    /// <summary>
    /// 背包系统配置
    /// </summary>
    public class InventoryConfig : ConfigBase
    {
        private const string KEY_DEFAULT_CAPACITY = "DefaultCapacity";
        private const string KEY_MAX_WEIGHT = "MaxWeight";
        private const string KEY_ENABLE_WEIGHT_LIMIT = "EnableWeightLimit";
        private const string KEY_DEFAULT_SORT_STRATEGY = "DefaultSortStrategy";
        private const string KEY_AUTO_MERGE_STACKS = "AutoMergeStacks";
        private const string KEY_MAX_STACK_SIZE = "MaxStackSize";

        /// <summary>
        /// 模块ID
        /// </summary>
        public const string MODULE_ID = "inventory";

        /// <summary>
        /// 获取默认背包容量
        /// </summary>
        public int DefaultCapacity => GetValue<int>(KEY_DEFAULT_CAPACITY, 20);

        /// <summary>
        /// 设置默认背包容量
        /// </summary>
        /// <param name="value">容量值</param>
        public void SetDefaultCapacity(int value)
        {
            SetValue(KEY_DEFAULT_CAPACITY, Math.Max(1, value));
        }

        /// <summary>
        /// 获取最大重量限制
        /// </summary>
        public float MaxWeight => GetValue<float>(KEY_MAX_WEIGHT, 100f);

        /// <summary>
        /// 设置最大重量限制
        /// </summary>
        /// <param name="value">重量值</param>
        public void SetMaxWeight(float value)
        {
            SetValue(KEY_MAX_WEIGHT, Math.Max(0f, value));
        }

        /// <summary>
        /// 获取是否启用重量限制
        /// </summary>
        public bool EnableWeightLimit => GetValue<bool>(KEY_ENABLE_WEIGHT_LIMIT, true);

        /// <summary>
        /// 设置是否启用重量限制
        /// </summary>
        /// <param name="value">是否启用</param>
        public void SetEnableWeightLimit(bool value)
        {
            SetValue(KEY_ENABLE_WEIGHT_LIMIT, value);
        }

        /// <summary>
        /// 获取默认排序策略
        /// </summary>
        public string DefaultSortStrategy => GetValue<string>(KEY_DEFAULT_SORT_STRATEGY, "type");

        /// <summary>
        /// 设置默认排序策略
        /// </summary>
        /// <param name="value">排序策略</param>
        public void SetDefaultSortStrategy(string value)
        {
            SetValue(KEY_DEFAULT_SORT_STRATEGY, string.IsNullOrEmpty(value) ? "type" : value);
        }

        /// <summary>
        /// 获取是否自动合并堆叠
        /// </summary>
        public bool AutoMergeStacks => GetValue<bool>(KEY_AUTO_MERGE_STACKS, true);

        /// <summary>
        /// 设置是否自动合并堆叠
        /// </summary>
        /// <param name="value">是否自动合并</param>
        public void SetAutoMergeStacks(bool value)
        {
            SetValue(KEY_AUTO_MERGE_STACKS, value);
        }

        /// <summary>
        /// 获取最大堆叠数量
        /// </summary>
        public int MaxStackSize => GetValue<int>(KEY_MAX_STACK_SIZE, 99);

        /// <summary>
        /// 设置最大堆叠数量
        /// </summary>
        /// <param name="value">堆叠数量</param>
        public void SetMaxStackSize(int value)
        {
            SetValue(KEY_MAX_STACK_SIZE, Math.Max(1, value));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public InventoryConfig()
            : base(MODULE_ID, "背包系统配置", new Version(1, 0, 0))
        {
        }

        /// <summary>
        /// 初始化默认值
        /// </summary>
        protected override void InitDefaultValues()
        {
            SetValue(KEY_DEFAULT_CAPACITY, 20);
            SetValue(KEY_MAX_WEIGHT, 100f);
            SetValue(KEY_ENABLE_WEIGHT_LIMIT, true);
            SetValue(KEY_DEFAULT_SORT_STRATEGY, "type");
            SetValue(KEY_AUTO_MERGE_STACKS, true);
            SetValue(KEY_MAX_STACK_SIZE, 99);
        }

        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <returns>验证结果</returns>
        public override (bool IsValid, string ErrorMessage) Validate()
        {
            var baseResult = base.Validate();
            if (!baseResult.IsValid)
                return baseResult;

            if (DefaultCapacity <= 0)
                return (false, "默认背包容量必须大于0");

            if (MaxWeight < 0)
                return (false, "最大重量限制不能为负数");

            if (MaxStackSize <= 0)
                return (false, "最大堆叠数量必须大于0");

            return (true, string.Empty);
        }
    }
}