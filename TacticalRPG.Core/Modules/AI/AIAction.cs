using System;

namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// AI行动类，表示AI决策的行动
    /// </summary>
    public class AIAction
    {
        /// <summary>
        /// 行动类型
        /// </summary>
        public AIActionType ActionType { get; set; }

        /// <summary>
        /// 目标X坐标（移动、技能目标等）
        /// </summary>
        public int TargetX { get; set; }

        /// <summary>
        /// 目标Y坐标（移动、技能目标等）
        /// </summary>
        public int TargetY { get; set; }

        /// <summary>
        /// 目标角色ID（攻击、技能、物品使用等）
        /// </summary>
        public Guid TargetCharacterId { get; set; }

        /// <summary>
        /// 技能ID（使用技能时）
        /// </summary>
        public Guid SkillId { get; set; }

        /// <summary>
        /// 物品ID（使用物品时）
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// 优先级，数值越高优先级越高
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 创建移动行动
        /// </summary>
        /// <param name="targetX">目标X坐标</param>
        /// <param name="targetY">目标Y坐标</param>
        /// <param name="priority">优先级</param>
        /// <returns>AI行动</returns>
        public static AIAction CreateMoveAction(int targetX, int targetY, int priority = 0)
        {
            return new AIAction
            {
                ActionType = AIActionType.Move,
                TargetX = targetX,
                TargetY = targetY,
                Priority = priority
            };
        }

        /// <summary>
        /// 创建攻击行动
        /// </summary>
        /// <param name="targetCharacterId">目标角色ID</param>
        /// <param name="priority">优先级</param>
        /// <returns>AI行动</returns>
        public static AIAction CreateAttackAction(Guid targetCharacterId, int priority = 0)
        {
            return new AIAction
            {
                ActionType = AIActionType.Attack,
                TargetCharacterId = targetCharacterId,
                Priority = priority
            };
        }

        /// <summary>
        /// 创建技能使用行动
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="targetX">目标X坐标</param>
        /// <param name="targetY">目标Y坐标</param>
        /// <param name="priority">优先级</param>
        /// <returns>AI行动</returns>
        public static AIAction CreateSkillAction(Guid skillId, int targetX, int targetY, int priority = 0)
        {
            return new AIAction
            {
                ActionType = AIActionType.UseSkill,
                SkillId = skillId,
                TargetX = targetX,
                TargetY = targetY,
                Priority = priority
            };
        }

        /// <summary>
        /// 创建物品使用行动
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="targetCharacterId">目标角色ID</param>
        /// <param name="priority">优先级</param>
        /// <returns>AI行动</returns>
        public static AIAction CreateItemAction(Guid itemId, Guid targetCharacterId, int priority = 0)
        {
            return new AIAction
            {
                ActionType = AIActionType.UseItem,
                ItemId = itemId,
                TargetCharacterId = targetCharacterId,
                Priority = priority
            };
        }

        /// <summary>
        /// 创建等待行动
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <returns>AI行动</returns>
        public static AIAction CreateWaitAction(int priority = 0)
        {
            return new AIAction
            {
                ActionType = AIActionType.Wait,
                Priority = priority
            };
        }

        /// <summary>
        /// 创建结束回合行动
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <returns>AI行动</returns>
        public static AIAction CreateEndTurnAction(int priority = 0)
        {
            return new AIAction
            {
                ActionType = AIActionType.EndTurn,
                Priority = priority
            };
        }
    }
}