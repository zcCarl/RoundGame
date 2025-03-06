using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TacticalRPG.Core.Modules.AI;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Implementation.Modules.AI
{
    /// <summary>
    /// 战术型AI控制器，根据战场情况灵活调整策略
    /// </summary>
    public class TacticalAIController : BaseAIController
    {
        /// <summary>
        /// 获取AI类型
        /// </summary>
        public override AIType Type => AIType.Tactical;

        /// <summary>
        /// 当前战术模式
        /// </summary>
        private TacticalMode _currentMode = TacticalMode.Balanced;

        /// <summary>
        /// 决策下一步行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <returns>AI行动决策结果</returns>
        public override async Task<AIAction> DecideNextAction(IBattleCharacter character, IBattle battle)
        {
            // 评估战场状态
            var evaluation = EvaluateBattlefield(character, battle);

            // 根据战场评估结果选择战术模式
            _currentMode = DetermineTacticalMode(character, battle, evaluation);

            // 获取所有可能的行动
            var possibleActions = new List<AIAction>();

            // 根据当前战术模式添加行动
            switch (_currentMode)
            {
                case TacticalMode.Aggressive:
                    AddAggressiveActions(character, battle, evaluation, possibleActions);
                    break;

                case TacticalMode.Defensive:
                    AddDefensiveActions(character, battle, evaluation, possibleActions);
                    break;

                case TacticalMode.Support:
                    AddSupportActions(character, battle, evaluation, possibleActions);
                    break;

                case TacticalMode.Balanced:
                default:
                    AddBalancedActions(character, battle, evaluation, possibleActions);
                    break;
            }

            // 添加等待行动
            possibleActions.Add(AIAction.CreateWaitAction(0));

            // 添加结束回合行动
            possibleActions.Add(AIAction.CreateEndTurnAction(-10));

            // 根据优先级选择最佳行动
            return possibleActions.OrderByDescending(a => a.Priority).FirstOrDefault()
                ?? AIAction.CreateEndTurnAction(0);
        }

        /// <summary>
        /// 确定当前战术模式
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <returns>战术模式</returns>
        private TacticalMode DetermineTacticalMode(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation)
        {
            // 计算各种战术模式的分数
            int aggressiveScore = CalculateAggressiveScore(character, battle, evaluation);
            int defensiveScore = CalculateDefensiveScore(character, battle, evaluation);
            int supportScore = CalculateSupportScore(character, battle, evaluation);
            int balancedScore = CalculateBalancedScore(character, battle, evaluation);

            // 选择得分最高的模式
            int maxScore = Math.Max(Math.Max(aggressiveScore, defensiveScore), Math.Max(supportScore, balancedScore));

            if (maxScore == aggressiveScore)
                return TacticalMode.Aggressive;
            else if (maxScore == defensiveScore)
                return TacticalMode.Defensive;
            else if (maxScore == supportScore)
                return TacticalMode.Support;
            else
                return TacticalMode.Balanced;
        }

        /// <summary>
        /// 计算进攻模式得分
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <returns>得分</returns>
        private int CalculateAggressiveScore(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation)
        {
            int score = 50; // 基础分数

            // 如果我方健康度高于敌方，增加进攻倾向
            if (evaluation.TeamHealth > evaluation.EnemyHealth + 20)
                score += 20;

            // 如果我方数量优势明显，增加进攻倾向
            if (evaluation.TeamNumberAdvantage > 20)
                score += 15;

            // 如果当前角色健康状态良好，增加进攻倾向
            if (evaluation.CurrentCharacterHealth > 70)
                score += 10;

            // 如果有可攻击的敌人，增加进攻倾向
            if (evaluation.AttackableEnemies.Count > 0)
                score += 15;

            // 如果敌方有生命值很低的角色，大幅增加进攻倾向
            if (evaluation.AttackableEnemies.Any(e => e.HealthPercentage < 30))
                score += 25;

            // 如果整体战场态势良好，增加进攻倾向
            if (evaluation.OverallSituation > 20)
                score += 15;

            return score;
        }

        /// <summary>
        /// 计算防守模式得分
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <returns>得分</returns>
        private int CalculateDefensiveScore(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation)
        {
            int score = 50; // 基础分数

            // 如果我方健康度低于敌方，增加防守倾向
            if (evaluation.TeamHealth < evaluation.EnemyHealth - 10)
                score += 20;

            // 如果我方数量劣势，增加防守倾向
            if (evaluation.TeamNumberAdvantage < -10)
                score += 15;

            // 如果当前角色健康状态不佳，增加防守倾向
            if (evaluation.CurrentCharacterHealth < 50)
                score += 20;

            // 如果处于危险位置，大幅增加防守倾向
            if (evaluation.IsInDangerousPosition)
                score += 30;

            // 如果有安全位置可以移动，增加防守倾向
            if (evaluation.SafePositions.Count > 0)
                score += 10;

            // 如果整体战场态势不佳，增加防守倾向
            if (evaluation.OverallSituation < -10)
                score += 15;

            return score;
        }

        /// <summary>
        /// 计算支援模式得分
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <returns>得分</returns>
        private int CalculateSupportScore(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation)
        {
            int score = 50; // 基础分数

            // 如果有需要治疗的队友，增加支援倾向
            if (evaluation.HealableAllies.Count > 0)
                score += 20;

            // 如果有队友生命值很低，大幅增加支援倾向
            if (evaluation.HealableAllies.Any(a => a.HealthPercentage < 30))
                score += 30;

            // 如果有治疗或增益技能可用，增加支援倾向
            var supportSkills = character.Character.Skills
                .Where(s => s.IsHealing || s.IsBuff)
                .ToList();

            if (supportSkills.Count > 0)
                score += 15;

            // 如果当前角色攻击力较低，增加支援倾向
            var combatStats = character.Character.CalculateCombatStats();
            if (combatStats.PhysicalAttack < 40 && combatStats.MagicAttack < 40)
                score += 10;

            // 如果队友数量多，增加支援倾向
            var teamCharacters = battle.GetTeamCharacters(character.Team);
            if (teamCharacters.Count > 3)
                score += 10;

            return score;
        }

        /// <summary>
        /// 计算平衡模式得分
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <returns>得分</returns>
        private int CalculateBalancedScore(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation)
        {
            int score = 60; // 基础分数较高，因为平衡模式是默认选择

            // 如果各项指标都比较均衡，增加平衡模式倾向
            if (Math.Abs(evaluation.TeamHealth - evaluation.EnemyHealth) < 20)
                score += 10;

            if (Math.Abs(evaluation.TeamNumberAdvantage) < 15)
                score += 10;

            if (evaluation.CurrentCharacterHealth > 40 && evaluation.CurrentCharacterHealth < 80)
                score += 10;

            // 如果战场态势接近均衡，增加平衡模式倾向
            if (Math.Abs(evaluation.OverallSituation) < 15)
                score += 15;

            // 如果既有攻击机会又有需要治疗的队友，增加平衡模式倾向
            if (evaluation.AttackableEnemies.Count > 0 && evaluation.HealableAllies.Count > 0)
                score += 15;

            return score;
        }

        /// <summary>
        /// 添加进攻模式行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        private void AddAggressiveActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 添加攻击行动（高优先级）
            if (evaluation.AttackableEnemies.Count > 0)
            {
                // 优先攻击生命值低的敌人
                var bestTarget = evaluation.AttackableEnemies
                    .OrderBy(e => e.HealthPercentage)
                    .ThenByDescending(e => e.ThreatLevel)
                    .First();

                actions.Add(AIAction.CreateAttackAction(bestTarget.CharacterId, 90));
            }

            // 添加攻击技能行动
            var attackSkills = character.Character.Skills
                .Where(s => !s.IsHealing && !s.IsBuff)
                .OrderByDescending(s => s.Power)
                .ToList();

            if (attackSkills.Any() && evaluation.AttackableEnemies.Any())
            {
                var bestSkill = attackSkills.First();
                var bestTarget = evaluation.AttackableEnemies
                    .OrderBy(e => e.HealthPercentage)
                    .First();

                actions.Add(AIAction.CreateSkillAction(
                    bestSkill.Id,
                    bestTarget.X,
                    bestTarget.Y,
                    85));
            }

            // 添加移动行动（如果没有可攻击的敌人）
            if (evaluation.AttackableEnemies.Count == 0)
            {
                // 获取敌方角色
                var enemyTeam = character.Team == BattleTeam.Player ? BattleTeam.Enemy : BattleTeam.Player;
                var enemies = battle.GetTeamCharacters(enemyTeam);

                if (enemies.Any())
                {
                    // 找到最近的敌人
                    var nearestEnemy = enemies
                        .OrderBy(e => Math.Abs(e.X - character.X) + Math.Abs(e.Y - character.Y))
                        .First();

                    // 计算移动范围
                    var moveRange = battle.CalculateMoveRange(character.Character.Id);

                    // 找到最接近敌人的位置
                    var bestPosition = moveRange
                        .OrderBy(p => Math.Abs(p.X - nearestEnemy.X) + Math.Abs(p.Y - nearestEnemy.Y))
                        .First();

                    actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 80));
                }
            }
        }

        /// <summary>
        /// 添加防守模式行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        private void AddDefensiveActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 如果处于危险位置，优先移动到安全位置
            if (evaluation.IsInDangerousPosition && evaluation.SafePositions.Count > 0)
            {
                var bestSafePosition = evaluation.SafePositions
                    .OrderByDescending(p => p.DefenseBonus)
                    .ThenByDescending(p => p.Value)
                    .First();

                actions.Add(AIAction.CreateMoveAction(bestSafePosition.X, bestSafePosition.Y, 95));
            }

            // 添加防御技能行动
            var defensiveSkills = character.Character.Skills
                .Where(s => s.IsDefensive)
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
            }

            // 如果有威胁很大的敌人在攻击范围内，添加攻击行动
            if (evaluation.AttackableEnemies.Count > 0)
            {
                var highThreatEnemy = evaluation.AttackableEnemies
                    .Where(e => e.ThreatLevel > 70)
                    .OrderByDescending(e => e.ThreatLevel)
                    .FirstOrDefault();

                if (highThreatEnemy != null)
                {
                    actions.Add(AIAction.CreateAttackAction(highThreatEnemy.CharacterId, 85));
                }
            }

            // 如果没有处于危险位置，但有更好的防御位置，考虑移动
            if (!evaluation.IsInDangerousPosition && evaluation.TacticalPositions.Count > 0)
            {
                var bestDefensivePosition = evaluation.TacticalPositions
                    .OrderByDescending(p => p.DefenseBonus)
                    .ThenBy(p => p.DangerLevel)
                    .First();

                actions.Add(AIAction.CreateMoveAction(bestDefensivePosition.X, bestDefensivePosition.Y, 70));
            }
        }

        /// <summary>
        /// 添加支援模式行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        private void AddSupportActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 添加治疗技能行动
            var healingSkills = character.Character.Skills
                .Where(s => s.IsHealing)
                .OrderByDescending(s => s.Power)
                .ToList();

            if (healingSkills.Any() && evaluation.HealableAllies.Count > 0)
            {
                var bestSkill = healingSkills.First();
                var mostInjuredAlly = evaluation.HealableAllies
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
                        95));
                }
                else
                {
                    // 如果不在范围内，尝试移动到可以治疗的位置
                    var moveRange = battle.CalculateMoveRange(character.Character.Id);
                    var bestPosition = moveRange
                        .Where(p => Math.Abs(p.X - mostInjuredAlly.X) + Math.Abs(p.Y - mostInjuredAlly.Y) <= bestSkill.Range)
                        .OrderBy(p => Math.Abs(p.X - mostInjuredAlly.X) + Math.Abs(p.Y - mostInjuredAlly.Y))
                        .FirstOrDefault();

                    if (bestPosition != default)
                    {
                        actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 90));
                    }
                }
            }

            // 添加增益技能行动
            var buffSkills = character.Character.Skills
                .Where(s => s.IsBuff)
                .OrderByDescending(s => s.Power)
                .ToList();

            if (buffSkills.Any())
            {
                var bestSkill = buffSkills.First();
                var teamCharacters = battle.GetTeamCharacters(character.Team);

                // 找到最强的队友
                var bestAlly = teamCharacters
                    .Where(c => c.Character.Id != character.Character.Id)
                    .OrderByDescending(c => c.Character.CalculateCombatStats().PhysicalAttack + c.Character.CalculateCombatStats().MagicAttack)
                    .FirstOrDefault();

                if (bestAlly != null)
                {
                    // 检查是否在技能范围内
                    int distance = Math.Abs(character.X - bestAlly.X) + Math.Abs(character.Y - bestAlly.Y);
                    if (distance <= bestSkill.Range)
                    {
                        actions.Add(AIAction.CreateSkillAction(
                            bestSkill.Id,
                            bestAlly.X,
                            bestAlly.Y,
                            85));
                    }
                }
            }

            // 如果有敌人在攻击范围内，添加攻击行动（低优先级）
            if (evaluation.AttackableEnemies.Count > 0)
            {
                var bestTarget = evaluation.AttackableEnemies
                    .OrderByDescending(e => e.ThreatLevel)
                    .First();

                actions.Add(AIAction.CreateAttackAction(bestTarget.CharacterId, 60));
            }
        }

        /// <summary>
        /// 添加平衡模式行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        private void AddBalancedActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 如果有生命值很低的队友需要治疗，优先考虑治疗
            var criticalAlly = evaluation.HealableAllies
                .Where(a => a.HealthPercentage < 30)
                .OrderBy(a => a.HealthPercentage)
                .FirstOrDefault();

            if (criticalAlly != null)
            {
                var healingSkills = character.Character.Skills
                    .Where(s => s.IsHealing)
                    .OrderByDescending(s => s.Power)
                    .ToList();

                if (healingSkills.Any())
                {
                    var bestSkill = healingSkills.First();
                    int distance = Math.Abs(character.X - criticalAlly.X) + Math.Abs(character.Y - criticalAlly.Y);

                    if (distance <= bestSkill.Range)
                    {
                        actions.Add(AIAction.CreateSkillAction(
                            bestSkill.Id,
                            criticalAlly.X,
                            criticalAlly.Y,
                            90));
                    }
                    else
                    {
                        // 尝试移动到可以治疗的位置
                        var moveRange = battle.CalculateMoveRange(character.Character.Id);
                        var bestPosition = moveRange
                            .Where(p => Math.Abs(p.X - criticalAlly.X) + Math.Abs(p.Y - criticalAlly.Y) <= bestSkill.Range)
                            .OrderBy(p => Math.Abs(p.X - criticalAlly.X) + Math.Abs(p.Y - criticalAlly.Y))
                            .FirstOrDefault();

                        if (bestPosition != default)
                        {
                            actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 85));
                        }
                    }
                }
            }
            // 如果有生命值很低的敌人可以攻击，优先考虑攻击
            else if (evaluation.AttackableEnemies.Any(e => e.HealthPercentage < 30))
            {
                var weakEnemy = evaluation.AttackableEnemies
                    .Where(e => e.HealthPercentage < 30)
                    .OrderBy(e => e.HealthPercentage)
                    .First();

                actions.Add(AIAction.CreateAttackAction(weakEnemy.CharacterId, 85));
            }
            // 如果处于危险位置，考虑移动到安全位置
            else if (evaluation.IsInDangerousPosition && evaluation.SafePositions.Count > 0)
            {
                var bestSafePosition = evaluation.SafePositions
                    .OrderByDescending(p => p.Value)
                    .First();

                actions.Add(AIAction.CreateMoveAction(bestSafePosition.X, bestSafePosition.Y, 80));
            }
            // 如果有可攻击的敌人，考虑攻击
            else if (evaluation.AttackableEnemies.Count > 0)
            {
                var bestTarget = evaluation.AttackableEnemies
                    .OrderByDescending(e => e.ThreatLevel)
                    .ThenBy(e => e.HealthPercentage)
                    .First();

                actions.Add(AIAction.CreateAttackAction(bestTarget.CharacterId, 75));
            }
            // 如果没有可攻击的敌人，考虑移动到战术位置
            else if (evaluation.TacticalPositions.Count > 0)
            {
                var bestPosition = evaluation.TacticalPositions
                    .OrderByDescending(p => p.Value)
                    .First();

                actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 70));
            }

            // 考虑使用技能
            var bestSkillAction = GetBestSkillAction(character, battle, evaluation);
            if (bestSkillAction != null)
            {
                actions.Add(bestSkillAction);
            }
        }

        /// <summary>
        /// 获取最佳技能行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <returns>技能行动</returns>
        private AIAction GetBestSkillAction(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation)
        {
            var skills = character.Character.Skills;
            if (skills == null || !skills.Any())
                return null;

            // 按照优先级排序技能
            var prioritizedSkills = new List<(ISkill Skill, int Priority, int TargetX, int TargetY)>();

            foreach (var skill in skills)
            {
                // 检查MP是否足够
                if (character.CurrentMP < skill.MPCost)
                    continue;

                int priority = 0;
                int targetX = 0;
                int targetY = 0;

                // 治疗技能
                if (skill.IsHealing && evaluation.HealableAllies.Any())
                {
                    var bestTarget = evaluation.HealableAllies
                        .OrderBy(a => a.HealthPercentage)
                        .First();

                    int distance = Math.Abs(character.X - bestTarget.X) + Math.Abs(character.Y - bestTarget.Y);
                    if (distance <= skill.Range)
                    {
                        priority = 100 - bestTarget.HealthPercentage;
                        targetX = bestTarget.X;
                        targetY = bestTarget.Y;
                    }
                }
                // 增益技能
                else if (skill.IsBuff)
                {
                    var teamCharacters = battle.GetTeamCharacters(character.Team);
                    var bestAlly = teamCharacters
                        .Where(c => c.Character.Id != character.Character.Id)
                        .OrderByDescending(c => c.Character.CalculateCombatStats().PhysicalAttack + c.Character.CalculateCombatStats().MagicAttack)
                        .FirstOrDefault();

                    if (bestAlly != null)
                    {
                        int distance = Math.Abs(character.X - bestAlly.X) + Math.Abs(character.Y - bestAlly.Y);
                        if (distance <= skill.Range)
                        {
                            priority = 60;
                            targetX = bestAlly.X;
                            targetY = bestAlly.Y;
                        }
                    }
                }
                // 攻击技能
                else if (evaluation.AttackableEnemies.Any())
                {
                    var bestTarget = evaluation.AttackableEnemies
                        .OrderBy(e => e.HealthPercentage)
                        .First();

                    int distance = Math.Abs(character.X - bestTarget.X) + Math.Abs(character.Y - bestTarget.Y);
                    if (distance <= skill.Range)
                    {
                        priority = 70;
                        targetX = bestTarget.X;
                        targetY = bestTarget.Y;
                    }
                }

                if (priority > 0)
                {
                    prioritizedSkills.Add((skill, priority, targetX, targetY));
                }
            }

            // 选择优先级最高的技能
            var bestSkill = prioritizedSkills
                .OrderByDescending(s => s.Priority)
                .FirstOrDefault();

            if (bestSkill != default)
            {
                return AIAction.CreateSkillAction(
                    bestSkill.Skill.Id,
                    bestSkill.TargetX,
                    bestSkill.TargetY,
                    bestSkill.Priority);
            }

            return null;
        }
    }

    /// <summary>
    /// 战术模式枚举
    /// </summary>
    public enum TacticalMode
    {
        /// <summary>
        /// 平衡模式
        /// </summary>
        Balanced,

        /// <summary>
        /// 进攻模式
        /// </summary>
        Aggressive,

        /// <summary>
        /// 防守模式
        /// </summary>
        Defensive,

        /// <summary>
        /// 支援模式
        /// </summary>
        Support
    }
}