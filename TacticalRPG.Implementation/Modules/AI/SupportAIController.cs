using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TacticalRPG.Core.Modules.AI;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Skill;

namespace TacticalRPG.Implementation.Modules.AI
{
    /// <summary>
    /// 支援型AI控制器，专注于治疗和增益技能
    /// </summary>
    public class SupportAIController : BaseAIController
    {
        /// <summary>
        /// 获取AI类型
        /// </summary>
        public override AIType Type => AIType.Support;

        /// <summary>
        /// 决定下一个行动
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="battle">战斗</param>
        /// <returns>AI行动</returns>
        public override async Task<AIAction> DecideNextAction(IBattleCharacter character, IBattle battle)
        {
            // 评估战场
            var evaluation = EvaluateBattlefield(character, battle);

            // 可能的行动列表
            var possibleActions = new List<AIAction>();

            // 添加治疗和增益技能行动（最高优先级）
            AddHealingAndSupportSkillActions(possibleActions, character, battle, evaluation);

            // 添加移动行动
            AddMoveActions(possibleActions, character, battle, evaluation);

            // 添加攻击行动（最低优先级）
            AddAttackActions(possibleActions, character, battle, evaluation);

            // 添加等待行动
            possibleActions.Add(new AIAction
            {
                Type = AIActionType.Wait,
                Priority = 10
            });

            // 添加结束回合行动
            possibleActions.Add(new AIAction
            {
                Type = AIActionType.EndTurn,
                Priority = 5
            });

            // 选择优先级最高的行动
            return possibleActions.OrderByDescending(a => a.Priority).FirstOrDefault();
        }

        /// <summary>
        /// 添加治疗和增益技能行动
        /// </summary>
        /// <param name="actions">行动列表</param>
        /// <param name="character">角色</param>
        /// <param name="battle">战斗</param>
        /// <param name="evaluation">战场评估</param>
        private void AddHealingAndSupportSkillActions(List<AIAction> actions, IBattleCharacter character, IBattle battle, BattleEvaluation evaluation)
        {
            // 获取角色的技能列表
            var skills = character.Character.Skills;
            if (skills == null || !skills.Any())
                return;

            // 获取战斗统计数据
            var combatStats = character.Character.CalculateCombatStats();

            // 寻找需要治疗的队友
            var alliesNeedingHealing = battle.Characters
                .Where(c => c.Team == character.Team && c.CurrentHP < c.Character.MaxHP * 0.8 && c != character)
                .OrderBy(c => (double)c.CurrentHP / c.Character.MaxHP)
                .ToList();

            // 寻找可以使用增益技能的队友
            var alliesForBuffs = battle.Characters
                .Where(c => c.Team == character.Team && c != character)
                .ToList();

            foreach (var skill in skills)
            {
                // 检查MP是否足够
                if (character.CurrentMP < skill.MPCost)
                    continue;

                // 治疗技能
                if (skill.IsHealing && alliesNeedingHealing.Any())
                {
                    foreach (var ally in alliesNeedingHealing)
                    {
                        // 检查是否在技能范围内
                        int distance = Math.Abs(character.X - ally.X) + Math.Abs(character.Y - ally.Y);
                        if (distance <= skill.Range)
                        {
                            // 计算优先级：血量越低，优先级越高
                            double healthPercentage = (double)ally.CurrentHP / ally.Character.MaxHP;
                            int priority = (int)(100 - (healthPercentage * 100)) + 50; // 50-150的优先级

                            actions.Add(new AIAction
                            {
                                Type = AIActionType.UseSkill,
                                TargetCharacterId = ally.Character.Id,
                                SkillId = skill.Id,
                                Priority = priority
                            });

                            // 只为最需要治疗的几个队友添加治疗行动
                            if (actions.Count(a => a.Type == AIActionType.UseSkill && a.SkillId == skill.Id) >= 3)
                                break;
                        }
                    }
                }
                // 增益技能
                else if (skill.IsBuff && alliesForBuffs.Any())
                {
                    foreach (var ally in alliesForBuffs)
                    {
                        // 检查是否在技能范围内
                        int distance = Math.Abs(character.X - ally.X) + Math.Abs(character.Y - ally.Y);
                        if (distance <= skill.Range)
                        {
                            // 计算优先级：基于角色的战斗力和当前状态
                            var allyStats = ally.Character.CalculateCombatStats();
                            int priority = 80; // 基础优先级

                            // 如果是强力角色，提高优先级
                            if (allyStats.PhysicalAttack > 50 || allyStats.MagicAttack > 50)
                                priority += 10;

                            actions.Add(new AIAction
                            {
                                Type = AIActionType.UseSkill,
                                TargetCharacterId = ally.Character.Id,
                                SkillId = skill.Id,
                                Priority = priority
                            });

                            // 限制增益技能的数量
                            if (actions.Count(a => a.Type == AIActionType.UseSkill && a.SkillId == skill.Id) >= 2)
                                break;
                        }
                    }
                }
                // 群体治疗或增益技能
                else if ((skill.IsHealing || skill.IsBuff) && skill.IsAreaEffect)
                {
                    // 寻找最佳施放位置
                    var bestPosition = FindBestAreaSkillPosition(character, battle, skill, character.Team);
                    if (bestPosition != null)
                    {
                        actions.Add(new AIAction
                        {
                            Type = AIActionType.UseSkill,
                            TargetX = bestPosition.X,
                            TargetY = bestPosition.Y,
                            SkillId = skill.Id,
                            Priority = 90 // 群体技能优先级高
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 添加移动行动
        /// </summary>
        /// <param name="actions">行动列表</param>
        /// <param name="character">角色</param>
        /// <param name="battle">战斗</param>
        /// <param name="evaluation">战场评估</param>
        private void AddMoveActions(List<AIAction> actions, IBattleCharacter character, IBattle battle, BattleEvaluation evaluation)
        {
            // 获取可移动范围
            var moveRange = battle.CalculateMoveRange(character.Character.Id);
            if (moveRange == null || !moveRange.Any())
                return;

            // 获取战斗统计数据
            var combatStats = character.Character.CalculateCombatStats();

            // 如果处于危险位置，优先移动到安全位置
            if (evaluation.IsInDangerousPosition)
            {
                var safePositions = evaluation.SafePositions
                    .Where(p => moveRange.Any(m => m.X == p.X && m.Y == p.Y))
                    .OrderByDescending(p => p.DefenseBonus)
                    .ToList();

                if (safePositions.Any())
                {
                    var bestSafePosition = safePositions.First();
                    actions.Add(new AIAction
                    {
                        Type = AIActionType.Move,
                        TargetX = bestSafePosition.X,
                        TargetY = bestSafePosition.Y,
                        Priority = 95 // 非常高的优先级
                    });
                    return;
                }
            }

            // 寻找需要治疗的队友
            var alliesNeedingHealing = battle.Characters
                .Where(c => c.Team == character.Team && c.CurrentHP < c.Character.MaxHP * 0.7 && c != character)
                .OrderBy(c => (double)c.CurrentHP / c.Character.MaxHP)
                .ToList();

            if (alliesNeedingHealing.Any())
            {
                // 获取角色的治疗技能
                var healingSkills = character.Character.Skills
                    .Where(s => s.IsHealing)
                    .OrderByDescending(s => s.Power)
                    .ToList();

                if (healingSkills.Any())
                {
                    var bestHealingSkill = healingSkills.First();
                    var bestAlly = alliesNeedingHealing.First();

                    // 计算移动到可以治疗的位置
                    var bestPosition = moveRange
                        .Where(p => Math.Abs(p.X - bestAlly.X) + Math.Abs(p.Y - bestAlly.Y) <= bestHealingSkill.Range)
                        .OrderBy(p => Math.Abs(p.X - bestAlly.X) + Math.Abs(p.Y - bestAlly.Y))
                        .FirstOrDefault();

                    if (bestPosition != null)
                    {
                        actions.Add(new AIAction
                        {
                            Type = AIActionType.Move,
                            TargetX = bestPosition.X,
                            TargetY = bestPosition.Y,
                            Priority = 90 // 高优先级
                        });
                        return;
                    }
                }
            }

            // 移动到战术位置
            var tacticalPositions = evaluation.TacticalPositions
                .Where(p => moveRange.Any(m => m.X == p.X && m.Y == p.Y))
                .OrderByDescending(p => p.DefenseBonus)
                .ToList();

            if (tacticalPositions.Any())
            {
                var bestTacticalPosition = tacticalPositions.First();
                actions.Add(new AIAction
                {
                    Type = AIActionType.Move,
                    TargetX = bestTacticalPosition.X,
                    TargetY = bestTacticalPosition.Y,
                    Priority = 60 // 中等优先级
                });
            }
        }

        /// <summary>
        /// 添加攻击行动
        /// </summary>
        /// <param name="actions">行动列表</param>
        /// <param name="character">角色</param>
        /// <param name="battle">战斗</param>
        /// <param name="evaluation">战场评估</param>
        private void AddAttackActions(List<AIAction> actions, IBattleCharacter character, IBattle battle, BattleEvaluation evaluation)
        {
            // 获取战斗统计数据
            var combatStats = character.Character.CalculateCombatStats();

            // 获取可攻击的敌人
            var attackableEnemies = evaluation.AttackableEnemies;
            if (attackableEnemies == null || !attackableEnemies.Any())
                return;

            // 计算每个敌人的优先级
            var enemiesWithPriority = attackableEnemies
                .Select(e => new { Enemy = e, Priority = CalculateTargetPriority(character, e, battle) })
                .OrderByDescending(e => e.Priority)
                .ToList();

            // 添加攻击行动（低优先级，因为支援型AI主要关注治疗和增益）
            if (enemiesWithPriority.Any())
            {
                var bestTarget = enemiesWithPriority.First();
                actions.Add(new AIAction
                {
                    Type = AIActionType.Attack,
                    TargetCharacterId = bestTarget.Enemy.Character.Id,
                    Priority = 40 // 低优先级
                });
            }
        }

        /// <summary>
        /// 计算目标优先级
        /// </summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="target">目标</param>
        /// <param name="battle">战斗</param>
        /// <returns>优先级值</returns>
        private int CalculateTargetPriority(IBattleCharacter attacker, IBattleCharacter target, IBattle battle)
        {
            // 获取战斗统计数据
            var attackerStats = attacker.Character.CalculateCombatStats();
            var targetStats = target.Character.CalculateCombatStats();

            // 计算目标的生命百分比
            double healthPercentage = (double)target.CurrentHP / target.Character.MaxHP;

            // 计算威胁等级（基于攻击力和防御力）
            int threatLevel = targetStats.PhysicalAttack + targetStats.MagicAttack -
                              (targetStats.PhysicalDefense + targetStats.MagicDefense) / 2;

            // 计算预期伤害
            int expectedDamage = Math.Max(1, attackerStats.PhysicalAttack - targetStats.PhysicalDefense);

            // 如果可以一击必杀，提高优先级
            bool canKill = expectedDamage >= target.CurrentHP;

            // 计算最终优先级
            int priority = (int)((1 - healthPercentage) * 30) + // 生命值低的敌人
                          threatLevel / 5 + // 威胁等级
                          (canKill ? 50 : 0); // 一击必杀奖励

            return priority;
        }

        /// <summary>
        /// 寻找最佳区域技能施放位置
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="battle">战斗</param>
        /// <param name="skill">技能</param>
        /// <param name="targetTeam">目标队伍</param>
        /// <returns>最佳位置</returns>
        private (int X, int Y)? FindBestAreaSkillPosition(IBattleCharacter character, IBattle battle, ISkill skill, BattleTeam targetTeam)
        {
            // 获取技能范围和效果范围
            int skillRange = skill.Range;
            int effectRange = skill.AreaEffectRange;

            // 获取所有可能的目标位置
            var possibleTargets = new List<(int X, int Y, int Score)>();

            // 遍历地图上的所有位置
            for (int x = 0; x < battle.Map.Width; x++)
            {
                for (int y = 0; y < battle.Map.Height; y++)
                {
                    // 检查是否在技能施放范围内
                    int distanceToCharacter = Math.Abs(character.X - x) + Math.Abs(character.Y - y);
                    if (distanceToCharacter > skillRange)
                        continue;

                    // 计算该位置的效果分数
                    int score = CalculateAreaSkillScore(x, y, effectRange, battle, targetTeam, skill.IsHealing || skill.IsBuff);

                    if (score > 0)
                    {
                        possibleTargets.Add((x, y, score));
                    }
                }
            }

            // 返回得分最高的位置
            return possibleTargets.OrderByDescending(t => t.Score).Select(t => ((int X, int Y))(t.X, t.Y)).FirstOrDefault();
        }

        /// <summary>
        /// 计算区域技能在指定位置的效果分数
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="effectRange">效果范围</param>
        /// <param name="battle">战斗</param>
        /// <param name="targetTeam">目标队伍</param>
        /// <param name="isPositiveEffect">是否为正面效果</param>
        /// <returns>效果分数</returns>
        private int CalculateAreaSkillScore(int x, int y, int effectRange, IBattle battle, BattleTeam targetTeam, bool isPositiveEffect)
        {
            int score = 0;

            // 遍历效果范围内的所有角色
            foreach (var character in battle.Characters)
            {
                int distance = Math.Abs(character.X - x) + Math.Abs(character.Y - y);
                if (distance <= effectRange)
                {
                    // 对于正面效果（治疗/增益），计算友方角色的分数
                    if (isPositiveEffect && character.Team == targetTeam)
                    {
                        // 治疗效果对低血量角色更有价值
                        double healthPercentage = (double)character.CurrentHP / character.Character.MaxHP;
                        score += (int)((1 - healthPercentage) * 100) + 10;
                    }
                    // 对于负面效果（伤害/减益），计算敌方角色的分数
                    else if (!isPositiveEffect && character.Team != targetTeam)
                    {
                        score += 10;
                    }
                }
            }

            return score;
        }
    }
}