using System;

namespace TacticalRPG.Core.Modules.Map
{
    /// <summary>
    /// 地图信息接口
    /// </summary>
    public interface IMapInfo
    {
        /// <summary>
        /// 地图ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 地图名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 地图宽度
        /// </summary>
        int Width { get; }

        /// <summary>
        /// 地图高度
        /// </summary>
        int Height { get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreatedTime { get; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        DateTime LastModifiedTime { get; }

        /// <summary>
        /// 地图描述
        /// </summary>
        string Description { get; }
    }
}