namespace DevSpaceWeb.Components.DynamicForm;

public class FormGeneratorComponentsRepository
{
    private Dictionary<string, Type> _ComponentDict = new Dictionary<string, Type>();

    public Type _DefaultComponent { get; private set; }

    public FormGeneratorComponentsRepository()
    {

    }
    public FormGeneratorComponentsRepository(Dictionary<string, Type> componentRegistrations, Type defaultComponent)
    {
        _ComponentDict = componentRegistrations;
        _DefaultComponent = defaultComponent;
    }

    public void RegisterComponent(string key, Type component)
    {
        _ComponentDict.Add(key, component);
    }

    public void RemoveComponent(string key)
    {
        _ComponentDict.Remove(key);
    }

    public Type GetComponent(string key)
    {
        Type outVar = null;

        _ComponentDict.TryGetValue(key, out outVar);

        return outVar ?? _DefaultComponent;
    }

    public void Clear()
    {
        _ComponentDict.Clear();
    }
}