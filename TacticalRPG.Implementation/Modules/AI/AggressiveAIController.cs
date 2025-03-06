using System;
using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.AI;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Implementation.Modules.AI
{
    /// <summary>
    /// 进攻型AI控制器，优先攻击敌人
    /// </summary>
    public class AggressiveAIController : BaseAIController
    {
        /// <summary>
        /// 获取AI类型
        /// </summary>
        public override AIType Type => AIType.Aggressive;

        /// <summary>
        /// 添加攻击行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        protected override void AddAttackActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 如果有可攻击的敌人，添加攻击行动，并提高优先级
            if (evaluation.AttackableEnemies.Count > 0)
            {
                var bestTarget = evaluation.AttackableEnemies.OrderByDescending(t => t.Priority).First();
                actions.Add(AIAction.CreateAttackAction(bestTarget.CharacterId, 90)); // 提高攻击优先级
            }
        }

        /// <summary>
        /// 添加移动行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        protected override void AddMoveActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            var moveRange = battle.CalculateMoveRange(character.Character.Id);
            var enemyTeam = character.Team == BattleTeam.Player ? BattleTeam.Enemy : BattleTeam.Player;
            var enemies = battle.GetTeamCharacters(enemyTeam);

            // 如果没有可攻击的敌人，尝试移动到能攻击敌人的位置
            if (evaluation.AttackableEnemies.Count == 0 && enemies.Count > 0)
            {
                // 找到最近的敌人
                var nearestEnemy = enemies.OrderBy(e =>
                    Math.Abs(e.X - character.X) +
                    Math.Abs(e.Y - character.Y)).First();

                // 计算向敌人移动的最佳位置
                var bestPosition = moveRange
                    .OrderBy(p => Math.Abs(p.X - nearestEnemy.X) + Math.Abs(p.Y - nearestEnemy.Y))
                    .FirstOrDefault();

                if (bestPosition != default)
                {
                    actions.Add(AIAction.CreateMoveAction(bestPosition.X, bestPosition.Y, 85));
                }
            }
            else
            {
                // 如果处于危险位置且生命值低于30%，考虑撤退
                if (evaluation.IsInDangerousPosition && evaluation.CurrentCharacterHealth < 30 && evaluation.SafePositions.Count > 0)
                {
                    var bestSafePosition = evaluation.SafePositions.OrderByDescending(p => p.Value).First();
                    actions.Add(AIAction.CreateMoveAction(bestSafePosition.X, bestSafePosition.Y, 70));
                }
                // 否则考虑移动到战术位置
                else if (evaluation.TacticalPositions.Count > 0)
                {
                    // 对于进攻型AI，优先选择可攻击敌人数量最多的位置
                    var bestTacticalPosition = evaluation.TacticalPositions
                        .OrderByDescending(p => p.AttackableEnemiesCount)
                        .ThenByDescending(p => p.Value)
                        .First();
                    actions.Add(AIAction.CreateMoveAction(bestTacticalPosition.X, bestTacticalPosition.Y, 60));
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
            // 进攻型AI更关注生命值低的敌人和预期伤害高的目标
            return threatLevel / 2 + (100 - healthPercentage) + expectedDamage * 2;
        }
    }
}