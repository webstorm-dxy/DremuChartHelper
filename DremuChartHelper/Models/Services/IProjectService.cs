using System.Collections.Generic;
using System.Threading.Tasks;
using DremuChartHelper.Models;

namespace DremuChartHelper.Models.Services;

/// <summary>
/// 项目服务接口 - 服务层模式
/// 定义项目相关的业务操作
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// 获取所有项目
    /// </summary>
    Task<List<ProjectInfo>> GetAllProjectsAsync();

    /// <summary>
    /// 根据路径获取项目
    /// </summary>
    /// <param name="projectPath">项目路径</param>
    Task<ProjectInfo?> GetProjectByPathAsync(string projectPath);

    /// <summary>
    /// 创建新项目
    /// </summary>
    /// <param name="name">项目名称</param>
    /// <param name="bpm">BPM</param>
    /// <param name="path">项目路径</param>
    Task<ProjectInfo> CreateProjectAsync(string name, string bpm, string path);

    /// <summary>
    /// 更新项目
    /// </summary>
    /// <param name="project">要更新的项目</param>
    Task UpdateProjectAsync(ProjectInfo project);

    /// <summary>
    /// 删除项目
    /// </summary>
    /// <param name="projectPath">项目路径</param>
    Task DeleteProjectAsync(string projectPath);
}