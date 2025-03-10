using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Framework.Events;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Map;
using TacticalRPG.Implementation.Modules.Character;
using TacticalRPG.Core.Modules.Config;
using TacticalRPG.Core.Modules.Skill;

namespace TacticalRPG.Implementation.Modules.Battle
{
    /// <summary>
    /// 战斗模块实现
    /// </summary>
    public class BattleModule : IBattleModule
    {
        private readonly IEventManager _eventManager;
        private readonly IMapModule _mapModule;
        private readonly ICharacterModule _characterModule;
        private readonly ILogger<BattleModule> _logger;
        private readonly IConfigManager _configManager;
        private readonly ISkillModule _skillModule;
        private readonly Dictionary<Guid, Battle> _battles = new Dictionary<Guid, Battle>();
        private bool _isInitialized = false;
        private bool _isActive = false;

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName => "Battle";

        /// <summary>
        /// 模块优先级
        /// </summary>
        public int Priority => 30;

        /// <summary>
        /// 当前战斗
        /// </summary>
        public IBattle CurrentBattle { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eventManager">事件管理器</param>
        /// <param name="mapModule">地图模块</param>
        /// <param name="characterModule">角色模块</param>
        /// <param name="skillModule">技能模块</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="configManager">配置管理器</param>
        public BattleModule(IEventManager eventManager, IMapModule mapModule, ICharacterModule characterModule, ISkillModule skillModule, ILogger<BattleModule> logger, IConfigManager configManager)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _mapModule = mapModule ?? throw new ArgumentNullException(nameof(mapModule));
            _characterModule = characterModule ?? throw new ArgumentNullException(nameof(characterModule));
            _skillModule = skillModule ?? throw new ArgumentNullException(nameof(skillModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        }

        /// <summary>
        /// 初始化模块
        /// </summary>
        public async Task Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _logger.LogInformation("战斗模块初始化");

            // 注册事件处理器
            // _eventManager.Subscribe<CharacterDiedEvent>(HandleCharacterDied);

            _isInitialized = true;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 启动模块
        /// </summary>
        public async Task Start()
        {
            if (!_isInitialized)
            {
                await Initialize();
            }

            if (_isActive)
            {
                return;
            }

            _logger.LogInformation("战斗模块启动");
            _isActive = true;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 暂停模块
        /// </summary>
        public Task Pause()
        {
            if (!_isActive)
            {
                return Task.CompletedTask;
            }

            _logger.LogInformation("战斗模块暂停");
            _isActive = false;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 恢复模块
        /// </summary>
        public Task Resume()
        {
            if (_isActive)
            {
                return Task.CompletedTask;
            }

            _logger.LogInformation("战斗模块恢复");
            _isActive = true;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 停止模块
        /// </summary>
        public Task Stop()
        {
            if (!_isActive)
            {
                return Task.CompletedTask;
            }

            _logger.LogInformation("战斗模块停止");
            _isActive = false;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        public Task Unload()
        {
            _logger.LogInformation("战斗模块卸载");

            // 注销事件处理器
            // _eventManager.Unsubscribe<CharacterDiedEvent>(HandleCharacterDied);

            _battles.Clear();
            CurrentBattle = null;

            _isInitialized = false;
            _isActive = false;

            return Task.CompletedTask;
        }

        /// <summary>
        /// 初始化模块
        /// </summary>
        public async Task InitializeAsync()
        {
            await Initialize();
        }

        /// <summary>
        /// 关闭模块
        /// </summary>
        public async Task ShutdownAsync()
        {
            await Unload();
        }

        /// <summary>
        /// 创建新战斗
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <param name="name">战斗名称</param>
        /// <returns>战斗ID</returns>
        public async Task<Guid> CreateBattleAsync(Guid mapId, string name)
        {
            _logger.LogInformation($"创建战斗: {name}, 使用地图: {mapId}");

            // 加载地图
            await _mapModule.LoadMapAsync(mapId);
            var map = _mapModule.GetCurrentMap();

            if (map == null)
            {
                throw new InvalidOperationException($"无法加载地图: {mapId}");
            }

            // 创建战斗
            var battleId = Guid.NewGuid();
            var battle = new Battle(battleId, name, map, _configManager);

            // 保存战斗
            _battles[battleId] = battle;

            // 发布战斗创建事件
            // await _eventManager.PublishAsync(new BattleCreatedEvent(battleId, name));

            return battleId;
        }

        /// <summary>
        /// 加载战斗
        /// </summary>
        /// <param name="battleId">战斗ID</param>
        /// <returns>是否成功</returns>
        public Task<bool> LoadBattleAsync(Guid battleId)
        {
            _logger.LogInformation($"加载战斗: {battleId}");

            if (!_battles.TryGetValue(battleId, out var battle))
            {
                return Task.FromResult(false);
            }

            CurrentBattle = battle;

            // 发布战斗加载事件
            // await _eventManager.PublishAsync(new BattleLoadedEvent(battleId, battle.Name));

            return Task.FromResult(true);
        }

        /// <summary>
        /// 结束当前战斗
        /// </summary>
        /// <returns>是否成功</returns>
        public Task<bool> EndCurrentBattleAsync()
        {
            if (CurrentBattle == null)
            {
                return Task.FromResult(false);
            }

            _logger.LogInformation($"结束战斗: {CurrentBattle.Id}, {CurrentBattle.Name}");

            var battleId = CurrentBattle.Id;
            var battleName = CurrentBattle.Name;

            // 保存战斗数据（如有需要）

            // 移除当前战斗
            CurrentBattle = null;

            // 发布战斗结束事件
            // await _eventManager.PublishAsync(new BattleEndedEvent(battleId, battleName));

            return Task.FromResult(true);
        }

        /// <summary>
        /// 添加角色到战斗
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="team">队伍</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>是否成功</returns>
        public bool AddCharacterToBattle(ICharacter character, BattleTeam team, int x, int y)
        {
            if (CurrentBattle == null)
            {
                return false;
            }

            _logger.LogInformation($"添加角色: {character.Name} 到队伍: {team} 位置: {x},{y}");

            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }

            // 添加到战斗
            return ((Battle)CurrentBattle).AddCharacter(character, team, x, y);
        }

        /// <summary>
        /// 从战斗中移除角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>是否成功</returns>
        public bool RemoveCharacterFromBattle(Guid characterId)
        {
            if (CurrentBattle == null)
            {
                return false;
            }

            _logger.LogInformation($"移除角色: {characterId}");

            var result = ((Battle)CurrentBattle).RemoveCharacter(characterId);

            if (result)
            {
                // 发布角色移除事件
                // _eventManager.Publish(new CharacterRemovedFromBattleEvent(characterId));
            }

            return result;
        }

        /// <summary>
        /// 获取所有战斗角色
        /// </summary>
        /// <returns>所有战斗角色</returns>
        public IReadOnlyList<IBattleCharacter> GetAllBattleCharacters()
        {
            if (CurrentBattle == null)
            {
                return new List<IBattleCharacter>().AsReadOnly();
            }

            return CurrentBattle.GetAllBattleCharacters();
        }

        /// <summary>
        /// 获取指定队伍的所有角色
        /// </summary>
        /// <param name="team">队伍</param>
        /// <returns>队伍角色</returns>
        public IReadOnlyList<IBattleCharacter> GetTeamCharacters(BattleTeam team)
        {
            if (CurrentBattle == null)
            {
                return new List<IBattleCharacter>().AsReadOnly();
            }

            return CurrentBattle.GetTeamCharacters(team);
        }

        /// <summary>
        /// 开始回合
        /// </summary>
        /// <returns>是否成功</returns>
        public Task<bool> StartTurnAsync()
        {
            if (CurrentBattle == null)
            {
                return Task.FromResult(false);
            }

            _logger.LogInformation("开始回合");

            var battle = (Battle)CurrentBattle;
            var result = battle.StartNextActorTurn();

            if (result)
            {
                // 发布回合开始事件
                // await _eventManager.PublishAsync(new TurnStartedEvent(battle.CurrentActor?.Character.Id ?? Guid.Empty));
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// 结束回合
        /// </summary>
        /// <returns>是否成功</returns>
        public Task<bool> EndTurnAsync()
        {
            if (CurrentBattle == null)
            {
                return Task.FromResult(false);
            }

            _logger.LogInformation("结束回合");

            var battle = (Battle)CurrentBattle;
            var result = battle.EndCurrentActorTurn();

            if (result)
            {
                // 发布回合结束事件
                // await _eventManager.PublishAsync(new TurnEndedEvent());

                // 检查战斗是否结束
                if (battle.CheckBattleEnd())
                {
                    // 发布战斗结果事件
                    // await _eventManager.PublishAsync(new BattleResultEvent(battle.State));
                }
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// 移动角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="targetX">目标X坐标</param>
        /// <param name="targetY">目标Y坐标</param>
        /// <returns>是否成功</returns>
        public Task<bool> MoveCharacterAsync(Guid characterId, int targetX, int targetY)
        {
            if (CurrentBattle == null)
            {
                return Task.FromResult(false);
            }

            _logger.LogInformation($"移动角色 {characterId} 到 {targetX},{targetY}");

            var battle = (Battle)CurrentBattle;
            var result = battle.MoveCharacter(characterId, targetX, targetY);

            if (result)
            {
                // 发布角色移动事件
                // await _eventManager.PublishAsync(new CharacterMovedEvent(characterId, targetX, targetY));
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// 执行攻击
        /// </summary>
        /// <param name="attackerId">攻击者ID</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>攻击结果</returns>
        public Task<BattleActionResult> ExecuteAttackAsync(Guid attackerId, Guid targetId)
        {
            if (CurrentBattle == null)
            {
                return Task.FromResult(new BattleActionResult(
                    false,
                    BattleActionType.Attack,
                    attackerId,
                    new List<Guid>(),
                    new List<int>(),
                    new List<bool>(),
                    new List<bool>(),
                    "战斗不存在"));
            }

            _logger.LogInformation($"角色 {attackerId} 攻击 {targetId}");

            var battle = (Battle)CurrentBattle;

            // 获取攻击者和目标
            var attacker = battle.GetBattleCharacter(attackerId);
            var target = battle.GetBattleCharacter(targetId);

            if (attacker == null || target == null)
            {
                return Task.FromResult(new BattleActionResult(
                    false,
                    BattleActionType.Attack,
                    attackerId,
                    new List<Guid> { targetId },
                    new List<int>(),
                    new List<bool>(),
                    new List<bool>(),
                    "攻击者或目标不存在"));
            }

            // 检查是否可以攻击
            if (attacker.HasActed)
            {
                return Task.FromResult(new BattleActionResult(
                    false,
                    BattleActionType.Attack,
                    attackerId,
                    new List<Guid> { targetId },
                    new List<int>(),
                    new List<bool>(),
                    new List<bool>(),
                    "角色已经行动过"));
            }

            // 计算攻击范围
            var attackRange = battle.CalculateAttackRange(attackerId);
            if (!attackRange.Any(p => p.X == target.X && p.Y == target.Y))
            {
                return Task.FromResult(new BattleActionResult(
                    false,
                    BattleActionType.Attack,
                    attackerId,
                    new List<Guid> { targetId },
                    new List<int>(),
                    new List<bool>(),
                    new List<bool>(),
                    "目标不在攻击范围内"));
            }

            // 计算伤害
            var attackerCombatStats = attacker.Character.CalculateCombatStats();
            var targetCombatStats = target.Character.CalculateCombatStats();

            var baseAttack = attackerCombatStats.PhysicalAttack;
            var baseDefense = targetCombatStats.PhysicalDefense;

            // 简单伤害计算公式
            var damage = Math.Max(1, baseAttack - baseDefense);

            // 应用伤害
            target.TakeDamage(damage);

            // 标记攻击者为已行动
            attacker.SetActed(true);

            // 检查目标是否阵亡
            var targetDied = !target.IsAlive();

            if (targetDied)
            {
                // 发布角色死亡事件
                // await _eventManager.PublishAsync(new CharacterDiedEvent(targetId));

                // 检查战斗是否结束
                battle.CheckBattleEnd();
            }

            return Task.FromResult(new BattleActionResult(
                true,
                BattleActionType.Attack,
                attackerId,
                new List<Guid> { targetId },
                new List<int> { damage },
                new List<bool> { targetDied },
                new List<bool> { true },
                "攻击成功"));
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <param name="targetX">目标X坐标</param>
        /// <param name="targetY">目标Y坐标</param>
        /// <returns>技能结果</returns>
        public async Task<BattleActionResult> UseSkillAsync(Guid characterId, Guid skillId, int targetX, int targetY)
        {
            try
            {
                // 1. 获取当前战斗
                if (CurrentBattle == null)
                {
                    _logger.LogWarning($"使用技能失败：当前没有进行中的战斗");
                    return new BattleActionResult(false, BattleActionType.Skill, characterId, null, null, null, null, "当前没有进行中的战斗");
                }

                // 2. 获取角色
                var character = CurrentBattle.GetBattleCharacter(characterId);
                if (character == null)
                {
                    _logger.LogWarning($"使用技能失败：未找到ID为 {characterId} 的角色");
                    return new BattleActionResult(false, BattleActionType.Skill, characterId, null, null, null, null, "未找到角色");
                }

                // 3. 获取技能
                var skill = _skillModule.GetSkill(skillId);
                if (skill == null)
                {
                    _logger.LogWarning($"使用技能失败：未找到ID为 {skillId} 的技能");
                    return new BattleActionResult(false, BattleActionType.Skill, characterId, null, null, null, null, "未找到技能");
                }

                // 4. 检查是否可以使用技能
                if (!_skillModule.CanUseSkill(characterId, skillId))
                {
                    _logger.LogWarning($"使用技能失败：角色 {characterId} 无法使用技能 {skill.Name}");
                    return new BattleActionResult(false, BattleActionType.Skill, characterId, null, null, null, null, "无法使用技能");
                }

                // 5. 检查目标位置是否在技能范围内
                var skillRange = _skillModule.CalculateSkillRange(skillId, character.X, character.Y);
                var targetPosition = (targetX, targetY);
                if (!skillRange.Contains(targetPosition))
                {
                    _logger.LogWarning($"使用技能失败：目标位置不在技能范围内");
                    return new BattleActionResult(false, BattleActionType.Skill, characterId, null, null, null, null, "目标位置不在技能范围内");
                }

                // 6. 计算技能影响区域
                var affectedArea = _skillModule.CalculateSkillArea(skillId, targetX, targetY);

                // 7. 获取受影响的角色
                var targetCharacterIds = new List<Guid>();
                var damageList = new List<int>();
                var isCriticalList = new List<bool>();
                var isKilledList = new List<bool>();

                foreach (var (x, y) in affectedArea)
                {
                    var targetCharacter = CurrentBattle.GetBattleCharacterAtPosition(x, y);
                    if (targetCharacter != null)
                    {
                        // 8. 检查目标是否合法（敌人/友方）
                        bool isValidTarget = false;
                        switch (skill.TargetType)
                        {
                            case TargetType.SingleEnemy:
                            case TargetType.AllEnemies:
                            case TargetType.AreaEnemy:
                                isValidTarget = targetCharacter.Team != character.Team;
                                break;
                            case TargetType.SingleAlly:
                            case TargetType.AllAllies:
                            case TargetType.AreaAlly:
                                isValidTarget = targetCharacter.Team == character.Team;
                                break;
                            case TargetType.Self:
                                isValidTarget = targetCharacter.Character.Id == character.Character.Id;
                                break;
                            case TargetType.AreaMixed:
                                isValidTarget = true;
                                break;
                        }

                        if (isValidTarget)
                        {
                            targetCharacterIds.Add(targetCharacter.Character.Id);

                            // 9. 计算技能效果（伤害/治疗等）
                            int effectValue = CalculateSkillEffect(character, targetCharacter, skill);
                            bool isCritical = CalculateIsCriticalHit(character, targetCharacter);
                            if (isCritical)
                            {
                                effectValue = (int)(effectValue * 1.5f);
                            }

                            // 10. 应用效果
                            bool isKilled = ApplySkillEffect(character, targetCharacter, skill, effectValue);

                            damageList.Add(effectValue);
                            isCriticalList.Add(isCritical);
                            isKilledList.Add(isKilled);
                        }
                    }
                }

                // 11. 消耗MP和设置冷却
                character.Character.ConsumeMP(skill.MPCost);
                skill.SetCurrentCooldown(skill.Cooldown);

                // 12. 触发事件
                // await _eventManager.PublishAsync(new SkillUsedEvent
                // {
                //     BattleId = CurrentBattle.Id,
                //     CasterId = characterId,
                //     SkillId = skillId,
                //     TargetIds = targetCharacterIds,
                //     Damages = damageList,
                //     IsCriticals = isCriticalList,
                //     IsKilleds = isKilledList
                // });

                _logger.LogInformation($"角色 {character.Character.Name} 使用技能 {skill.Name} 成功");
                return new BattleActionResult(
                true,
                BattleActionType.Skill,
                characterId,
                    targetCharacterIds,
                    damageList,
                    isCriticalList,
                    isKilledList,
                    "技能使用成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"使用技能失败：角色ID {characterId}，技能ID {skillId}");
                return new BattleActionResult(false, BattleActionType.Skill, characterId, null, null, null, null, $"使用技能失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 计算技能效果值
        /// </summary>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <param name="skill">技能</param>
        /// <returns>效果值（伤害/治疗等）</returns>
        private int CalculateSkillEffect(IBattleCharacter caster, IBattleCharacter target, ISkill skill)
        {
            int baseValue = skill.BasePower;

            // 根据技能类型和角色属性计算实际效果值
            switch (skill.Type)
            {
                case SkillType.Physical:
                    baseValue += caster.Character.Strength * 2;
                    baseValue -= target.Character.Constitution;
                    break;
                case SkillType.Magical:
                    baseValue += caster.Character.Intelligence * 2;
                    baseValue -= target.Character.Intelligence / 2;
                    break;
                case SkillType.Healing:
                    baseValue += caster.Character.Intelligence * 2;
                    break;
                    // 其他技能类型的计算...
            }

            // 确保最小效果值
            return Math.Max(1, baseValue);
        }

        /// <summary>
        /// 计算是否暴击
        /// </summary>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>是否暴击</returns>
        private bool CalculateIsCriticalHit(IBattleCharacter caster, IBattleCharacter target)
        {
            // 基础暴击率 5%
            int baseCritRate = 5;

            // 根据施法者的幸运值增加暴击率
            int critRate = baseCritRate + caster.Character.Luck / 5;

            // 随机判定
            return new Random().Next(100) < critRate;
        }

        /// <summary>
        /// 应用技能效果
        /// </summary>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <param name="skill">技能</param>
        /// <param name="effectValue">效果值</param>
        /// <returns>目标是否死亡</returns>
        private bool ApplySkillEffect(IBattleCharacter caster, IBattleCharacter target, ISkill skill, int effectValue)
        {
            switch (skill.Type)
            {
                case SkillType.Physical:
                case SkillType.Magical:
                    // 伤害技能
                    target.TakeDamage(effectValue);
                    break;
                case SkillType.Healing:
                    // 治疗技能
                    target.HealHP(effectValue);
                    break;
                case SkillType.Buff:
                    // 增益技能 - 这里需要实现buff效果
                    // 暂时省略
                    break;
                case SkillType.Debuff:
                    // 减益技能 - 这里需要实现debuff效果
                    // 暂时省略
                    break;
                    // 其他技能类型...
            }

            // 判断目标是否死亡
            bool isKilled = target.CurrentHP <= 0;
            if (isKilled)
            {
                // TODO: 处理角色死亡
                // _eventManager.PublishAsync(new CharacterDiedEvent { BattleId = CurrentBattle.Id, CharacterId = target.Character.Id });
            }

            return isKilled;
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>物品使用结果</returns>
        public Task<BattleActionResult> UseItemAsync(Guid characterId, Guid itemId, Guid targetId)
        {
            // 物品使用逻辑（后续实现）
            return Task.FromResult(new BattleActionResult(
                true,
                BattleActionType.Item,
                characterId,
                new List<Guid>(),
                new List<int>(),
                new List<bool>(),
                new List<bool>(),
                "物品使用成功"));
        }

        /// <summary>
        /// 计算可移动范围
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>可移动坐标列表</returns>
        public IReadOnlyList<(int X, int Y)> CalculateMoveRange(Guid characterId)
        {
            if (CurrentBattle == null)
            {
                return new List<(int X, int Y)>().AsReadOnly();
            }

            return ((Battle)CurrentBattle).CalculateMoveRange(characterId);
        }

        /// <summary>
        /// 计算攻击范围
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>可攻击坐标列表</returns>
        public IReadOnlyList<(int X, int Y)> CalculateAttackRange(Guid characterId)
        {
            if (CurrentBattle == null)
            {
                return new List<(int X, int Y)>().AsReadOnly();
            }

            return ((Battle)CurrentBattle).CalculateAttackRange(characterId);
        }

        /// <summary>
        /// 计算技能范围
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <returns>技能范围坐标列表</returns>
        public IReadOnlyList<(int X, int Y)> CalculateSkillRange(Guid characterId, Guid skillId)
        {
            // 技能范围计算逻辑（后续实现）
            return new List<(int X, int Y)>().AsReadOnly();
        }
    }
}