using System;
using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.AI;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Implementation.Modules.AI
{
    /// <summary>
    /// 防守型AI控制器，优先保护自己和队友
    /// </summary>
    public class DefensiveAIController : BaseAIController
    {
        /// <summary>
        /// 获取AI类型
        /// </summary>
        public override AIType Type => AIType.Defensive;

        /// <summary>
        /// 添加移动行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        protected override void AddMoveActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 如果处于危险位置，优先考虑移动到安全位置，并提高优先级
            if (evaluation.IsInDangerousPosition && evaluation.SafePositions.Count > 0)
            {
                var bestSafePosition = evaluation.SafePositions.OrderByDescending(p => p.Value).First();
                actions.Add(AIAction.CreateMoveAction(bestSafePosition.X, bestSafePosition.Y, 90)); // 提高安全移动的优先级
                return;
            }

            // 如果有需要治疗的队友，尝试移动到队友附近
            if (evaluation.HealableAllies.Count > 0)
            {
                var moveRange = battle.CalculateMoveRange(character.Character.Id);
                var mostInjuredAlly = evaluation.HealableAllies.OrderByDescending(a => a.Priority).First();

                // 找到最接近受伤队友的位置
                var bestPosition = moveRange
                    .OrderBy(p => Math.Abs(p.X - mostInjuredAlly.X) + Math.Abs(p.Y - mostInjuredAlly.Y))
                    .FirstOrDefault();

                if (bestPosition != default)
                {
                    actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 80));
                    return;
                }
            }

            // 否则考虑移动到战术位置，优先选择防御加成高的位置
            if (evaluation.TacticalPositions.Count > 0)
            {
                var bestTacticalPosition = evaluation.TacticalPositions
                    .OrderByDescending(p => p.DefenseBonus)
                    .ThenBy(p => p.DangerLevel)
                    .ThenByDescending(p => p.Value)
                    .First();
                actions.Add(AIAction.CreateMoveAction(bestTacticalPosition.X, bestTacticalPosition.Y, 70));
            }
        }

        /// <summary>
        /// 添加攻击行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        protected override void AddAttackActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 如果有可攻击的敌人，添加攻击行动，但优先级较低
            if (evaluation.AttackableEnemies.Count > 0)
            {
                // 防守型AI优先攻击威胁等级高的敌人
                var bestTarget = evaluation.AttackableEnemies.OrderByDescending(t => t.ThreatLevel).First();
                actions.Add(AIAction.CreateAttackAction(bestTarget.CharacterId, 60)); // 降低攻击优先级
            }
        }

        /// <summary>
        /// 添加技能行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        protected override void AddSkillActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 如果有可用技能，添加技能行动，优先使用防御和治疗技能
            if (evaluation.AvailableSkills.Count > 0)
            {
                // 防守型AI优先使用防御和治疗技能
                var defensiveSkills = evaluation.AvailableSkills
                    .Where(s => s.SkillType.Contains("Defense") || s.SkillType.Contains("Heal"))
                    .OrderByDescending(s => s.Value);

                if (defensiveSkills.Any())
                {
                    var bestSkill = defensiveSkills.First();
                    actions.Add(AIAction.CreateSkillAction(bestSkill.SkillId, bestSkill.BestTargetX, bestSkill.BestTargetY, bestSkill.Value + 20)); // 提高防御技能优先级
                }
                else
                {
                    var bestSkill = evaluation.AvailableSkills.OrderByDescending(s => s.Value).First();
                    actions.Add(AIAction.CreateSkillAction(bestSkill.SkillId, bestSkill.BestTargetX, bestSkill.BestTargetY, bestSkill.Value));
                }
            }
        }

        /// <summary>
        /// 计算目标优先级
        /// </summary>
        /// <param name="healthPercentage">目标生命值百分比</param>
        /// <param name="threatLevel">目标威胁等级</param>
        /// <param name="expectedDamage">预期伤害</param>
        /// <returns>目标优先级</returns>
        protected override int CalculateTargetPriority(int healthPercentage, int threatLevel, int expectedDamage)
        {
            // 防守型AI更关注威胁等级高的敌人
            return threatLevel * 2 + (100 - healthPercentage) / 3 + expectedDamage / 2;
        }
    }
}