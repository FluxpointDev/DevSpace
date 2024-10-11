using Microsoft.AspNetCore.Components.Rendering;
using System.Reflection;

namespace DevSpaceWeb.Components.DynamicForm;

/// 
/// Helper interface for rendering values in components, needs to be non-generic for the form generator
/// 
public interface IRenderChildren
{
    /// 
    /// Function that will render the children for 
    /// 
    /// The element type of the 
    /// The builder for rendering a tree
    /// The index of the element
    /// The model for the form
    /// The property that is filled by the 
    void RenderChildren(RenderTreeBuilder builder, int index, object dataContext,
        PropertyInfo propInfoValue);
}
