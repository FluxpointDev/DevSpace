using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DevSpaceWeb.Components.DynamicForm;

public class FormGeneratorComponent : OwningComponentBase
{
    [Parameter] public object? DataContext { get; set; }

    [Parameter] public EventCallback<EditContext> OnValidSubmit { get; set; }

    public IEnumerable<DynamicFormItem>? FormItems { get; set; }

    protected override void OnParametersSet()
    {
        if (FormItems == null)
            FormItems = DataContext.GetType().GetProperties().Select(x => new DynamicFormItem(DataContext, x));
    }


}
