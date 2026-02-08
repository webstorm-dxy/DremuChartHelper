using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using DremuChartHelper.Models.GorgeLinker.Filters;
using DremuChartHelper.Models.GorgeLinker.Repositories;
using DremuChartHelper.Models.GorgeLinker.Services;
using DremuChartHelper.Models.Repositories;
using DremuChartHelper.Models.Services;
using DremuChartHelper.ViewModels;

namespace DremuChartHelper.Configuration;

/// <summary>
/// 服务集合扩展 - 配置依赖注入
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册 GorgeStudio 相关服务
    /// </summary>
    public static IServiceCollection AddGorgeLinkerServices(
        this IServiceCollection services,
        string? serverUrl = null)
    {
        serverUrl ??= Environment.GetEnvironmentVariable("DREMU_SERVER_URL") ?? "http://localhost:14324";
        // 仓储
        services.AddSingleton<IChartRepository>(sp => new GorgeStudioChartRepository(serverUrl));

        // 领域服务
        services.AddSingleton<IChartDataService, ChartDataService>();

        // 注册具体过滤器
        services.AddSingleton<IElementFilter, JudgementLineFilter>();

        // 过滤器管理器，注入所有过滤器
        services.AddSingleton<FilterManager>(sp =>
        {
            var repo = sp.GetRequiredService<IChartRepository>();
            var chartService = sp.GetRequiredService<IChartDataService>();
            var filters = sp.GetServices<IElementFilter>();
            return new FilterManager(repo, chartService, filters);
        });

        return services;
    }

    /// <summary>
    /// 注册项目管理相关服务
    /// </summary>
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        // 配置文件路径
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appDataPath, "DremuChartHelper");
        var filePath = Path.Combine(appFolder, "editor_data.json");

        // 仓储
        services.AddSingleton<IProjectRepository>(sp => new ProjectJsonRepository(filePath));

        // 领域服务
        services.AddSingleton<IProjectService, ProjectService>();

        return services;
    }

    /// <summary>
    /// 注册 ViewModels
    /// </summary>
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ProjectManagerViewModel>();
        services.AddTransient<ResourceManagerViewModel>();
        services.AddTransient<CurveEditorViewModel>();

        return services;
    }

    /// <summary>
    /// 配置所有应用程序服务
    /// </summary>
    public static IServiceCollection ConfigureAppServices(
        this IServiceCollection services,
        string? gorgeStudioUrl = null)
    {
        services.AddGorgeLinkerServices(gorgeStudioUrl);
        services.AddProjectServices();
        services.AddViewModels();

        return services;
    }
}
