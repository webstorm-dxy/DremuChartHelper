using System.Threading.Tasks;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker.Filters;

public abstract class ElementFilterBase : IElementFilter
{
    public abstract string Name { get; }

    public virtual bool ShouldProcess(ElementInformation element)
    {
        return true;
    }

    public abstract Task ProcessElementsAsync(ElementInformation[] elements);
}
