namespace DevSpaceWeb.Components.DynamicForm;

//public class FormElementComponent : OwningComponentBase
//{
//    private string _Label;
//    private FormGeneratorComponentsRepository _repo;
//    /// 
//    /// Bindable property to set the class
//    /// 
//    public string CssClass { get => string.Join(" ", CssClasses.ToArray()); }
//    /// 
//    /// Setter for the classes of the form container
//    /// 
//    [Parameter] public List<string> CssClasses { get; set; }

//    /// 
//    /// Will set the 'class' of the all the controls. Useful when a framework needs to implement a class for all form elements
//    /// 
//    [Parameter] public List<string> DefaultFieldClasses { get; set; }
//    /// 
//    /// The identifier for the "/> used by the label element
//    /// 
//    [Parameter] public string Id { get; set; }

//    /// 
//    /// The label for the , if not set, it will check for a  on the 
//    /// 
//    [Parameter]
//    public string Label
//    {
//        get
//        {
//            var dd = CascadedEditContext.Model
//                 .GetType()
//                 .GetProperty(FieldIdentifier.Name)
//                 .GetCustomAttributes(typeof(DisplayAttribute), false)
//                 .FirstOrDefault() as DisplayAttribute;

//            return _Label ?? dd?.Name;
//        }
//        set { _Label = value; }
//    }

//    /// 
//    /// The property that should generate a formcontrol
//    /// 
//    [Parameter] public PropertyInfo FieldIdentifier { get; set; }

//    /// 
//    /// Get the  instance. This instance will be used to fill out the values inputted by the user
//    /// 
//    [CascadingParameter] EditContext CascadedEditContext { get; set; }

//    protected override void OnInitialized()
//    {
//        // setup the repo containing the mappings
//        _repo = ScopedServices.GetService(typeof(FormGeneratorComponentsRepository)) as FormGeneratorComponentsRepository;
//    }

//    /// 
//    /// A method thar renders the form control based on the 
//    /// 
//    /// 
//    /// 
//    public RenderFragment CreateComponent(System.Reflection.PropertyInfo propInfoValue) => builder =>
//    {
//        // Get the mapped control based on the property type
//        var componentType = _repo.GetComponent(propInfoValue.PropertyType.ToString());
//        if (componentType == null)
//            throw new Exception($"No component found for: {propInfoValue.PropertyType.ToString()}");
//        // Set the found component
//        var elementType = componentType;
//        // When the elementType that is rendered is a generic Set the propertyType as the generic type
//        if (elementType.IsGenericTypeDefinition)
//        {
//            Type[] typeArgs = { propInfoValue.PropertyType };
//            elementType = elementType.MakeGenericType(typeArgs);
//        }
//        // Activate the the Type so that the methods can be called
//        var instance = Activator.CreateInstance(elementType);
//        // Get the generic CreateFormComponent and set the property type of the model and the elementType that is rendered
//        MethodInfo method = typeof(FormElementComponent).GetMethod(nameof(FormElementComponent.CreateFormComponent));
//        MethodInfo genericMethod = method.MakeGenericMethod(propInfoValue.PropertyType, elementType);
//        // Execute the method with the following parameters
//        genericMethod.Invoke(this, new object[] { this, CascadedEditContext.Model, propInfoValue, builder, instance });
//    };

//    /// 
//    /// Creates the component that is rendered in the form
//    /// 
//    /// The type of the property
//    /// The type of the form element, should be based on , like a 
//    /// This  
//    /// The Model instance
//    /// The property that is being rendered
//    /// The render tree of this element
//    /// THe control instance
//    public void CreateFormComponent(object target,
//        object dataContext,
//        PropertyInfo propInfoValue, RenderTreeBuilder builder, InputBase instance)
//    {
//        // Create the component based on the mapped Element Type
//        builder.OpenComponent(0, typeof(TElement));

//        // Bind the value of the input base the the propery of the model instance
//        var s = propInfoValue.GetValue(dataContext);
//        builder.AddAttribute(1, nameof(InputBase.Value), s);

//        // Create the handler for ValueChanged. This wil update the model instance with the input
//        builder.AddAttribute(3, nameof(InputBase.ValueChanged),
//                Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck(
//                    EventCallback.Factory.Create(
//                        target, EventCallback.Factory.
//                        CreateInferred(target, __value => propInfoValue.SetValue(dataContext, __value),
//                        (T)propInfoValue.GetValue(dataContext)))));

//        // Create an expression to set the ValueExpression-attribute.
//        var constant = Expression.Constant(dataContext, dataContext.GetType());
//        var exp = Expression.Property(constant, propInfoValue.Name);
//        var lamb = Expression.Lambda>(exp);
//        builder.AddAttribute(4, nameof(InputBase.ValueExpression), lamb);

//        // Set the class for the the formelement.
//        builder.AddAttribute(5, "class", GetDefaultFieldClasses(instance));

//        CheckForInterfaceActions(this, CascadedEditContext.Model, propInfoValue, builder, instance, 6);

//        builder.CloseComponent();

//    }
//    private void CheckForInterfaceActions(object target,
//    object dataContext,
//        PropertyInfo propInfoValue, RenderTreeBuilder builder, InputBase instance, int indexBuilder)
//    {
//        // overriding the default classes for FormElement
//        if (TypeImplementsInterface(typeof(TElement), typeof(IRenderAsFormElement)))
//        {
//            this.CssClasses.AddRange((instance as IRenderAsFormElement).FormElementClasses);
//        }

//        // Check if the component has the IRenderChildren and renderen them in the form control
//        if (TypeImplementsInterface(typeof(TElement), typeof(IRenderChildren)))
//        {
//            (instance as IRenderChildren).RenderChildren(builder, indexBuilder, dataContext, propInfoValue);
//        }
//    }

//    /// 
//    /// Merges the default control classes with the  'class' key
//    /// 
//    /// The property type of the formelement
//    /// The instance of the component representing the form control
//    /// 
//    private string GetDefaultFieldClasses(InputBase instance)
//    {

//        var output = DefaultFieldClasses == null ? "" : string.Join(" ", DefaultFieldClasses);

//        if (instance == null)
//            return output;

//        var AdditionalAttributes = instance.AdditionalAttributes;

//        if (AdditionalAttributes != null &&
//              AdditionalAttributes.TryGetValue("class", out var @class) &&
//              !string.IsNullOrEmpty(Convert.ToString(@class)))
//        {
//            return $"{@class} {output}";
//        }

//        return output;
//    }

//    private bool IsTypeDerivedFromGenericType(Type typeToCheck, Type genericType)
//    {
//        if (typeToCheck == typeof(object))
//        {
//            return false;
//        }
//        else if (typeToCheck == null)
//        {
//            return false;
//        }
//        else if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
//        {
//            return true;
//        }
//        else
//        {
//            return IsTypeDerivedFromGenericType(typeToCheck.BaseType, genericType);
//        }
//    }
//    private static bool TypeImplementsInterface(Type type, Type typeToImplement)
//    {
//        Type foundInterface = type
//            .GetInterfaces()
//            .Where(i =>
//            {
//                return i.Name == typeToImplement.Name;
//            })
//            .Select(i => i)
//            .FirstOrDefault();

//        return foundInterface != null;
//    }

//}
