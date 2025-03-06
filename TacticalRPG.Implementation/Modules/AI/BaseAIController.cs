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
    /// 基础AI控制器，作为所有AI策略的基类
    /// </summary>
    public abstract class BaseAIController : IAIController
    {
        /// <summary>
        /// 获取AI类型
        /// </summary>
        public abstract AIType Type { get; }

        /// <summary>
        /// 获取被控制的角色ID
        /// </summary>
        public Guid CharacterId { get; private set; }

        /// <summary>
        /// 设置被控制的角色ID
        /// </summary>
        /// <param name="characterId">角色ID</param>
        public void SetCharacterId(Guid characterId)
        {
            CharacterId = characterId;
        }

        /// <summary>
        /// 决策下一步行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <returns>AI行动决策结果</returns>
        public virtual async Task<AIAction> DecideNextAction(IBattleCharacter character, IBattle battle)
        {
            // 评估战场状态
            var evaluation = EvaluateBattlefield(character, battle);

            // 获取所有可能的行动
            var possibleActions = new List<AIAction>();

            // 添加移动行动
            AddMoveActions(character, battle, evaluation, possibleActions);

            // 添加攻击行动
            AddAttackActions(character, battle, evaluation, possibleActions);

            // 添加技能行动
            AddSkillActions(character, battle, evaluation, possibleActions);

            // 添加物品行动
            AddItemActions(character, battle, evaluation, possibleActions);

            // 添加等待行动
            possibleActions.Add(AIAction.CreateWaitAction(0));

            // 添加结束回合行动
            possibleActions.Add(AIAction.CreateEndTurnAction(-10));

            // 根据优先级选择最佳行动
            return possibleActions.OrderByDescending(a => a.Priority).FirstOrDefault()
                ?? AIAction.CreateEndTurnAction(0);
        }

        /// <summary>
        /// 评估当前战场状态
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <returns>战场评估结果</returns>
        public virtual BattleEvaluation EvaluateBattlefield(IBattleCharacter character, IBattle battle)
        {
            var evaluation = new BattleEvaluation();

            // 获取我方和敌方队伍
            var myTeam = character.Team;
            var enemyTeam = myTeam == BattleTeam.Player ? BattleTeam.Enemy : BattleTeam.Player;

            // 获取我方和敌方角色
            var teamCharacters = battle.GetTeamCharacters(myTeam);
            var enemyCharacters = battle.GetTeamCharacters(enemyTeam);

            // 计算队伍健康度
            evaluation.TeamHealth = CalculateTeamHealth(teamCharacters);
            evaluation.EnemyHealth = CalculateTeamHealth(enemyCharacters);

            // 计算数量优势
            evaluation.TeamNumberAdvantage = CalculateNumberAdvantage(teamCharacters.Count, enemyCharacters.Count);

            // 计算当前角色健康状态
            evaluation.CurrentCharacterHealth = CalculateCharacterHealth(character);

            // 检查是否处于危险位置
            evaluation.IsInDangerousPosition = IsInDangerousPosition(character, battle, enemyCharacters);

            // 评估可攻击的敌人
            evaluation.AttackableEnemies = EvaluateAttackableEnemies(character, battle, enemyCharacters);

            // 评估需要治疗的队友
            evaluation.HealableAllies = EvaluateHealableAllies(character, battle, teamCharacters);

            // 评估安全位置
            evaluation.SafePositions = EvaluateSafePositions(character, battle, enemyCharacters);

            // 评估战术位置
            evaluation.TacticalPositions = EvaluateTacticalPositions(character, battle, enemyCharacters);

            // 计算整体战场态势
            evaluation.OverallSituation = CalculateOverallSituation(evaluation);

            return evaluation;
        }

        /// <summary>
        /// 添加移动行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        protected virtual void AddMoveActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 如果处于危险位置，优先考虑移动到安全位置
            if (evaluation.IsInDangerousPosition && evaluation.SafePositions.Count > 0)
            {
                var bestSafePosition = evaluation.SafePositions.OrderByDescending(p => p.Value).First();
                actions.Add(AIAction.CreateMoveAction(bestSafePosition.X, bestSafePosition.Y, 80));
                return;
            }

            // 否则考虑移动到战术位置
            if (evaluation.TacticalPositions.Count > 0)
            {
                var bestTacticalPosition = evaluation.TacticalPositions.OrderByDescending(p => p.Value).First();
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
        protected virtual void AddAttackActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 如果有可攻击的敌人，添加攻击行动
            if (evaluation.AttackableEnemies.Count > 0)
            {
                var bestTarget = evaluation.AttackableEnemies.OrderByDescending(t => t.Priority).First();
                actions.Add(AIAction.CreateAttackAction(bestTarget.CharacterId, 70));
            }
        }

        /// <summary>
        /// 添加技能行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        protected virtual void AddSkillActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 如果有可用技能，添加技能行动
            if (evaluation.AvailableSkills.Count > 0)
            {
                var bestSkill = evaluation.AvailableSkills.OrderByDescending(s => s.Value).First();
                actions.Add(AIAction.CreateSkillAction(bestSkill.SkillId, bestSkill.BestTargetX, bestSkill.BestTargetY, bestSkill.Value));
            }
        }

        /// <summary>
        /// 添加物品行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="evaluation">战场评估</param>
        /// <param name="actions">行动列表</param>
        protected virtual void AddItemActions(IBattleCharacter character, IBattle battle, BattleEvaluation evaluation, List<AIAction> actions)
        {
            // 如果有可用物品，添加物品行动
            if (evaluation.AvailableItems.Count > 0)
            {
                var bestItem = evaluation.AvailableItems.OrderByDescending(i => i.Value).First();
                actions.Add(AIAction.CreateItemAction(bestItem.ItemId, bestItem.BestTargetId, bestItem.Value));
            }
        }

        /// <summary>
        /// 计算队伍健康度
        /// </summary>
        /// <param name="characters">队伍角色列表</param>
        /// <returns>队伍健康度（0-100）</returns>
        protected virtual int CalculateTeamHealth(IReadOnlyList<IBattleCharacter> characters)
        {
            if (characters.Count == 0)
                return 0;

            int totalHealth = characters.Sum(c => c.CurrentHP * 100 / Math.Max(1, c.Character.MaxHP));
            return totalHealth / characters.Count;
        }

        /// <summary>
        /// 计算数量优势
        /// </summary>
        /// <param name="teamCount">我方数量</param>
        /// <param name="enemyCount">敌方数量</param>
        /// <returns>数量优势（-100到100）</returns>
        protected virtual int CalculateNumberAdvantage(int teamCount, int enemyCount)
        {
            if (teamCount == 0 && enemyCount == 0)
                return 0;

            int totalCount = teamCount + enemyCount;
            return (teamCount - enemyCount) * 100 / Math.Max(1, totalCount);
        }

        /// <summary>
        /// 计算角色健康状态
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <returns>角色健康状态（0-100）</returns>
        protected virtual int CalculateCharacterHealth(IBattleCharacter character)
        {
            return character.CurrentHP * 100 / Math.Max(1, character.Character.MaxHP);
        }

        /// <summary>
        /// 检查是否处于危险位置
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="enemyCharacters">敌方角色列表</param>
        /// <returns>是否处于危险位置</returns>
        protected virtual bool IsInDangerousPosition(IBattleCharacter character, IBattle battle, IReadOnlyList<IBattleCharacter> enemyCharacters)
        {
            // 简单实现：如果有敌人在攻击范围内，则认为处于危险位置
            foreach (var enemy in enemyCharacters)
            {
                var attackRange = battle.CalculateAttackRange(enemy.Character.Id);
                if (attackRange.Any(p => p.X == character.X && p.Y == character.Y))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 评估可攻击的敌人
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="enemyCharacters">敌方角色列表</param>
        /// <returns>可攻击的敌人评估列表</returns>
        protected virtual List<TargetEvaluation> EvaluateAttackableEnemies(IBattleCharacter character, IBattle battle, IReadOnlyList<IBattleCharacter> enemyCharacters)
        {
            var attackableEnemies = new List<TargetEvaluation>();
            var attackRange = battle.CalculateAttackRange(character.Character.Id);

            foreach (var enemy in enemyCharacters)
            {
                if (attackRange.Any(p => p.X == enemy.X && p.Y == enemy.Y))
                {
                    var healthPercentage = enemy.CurrentHP * 100 / Math.Max(1, enemy.Character.MaxHP);
                    var threatLevel = CalculateThreatLevel(enemy);
                    var expectedDamage = CalculateExpectedDamage(character, enemy);
                    var priority = CalculateTargetPriority(healthPercentage, threatLevel, expectedDamage);

                    attackableEnemies.Add(new TargetEvaluation
                    {
                        CharacterId = enemy.Character.Id,
                        X = enemy.X,
                        Y = enemy.Y,
                        HealthPercentage = healthPercentage,
                        ThreatLevel = threatLevel,
                        ExpectedDamage = expectedDamage,
                        Priority = priority
                    });
                }
            }

            return attackableEnemies;
        }

        /// <summary>
        /// 评估需要治疗的队友
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="teamCharacters">队伍角色列表</param>
        /// <returns>需要治疗的队友评估列表</returns>
        protected virtual List<TargetEvaluation> EvaluateHealableAllies(IBattleCharacter character, IBattle battle, IReadOnlyList<IBattleCharacter> teamCharacters)
        {
            // 简单实现：生命值低于50%的队友需要治疗
            var healableAllies = new List<TargetEvaluation>();

            foreach (var ally in teamCharacters)
            {
                if (ally.Character.Id != character.Character.Id)
                {
                    var healthPercentage = ally.CurrentHP * 100 / Math.Max(1, ally.Character.MaxHP);
                    if (healthPercentage < 50)
                    {
                        healableAllies.Add(new TargetEvaluation
                        {
                            CharacterId = ally.Character.Id,
                            X = ally.X,
                            Y = ally.Y,
                            HealthPercentage = healthPercentage,
                            Priority = 100 - healthPercentage // 生命值越低，优先级越高
                        });
                    }
                }
            }

            return healableAllies;
        }

        /// <summary>
        /// 评估安全位置
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="enemyCharacters">敌方角色列表</param>
        /// <returns>安全位置评估列表</returns>
        protected virtual List<PositionEvaluation> EvaluateSafePositions(IBattleCharacter character, IBattle battle, IReadOnlyList<IBattleCharacter> enemyCharacters)
        {
            var safePositions = new List<PositionEvaluation>();
            var moveRange = battle.CalculateMoveRange(character.Character.Id);

            // 计算所有敌人的攻击范围
            var enemyAttackRanges = new List<(int X, int Y)>();
            foreach (var enemy in enemyCharacters)
            {
                var attackRange = battle.CalculateAttackRange(enemy.Character.Id);
                enemyAttackRanges.AddRange(attackRange);
            }

            // 评估每个可移动位置
            foreach (var position in moveRange)
            {
                // 如果该位置不在敌人攻击范围内，则认为是安全位置
                if (!enemyAttackRanges.Any(p => p.X == position.X && p.Y == position.Y))
                {
                    var terrain = battle.Map.GetTerrain(position.X, position.Y);
                    var defenseBonus = terrain?.DefenseBonus ?? 0;

                    safePositions.Add(new PositionEvaluation
                    {
                        X = position.X,
                        Y = position.Y,
                        Value = 50 + defenseBonus, // 基础价值50，加上地形防御加成
                        DefenseBonus = defenseBonus,
                        DangerLevel = 0 // 安全位置危险等级为0
                    });
                }
            }

            return safePositions;
        }

        /// <summary>
        /// 评估战术位置
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <param name="enemyCharacters">敌方角色列表</param>
        /// <returns>战术位置评估列表</returns>
        protected virtual List<PositionEvaluation> EvaluateTacticalPositions(IBattleCharacter character, IBattle battle, IReadOnlyList<IBattleCharacter> enemyCharacters)
        {
            var tacticalPositions = new List<PositionEvaluation>();
            var moveRange = battle.CalculateMoveRange(character.Character.Id);
            var combatStats = character.Character.CalculateCombatStats();

            // 评估每个可移动位置
            foreach (var position in moveRange)
            {
                // 计算从该位置可攻击的敌人数量
                int attackableEnemiesCount = 0;
                foreach (var enemy in enemyCharacters)
                {
                    int distance = Math.Abs(position.X - enemy.X) + Math.Abs(position.Y - enemy.Y);
                    if (distance <= combatStats.AttackRange)
                    {
                        attackableEnemiesCount++;
                    }
                }

                // 计算该位置的危险等级
                int dangerLevel = 0;
                foreach (var enemy in enemyCharacters)
                {
                    int distance = Math.Abs(position.X - enemy.X) + Math.Abs(position.Y - enemy.Y);
                    var enemyCombatStats = enemy.Character.CalculateCombatStats();
                    if (distance <= enemyCombatStats.AttackRange)
                    {
                        dangerLevel += 20; // 每个可能攻击到该位置的敌人增加20点危险等级
                    }
                }

                // 获取地形防御加成
                var terrain = battle.Map.GetTerrain(position.X, position.Y);
                var defenseBonus = terrain?.DefenseBonus ?? 0;

                // 计算位置价值
                int value = attackableEnemiesCount * 10 + defenseBonus - dangerLevel / 2;

                tacticalPositions.Add(new PositionEvaluation
                {
                    X = position.X,
                    Y = position.Y,
                    Value = Math.Max(0, Math.Min(100, value)), // 确保价值在0-100范围内
                    AttackableEnemiesCount = attackableEnemiesCount,
                    DefenseBonus = defenseBonus,
                    DangerLevel = dangerLevel
                });
            }

            return tacticalPositions;
        }

        /// <summary>
        /// 计算目标威胁等级
        /// </summary>
        /// <param name="target">目标角色</param>
        /// <returns>威胁等级（0-100）</returns>
        protected virtual int CalculateThreatLevel(IBattleCharacter target)
        {
            // 简单实现：基于攻击力和生命值计算威胁等级
            var combatStats = target.Character.CalculateCombatStats();
            int attackThreat = combatStats.PhysicalAttack * 2;
            int healthThreat = target.CurrentHP;
            return Math.Min(100, (attackThreat + healthThreat) / 3);
        }

        /// <summary>
        /// 计算预期伤害
        /// </summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="target">目标</param>
        /// <returns>预期伤害值</returns>
        protected virtual int CalculateExpectedDamage(IBattleCharacter attacker, IBattleCharacter target)
        {
            // 简单实现：攻击力减去防御力的差值
            var attackerStats = attacker.Character.CalculateCombatStats();
            var targetStats = target.Character.CalculateCombatStats();
            return Math.Max(1, attackerStats.PhysicalAttack - targetStats.PhysicalDefense / 2);
        }

        /// <summary>
        /// 计算目标优先级
        /// </summary>
        /// <param name="healthPercentage">目标生命值百分比</param>
        /// <param name="threatLevel">目标威胁等级</param>
        /// <param name="expectedDamage">预期伤害</param>
        /// <returns>目标优先级</returns>
        protected virtual int CalculateTargetPriority(int healthPercentage, int threatLevel, int expectedDamage)
        {
            // 简单实现：威胁等级越高，生命值越低，预期伤害越高，优先级越高
            return threatLevel + (100 - healthPercentage) / 2 + expectedDamage;
        }

        /// <summary>
        /// 计算整体战场态势
        /// </summary>
        /// <param name="evaluation">战场评估</param>
        /// <returns>整体战场态势（-100到100）</returns>
        protected virtual int CalculateOverallSituation(BattleEvaluation evaluation)
        {
            // 简单实现：基于队伍健康度、数量优势和位置优势计算整体态势
            return (evaluation.TeamHealth - evaluation.EnemyHealth) / 2
                + evaluation.TeamNumberAdvantage / 2
                + evaluation.TeamPositionAdvantage / 2;
        }
    }
}