using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace DevSpaceWeb.Components.DynamicForm;

public class FormGeneratorComponent : OwningComponentBase
{
    [Parameter] public object DataContext { get; set; }

    [Parameter] public EventCallback<EditContext> OnValidSubmit { get; set; }


    public PropertyInfo[] Properties = Array.Empty<PropertyInfo>();

    private FormGeneratorComponentsRepository _repo;

    protected override void OnInitialized()
    {
        _repo = ScopedServices.GetService(typeof(FormGeneratorComponentsRepository)) as FormGeneratorComponentsRepository;
    }

    protected override void OnParametersSet()
    {
        Properties = DataContext.GetType().GetProperties();
    }

    public string Label { get; set; }
    public string GetLabelText(PropertyInfo prop)
    {
        bool IsUppercase = false;
        StringBuilder str = new StringBuilder();
        foreach(var i in prop.Name)
        {
            if (Char.IsUpper(i))
            {
                if (!IsUppercase)
                {
                    str.Append(' ');
                    IsUppercase = true;
                }
                
            }
            else
            {
                if (IsUppercase)
                {
                    IsUppercase = false;
                }
            }
            str.Append(i);
        }
        Label = str.ToString();
        var DisplayName = prop.GetCustomAttribute<DisplayAttribute>();
        if (DisplayName != null && !string.IsNullOrEmpty(DisplayName.Name))
            Label = DisplayName.Name;
        return Label;
    }

    public RenderFragment RenderFormElement(object model, PropertyInfo propInfoValue) => builder =>
    {
        Console.WriteLine("Name: " + propInfoValue.Name);
        Console.WriteLine("Component: " + propInfoValue.PropertyType.Name);
        Console.WriteLine("Comp: " + _repo.GetComponent(propInfoValue.PropertyType.Name).Name);
        builder.OpenComponent(0, _repo.GetComponent(propInfoValue.PropertyType.Name));
        builder.AddAttribute(1, "value", propInfoValue.GetValue(model));

        builder.AddAttribute(2, "valuechanged", EventCallback.Factory.Create<string>(this, (value) =>
        {
            propInfoValue.SetValue(model, value);
        }));

        //builder.AddAttribute<ChangeEventArgs>(2, "oninput", EventCallback.Factory.CreateBinder(
        //(object)this, (Action<string?>)(__value => {
        //    propInfoValue.SetValue(model, __value);
        //}), (string?)propInfoValue.GetValue(model)));

        //builder.AddAttribute(2, "oninput", EventCallback.Factory.Create(this, (value) =>
        //{
        //    propInfoValue.SetValue(model, value.Value);
        //}));
        builder.AddAttribute(3, "style", "width: 100%");
        builder.AddAttribute(4, "aria-label", $"{Label} Text");

        MaxLengthAttribute? MaxLength = propInfoValue.GetCustomAttribute<MaxLengthAttribute>();
        if (MaxLength != null)
            builder.AddAttribute(5, "maxlength", (Int64)MaxLength.Length);

        AutoCompleteAttribute? AutoComplete = propInfoValue.GetCustomAttribute<AutoCompleteAttribute>();
        if (AutoComplete != null)
            builder.AddAttribute(6, "autocomplete", AutoComplete.Type.GetType().GetMember(AutoComplete.Type.ToString()).First().GetCustomAttribute<DisplayAttribute>().Name);
        else
            builder.AddAttribute(6, "autocomplete", "off");

        builder.CloseComponent();
    };
}
