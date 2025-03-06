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
    /// 战场评估器，用于评估战场状况
    /// </summary>
    public class AIBattleEvaluator : IAIBattleEvaluator
    {
        /// <summary>
        /// 评估战场状况
        /// </summary>
        /// <param name="character">当前角色</param>
        /// <param name="battle">战斗实例</param>
        /// <returns>战场评估结果</returns>
        public BattleEvaluation EvaluateBattle(IBattleCharacter character, IBattle battle)
        {
            var evaluation = new BattleEvaluation
            {
                CurrentCharacterHealth = CalculateHealthPercentage(character),
                IsInDangerousPosition = IsInDangerousPosition(character, battle),
                SafePositions = FindSafePositions(character, battle),
                TacticalPositions = FindTacticalPositions(character, battle),
                AttackableEnemies = FindAttackableEnemies(character, battle),
                HealableAllies = FindHealableAllies(character, battle),
                AvailableSkills = FindAvailableSkills(character, battle),
                AvailableItems = FindAvailableItems(character, battle)
            };

            return evaluation;
        }

        /// <summary>
        /// 计算角色生命值百分比
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <returns>生命值百分比</returns>
        private int CalculateHealthPercentage(IBattleCharacter character)
        {
            int maxHP = character.Character.MaxHP;
            if (maxHP <= 0)
                return 100;

            return character.CurrentHP * 100 / maxHP;
        }

        /// <summary>
        /// 判断角色是否处于危险位置
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">战斗实例</param>
        /// <returns>是否处于危险位置</returns>
        private bool IsInDangerousPosition(IBattleCharacter character, IBattle battle)
        {
            // 获取敌方队伍
            var enemyTeam = character.Team == BattleTeam.Player ? BattleTeam.Enemy : BattleTeam.Player;
            var enemies = battle.GetTeamCharacters(enemyTeam);

            // 计算有多少敌人可以攻击到当前位置
            int threateningEnemies = 0;
            foreach (var enemy in enemies)
            {
                var combatStats = enemy.Character.CalculateCombatStats();
                int distance = Math.Abs(enemy.X - character.X) + Math.Abs(enemy.Y - character.Y);

                // 如果敌人在攻击范围内
                if (distance <= combatStats.AttackRange)
                {
                    threateningEnemies++;
                }
            }

            // 如果有2个或以上敌人可以攻击到当前位置，或者生命值低于30%且有敌人可以攻击到，则认为处于危险位置
            return threateningEnemies >= 2 || (CalculateHealthPercentage(character) < 30 && threateningEnemies > 0);
        }

        /// <summary>
        /// 寻找安全位置
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">战斗实例</param>
        /// <returns>安全位置列表</returns>
        private List<TacticalPosition> FindSafePositions(IBattleCharacter character, IBattle battle)
        {
            var safePositions = new List<TacticalPosition>();
            var moveRange = battle.CalculateMoveRange(character.Character.Id);

            // 获取敌方队伍
            var enemyTeam = character.Team == BattleTeam.Player ? BattleTeam.Enemy : BattleTeam.Player;
            var enemies = battle.GetTeamCharacters(enemyTeam);

            // 计算每个可移动位置的安全度
            foreach (var position in moveRange)
            {
                int threateningEnemies = 0;
                int defenseBonus = battle.Map.GetTerrain(position.X, position.Y)?.DefenseBonus ?? 0;

                foreach (var enemy in enemies)
                {
                    var combatStats = enemy.Character.CalculateCombatStats();
                    int distance = Math.Abs(enemy.X - position.X) + Math.Abs(enemy.Y - position.Y);

                    // 如果敌人在攻击范围内
                    if (distance <= combatStats.AttackRange)
                    {
                        threateningEnemies++;
                    }
                }

                // 如果没有敌人可以攻击到，或者只有一个敌人可以攻击到且地形防御加成高，则认为是安全位置
                if (threateningEnemies == 0 || (threateningEnemies == 1 && defenseBonus >= 20))
                {
                    int value = 100 - threateningEnemies * 30 + defenseBonus;
                    safePositions.Add(new TacticalPosition
                    {
                        X = position.X,
                        Y = position.Y,
                        Value = value,
                        DefenseBonus = defenseBonus,
                        DangerLevel = threateningEnemies
                    });
                }
            }

            return safePositions;
        }

        /// <summary>
        /// 寻找战术位置
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">战斗实例</param>
        /// <returns>战术位置列表</returns>
        private List<TacticalPosition> FindTacticalPositions(IBattleCharacter character, IBattle battle)
        {
            var tacticalPositions = new List<TacticalPosition>();
            var moveRange = battle.CalculateMoveRange(character.Character.Id);

            // 获取敌方队伍
            var enemyTeam = character.Team == BattleTeam.Player ? BattleTeam.Enemy : BattleTeam.Player;
            var enemies = battle.GetTeamCharacters(enemyTeam);

            // 获取友方队伍
            var allies = battle.GetTeamCharacters(character.Team)
                .Where(c => c.Character.Id != character.Character.Id)
                .ToList();

            // 计算每个可移动位置的战术价值
            foreach (var position in moveRange)
            {
                int threateningEnemies = 0;
                int defenseBonus = battle.Map.GetTerrain(position.X, position.Y)?.DefenseBonus ?? 0;
                int attackableEnemies = 0;
                int proximityToAllies = 0;

                // 计算有多少敌人可以攻击到该位置
                foreach (var enemy in enemies)
                {
                    var enemyCombatStats = enemy.Character.CalculateCombatStats();
                    int distanceToEnemy = Math.Abs(enemy.X - position.X) + Math.Abs(enemy.Y - position.Y);

                    if (distanceToEnemy <= enemyCombatStats.AttackRange)
                    {
                        threateningEnemies++;
                    }
                }

                // 计算从该位置可以攻击到多少敌人
                var characterCombatStats = character.Character.CalculateCombatStats();
                foreach (var enemy in enemies)
                {
                    int distanceToEnemy = Math.Abs(enemy.X - position.X) + Math.Abs(enemy.Y - position.Y);

                    if (distanceToEnemy <= characterCombatStats.AttackRange)
                    {
                        attackableEnemies++;
                    }
                }

                // 计算与队友的接近度
                foreach (var ally in allies)
                {
                    int distanceToAlly = Math.Abs(ally.X - position.X) + Math.Abs(ally.Y - position.Y);

                    // 如果距离小于等于3，增加接近度
                    if (distanceToAlly <= 3)
                    {
                        proximityToAllies += 3 - distanceToAlly;
                    }
                }

                // 计算战术价值
                int value = attackableEnemies * 20 + defenseBonus + proximityToAllies * 5 - threateningEnemies * 15;

                tacticalPositions.Add(new TacticalPosition
                {
                    X = position.X,
                    Y = position.Y,
                    Value = value,
                    DefenseBonus = defenseBonus,
                    DangerLevel = threateningEnemies
                });
            }

            return tacticalPositions;
        }

        /// <summary>
        /// 寻找可攻击的敌人
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">战斗实例</param>
        /// <returns>可攻击敌人列表</returns>
        private List<TargetInfo> FindAttackableEnemies(IBattleCharacter character, IBattle battle)
        {
            var attackableEnemies = new List<TargetInfo>();

            // 获取敌方队伍
            var enemyTeam = character.Team == BattleTeam.Player ? BattleTeam.Enemy : BattleTeam.Player;
            var enemies = battle.GetTeamCharacters(enemyTeam);

            // 获取角色战斗属性
            var combatStats = character.Character.CalculateCombatStats();

            foreach (var enemy in enemies)
            {
                int distance = Math.Abs(enemy.X - character.X) + Math.Abs(enemy.Y - character.Y);

                // 如果敌人在攻击范围内
                if (distance <= combatStats.AttackRange)
                {
                    // 计算预期伤害
                    int expectedDamage = CalculateExpectedDamage(character, enemy);

                    // 计算敌人生命值百分比
                    int healthPercentage = enemy.CurrentHP * 100 / Math.Max(1, enemy.Character.MaxHP);

                    // 计算敌人威胁等级
                    int threatLevel = CalculateThreatLevel(enemy, battle);

                    attackableEnemies.Add(new TargetInfo
                    {
                        CharacterId = enemy.Character.Id,
                        X = enemy.X,
                        Y = enemy.Y,
                        HealthPercentage = healthPercentage,
                        ThreatLevel = threatLevel,
                        ExpectedDamage = expectedDamage,
                        Priority = CalculateTargetPriority(healthPercentage, threatLevel, expectedDamage)
                    });
                }
            }

            return attackableEnemies;
        }

        /// <summary>
        /// 寻找可治疗的队友
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">战斗实例</param>
        /// <returns>可治疗队友列表</returns>
        private List<TargetInfo> FindHealableAllies(IBattleCharacter character, IBattle battle)
        {
            var healableAllies = new List<TargetInfo>();

            // 获取友方队伍
            var allies = battle.GetTeamCharacters(character.Team)
                .Where(c => c.Character.Id != character.Character.Id)
                .ToList();

            // 获取治疗技能的最大范围
            int maxHealRange = character.Character.Skills
                .Where(s => s.IsHealing)
                .Select(s => s.Range)
                .DefaultIfEmpty(0)
                .Max();

            foreach (var ally in allies)
            {
                int distance = Math.Abs(ally.X - character.X) + Math.Abs(ally.Y - character.Y);

                // 如果队友在治疗范围内且生命值不满
                if (distance <= maxHealRange && ally.CurrentHP < ally.Character.MaxHP)
                {
                    // 计算队友生命值百分比
                    int healthPercentage = ally.CurrentHP * 100 / Math.Max(1, ally.Character.MaxHP);

                    // 计算队友的战斗力
                    int combatPower = CalculateCombatPower(ally);

                    // 计算优先级：生命值越低，战斗力越高，优先级越高
                    int priority = (100 - healthPercentage) * 2 + combatPower / 10;

                    healableAllies.Add(new TargetInfo
                    {
                        CharacterId = ally.Character.Id,
                        X = ally.X,
                        Y = ally.Y,
                        HealthPercentage = healthPercentage,
                        ThreatLevel = 0,
                        ExpectedDamage = 0,
                        Priority = priority
                    });
                }
            }

            return healableAllies;
        }

        /// <summary>
        /// 寻找可用技能
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">战斗实例</param>
        /// <returns>可用技能列表</returns>
        private List<SkillInfo> FindAvailableSkills(IBattleCharacter character, IBattle battle)
        {
            var availableSkills = new List<SkillInfo>();

            // 获取所有技能
            var skills = character.Character.Skills.ToList();

            // 获取敌方队伍
            var enemyTeam = character.Team == BattleTeam.Player ? BattleTeam.Enemy : BattleTeam.Player;
            var enemies = battle.GetTeamCharacters(enemyTeam);

            // 获取友方队伍
            var allies = battle.GetTeamCharacters(character.Team)
                .Where(c => c.Character.Id != character.Character.Id)
                .ToList();

            foreach (var skill in skills)
            {
                // 检查MP是否足够
                if (character.CurrentMP < skill.MPCost)
                    continue;

                // 根据技能类型找到最佳目标
                int bestTargetX = character.X;
                int bestTargetY = character.Y;
                int value = 0;

                // 治疗技能
                if (skill.IsHealing)
                {
                    // 如果是自我治疗技能
                    if (skill.TargetType == SkillTargetType.Self)
                    {
                        int healthPercentage = CalculateHealthPercentage(character);
                        value = (100 - healthPercentage) * skill.Power / 10;
                    }
                    // 如果是队友治疗技能
                    else
                    {
                        var injuredAllies = allies.Where(a => a.CurrentHP < a.Character.MaxHP).ToList();

                        if (injuredAllies.Any())
                        {
                            var bestAlly = injuredAllies
                                .OrderBy(a => a.CurrentHP * 100 / Math.Max(1, a.Character.MaxHP))
                                .First();

                            int distance = Math.Abs(bestAlly.X - character.X) + Math.Abs(bestAlly.Y - character.Y);

                            if (distance <= skill.Range)
                            {
                                bestTargetX = bestAlly.X;
                                bestTargetY = bestAlly.Y;

                                int healthPercentage = bestAlly.CurrentHP * 100 / Math.Max(1, bestAlly.Character.MaxHP);
                                value = (100 - healthPercentage) * skill.Power / 10;
                            }
                        }
                    }
                }
                // 攻击技能
                else if (!skill.IsDefensive && !skill.IsBuff)
                {
                    if (enemies.Any())
                    {
                        var bestEnemy = enemies
                            .OrderBy(e => e.CurrentHP * 100 / Math.Max(1, e.Character.MaxHP))
                            .First();

                        int distance = Math.Abs(bestEnemy.X - character.X) + Math.Abs(bestEnemy.Y - character.Y);

                        if (distance <= skill.Range)
                        {
                            bestTargetX = bestEnemy.X;
                            bestTargetY = bestEnemy.Y;

                            int healthPercentage = bestEnemy.CurrentHP * 100 / Math.Max(1, bestEnemy.Character.MaxHP);
                            value = (100 - healthPercentage) * skill.Power / 10;
                        }
                    }
                }
                // 防御技能
                else if (skill.IsDefensive)
                {
                    int healthPercentage = CalculateHealthPercentage(character);
                    value = (100 - healthPercentage) * skill.Power / 20 + 30;
                }
                // 增益技能
                else if (skill.IsBuff)
                {
                    // 如果是自我增益技能
                    if (skill.TargetType == SkillTargetType.Self)
                    {
                        value = skill.Power / 2 + 20;
                    }
                    // 如果是队友增益技能
                    else
                    {
                        if (allies.Any())
                        {
                            var bestAlly = allies
                                .OrderByDescending(a => CalculateCombatPower(a))
                                .First();

                            int distance = Math.Abs(bestAlly.X - character.X) + Math.Abs(bestAlly.Y - character.Y);

                            if (distance <= skill.Range)
                            {
                                bestTargetX = bestAlly.X;
                                bestTargetY = bestAlly.Y;
                                value = skill.Power / 2 + 20;
                            }
                        }
                    }
                }

                // 如果技能有价值，添加到可用技能列表
                if (value > 0)
                {
                    availableSkills.Add(new SkillInfo
                    {
                        SkillId = skill.Id,
                        SkillType = GetSkillType(skill),
                        BestTargetX = bestTargetX,
                        BestTargetY = bestTargetY,
                        Value = value
                    });
                }
            }

            return availableSkills;
        }

        /// <summary>
        /// 寻找可用物品
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">战斗实例</param>
        /// <returns>可用物品列表</returns>
        private List<ItemInfo> FindAvailableItems(IBattleCharacter character, IBattle battle)
        {
            var availableItems = new List<ItemInfo>();

            // 获取所有物品
            var items = character.Character.Items.ToList();

            // 获取友方队伍
            var allies = battle.GetTeamCharacters(character.Team)
                .Where(c => c.Character.Id != character.Character.Id)
                .ToList();

            foreach (var item in items)
            {
                // 根据物品类型找到最佳目标
                int bestTargetId = character.Character.Id;
                int value = 0;

                // 治疗物品
                if (item.Type.Contains("Heal"))
                {
                    // 检查自己是否需要治疗
                    int healthPercentage = CalculateHealthPercentage(character);

                    if (healthPercentage < 70)
                    {
                        value = (100 - healthPercentage) * item.Power / 10;
                    }
                    // 检查队友是否需要治疗
                    else
                    {
                        var injuredAllies = allies.Where(a => a.CurrentHP < a.Character.MaxHP).ToList();

                        if (injuredAllies.Any())
                        {
                            var bestAlly = injuredAllies
                                .OrderBy(a => a.CurrentHP * 100 / Math.Max(1, a.Character.MaxHP))
                                .First();

                            int allyHealthPercentage = bestAlly.CurrentHP * 100 / Math.Max(1, bestAlly.Character.MaxHP);

                            if (allyHealthPercentage < 50)
                            {
                                bestTargetId = bestAlly.Character.Id;
                                value = (100 - allyHealthPercentage) * item.Power / 10;
                            }
                        }
                    }
                }
                // MP恢复物品
                else if (item.Type.Contains("MP"))
                {
                    int maxMP = character.Character.MaxMP;
                    int currentMP = character.CurrentMP;
                    int mpPercentage = currentMP * 100 / Math.Max(1, maxMP);

                    if (mpPercentage < 50)
                    {
                        value = (100 - mpPercentage) * item.Power / 20;
                    }
                }
                // 攻击增强物品
                else if (item.Type.Contains("Attack"))
                {
                    value = item.Power / 2;
                }
                // 防御增强物品
                else if (item.Type.Contains("Defense"))
                {
                    value = item.Power / 2;
                }

                // 如果物品有价值，添加到可用物品列表
                if (value > 0)
                {
                    availableItems.Add(new ItemInfo
                    {
                        ItemId = item.Id,
                        ItemType = item.Type,
                        BestTargetId = bestTargetId,
                        Value = value
                    });
                }
            }

            return availableItems;
        }

        /// <summary>
        /// 计算预期伤害
        /// </summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="defender">防御者</param>
        /// <returns>预期伤害</returns>
        private int CalculateExpectedDamage(IBattleCharacter attacker, IBattleCharacter defender)
        {
            var attackerStats = attacker.Character.CalculateCombatStats();
            var defenderStats = defender.Character.CalculateCombatStats();

            // 简化的伤害计算公式
            int physicalDamage = Math.Max(0, attackerStats.PhysicalAttack - defenderStats.PhysicalDefense);
            int magicDamage = Math.Max(0, attackerStats.MagicAttack - defenderStats.MagicDefense);

            return physicalDamage + magicDamage;
        }

        /// <summary>
        /// 计算威胁等级
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">战斗实例</param>
        /// <returns>威胁等级</returns>
        private int CalculateThreatLevel(IBattleCharacter character, IBattle battle)
        {
            var combatStats = character.Character.CalculateCombatStats();

            // 简化的威胁等级计算公式
            int attackPower = combatStats.PhysicalAttack + combatStats.MagicAttack;
            int healthPercentage = character.CurrentHP * 100 / Math.Max(1, character.Character.MaxHP);

            return attackPower / 10 + (healthPercentage > 50 ? 20 : 0);
        }

        /// <summary>
        /// 计算战斗力
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <returns>战斗力</returns>
        private int CalculateCombatPower(IBattleCharacter character)
        {
            var combatStats = character.Character.CalculateCombatStats();

            // 简化的战斗力计算公式
            return combatStats.PhysicalAttack + combatStats.MagicAttack + combatStats.PhysicalDefense + combatStats.MagicDefense;
        }

        /// <summary>
        /// 计算目标优先级
        /// </summary>
        /// <param name="healthPercentage">生命值百分比</param>
        /// <param name="threatLevel">威胁等级</param>
        /// <param name="expectedDamage">预期伤害</param>
        /// <returns>目标优先级</returns>
        private int CalculateTargetPriority(int healthPercentage, int threatLevel, int expectedDamage)
        {
            // 简化的目标优先级计算公式
            return (100 - healthPercentage) + threatLevel + expectedDamage;
        }

        /// <summary>
        /// 获取技能类型
        /// </summary>
        /// <param name="skill">技能</param>
        /// <returns>技能类型</returns>
        private string GetSkillType(ISkill skill)
        {
            if (skill.IsHealing)
                return "Heal";
            if (skill.IsDefensive)
                return "Defense";
            if (skill.IsBuff)
                return "Buff";
            return "Attack";
        }
    }
}