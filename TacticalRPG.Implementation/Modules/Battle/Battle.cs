using System;
using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Map;
using TacticalRPG.Implementation.Modules.Character;
using TacticalRPG.Core.Modules.Config;

namespace TacticalRPG.Implementation.Modules.Battle
{
    /// <summary>
    /// 战斗类实现，管理战斗状态
    /// </summary>
    public class Battle : IBattle
    {
        private readonly Dictionary<Guid, IBattleCharacter> _characters = new Dictionary<Guid, IBattleCharacter>();
        private readonly Dictionary<BattleTeam, List<IBattleCharacter>> _teamCharacters = new Dictionary<BattleTeam, List<IBattleCharacter>>();
        private readonly List<IBattleCharacter> _turnOrder = new List<IBattleCharacter>();
        private readonly IConfigManager _configManager;
        private int _currentTurnIndex = -1;

        /// <summary>
        /// 战斗ID
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 战斗名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 战斗地图
        /// </summary>
        public IMap Map { get; }

        /// <summary>
        /// 当前回合
        /// </summary>
        public int CurrentTurn { get; private set; }

        /// <summary>
        /// 当前行动角色
        /// </summary>
        public IBattleCharacter CurrentActor { get; private set; }

        /// <summary>
        /// 战斗状态
        /// </summary>
        public BattleState State { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">战斗ID</param>
        /// <param name="name">战斗名称</param>
        /// <param name="map">战斗地图</param>
        /// <param name="configManager">配置管理器</param>
        public Battle(Guid id, string name, IMap map, IConfigManager configManager)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Map = map ?? throw new ArgumentNullException(nameof(map));
            _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            State = BattleState.Preparing;

            // 初始化队伍字典
            _teamCharacters[BattleTeam.Player] = new List<IBattleCharacter>();
            _teamCharacters[BattleTeam.Enemy] = new List<IBattleCharacter>();
        }

        /// <summary>
        /// 添加角色到战斗
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="team">队伍</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>操作结果</returns>
        public bool AddCharacter(CombatCharacter character, BattleTeam team, int x, int y)
        {
            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }

            if (State != BattleState.NotStarted)
            {
                return false; // 战斗已开始，无法添加角色
            }

            if (!Map.IsPositionInBounds(x, y))
            {
                return false; // 坐标超出地图范围
            }

            if (_characters.ContainsKey(character.Id))
            {
                return false; // 角色已在战斗中
            }

            // 检查该位置是否已有其他角色
            if (GetBattleCharacterAtPosition(x, y) != null)
            {
                return false; // 该位置已有其他角色
            }

            // 创建战斗角色
            var battleCharacter = new BattleCharacter(character, team, x, y);

            // 添加到战斗
            _characters[character.Id] = battleCharacter;
            _teamCharacters[team].Add(battleCharacter);

            return true;
        }

        /// <summary>
        /// 添加战斗角色到战斗
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="team">队伍</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>操作结果</returns>
        public bool AddCharacter(IBattleCharacter character, BattleTeam team, int x, int y)
        {
            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }

            if (State != BattleState.NotStarted)
            {
                return false; // 战斗已开始，无法添加角色
            }

            if (!Map.IsPositionInBounds(x, y))
            {
                return false; // 坐标超出地图范围
            }

            if (_characters.ContainsKey(character.Character.Id))
            {
                return false; // 角色已在战斗中
            }

            // 检查该位置是否已有其他角色
            if (GetBattleCharacterAtPosition(x, y) != null)
            {
                return false; // 该位置已有其他角色
            }

            // 设置角色位置
            character.SetPosition(x, y);

            // 添加到战斗
            _characters[character.Character.Id] = character;
            _teamCharacters[team].Add(character);

            return true;
        }

        /// <summary>
        /// 从战斗中移除角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        public bool RemoveCharacter(Guid characterId)
        {
            if (!_characters.TryGetValue(characterId, out var character))
            {
                return false; // 角色不在战斗中
            }

            // 从队伍中移除
            _teamCharacters[character.Team].Remove(character);

            // 从回合顺序中移除
            _turnOrder.Remove(character);

            // 如果是当前行动角色，则移至下一个
            if (CurrentActor == character)
            {
                EndCurrentActorTurn();
            }

            // 从角色字典中移除
            _characters.Remove(characterId);

            // 检查战斗是否结束
            CheckBattleEnd();

            return true;
        }

        /// <summary>
        /// 获取战斗角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>战斗角色</returns>
        public IBattleCharacter GetBattleCharacter(Guid characterId)
        {
            _characters.TryGetValue(characterId, out var character);
            return character;
        }

        /// <summary>
        /// 获取指定位置的战斗角色
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>战斗角色，如果该位置没有角色则返回null</returns>
        public IBattleCharacter GetBattleCharacterAtPosition(int x, int y)
        {
            return _characters.Values.FirstOrDefault(c => c.X == x && c.Y == y);
        }

        /// <summary>
        /// 获取所有战斗角色
        /// </summary>
        /// <returns>所有战斗角色</returns>
        public IReadOnlyList<IBattleCharacter> GetAllBattleCharacters()
        {
            return _characters.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// 获取指定队伍的所有角色
        /// </summary>
        /// <param name="team">队伍</param>
        /// <returns>队伍角色</returns>
        public IReadOnlyList<IBattleCharacter> GetTeamCharacters(BattleTeam team)
        {
            if (_teamCharacters.TryGetValue(team, out var characters))
            {
                return characters.AsReadOnly();
            }

            return new List<IBattleCharacter>().AsReadOnly();
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        /// <returns>操作结果</returns>
        public bool StartBattle()
        {
            if (State != BattleState.NotStarted)
            {
                return false; // 战斗已开始
            }

            if (_characters.Count == 0)
            {
                return false; // 战斗中没有角色
            }

            // 生成回合顺序
            GenerateTurnOrder();

            // 设置战斗状态
            State = BattleState.InProgress;
            CurrentTurn = 1;

            // 重置角色状态
            foreach (var character in _characters.Values)
            {
                character.ResetTurnState();
            }

            // 设置第一个行动角色
            _currentTurnIndex = 0;
            CurrentActor = _turnOrder.Count > 0 ? _turnOrder[_currentTurnIndex] : null;

            return true;
        }

        /// <summary>
        /// 开始下一个角色的回合
        /// </summary>
        /// <returns>操作结果</returns>
        public bool StartNextActorTurn()
        {
            if (State != BattleState.InProgress)
            {
                return false; // 战斗未开始或已结束
            }

            if (_turnOrder.Count == 0)
            {
                return false; // 回合顺序为空
            }

            // 如果是第一个角色
            if (_currentTurnIndex == -1)
            {
                _currentTurnIndex = 0;
                CurrentActor = _turnOrder[_currentTurnIndex];
                CurrentActor.ResetTurnState();
                return true;
            }

            // 移至下一个角色
            _currentTurnIndex = (_currentTurnIndex + 1) % _turnOrder.Count;

            // 如果回到第一个角色，则开始新回合
            if (_currentTurnIndex == 0)
            {
                CurrentTurn++;

                // 重置所有角色的回合状态
                foreach (var character in _characters.Values)
                {
                    character.ResetTurnState();
                }
            }

            // 设置当前行动角色
            CurrentActor = _turnOrder[_currentTurnIndex];

            return true;
        }

        /// <summary>
        /// 结束当前角色的回合
        /// </summary>
        /// <returns>操作结果</returns>
        public bool EndCurrentActorTurn()
        {
            if (State != BattleState.InProgress || CurrentActor == null)
            {
                return false; // 战斗未开始或已结束，或无当前角色
            }

            // 标记当前角色为已行动
            CurrentActor.SetActed(true);
            CurrentActor.SetMoved(true);

            return true;
        }

        /// <summary>
        /// 检查战斗是否结束
        /// </summary>
        /// <returns>战斗是否结束</returns>
        public bool CheckBattleEnd()
        {
            if (State != BattleState.InProgress)
            {
                return false;
            }

            // 检查玩家队伍是否全部阵亡
            bool allPlayersDead = _teamCharacters[BattleTeam.Player].All(c => !c.IsAlive());

            // 检查敌人队伍是否全部阵亡
            bool allEnemiesDead = _teamCharacters[BattleTeam.Enemy].All(c => !c.IsAlive());

            if (allPlayersDead)
            {
                State = BattleState.PlayerDefeat;
                return true;
            }

            if (allEnemiesDead)
            {
                State = BattleState.PlayerVictory;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移动角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="targetX">目标X坐标</param>
        /// <param name="targetY">目标Y坐标</param>
        /// <returns>操作结果</returns>
        public bool MoveCharacter(Guid characterId, int targetX, int targetY)
        {
            if (State != BattleState.InProgress)
            {
                return false;
            }

            if (!Map.IsPositionInBounds(targetX, targetY))
            {
                return false;
            }

            if (!_characters.TryGetValue(characterId, out var character))
            {
                return false;
            }

            if (character != CurrentActor)
            {
                return false;
            }

            if (character.HasMoved)
            {
                return false;
            }

            // 检查目标位置是否有其他角色
            if (GetBattleCharacterAtPosition(targetX, targetY) != null)
            {
                return false;
            }

            // 计算移动范围
            var moveRange = CalculateMoveRange(characterId);
            if (!moveRange.Any(p => p.X == targetX && p.Y == targetY))
            {
                return false;
            }

            // 移动角色
            character.SetPosition(targetX, targetY);
            character.SetMoved(true);

            return true;
        }

        /// <summary>
        /// 计算移动范围
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>可移动的坐标列表</returns>
        public IReadOnlyList<(int X, int Y)> CalculateMoveRange(Guid characterId)
        {
            if (!_characters.TryGetValue(characterId, out var character))
            {
                return new List<(int X, int Y)>();
            }

            if (!character.IsAlive())
            {
                return new List<(int X, int Y)>();
            }

            // 获取角色移动力
            var combatStats = character.Character.CalculateCombatStats();
            int movement = combatStats.Movement;

            // 使用广度优先搜索计算移动范围
            var result = new List<(int X, int Y)>();
            var visited = new HashSet<(int X, int Y)>();
            var queue = new Queue<(int X, int Y, int RemainingMovement)>();

            queue.Enqueue((character.X, character.Y, movement));
            visited.Add((character.X, character.Y));

            while (queue.Count > 0)
            {
                var (x, y, remainingMovement) = queue.Dequeue();

                // 添加当前位置到结果中（除了起始位置）
                if (x != character.X || y != character.Y)
                {
                    result.Add((x, y));
                }

                if (remainingMovement <= 0)
                {
                    continue;
                }

                // 检查四个方向
                var directions = new[] { (0, 1), (1, 0), (0, -1), (-1, 0) };
                foreach (var (dx, dy) in directions)
                {
                    int newX = x + dx;
                    int newY = y + dy;

                    if (!Map.IsPositionInBounds(newX, newY))
                    {
                        continue;
                    }

                    if (visited.Contains((newX, newY)))
                    {
                        continue;
                    }

                    // 检查是否有其他角色占据该位置
                    if (GetBattleCharacterAtPosition(newX, newY) != null)
                    {
                        continue;
                    }

                    // 获取地形移动消耗
                    var terrain = Map.GetTerrain(newX, newY);
                    int movementCost = terrain.MovementCost;

                    if (remainingMovement >= movementCost)
                    {
                        queue.Enqueue((newX, newY, remainingMovement - movementCost));
                        visited.Add((newX, newY));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 计算攻击范围
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>可攻击的坐标列表</returns>
        public IReadOnlyList<(int X, int Y)> CalculateAttackRange(Guid characterId)
        {
            if (!_characters.TryGetValue(characterId, out var character))
            {
                return new List<(int X, int Y)>();
            }

            if (!character.IsAlive())
            {
                return new List<(int X, int Y)>();
            }

            // 获取角色攻击范围
            var combatStats = character.Character.CalculateCombatStats();
            int attackRange = combatStats.AttackRange;

            var result = new List<(int X, int Y)>();

            // 计算攻击范围内的所有坐标
            for (int x = character.X - attackRange; x <= character.X + attackRange; x++)
            {
                for (int y = character.Y - attackRange; y <= character.Y + attackRange; y++)
                {
                    // 检查是否在地图范围内
                    if (!Map.IsPositionInBounds(x, y))
                    {
                        continue;
                    }

                    // 计算曼哈顿距离
                    int distance = Math.Abs(x - character.X) + Math.Abs(y - character.Y);
                    if (distance <= attackRange)
                    {
                        result.Add((x, y));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 计算技能范围
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <returns>技能可影响的坐标列表</returns>
        public IReadOnlyList<(int X, int Y)> CalculateSkillRange(Guid characterId, Guid skillId)
        {
            // 此处需要访问技能模块获取技能信息，后续实现
            return new List<(int X, int Y)>();
        }

        /// <summary>
        /// 生成回合顺序
        /// </summary>
        private void GenerateTurnOrder()
        {
            _turnOrder.Clear();

            // 首先添加玩家队伍
            foreach (var character in _teamCharacters[BattleTeam.Player])
            {
                _turnOrder.Add(character);
            }

            // 然后添加敌方队伍
            foreach (var character in _teamCharacters[BattleTeam.Enemy])
            {
                _turnOrder.Add(character);
            }

            // 获取战斗配置
            var battleConfig = _configManager?.GetConfig<BattleConfig>(BattleConfig.MODULE_ID);
            float speedRandomFactor = battleConfig?.SpeedRandomFactor ?? 0.05f;

            // 基于速度和少量随机因素排序
            var random = new Random();
            _turnOrder.Sort((a, b) =>
            {
                // 获取基础速度属性
                int speedA = a.Character.Agility; // 使用敏捷属性作为速度基础
                int speedB = b.Character.Agility;

                // 添加小量随机因素（速度的随机因子波动），从配置获取
                int randomRangeA = (int)(speedA * speedRandomFactor);
                int randomRangeB = (int)(speedB * speedRandomFactor);

                speedA += random.Next(-randomRangeA, randomRangeA + 1);
                speedB += random.Next(-randomRangeB, randomRangeB + 1);

                // 速度高的先行动
                return speedB.CompareTo(speedA);
            });

            _currentTurnIndex = -1;
        }

        public bool AddCharacter(ICharacter character, BattleTeam team, int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}