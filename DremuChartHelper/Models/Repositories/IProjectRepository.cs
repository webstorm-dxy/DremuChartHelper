using System.Collections.Generic;
using System.Threading.Tasks;
using DremuChartHelper.Models;

namespace DremuChartHelper.Models.Repositories;

/// <summary>
/// 项目仓储接口 - 仓储模式
/// 抽象项目数据的持久化操作
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// 加载所有项目
    /// </summary>
    /// <returns>项目列表</returns>
    Task<List<ProjectInfo>> LoadAllAsync();

    /// <summary>
    /// 保存所有项目
    /// </summary>
    /// <param name="projects">要保存的项目列表</param>
    Task SaveAllAsync(List<ProjectInfo> projects);
}