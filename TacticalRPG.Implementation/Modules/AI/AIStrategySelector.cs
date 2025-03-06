using System;
using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.AI;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Implementation.Modules.AI
{
    /// <summary>
    /// AI策略选择器，用于根据角色特性选择合适的AI策略
    /// </summary>
    public class AIStrategySelector : IAIStrategySelector
    {
        private readonly IAIControllerFactory _controllerFactory;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="controllerFactory">AI控制器工厂</param>
        public AIStrategySelector(IAIControllerFactory controllerFactory)
        {
            _controllerFactory = controllerFactory;
        }

        /// <summary>
        /// 为角色选择合适的AI策略
        /// </summary>
        /// <param name="character">角色</param>
        /// <returns>AI控制器</returns>
        public IAIController SelectStrategy(ICharacter character)
        {
            // 如果角色已经指定了AI类型，直接使用
            if (character.AIType != AIType.None)
            {
                return _controllerFactory.CreateController(character.AIType);
            }

            // 根据角色特性选择合适的AI策略
            AIType aiType = DetermineAIType(character);
            return _controllerFactory.CreateController(aiType);
        }

        /// <summary>
        /// 根据角色特性确定AI类型
        /// </summary>
        /// <param name="character">角色</param>
        /// <returns>AI类型</returns>
        private AIType DetermineAIType(ICharacter character)
        {
            // 获取角色战斗属性
            var combatStats = character.CalculateCombatStats();

            // 获取角色技能
            var skills = character.Skills.ToList();

            // 计算各种能力的比例
            int attackPower = combatStats.PhysicalAttack + combatStats.MagicAttack;
            int defensePower = combatStats.PhysicalDefense + combatStats.MagicDefense;
            int healingPower = skills.Where(s => s.IsHealing).Sum(s => s.Power);
            int buffPower = skills.Where(s => s.IsBuff).Sum(s => s.Power);

            // 计算各种AI类型的得分
            Dictionary<AIType, int> scores = new Dictionary<AIType, int>
            {
                { AIType.Aggressive, CalculateAggressiveScore(character, attackPower, defensePower, healingPower, buffPower) },
                { AIType.Defensive, CalculateDefensiveScore(character, attackPower, defensePower, healingPower, buffPower) },
                { AIType.Balanced, CalculateBalancedScore(character, attackPower, defensePower, healingPower, buffPower) },
                { AIType.Berserker, CalculateBerserkerScore(character, attackPower, defensePower, healingPower, buffPower) },
                { AIType.Healer, CalculateHealerScore(character, attackPower, defensePower, healingPower, buffPower) },
                { AIType.Support, CalculateSupportScore(character, attackPower, defensePower, healingPower, buffPower) },
                { AIType.Tactical, CalculateTacticalScore(character, attackPower, defensePower, healingPower, buffPower) },
                { AIType.Passive, CalculatePassiveScore(character, attackPower, defensePower, healingPower, buffPower) }
            };

            // 返回得分最高的AI类型
            return scores.OrderByDescending(s => s.Value).First().Key;
        }

        /// <summary>
        /// 计算进攻型AI得分
        /// </summary>
        private int CalculateAggressiveScore(ICharacter character, int attackPower, int defensePower, int healingPower, int buffPower)
        {
            int score = 0;

            // 攻击力高的角色更适合进攻型AI
            score += attackPower * 2;

            // 防御力低的角色不太适合进攻型AI
            score -= defensePower;

            // 治疗能力低的角色更适合进攻型AI
            score -= healingPower * 2;

            // 增益能力适中的角色适合进攻型AI
            score += buffPower;

            // 生命值低的角色不太适合进攻型AI
            score -= (character.MaxHP < 100) ? 50 : 0;

            // 移动力高的角色更适合进攻型AI
            score += (character.MovementRange > 4) ? 50 : 0;

            return score;
        }

        /// <summary>
        /// 计算防守型AI得分
        /// </summary>
        private int CalculateDefensiveScore(ICharacter character, int attackPower, int defensePower, int healingPower, int buffPower)
        {
            int score = 0;

            // 攻击力低的角色更适合防守型AI
            score -= attackPower;

            // 防御力高的角色更适合防守型AI
            score += defensePower * 2;

            // 治疗能力适中的角色适合防守型AI
            score += healingPower;

            // 增益能力适中的角色适合防守型AI
            score += buffPower;

            // 生命值高的角色更适合防守型AI
            score += (character.MaxHP > 150) ? 50 : 0;

            // 移动力低的角色更适合防守型AI
            score += (character.MovementRange < 4) ? 50 : 0;

            return score;
        }

        /// <summary>
        /// 计算平衡型AI得分
        /// </summary>
        private int CalculateBalancedScore(ICharacter character, int attackPower, int defensePower, int healingPower, int buffPower)
        {
            int score = 0;

            // 攻击力适中的角色更适合平衡型AI
            score += attackPower;

            // 防御力适中的角色更适合平衡型AI
            score += defensePower;

            // 治疗能力适中的角色适合平衡型AI
            score += healingPower;

            // 增益能力适中的角色适合平衡型AI
            score += buffPower;

            // 各项能力均衡的角色更适合平衡型AI
            int variance = CalculateVariance(new[] { attackPower, defensePower, healingPower, buffPower });
            score -= variance;

            return score;
        }

        /// <summary>
        /// 计算狂战士型AI得分
        /// </summary>
        private int CalculateBerserkerScore(ICharacter character, int attackPower, int defensePower, int healingPower, int buffPower)
        {
            int score = 0;

            // 攻击力极高的角色更适合狂战士型AI
            score += attackPower * 3;

            // 防御力极低的角色更适合狂战士型AI
            score -= defensePower * 2;

            // 治疗能力极低的角色更适合狂战士型AI
            score -= healingPower * 3;

            // 增益能力低的角色更适合狂战士型AI
            score -= buffPower;

            // 生命值低的角色不太适合狂战士型AI
            score -= (character.MaxHP < 80) ? 100 : 0;

            // 移动力高的角色更适合狂战士型AI
            score += (character.MovementRange > 5) ? 100 : 0;

            return score;
        }

        /// <summary>
        /// 计算治疗型AI得分
        /// </summary>
        private int CalculateHealerScore(ICharacter character, int attackPower, int defensePower, int healingPower, int buffPower)
        {
            int score = 0;

            // 攻击力低的角色更适合治疗型AI
            score -= attackPower;

            // 防御力适中的角色适合治疗型AI
            score += defensePower;

            // 治疗能力极高的角色更适合治疗型AI
            score += healingPower * 3;

            // 增益能力适中的角色适合治疗型AI
            score += buffPower;

            // 没有治疗技能的角色不适合治疗型AI
            if (character.Skills.All(s => !s.IsHealing))
            {
                score = -1000;
            }

            return score;
        }

        /// <summary>
        /// 计算支援型AI得分
        /// </summary>
        private int CalculateSupportScore(ICharacter character, int attackPower, int defensePower, int healingPower, int buffPower)
        {
            int score = 0;

            // 攻击力低的角色更适合支援型AI
            score -= attackPower;

            // 防御力适中的角色适合支援型AI
            score += defensePower;

            // 治疗能力高的角色更适合支援型AI
            score += healingPower * 2;

            // 增益能力极高的角色更适合支援型AI
            score += buffPower * 3;

            // 没有增益技能的角色不适合支援型AI
            if (character.Skills.All(s => !s.IsBuff))
            {
                score = -1000;
            }

            return score;
        }

        /// <summary>
        /// 计算战术型AI得分
        /// </summary>
        private int CalculateTacticalScore(ICharacter character, int attackPower, int defensePower, int healingPower, int buffPower)
        {
            int score = 0;

            // 攻击力适中的角色适合战术型AI
            score += attackPower;

            // 防御力适中的角色适合战术型AI
            score += defensePower;

            // 治疗能力适中的角色适合战术型AI
            score += healingPower;

            // 增益能力适中的角色适合战术型AI
            score += buffPower;

            // 技能多样性高的角色更适合战术型AI
            int skillDiversity = character.Skills.Select(s => s.GetType().Name).Distinct().Count();
            score += skillDiversity * 20;

            // 移动力高的角色更适合战术型AI
            score += (character.MovementRange > 4) ? 50 : 0;

            return score;
        }

        /// <summary>
        /// 计算被动型AI得分
        /// </summary>
        private int CalculatePassiveScore(ICharacter character, int attackPower, int defensePower, int healingPower, int buffPower)
        {
            int score = 0;

            // 攻击力极低的角色更适合被动型AI
            score -= attackPower * 2;

            // 防御力极高的角色更适合被动型AI
            score += defensePower * 3;

            // 治疗能力高的角色更适合被动型AI
            score += healingPower * 2;

            // 增益能力高的角色更适合被动型AI
            score += buffPower * 2;

            // 生命值低的角色更适合被动型AI
            score += (character.MaxHP < 100) ? 50 : 0;

            // 移动力高的角色更适合被动型AI（便于逃跑）
            score += (character.MovementRange > 4) ? 50 : 0;

            return score;
        }

        /// <summary>
        /// 计算数组方差
        /// </summary>
        /// <param name="values">数值数组</param>
        /// <returns>方差</returns>
        private int CalculateVariance(int[] values)
        {
            if (values.Length == 0)
                return 0;

            double mean = values.Average();
            double sumOfSquares = values.Sum(v => Math.Pow(v - mean, 2));
            return (int)Math.Sqrt(sumOfSquares / values.Length);
        }
    }
}