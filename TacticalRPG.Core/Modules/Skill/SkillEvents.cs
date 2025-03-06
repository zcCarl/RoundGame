using System;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Skill
{
    /// <summary>
    /// 技能使用事件参数
    /// </summary>
    public class SkillUseEventArgs : EventArgs
    {
        /// <summary>
        /// 施法者
        /// </summary>
        public ICharacter Caster { get; }

        /// <summary>
        /// 技能
        /// </summary>
        public ISkill Skill { get; }

        /// <summary>
        /// 目标坐标X
        /// </summary>
        public int TargetX { get; }

        /// <summary>
        /// 目标坐标Y
        /// </summary>
        public int TargetY { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="caster">施法者</param>
        /// <param name="skill">技能</param>
        /// <param name="targetX">目标坐标X</param>
        /// <param name="targetY">目标坐标Y</param>
        public SkillUseEventArgs(ICharacter caster, ISkill skill, int targetX, int targetY)
        {
            Caster = caster;
            Skill = skill;
            TargetX = targetX;
            TargetY = targetY;
        }
    }

    /// <summary>
    /// 技能效果应用事件参数
    /// </summary>
    public class SkillEffectApplyEventArgs : EventArgs
    {
        /// <summary>
        /// 施法者
        /// </summary>
        public ICharacter Caster { get; }

        /// <summary>
        /// 目标
        /// </summary>
        public ICharacter Target { get; }

        /// <summary>
        /// 技能
        /// </summary>
        public ISkill Skill { get; }

        /// <summary>
        /// 效果
        /// </summary>
        public ISkillEffect Effect { get; }

        /// <summary>
        /// 实际效果值
        /// </summary>
        public int EffectValue { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <param name="skill">技能</param>
        /// <param name="effect">效果</param>
        /// <param name="effectValue">实际效果值</param>
        public SkillEffectApplyEventArgs(ICharacter caster, ICharacter target, ISkill skill, ISkillEffect effect, int effectValue)
        {
            Caster = caster;
            Target = target;
            Skill = skill;
            Effect = effect;
            EffectValue = effectValue;
        }
    }

    /// <summary>
    /// 技能学习事件参数
    /// </summary>
    public class SkillLearnEventArgs : EventArgs
    {
        /// <summary>
        /// 角色
        /// </summary>
        public ICharacter Character { get; }

        /// <summary>
        /// 技能
        /// </summary>
        public ISkill Skill { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="skill">技能</param>
        public SkillLearnEventArgs(ICharacter character, ISkill skill)
        {
            Character = character;
            Skill = skill;
        }
    }

    /// <summary>
    /// 技能冷却变化事件参数
    /// </summary>
    public class SkillCooldownEventArgs : EventArgs
    {
        /// <summary>
        /// 角色
        /// </summary>
        public ICharacter Character { get; }

        /// <summary>
        /// 技能
        /// </summary>
        public ISkill Skill { get; }

        /// <summary>
        /// 旧冷却值
        /// </summary>
        public int OldCooldown { get; }

        /// <summary>
        /// 新冷却值
        /// </summary>
        public int NewCooldown { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="skill">技能</param>
        /// <param name="oldCooldown">旧冷却值</param>
        /// <param name="newCooldown">新冷却值</param>
        public SkillCooldownEventArgs(ICharacter character, ISkill skill, int oldCooldown, int newCooldown)
        {
            Character = character;
            Skill = skill;
            OldCooldown = oldCooldown;
            NewCooldown = newCooldown;
        }
    }
}