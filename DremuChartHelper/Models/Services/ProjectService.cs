using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DremuChartHelper.Models;
using DremuChartHelper.Models.Repositories;

namespace DremuChartHelper.Models.Services;

/// <summary>
/// 项目服务实现 - 服务层模式
/// 封装项目相关的业务逻辑和验证
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;

    public ProjectService(IProjectRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<List<ProjectInfo>> GetAllProjectsAsync()
    {
        return await _repository.LoadAllAsync();
    }

    public async Task<ProjectInfo?> GetProjectByPathAsync(string projectPath)
    {
        var projects = await _repository.LoadAllAsync();
        return projects.FirstOrDefault(p => p.Path == projectPath);
    }

    public async Task<ProjectInfo> CreateProjectAsync(string name, string bpm, string path)
    {
        // 业务规则验证
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("项目名称不能为空", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(bpm))
        {
            throw new ArgumentException("BPM 不能为空", nameof(bpm));
        }

        // 验证 BPM 是否为有效的数字
        if (!int.TryParse(bpm, out int bpmValue) || bpmValue <= 0)
        {
            throw new ArgumentException("BPM 必须是大于 0 的数字", nameof(bpm));
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("项目路径不能为空", nameof(path));
        }

        var newProject = new ProjectInfo
        {
            Name = name,
            Bpm = bpm,
            Path = path
        };

        var projects = await _repository.LoadAllAsync();
        projects.Add(newProject);
        await _repository.SaveAllAsync(projects);

        return newProject;
    }

    public async Task UpdateProjectAsync(ProjectInfo project)
    {
        if (project == null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        if (string.IsNullOrWhiteSpace(project.Name))
        {
            throw new ArgumentException("项目名称不能为空");
        }

        if (string.IsNullOrWhiteSpace(project.Bpm))
        {
            throw new ArgumentException("BPM 不能为空");
        }

        // 验证 BPM 是否为有效的数字
        if (!int.TryParse(project.Bpm, out int bpmValue) || bpmValue <= 0)
        {
            throw new ArgumentException("BPM 必须是大于 0 的数字");
        }

        var projects = await _repository.LoadAllAsync();
        var existingProject = projects.FirstOrDefault(p => p.Path == project.Path);

        if (existingProject == null)
        {
            throw new InvalidOperationException($"项目不存在: {project.Path}");
        }

        // 更新项目信息
        existingProject.Name = project.Name;
        existingProject.Bpm = project.Bpm;
        // Path 保持不变

        await _repository.SaveAllAsync(projects);
    }

    public async Task DeleteProjectAsync(string projectPath)
    {
        var projects = await _repository.LoadAllAsync();
        var project = projects.FirstOrDefault(p => p.Path == projectPath);

        if (project == null)
        {
            throw new InvalidOperationException($"项目不存在: {projectPath}");
        }

        projects.Remove(project);
        await _repository.SaveAllAsync(projects);
    }
}