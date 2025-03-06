using System;
using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.AI;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Map;
using TacticalRPG.Core.Modules.Skill;

namespace TacticalRPG.Implementation.Modules.AI
{
    /// <summary>
    /// 治疗型AI控制器，专注于治疗队友
    /// </summary>
    public class HealerAIController : BaseAIController
    {
        /// <summary>
        /// 获取AI类型
        /// </summary>
        public override AIType Type => AIType.Healer;

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
                actions.Add(AIAction.CreateMoveAction(bestSafePosition.X, bestSafePosition.Y, 95));
                return;
            }

            // 获取治疗技能
            var healingSkills = character.Character.Skills
                .Where(s => s.IsHealing)
                .OrderByDescending(s => s.Power)
                .ToList();

            if (!healingSkills.Any())
                return;

            // 获取最强的治疗技能
            var bestHealingSkill = healingSkills.First();

            // 如果有需要治疗的队友
            if (evaluation.HealableAllies.Count > 0)
            {
                // 获取移动范围
                var moveRange = battle.CalculateMoveRange(character.Character.Id);
                if (!moveRange.Any())
                    return;

                // 找到生命值最低的队友
                var mostInjuredAlly = evaluation.HealableAllies
                    .OrderBy(a => a.HealthPercentage)
                    .First();

                // 尝试找到可以治疗到最需要治疗的队友的位置
                var bestPosition = moveRange
                    .Where(p => Math.Abs(p.X - mostInjuredAlly.X) + Math.Abs(p.Y - mostInjuredAlly.Y) <= bestHealingSkill.Range)
                    .OrderBy(p => Math.Abs(p.X - mostInjuredAlly.X) + Math.Abs(p.Y - mostInjuredAlly.Y))
                    .ThenByDescending(p => evaluation.SafePositions.Any(sp => sp.X == p.X && sp.Y == p.Y) ? 1 : 0)
                    .FirstOrDefault();

                if (bestPosition != default)
                {
                    actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 90));
                    return;
                }

                // 如果找不到可以治疗到最需要治疗的队友的位置，尝试找到可以治疗到任何需要治疗的队友的位置
                foreach (var ally in evaluation.HealableAllies)
                {
                    bestPosition = moveRange
                        .Where(p => Math.Abs(p.X - ally.X) + Math.Abs(p.Y - ally.Y) <= bestHealingSkill.Range)
                        .OrderBy(p => Math.Abs(p.X - ally.X) + Math.Abs(p.Y - ally.Y))
                        .ThenByDescending(p => evaluation.SafePositions.Any(sp => sp.X == p.X && sp.Y == p.Y) ? 1 : 0)
                        .FirstOrDefault();

                    if (bestPosition != default)
                    {
                        actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 85));
                        return;
                    }
                }

                // 如果找不到可以直接治疗的位置，移动到最接近最需要治疗的队友的位置
                bestPosition = moveRange
                    .OrderBy(p => Math.Abs(p.X - mostInjuredAlly.X) + Math.Abs(p.Y - mostInjuredAlly.Y))
                    .ThenByDescending(p => evaluation.SafePositions.Any(sp => sp.X == p.X && sp.Y == p.Y) ? 1 : 0)
                    .First();

                actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 80));
                return;
            }

            // 如果没有需要治疗的队友，尝试跟随队伍中生命值最高的角色
            var teamCharacters = battle.GetTeamCharacters(character.Team)
                .Where(c => c.Character.Id != character.Character.Id)
                .ToList();

            if (teamCharacters.Any())
            {
                var strongestAlly = teamCharacters
                    .OrderByDescending(c => c.CurrentHP * 100 / Math.Max(1, c.Character.MaxHP))
                    .ThenByDescending(c => c.Character.CalculateCombatStats().PhysicalAttack + c.Character.CalculateCombatStats().MagicAttack)
                    .First();

                // 获取移动范围
                var moveRange = battle.CalculateMoveRange(character.Character.Id);
                if (!moveRange.Any())
                    return;

                // 找到最接近最强队友的安全位置
                var bestPosition = moveRange
                    .Where(p => evaluation.SafePositions.Any(sp => sp.X == p.X && sp.Y == p.Y))
                    .OrderBy(p => Math.Abs(p.X - strongestAlly.X) + Math.Abs(p.Y - strongestAlly.Y))
                    .FirstOrDefault();

                if (bestPosition != default)
                {
                    actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 75));
                    return;
                }

                // 如果找不到安全位置，找到最接近最强队友的位置
                bestPosition = moveRange
                    .OrderBy(p => Math.Abs(p.X - strongestAlly.X) + Math.Abs(p.Y - strongestAlly.Y))
                    .First();

                actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 70));
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
            // 治疗型AI只在没有需要治疗的队友时考虑攻击，且优先级较低
            if (evaluation.HealableAllies.Count == 0 && evaluation.AttackableEnemies.Count > 0)
            {
                var bestTarget = evaluation.AttackableEnemies
                    .OrderBy(e => e.HealthPercentage)
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

            // 如果有需要治疗的队友，优先使用治疗技能
            if (evaluation.HealableAllies.Count > 0)
            {
                // 获取治疗技能
                var healingSkills = skills
                    .Where(s => s.IsHealing && character.CurrentMP >= s.MPCost)
                    .OrderByDescending(s => s.Power)
                    .ToList();

                if (healingSkills.Any())
                {
                    // 找到生命值最低的队友
                    var mostInjuredAlly = evaluation.HealableAllies
                        .OrderBy(a => a.HealthPercentage)
                        .First();

                    // 尝试使用最强的治疗技能
                    foreach (var skill in healingSkills)
                    {
                        // 检查是否在技能范围内
                        int distance = Math.Abs(character.X - mostInjuredAlly.X) + Math.Abs(character.Y - mostInjuredAlly.Y);
                        if (distance <= skill.Range)
                        {
                            actions.Add(AIAction.CreateSkillAction(
                                skill.Id,
                                mostInjuredAlly.X,
                                mostInjuredAlly.Y,
                                100));
                            return;
                        }
                    }

                    // 如果找不到可以治疗到最需要治疗的队友的技能，尝试治疗其他队友
                    foreach (var ally in evaluation.HealableAllies)
                    {
                        foreach (var skill in healingSkills)
                        {
                            int distance = Math.Abs(character.X - ally.X) + Math.Abs(character.Y - ally.Y);
                            if (distance <= skill.Range)
                            {
                                actions.Add(AIAction.CreateSkillAction(
                                    skill.Id,
                                    ally.X,
                                    ally.Y,
                                    95));
                                return;
                            }
                        }
                    }
                }

                // 如果没有可用的治疗技能，尝试使用群体治疗技能
                var aoeHealingSkills = skills
                    .Where(s => s.IsHealing && s.IsAreaEffect && character.CurrentMP >= s.MPCost)
                    .OrderByDescending(s => s.Power)
                    .ToList();

                if (aoeHealingSkills.Any())
                {
                    var bestSkill = aoeHealingSkills.First();

                    // 找到最佳施放位置
                    var bestPosition = FindBestHealingPosition(character, battle, bestSkill);
                    if (bestPosition != null)
                    {
                        actions.Add(AIAction.CreateSkillAction(
                            bestSkill.Id,
                            bestPosition.Value.X,
                            bestPosition.Value.Y,
                            90));
                        return;
                    }
                }
            }

            // 如果自己生命值低于50%，尝试使用自我治疗技能
            if (evaluation.CurrentCharacterHealth < 50)
            {
                var selfHealingSkills = skills
                    .Where(s => s.IsHealing && s.TargetType == SkillTargetType.Self && character.CurrentMP >= s.MPCost)
                    .OrderByDescending(s => s.Power)
                    .ToList();

                if (selfHealingSkills.Any())
                {
                    var bestSkill = selfHealingSkills.First();
                    actions.Add(AIAction.CreateSkillAction(
                        bestSkill.Id,
                        character.X,
                        character.Y,
                        85));
                    return;
                }
            }

            // 如果没有需要治疗的队友，尝试使用增益技能
            if (evaluation.HealableAllies.Count == 0)
            {
                var buffSkills = skills
                    .Where(s => s.IsBuff && character.CurrentMP >= s.MPCost)
                    .OrderByDescending(s => s.Power)
                    .ToList();

                if (buffSkills.Any())
                {
                    var bestSkill = buffSkills.First();

                    // 找到最强的队友
                    var teamCharacters = battle.GetTeamCharacters(character.Team)
                        .Where(c => c.Character.Id != character.Character.Id)
                        .ToList();

                    if (teamCharacters.Any())
                    {
                        var bestAlly = teamCharacters
                            .OrderByDescending(c => c.Character.CalculateCombatStats().PhysicalAttack + c.Character.CalculateCombatStats().MagicAttack)
                            .First();

                        // 检查是否在技能范围内
                        int distance = Math.Abs(character.X - bestAlly.X) + Math.Abs(character.Y - bestAlly.Y);
                        if (distance <= bestSkill.Range)
                        {
                            actions.Add(AIAction.CreateSkillAction(
                                bestSkill.Id,
                                bestAlly.X,
                                bestAlly.Y,
                                80));
                            return;
                        }
                    }
                }
            }

            // 如果没有需要治疗的队友，也没有可用的增益技能，尝试使用攻击技能
            if (evaluation.HealableAllies.Count == 0 && evaluation.AttackableEnemies.Count > 0)
            {
                var attackSkills = skills
                    .Where(s => !s.IsHealing && !s.IsBuff && character.CurrentMP >= s.MPCost)
                    .OrderByDescending(s => s.Power)
                    .ToList();

                if (attackSkills.Any())
                {
                    var bestSkill = attackSkills.First();
                    var bestTarget = evaluation.AttackableEnemies
                        .OrderBy(e => e.HealthPercentage)
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
            // 如果MP不足以使用治疗技能，优先使用恢复MP的物品
            if (character.CurrentMP < 20 && evaluation.AvailableItems.Any(i => i.ItemType.Contains("MP")))
            {
                var mpItem = evaluation.AvailableItems
                    .Where(i => i.ItemType.Contains("MP"))
                    .OrderByDescending(i => i.Value)
                    .First();

                actions.Add(AIAction.CreateItemAction(mpItem.ItemId, character.Character.Id, 90));
                return;
            }

            // 如果自己生命值低于30%，使用治疗物品
            if (evaluation.CurrentCharacterHealth < 30 && evaluation.AvailableItems.Any(i => i.ItemType.Contains("Heal")))
            {
                var healingItem = evaluation.AvailableItems
                    .Where(i => i.ItemType.Contains("Heal"))
                    .OrderByDescending(i => i.Value)
                    .First();

                actions.Add(AIAction.CreateItemAction(healingItem.ItemId, character.Character.Id, 85));
                return;
            }

            // 如果有队友生命值低于30%，使用治疗物品
            if (evaluation.HealableAllies.Any(a => a.HealthPercentage < 30) && evaluation.AvailableItems.Any(i => i.ItemType.Contains("Heal")))
            {
                var healingItem = evaluation.AvailableItems
                    .Where(i => i.ItemType.Contains("Heal"))
                    .OrderByDescending(i => i.Value)
                    .First();

                var mostInjuredAlly = evaluation.HealableAllies
                    .Where(a => a.HealthPercentage < 30)
                    .OrderBy(a => a.HealthPercentage)
                    .First();

                actions.Add(AIAction.CreateItemAction(healingItem.ItemId, mostInjuredAlly.CharacterId, 80));
            }
        }

        /// <summary>
        /// 找到最佳治疗位置
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="skill">技能</param>
        /// <returns>最佳位置</returns>
        private (int X, int Y)? FindBestHealingPosition(IBattleCharacter character, IBattle battle, ISkill skill)
        {
            // 获取队友
            var allies = battle.GetTeamCharacters(character.Team)
                .Where(c => c.Character.Id != character.Character.Id && c.CurrentHP < c.Character.MaxHP)
                .ToList();

            if (!allies.Any())
                return null;

            // 计算每个可能的目标位置的得分
            var positions = new List<(int X, int Y, int Score)>();

            for (int x = 0; x < battle.Map.Width; x++)
            {
                for (int y = 0; y < battle.Map.Height; y++)
                {
                    // 检查是否在技能施放范围内
                    int distanceToCharacter = Math.Abs(character.X - x) + Math.Abs(character.Y - y);
                    if (distanceToCharacter > skill.Range)
                        continue;

                    // 计算该位置可以治疗到的队友数量和总治疗价值
                    int alliesInRange = 0;
                    int totalHealValue = 0;

                    foreach (var ally in allies)
                    {
                        int distanceToAlly = Math.Abs(x - ally.X) + Math.Abs(y - ally.Y);
                        if (distanceToAlly <= skill.AreaEffectRange)
                        {
                            alliesInRange++;
                            int healthPercentage = ally.CurrentHP * 100 / Math.Max(1, ally.Character.MaxHP);
                            totalHealValue += 100 - healthPercentage;
                        }
                    }

                    if (alliesInRange > 0)
                    {
                        positions.Add((x, y, alliesInRange * 10 + totalHealValue));
                    }
                }
            }

            // 返回得分最高的位置
            return positions.OrderByDescending(p => p.Score).Select(p => ((int X, int Y))(p.X, p.Y)).FirstOrDefault();
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
            // 治疗型AI主要关注生命值低的敌人
            return (100 - healthPercentage) * 3;
        }
    }
}