using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace DremuChartHelper.ViewModels;

/// <summary>
/// ViewModel 基类 - 提供服务访问功能
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    /// <summary>
    /// 获取服务实例的辅助方法
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例</returns>
    protected T GetService<T>() where T : notnull
    {
        if (App.ServiceProvider == null)
        {
            throw new InvalidOperationException("ServiceProvider 尚未初始化");
        }

        return App.ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// 尝试获取服务实例
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例，如果未找到则返回 null</returns>
    protected T? TryGetService<T>() where T : class
    {
        return App.ServiceProvider?.GetService<T>();
    }
}