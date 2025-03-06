using System;
using System.Collections.Generic;

namespace TacticalRPG.Core.Modules.Battle
{
    /// <summary>
    /// 战斗行动结果类
    /// </summary>
    public class BattleActionResult
    {
        /// <summary>
        /// 行动是否成功
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// 行动类型
        /// </summary>
        public BattleActionType ActionType { get; }

        /// <summary>
        /// 行动发起者ID
        /// </summary>
        public Guid ActorId { get; }

        /// <summary>
        /// 目标ID列表
        /// </summary>
        public IReadOnlyList<Guid> TargetIds { get; }

        /// <summary>
        /// 伤害值列表，与目标ID列表一一对应
        /// </summary>
        public IReadOnlyList<int> DamageValues { get; }

        /// <summary>
        /// 是否暴击列表，与目标ID列表一一对应
        /// </summary>
        public IReadOnlyList<bool> IsCriticalHits { get; }

        /// <summary>
        /// 是否命中列表，与目标ID列表一一对应
        /// </summary>
        public IReadOnlyList<bool> IsHits { get; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isSuccess">行动是否成功</param>
        /// <param name="actionType">行动类型</param>
        /// <param name="actorId">行动发起者ID</param>
        /// <param name="targetIds">目标ID列表</param>
        /// <param name="damageValues">伤害值列表</param>
        /// <param name="isCriticalHits">是否暴击列表</param>
        /// <param name="isHits">是否命中列表</param>
        /// <param name="message">消息</param>
        public BattleActionResult(
            bool isSuccess,
            BattleActionType actionType,
            Guid actorId,
            IReadOnlyList<Guid> targetIds,
            IReadOnlyList<int> damageValues,
            IReadOnlyList<bool> isCriticalHits,
            IReadOnlyList<bool> isHits,
            string message)
        {
            IsSuccess = isSuccess;
            ActionType = actionType;
            ActorId = actorId;
            TargetIds = targetIds ?? new List<Guid>();
            DamageValues = damageValues ?? new List<int>();
            IsCriticalHits = isCriticalHits ?? new List<bool>();
            IsHits = isHits ?? new List<bool>();
            Message = message ?? string.Empty;
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        /// <param name="actionType">行动类型</param>
        /// <param name="actorId">行动发起者ID</param>
        /// <param name="message">失败消息</param>
        /// <returns>失败的战斗行动结果</returns>
        public static BattleActionResult CreateFailure(BattleActionType actionType, Guid actorId, string message)
        {
            return new BattleActionResult(
                false,
                actionType,
                actorId,
                new List<Guid>(),
                new List<int>(),
                new List<bool>(),
                new List<bool>(),
                message);
        }
    }
}