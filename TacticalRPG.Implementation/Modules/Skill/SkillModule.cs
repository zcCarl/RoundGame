using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Skill;

namespace TacticalRPG.Implementation.Modules.Skill
{
    /// <summary>
    /// 技能模块实现
    /// </summary>
    public class SkillModule : BaseGameModule, ISkillModule
    {
        private readonly Dictionary<Guid, ISkill> _skills = new Dictionary<Guid, ISkill>();
        private readonly ISkillEffectManager _effectManager;
        private readonly ISkillEffectFactory _effectFactory;
        private readonly ICharacterModule _characterModule;
        private bool _isInitialized = false;

        /// <summary>
        /// 模块名称
        /// </summary>
        public override string ModuleName => "Skill";

        /// <summary>
        /// 模块优先级
        /// </summary>
        public override int Priority => 40;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameSystem">游戏系统</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="effectManager">效果管理器</param>
        /// <param name="effectFactory">效果工厂</param>
        /// <param name="characterModule">角色模块</param>
        public SkillModule(
            IGameSystem gameSystem,
            ILogger<SkillModule> logger,
            ISkillEffectManager effectManager,
            ISkillEffectFactory effectFactory,
            ICharacterModule characterModule) : base(gameSystem, logger)
        {
            _effectManager = effectManager ?? throw new ArgumentNullException(nameof(effectManager));
            _effectFactory = effectFactory ?? throw new ArgumentNullException(nameof(effectFactory));
            _characterModule = characterModule ?? throw new ArgumentNullException(nameof(characterModule));
        }

        /// <summary>
        /// 模块初始化
        /// </summary>
        public override async Task Initialize()
        {
            if (_isInitialized)
                return;

            Logger.LogInformation("技能模块初始化中...");

            // 注册事件处理器
            // EventManager.Subscribe<CharacterLevelUpEvent>(HandleCharacterLevelUp);

            _isInitialized = true;
            await base.Initialize();
        }

        /// <summary>
        /// 创建技能
        /// </summary>
        /// <param name="name">技能名称</param>
        /// <param name="description">技能描述</param>
        /// <param name="skillType">技能类型</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="range">技能范围</param>
        /// <param name="area">影响区域</param>
        /// <param name="mpCost">魔法消耗</param>
        /// <param name="cooldown">冷却回合</param>
        /// <returns>新技能的ID</returns>
        public Task<Guid> CreateSkillAsync(
            string name,
            string description,
            SkillType skillType,
            TargetType targetType,
            int range,
            int area,
            int mpCost,
            int cooldown)
        {
            try
            {
                var id = Guid.NewGuid();
                var skill = new Skill(
                    id,
                    name,
                    description,
                    skillType,
                    targetType,
                    range,
                    area,
                    mpCost,
                    cooldown);

                _skills[id] = skill;

                Logger.LogInformation($"创建技能：{name}，ID：{id}");
                return Task.FromResult(id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"创建技能失败：{name}");
                throw;
            }
        }

        /// <summary>
        /// 获取技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <returns>技能对象</returns>
        public ISkill GetSkill(Guid skillId)
        {
            if (_skills.TryGetValue(skillId, out var skill))
            {
                return skill;
            }

            Logger.LogWarning($"未找到ID为 {skillId} 的技能");
            return null;
        }

        /// <summary>
        /// 获取所有技能
        /// </summary>
        /// <returns>所有技能的列表</returns>
        public IReadOnlyList<ISkill> GetAllSkills()
        {
            return _skills.Values.ToList();
        }

        /// <summary>
        /// 获取指定类型的所有技能
        /// </summary>
        /// <param name="skillType">技能类型</param>
        /// <returns>指定类型的所有技能</returns>
        public IReadOnlyList<ISkill> GetSkillsByType(SkillType skillType)
        {
            return _skills.Values
                .Where(s => s.Type == skillType)
                .ToList();
        }

        /// <summary>
        /// 获取指定职业可学习的所有技能
        /// </summary>
        /// <param name="characterClass">角色职业</param>
        /// <returns>指定职业可学习的所有技能</returns>
        public IReadOnlyList<ISkill> GetSkillsByClass(CharacterClass characterClass)
        {
            return _skills.Values
                .Where(s => s.LearnableClasses.Contains(characterClass))
                .ToList();
        }

        /// <summary>
        /// 保存技能
        /// </summary>
        /// <param name="skill">技能对象</param>
        /// <returns>操作结果</returns>
        public Task<bool> SaveSkillAsync(ISkill skill)
        {
            if (skill == null)
            {
                throw new ArgumentNullException(nameof(skill));
            }

            try
            {
                _skills[skill.Id] = skill;
                Logger.LogInformation($"保存技能：{skill.Name}，ID：{skill.Id}");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"保存技能失败：{skill.Name}，ID：{skill.Id}");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 删除技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <returns>操作结果</returns>
        public Task<bool> DeleteSkillAsync(Guid skillId)
        {
            try
            {
                if (_skills.TryGetValue(skillId, out var skill))
                {
                    _skills.Remove(skillId);
                    Logger.LogInformation($"删除技能：{skill.Name}，ID：{skillId}");
                    return Task.FromResult(true);
                }

                Logger.LogWarning($"删除技能失败：未找到ID为 {skillId} 的技能");
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"删除技能失败：ID：{skillId}");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 计算技能范围
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="x">中心X坐标</param>
        /// <param name="y">中心Y坐标</param>
        /// <returns>技能可影响的坐标列表</returns>
        public IReadOnlyList<(int X, int Y)> CalculateSkillRange(Guid skillId, int x, int y)
        {
            var skill = GetSkill(skillId);
            if (skill == null)
            {
                Logger.LogWarning($"计算技能范围失败：未找到ID为 {skillId} 的技能");
                return new List<(int X, int Y)>();
            }

            var result = new List<(int X, int Y)>();

            // 根据技能范围计算可影响的坐标
            for (int i = -skill.Range; i <= skill.Range; i++)
            {
                for (int j = -skill.Range; j <= skill.Range; j++)
                {
                    // 使用曼哈顿距离计算
                    if (Math.Abs(i) + Math.Abs(j) <= skill.Range)
                    {
                        result.Add((x + i, y + j));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 计算技能影响区域
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="targetX">目标X坐标</param>
        /// <param name="targetY">目标Y坐标</param>
        /// <returns>技能影响的坐标列表</returns>
        public IReadOnlyList<(int X, int Y)> CalculateSkillArea(Guid skillId, int targetX, int targetY)
        {
            var skill = GetSkill(skillId);
            if (skill == null)
            {
                Logger.LogWarning($"计算技能影响区域失败：未找到ID为 {skillId} 的技能");
                return new List<(int X, int Y)>();
            }

            var result = new List<(int X, int Y)>();

            // 根据技能影响区域计算可影响的坐标
            for (int i = -skill.Area; i <= skill.Area; i++)
            {
                for (int j = -skill.Area; j <= skill.Area; j++)
                {
                    // 使用曼哈顿距离计算
                    if (Math.Abs(i) + Math.Abs(j) <= skill.Area)
                    {
                        result.Add((targetX + i, targetY + j));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 检查角色是否可以学习技能
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <returns>是否可以学习</returns>
        public bool CanLearnSkill(Guid characterId, Guid skillId)
        {
            var skill = GetSkill(skillId);
            if (skill == null)
            {
                Logger.LogWarning($"检查技能学习条件失败：未找到ID为 {skillId} 的技能");
                return false;
            }

            // 获取角色对象
            var character = _characterModule.GetCharacter(characterId);
            if (character == null)
            {
                Logger.LogWarning($"检查技能学习条件失败：未找到ID为 {characterId} 的角色");
                return false;
            }

            // 检查职业是否匹配
            return skill.CanLearn(character);
        }

        /// <summary>
        /// 检查角色是否可以使用技能
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <returns>是否可以使用</returns>
        public bool CanUseSkill(Guid characterId, Guid skillId)
        {
            var skill = GetSkill(skillId);
            if (skill == null)
            {
                Logger.LogWarning($"检查技能使用条件失败：未找到ID为 {skillId} 的技能");
                return false;
            }

            // 获取角色对象
            var character = _characterModule.GetCharacter(characterId);
            if (character == null)
            {
                Logger.LogWarning($"检查技能使用条件失败：未找到ID为 {characterId} 的角色");
                return false;
            }

            // 检查MP是否足够
            if (character is ICombatCharacter combatChar && combatChar.CurrentMP < skill.MPCost)
            {
                return false;
            }

            // 检查冷却是否完成
            if (!skill.IsReady())
            {
                return false;
            }

            // 其他条件检查，如状态效果等
            // TODO: 实现更多条件检查

            return true;
        }
    }
}