using System;
using TacticalRPG.Core.Modules.Config;
using TacticalRPG.Implementation.Modules.Config;

namespace TacticalRPG.Implementation.Modules.Battle
{
    /// <summary>
    /// 战斗系统配置
    /// </summary>
    public class BattleConfig : ConfigBase
    {
        private const string KEY_SPEED_RANDOM_FACTOR = "SpeedRandomFactor";
        private const string KEY_MAX_TURNS = "MaxTurns";
        private const string KEY_CRITICAL_HIT_CHANCE = "CriticalHitChance";
        private const string KEY_CRITICAL_HIT_DAMAGE_MULTIPLIER = "CriticalHitDamageMultiplier";
        private const string KEY_ENABLE_FRIENDLY_FIRE = "EnableFriendlyFire";
        private const string KEY_DEFAULT_ACTION_POINTS = "DefaultActionPoints";
        private const string KEY_DEFAULT_MOVEMENT_POINTS = "DefaultMovementPoints";

        /// <summary>
        /// 模块ID
        /// </summary>
        public const string MODULE_ID = "battle";

        /// <summary>
        /// 获取速度随机因子（影响回合顺序的随机性）
        /// </summary>
        public float SpeedRandomFactor => GetValue<float>(KEY_SPEED_RANDOM_FACTOR, 0.05f);

        /// <summary>
        /// 设置速度随机因子
        /// </summary>
        /// <param name="value">随机因子值（0-1之间）</param>
        public void SetSpeedRandomFactor(float value)
        {
            SetValue(KEY_SPEED_RANDOM_FACTOR, Math.Clamp(value, 0f, 1f));
        }

        /// <summary>
        /// 获取最大回合数（防止战斗无限进行）
        /// </summary>
        public int MaxTurns => GetValue<int>(KEY_MAX_TURNS, 100);

        /// <summary>
        /// 设置最大回合数
        /// </summary>
        /// <param name="value">回合数</param>
        public void SetMaxTurns(int value)
        {
            SetValue(KEY_MAX_TURNS, Math.Max(1, value));
        }

        /// <summary>
        /// 获取暴击几率
        /// </summary>
        public float CriticalHitChance => GetValue<float>(KEY_CRITICAL_HIT_CHANCE, 0.05f);

        /// <summary>
        /// 设置暴击几率
        /// </summary>
        /// <param name="value">暴击几率（0-1之间）</param>
        public void SetCriticalHitChance(float value)
        {
            SetValue(KEY_CRITICAL_HIT_CHANCE, Math.Clamp(value, 0f, 1f));
        }

        /// <summary>
        /// 获取暴击伤害倍率
        /// </summary>
        public float CriticalHitDamageMultiplier => GetValue<float>(KEY_CRITICAL_HIT_DAMAGE_MULTIPLIER, 1.5f);

        /// <summary>
        /// 设置暴击伤害倍率
        /// </summary>
        /// <param name="value">伤害倍率</param>
        public void SetCriticalHitDamageMultiplier(float value)
        {
            SetValue(KEY_CRITICAL_HIT_DAMAGE_MULTIPLIER, Math.Max(1f, value));
        }

        /// <summary>
        /// 获取是否启用友军伤害
        /// </summary>
        public bool EnableFriendlyFire => GetValue<bool>(KEY_ENABLE_FRIENDLY_FIRE, false);

        /// <summary>
        /// 设置是否启用友军伤害
        /// </summary>
        /// <param name="value">是否启用</param>
        public void SetEnableFriendlyFire(bool value)
        {
            SetValue(KEY_ENABLE_FRIENDLY_FIRE, value);
        }

        /// <summary>
        /// 获取默认行动点数
        /// </summary>
        public int DefaultActionPoints => GetValue<int>(KEY_DEFAULT_ACTION_POINTS, 3);

        /// <summary>
        /// 设置默认行动点数
        /// </summary>
        /// <param name="value">点数</param>
        public void SetDefaultActionPoints(int value)
        {
            SetValue(KEY_DEFAULT_ACTION_POINTS, Math.Max(1, value));
        }

        /// <summary>
        /// 获取默认移动点数
        /// </summary>
        public int DefaultMovementPoints => GetValue<int>(KEY_DEFAULT_MOVEMENT_POINTS, 5);

        /// <summary>
        /// 设置默认移动点数
        /// </summary>
        /// <param name="value">点数</param>
        public void SetDefaultMovementPoints(int value)
        {
            SetValue(KEY_DEFAULT_MOVEMENT_POINTS, Math.Max(0, value));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BattleConfig()
            : base(MODULE_ID, "战斗系统配置", new Version(1, 0, 0))
        {
        }

        /// <summary>
        /// 初始化默认值
        /// </summary>
        protected override void InitDefaultValues()
        {
            SetValue(KEY_SPEED_RANDOM_FACTOR, 0.05f);
            SetValue(KEY_MAX_TURNS, 100);
            SetValue(KEY_CRITICAL_HIT_CHANCE, 0.05f);
            SetValue(KEY_CRITICAL_HIT_DAMAGE_MULTIPLIER, 1.5f);
            SetValue(KEY_ENABLE_FRIENDLY_FIRE, false);
            SetValue(KEY_DEFAULT_ACTION_POINTS, 3);
            SetValue(KEY_DEFAULT_MOVEMENT_POINTS, 5);
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

            if (SpeedRandomFactor < 0 || SpeedRandomFactor > 1)
                return (false, "速度随机因子必须在0到1之间");

            if (MaxTurns <= 0)
                return (false, "最大回合数必须大于0");

            if (CriticalHitChance < 0 || CriticalHitChance > 1)
                return (false, "暴击几率必须在0到1之间");

            if (CriticalHitDamageMultiplier < 1)
                return (false, "暴击伤害倍率必须大于等于1");

            if (DefaultActionPoints <= 0)
                return (false, "默认行动点数必须大于0");

            if (DefaultMovementPoints < 0)
                return (false, "默认移动点数不能为负数");

            return (true, string.Empty);
        }
    }
}