using System;
using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.AI;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Implementation.Modules.AI
{
    /// <summary>
    /// 被动型AI控制器，主要专注于防御和躲避
    /// </summary>
    public class PassiveAIController : BaseAIController
    {
        /// <summary>
        /// 获取AI类型
        /// </summary>
        public override AIType Type => AIType.Passive;

        /// <summary>
        /// 添加移动行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        protected override void AddMoveActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 如果处于危险位置，优先考虑移动到安全位置
            if (evaluation.IsInDangerousPosition && evaluation.SafePositions.Count > 0)
            {
                // 选择防御加成最高的安全位置
                var bestSafePosition = evaluation.SafePositions
                    .OrderByDescending(p => p.DefenseBonus)
                    .ThenByDescending(p => p.Value)
                    .First();

                actions.Add(AIAction.CreateMoveAction(bestSafePosition.X, bestSafePosition.Y, 100));
                return;
            }

            // 如果不在危险位置，但有更好的防御位置，考虑移动
            if (!evaluation.IsInDangerousPosition && evaluation.TacticalPositions.Count > 0)
            {
                // 选择防御加成最高且危险等级最低的位置
                var bestDefensivePosition = evaluation.TacticalPositions
                    .OrderByDescending(p => p.DefenseBonus)
                    .ThenBy(p => p.DangerLevel)
                    .First();

                // 只有当新位置比当前位置更好时才移动
                var currentPositionValue = evaluation.TacticalPositions
                    .FirstOrDefault(p => p.X == character.X && p.Y == character.Y)?.Value ?? 0;
                var currentPositionDefense = evaluation.TacticalPositions
                    .FirstOrDefault(p => p.X == character.X && p.Y == character.Y)?.DefenseBonus ?? 0;

                if (bestDefensivePosition.DefenseBonus > currentPositionDefense ||
                    (bestDefensivePosition.DefenseBonus == currentPositionDefense && bestDefensivePosition.Value > currentPositionValue))
                {
                    actions.Add(AIAction.CreateMoveAction(bestDefensivePosition.X, bestDefensivePosition.Y, 80));
                }
            }

            // 如果敌人太近，尝试远离敌人
            var enemyTeam = character.Team == BattleTeam.Player ? BattleTeam.Enemy : BattleTeam.Player;
            var enemies = battle.GetTeamCharacters(enemyTeam);

            if (enemies.Any())
            {
                // 找到最近的敌人
                var nearestEnemy = enemies
                    .OrderBy(e => Math.Abs(e.X - character.X) + Math.Abs(e.Y - character.Y))
                    .First();

                int distanceToNearestEnemy = Math.Abs(nearestEnemy.X - character.X) + Math.Abs(nearestEnemy.Y - character.Y);

                // 如果敌人太近（距离小于3），尝试远离
                if (distanceToNearestEnemy < 3)
                {
                    var moveRange = battle.CalculateMoveRange(character.Character.Id);

                    // 找到最远离敌人的安全位置
                    var bestPosition = moveRange
                        .Where(p => evaluation.SafePositions.Any(sp => sp.X == p.X && sp.Y == p.Y))
                        .OrderByDescending(p => Math.Abs(p.X - nearestEnemy.X) + Math.Abs(p.Y - nearestEnemy.Y))
                        .FirstOrDefault();

                    if (bestPosition != default)
                    {
                        actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 90));
                        return;
                    }

                    // 如果找不到安全位置，找到最远离敌人的位置
                    bestPosition = moveRange
                        .OrderByDescending(p => Math.Abs(p.X - nearestEnemy.X) + Math.Abs(p.Y - nearestEnemy.Y))
                        .FirstOrDefault();

                    if (bestPosition != default)
                    {
                        actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 85));
                    }
                }
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
            // 被动型AI只在安全的情况下考虑攻击，且优先级较低
            if (!evaluation.IsInDangerousPosition && evaluation.AttackableEnemies.Count > 0)
            {
                // 优先攻击威胁等级高的敌人
                var bestTarget = evaluation.AttackableEnemies
                    .OrderByDescending(e => e.ThreatLevel)
                    .First();

                actions.Add(AIAction.CreateAttackAction(bestTarget.CharacterId, 50));
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
            // 获取所有技能
            var skills = character.Character.Skills.ToList();
            if (!skills.Any())
                return;

            // 优先使用防御技能
            var defensiveSkills = skills
                .Where(s => s.IsDefensive && character.CurrentMP >= s.MPCost)
                .OrderByDescending(s => s.Power)
                .ToList();

            if (defensiveSkills.Any())
            {
                var bestSkill = defensiveSkills.First();
                actions.Add(AIAction.CreateSkillAction(
                    bestSkill.Id,
                    character.X,
                    character.Y,
                    90));
                return;
            }

            // 如果生命值低于50%，尝试使用治疗技能
            if (evaluation.CurrentCharacterHealth < 50)
            {
                var healingSkills = skills
                    .Where(s => s.IsHealing && s.TargetType == SkillTargetType.Self && character.CurrentMP >= s.MPCost)
                    .OrderByDescending(s => s.Power)
                    .ToList();

                if (healingSkills.Any())
                {
                    var bestSkill = healingSkills.First();
                    actions.Add(AIAction.CreateSkillAction(
                        bestSkill.Id,
                        character.X,
                        character.Y,
                        85));
                    return;
                }
            }

            // 如果有队友生命值低于50%，尝试使用治疗技能
            if (evaluation.HealableAllies.Any(a => a.HealthPercentage < 50))
            {
                var healingSkills = skills
                    .Where(s => s.IsHealing && s.TargetType != SkillTargetType.Self && character.CurrentMP >= s.MPCost)
                    .OrderByDescending(s => s.Power)
                    .ToList();

                if (healingSkills.Any())
                {
                    var bestSkill = healingSkills.First();
                    var mostInjuredAlly = evaluation.HealableAllies
                        .Where(a => a.HealthPercentage < 50)
                        .OrderBy(a => a.HealthPercentage)
                        .First();

                    // 检查是否在技能范围内
                    int distance = Math.Abs(character.X - mostInjuredAlly.X) + Math.Abs(character.Y - mostInjuredAlly.Y);
                    if (distance <= bestSkill.Range)
                    {
                        actions.Add(AIAction.CreateSkillAction(
                            bestSkill.Id,
                            mostInjuredAlly.X,
                            mostInjuredAlly.Y,
                            80));
                        return;
                    }
                }
            }

            // 如果不在危险位置，尝试使用增益技能
            if (!evaluation.IsInDangerousPosition)
            {
                var buffSkills = skills
                    .Where(s => s.IsBuff && character.CurrentMP >= s.MPCost)
                    .OrderByDescending(s => s.Power)
                    .ToList();

                if (buffSkills.Any())
                {
                    var bestSkill = buffSkills.First();
                    actions.Add(AIAction.CreateSkillAction(
                        bestSkill.Id,
                        character.X,
                        character.Y,
                        70));
                    return;
                }
            }

            // 如果不在危险位置，尝试使用远程攻击技能
            if (!evaluation.IsInDangerousPosition && evaluation.AttackableEnemies.Count > 0)
            {
                var attackSkills = skills
                    .Where(s => !s.IsHealing && !s.IsDefensive && !s.IsBuff && s.Range > 1 && character.CurrentMP >= s.MPCost)
                    .OrderByDescending(s => s.Power)
                    .ToList();

                if (attackSkills.Any())
                {
                    var bestSkill = attackSkills.First();
                    var bestTarget = evaluation.AttackableEnemies
                        .OrderByDescending(e => e.ThreatLevel)
                        .First();

                    // 检查是否在技能范围内
                    int distance = Math.Abs(character.X - bestTarget.X) + Math.Abs(character.Y - bestTarget.Y);
                    if (distance <= bestSkill.Range)
                    {
                        actions.Add(AIAction.CreateSkillAction(
                            bestSkill.Id,
                            bestTarget.X,
                            bestTarget.Y,
                            60));
                    }
                }
            }
        }

        /// <summary>
        /// 添加物品行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        protected override void AddItemActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 如果生命值低于30%，优先使用治疗物品
            if (evaluation.CurrentCharacterHealth < 30 && evaluation.AvailableItems.Any(i => i.ItemType.Contains("Heal")))
            {
                var healingItem = evaluation.AvailableItems
                    .Where(i => i.ItemType.Contains("Heal"))
                    .OrderByDescending(i => i.Value)
                    .First();

                actions.Add(AIAction.CreateItemAction(healingItem.ItemId, character.Character.Id, 85));
                return;
            }

            // 如果MP低于30%，使用恢复MP的物品
            int maxMP = character.Character.MaxMP;
            int currentMP = character.CurrentMP;
            int mpPercentage = currentMP * 100 / Math.Max(1, maxMP);

            if (mpPercentage < 30 && evaluation.AvailableItems.Any(i => i.ItemType.Contains("MP")))
            {
                var mpItem = evaluation.AvailableItems
                    .Where(i => i.ItemType.Contains("MP"))
                    .OrderByDescending(i => i.Value)
                    .First();

                actions.Add(AIAction.CreateItemAction(mpItem.ItemId, character.Character.Id, 80));
                return;
            }

            // 使用增强防御力的物品
            if (evaluation.AvailableItems.Any(i => i.ItemType.Contains("Defense")))
            {
                var defenseItem = evaluation.AvailableItems
                    .Where(i => i.ItemType.Contains("Defense"))
                    .OrderByDescending(i => i.Value)
                    .First();

                actions.Add(AIAction.CreateItemAction(defenseItem.ItemId, character.Character.Id, 75));
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
            // 被动型AI主要关注威胁等级高的敌人
            return threatLevel * 2;
        }
    }
}