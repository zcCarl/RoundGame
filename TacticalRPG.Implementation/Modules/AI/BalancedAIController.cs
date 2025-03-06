using System;
using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.AI;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Implementation.Modules.AI
{
    /// <summary>
    /// 平衡型AI控制器，在攻防之间保持平衡
    /// </summary>
    public class BalancedAIController : BaseAIController
    {
        /// <summary>
        /// 获取AI类型
        /// </summary>
        public override AIType Type => AIType.Balanced;

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
                var bestSafePosition = evaluation.SafePositions.OrderByDescending(p => p.Value).First();
                actions.Add(AIAction.CreateMoveAction(bestSafePosition.X, bestSafePosition.Y, 85));
                return;
            }

            // 如果有需要治疗的队友，尝试移动到队友附近
            if (evaluation.HealableAllies.Count > 0 && character.Character.Skills.Any(s => s.IsHealing))
            {
                var moveRange = battle.CalculateMoveRange(character.Character.Id);
                var mostInjuredAlly = evaluation.HealableAllies.OrderByDescending(a => a.Priority).First();
                var healingSkill = character.Character.Skills.First(s => s.IsHealing);

                // 找到最接近受伤队友的位置，且在治疗范围内
                var bestPosition = moveRange
                    .Where(p => Math.Abs(p.X - mostInjuredAlly.X) + Math.Abs(p.Y - mostInjuredAlly.Y) <= healingSkill.Range)
                    .OrderBy(p => Math.Abs(p.X - mostInjuredAlly.X) + Math.Abs(p.Y - mostInjuredAlly.Y))
                    .FirstOrDefault();

                if (bestPosition != default)
                {
                    actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 75));
                    return;
                }
            }

            // 如果有敌人，尝试移动到攻击范围内
            var enemyTeam = character.Team == BattleTeam.Player ? BattleTeam.Enemy : BattleTeam.Player;
            var enemies = battle.GetTeamCharacters(enemyTeam);

            if (enemies.Any())
            {
                var moveRange = battle.CalculateMoveRange(character.Character.Id);
                var combatStats = character.Character.CalculateCombatStats();

                // 找到最适合攻击的敌人（优先选择生命值低的）
                var bestEnemy = enemies
                    .OrderBy(e => e.CurrentHP * 100 / Math.Max(1, e.Character.MaxHP))
                    .ThenBy(e => Math.Abs(e.X - character.X) + Math.Abs(e.Y - character.Y))
                    .First();

                // 找到可以攻击到敌人的位置
                var bestPosition = moveRange
                    .Where(p => Math.Abs(p.X - bestEnemy.X) + Math.Abs(p.Y - bestEnemy.Y) <= combatStats.AttackRange)
                    .OrderByDescending(p => evaluation.TacticalPositions.FirstOrDefault(tp => tp.X == p.X && tp.Y == p.Y)?.Value ?? 0)
                    .FirstOrDefault();

                if (bestPosition != default)
                {
                    actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 70));
                    return;
                }

                // 如果找不到可以直接攻击的位置，移动到最接近敌人的位置
                bestPosition = moveRange
                    .OrderBy(p => Math.Abs(p.X - bestEnemy.X) + Math.Abs(p.Y - bestEnemy.Y))
                    .FirstOrDefault();

                if (bestPosition != default)
                {
                    actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 65));
                    return;
                }
            }

            // 否则考虑移动到战术位置
            if (evaluation.TacticalPositions.Count > 0)
            {
                var bestTacticalPosition = evaluation.TacticalPositions
                    .OrderByDescending(p => p.Value)
                    .First();
                actions.Add(AIAction.CreateMoveAction(bestTacticalPosition.X, bestTacticalPosition.Y, 60));
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
            // 如果有可攻击的敌人，添加攻击行动
            if (evaluation.AttackableEnemies.Count > 0)
            {
                // 平衡型AI优先攻击生命值低的敌人，其次是威胁等级高的敌人
                var bestTarget = evaluation.AttackableEnemies
                    .OrderBy(e => e.HealthPercentage)
                    .ThenByDescending(e => e.ThreatLevel)
                    .First();

                actions.Add(AIAction.CreateAttackAction(bestTarget.CharacterId, 75));
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
            // 如果有可用技能，添加技能行动
            if (evaluation.AvailableSkills.Count > 0)
            {
                // 如果有队友生命值低于50%，优先使用治疗技能
                if (evaluation.HealableAllies.Any(a => a.HealthPercentage < 50))
                {
                    var healingSkills = evaluation.AvailableSkills
                        .Where(s => s.SkillType.Contains("Heal"))
                        .OrderByDescending(s => s.Value);

                    if (healingSkills.Any())
                    {
                        var bestSkill = healingSkills.First();
                        actions.Add(AIAction.CreateSkillAction(bestSkill.SkillId, bestSkill.BestTargetX, bestSkill.BestTargetY, 80));
                        return;
                    }
                }

                // 如果自己生命值低于30%，优先使用防御技能
                if (evaluation.CurrentCharacterHealth < 30)
                {
                    var defensiveSkills = evaluation.AvailableSkills
                        .Where(s => s.SkillType.Contains("Defense"))
                        .OrderByDescending(s => s.Value);

                    if (defensiveSkills.Any())
                    {
                        var bestSkill = defensiveSkills.First();
                        actions.Add(AIAction.CreateSkillAction(bestSkill.SkillId, bestSkill.BestTargetX, bestSkill.BestTargetY, 85));
                        return;
                    }
                }

                // 如果有敌人生命值低于30%，优先使用攻击技能
                if (evaluation.AttackableEnemies.Any(e => e.HealthPercentage < 30))
                {
                    var attackSkills = evaluation.AvailableSkills
                        .Where(s => !s.SkillType.Contains("Heal") && !s.SkillType.Contains("Defense") && !s.SkillType.Contains("Buff"))
                        .OrderByDescending(s => s.Value);

                    if (attackSkills.Any())
                    {
                        var bestSkill = attackSkills.First();
                        actions.Add(AIAction.CreateSkillAction(bestSkill.SkillId, bestSkill.BestTargetX, bestSkill.BestTargetY, 80));
                        return;
                    }
                }

                // 否则使用价值最高的技能
                var bestAvailableSkill = evaluation.AvailableSkills.OrderByDescending(s => s.Value).First();
                actions.Add(AIAction.CreateSkillAction(bestAvailableSkill.SkillId, bestAvailableSkill.BestTargetX, bestAvailableSkill.BestTargetY, 70));
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
            // 如果有可用物品，添加物品行动
            if (evaluation.AvailableItems.Count > 0)
            {
                // 如果生命值低于30%，优先使用治疗物品
                if (evaluation.CurrentCharacterHealth < 30)
                {
                    var healingItems = evaluation.AvailableItems
                        .Where(i => i.ItemType.Contains("Heal"))
                        .OrderByDescending(i => i.Value);

                    if (healingItems.Any())
                    {
                        var bestItem = healingItems.First();
                        actions.Add(AIAction.CreateItemAction(bestItem.ItemId, bestItem.BestTargetId, 85));
                        return;
                    }
                }

                // 如果MP低于30%，优先使用恢复MP的物品
                int maxMP = character.Character.MaxMP;
                int currentMP = character.CurrentMP;
                int mpPercentage = currentMP * 100 / Math.Max(1, maxMP);

                if (mpPercentage < 30)
                {
                    var mpItems = evaluation.AvailableItems
                        .Where(i => i.ItemType.Contains("MP"))
                        .OrderByDescending(i => i.Value);

                    if (mpItems.Any())
                    {
                        var bestItem = mpItems.First();
                        actions.Add(AIAction.CreateItemAction(bestItem.ItemId, bestItem.BestTargetId, 80));
                        return;
                    }
                }

                // 否则使用价值最高的物品
                var bestAvailableItem = evaluation.AvailableItems.OrderByDescending(i => i.Value).First();
                actions.Add(AIAction.CreateItemAction(bestAvailableItem.ItemId, bestAvailableItem.BestTargetId, 70));
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
            // 平衡型AI同时考虑生命值、威胁等级和预期伤害
            return (100 - healthPercentage) / 2 + threatLevel / 2 + expectedDamage / 2;
        }
    }
}