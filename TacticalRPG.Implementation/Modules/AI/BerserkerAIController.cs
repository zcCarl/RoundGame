using System;
using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.AI;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Implementation.Modules.AI
{
    /// <summary>
    /// 狂战士型AI控制器，完全专注于攻击，不考虑防御
    /// </summary>
    public class BerserkerAIController : BaseAIController
    {
        /// <summary>
        /// 获取AI类型
        /// </summary>
        public override AIType Type => AIType.Berserker;

        /// <summary>
        /// 添加移动行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        protected override void AddMoveActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 获取敌方队伍
            var enemyTeam = character.Team == BattleTeam.Player ? BattleTeam.Enemy : BattleTeam.Player;
            var enemies = battle.GetTeamCharacters(enemyTeam);

            if (!enemies.Any())
                return;

            // 计算移动范围
            var moveRange = battle.CalculateMoveRange(character.Character.Id);
            if (!moveRange.Any())
                return;

            // 获取战斗属性
            var combatStats = character.Character.CalculateCombatStats();

            // 找到生命值最低的敌人
            var weakestEnemy = enemies
                .OrderBy(e => e.CurrentHP * 100 / Math.Max(1, e.Character.MaxHP))
                .First();

            // 尝试找到可以攻击到最弱敌人的位置
            var bestPosition = moveRange
                .Where(p => Math.Abs(p.X - weakestEnemy.X) + Math.Abs(p.Y - weakestEnemy.Y) <= combatStats.AttackRange)
                .OrderBy(p => Math.Abs(p.X - weakestEnemy.X) + Math.Abs(p.Y - weakestEnemy.Y))
                .FirstOrDefault();

            if (bestPosition != default)
            {
                actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 95));
                return;
            }

            // 如果找不到可以攻击到最弱敌人的位置，尝试找到可以攻击到任何敌人的位置
            foreach (var enemy in enemies)
            {
                bestPosition = moveRange
                    .Where(p => Math.Abs(p.X - enemy.X) + Math.Abs(p.Y - enemy.Y) <= combatStats.AttackRange)
                    .OrderBy(p => Math.Abs(p.X - enemy.X) + Math.Abs(p.Y - enemy.Y))
                    .FirstOrDefault();

                if (bestPosition != default)
                {
                    actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 90));
                    return;
                }
            }

            // 如果找不到可以直接攻击的位置，移动到最接近最弱敌人的位置
            bestPosition = moveRange
                .OrderBy(p => Math.Abs(p.X - weakestEnemy.X) + Math.Abs(p.Y - weakestEnemy.Y))
                .First();

            actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 85));
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
            // 如果有可攻击的敌人，添加攻击行动（高优先级）
            if (evaluation.AttackableEnemies.Count > 0)
            {
                // 狂战士型AI优先攻击生命值低的敌人
                var bestTarget = evaluation.AttackableEnemies
                    .OrderBy(e => e.HealthPercentage)
                    .First();

                actions.Add(AIAction.CreateAttackAction(bestTarget.CharacterId, 100));
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
            // 获取所有攻击技能
            var attackSkills = character.Character.Skills
                .Where(s => !s.IsHealing && !s.IsDefensive && !s.IsBuff)
                .OrderByDescending(s => s.Power)
                .ToList();

            if (!attackSkills.Any())
                return;

            // 获取敌方队伍
            var enemyTeam = character.Team == BattleTeam.Player ? BattleTeam.Enemy : BattleTeam.Player;
            var enemies = battle.GetTeamCharacters(enemyTeam);

            if (!enemies.Any())
                return;

            // 找到生命值最低的敌人
            var weakestEnemy = enemies
                .OrderBy(e => e.CurrentHP * 100 / Math.Max(1, e.Character.MaxHP))
                .First();

            // 尝试使用最强的攻击技能
            foreach (var skill in attackSkills)
            {
                // 检查MP是否足够
                if (character.CurrentMP < skill.MPCost)
                    continue;

                // 检查是否在技能范围内
                int distance = Math.Abs(character.X - weakestEnemy.X) + Math.Abs(character.Y - weakestEnemy.Y);
                if (distance <= skill.Range)
                {
                    actions.Add(AIAction.CreateSkillAction(
                        skill.Id,
                        weakestEnemy.X,
                        weakestEnemy.Y,
                        95));
                    return;
                }
            }

            // 如果没有可用的攻击技能，尝试使用自我增益技能
            var buffSkills = character.Character.Skills
                .Where(s => s.IsBuff && !s.IsHealing && !s.IsDefensive)
                .OrderByDescending(s => s.Power)
                .ToList();

            foreach (var skill in buffSkills)
            {
                // 检查MP是否足够
                if (character.CurrentMP < skill.MPCost)
                    continue;

                actions.Add(AIAction.CreateSkillAction(
                    skill.Id,
                    character.X,
                    character.Y,
                    90));
                return;
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
            // 狂战士型AI只在生命值极低时使用治疗物品
            if (evaluation.CurrentCharacterHealth < 20 && evaluation.AvailableItems.Any(i => i.ItemType.Contains("Heal")))
            {
                var healingItem = evaluation.AvailableItems
                    .Where(i => i.ItemType.Contains("Heal"))
                    .OrderByDescending(i => i.Value)
                    .First();

                actions.Add(AIAction.CreateItemAction(healingItem.ItemId, character.Character.Id, 85));
            }
            // 如果MP不足以使用技能，使用恢复MP的物品
            else if (character.CurrentMP < 10 && evaluation.AvailableItems.Any(i => i.ItemType.Contains("MP")))
            {
                var mpItem = evaluation.AvailableItems
                    .Where(i => i.ItemType.Contains("MP"))
                    .OrderByDescending(i => i.Value)
                    .First();

                actions.Add(AIAction.CreateItemAction(mpItem.ItemId, character.Character.Id, 80));
            }
            // 使用增强攻击力的物品
            else if (evaluation.AvailableItems.Any(i => i.ItemType.Contains("Attack")))
            {
                var attackItem = evaluation.AvailableItems
                    .Where(i => i.ItemType.Contains("Attack"))
                    .OrderByDescending(i => i.Value)
                    .First();

                actions.Add(AIAction.CreateItemAction(attackItem.ItemId, character.Character.Id, 75));
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
            // 狂战士型AI主要关注生命值低的敌人和预期伤害高的目标
            return (100 - healthPercentage) * 2 + expectedDamage;
        }
    }
}