namespace DevSpaceShared;
public class InfoTest
{
    public void Run()
    {
        Console.WriteLine("--- System Information --- ---");

        // Use this
        //RuntimeInformation.

        //Might be useful for error logs
        //Console.WriteLine($"Stacktrace: {Environment.StackTrace}");

        foreach (System.Reflection.Assembly i in AppDomain.CurrentDomain.GetAssemblies())
        {
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(i));
            //if (i.FullName.Contains("DevSpace"))
            //    break;
        }
        Console.WriteLine("--- --- --- -- --- --- --- ---");
    }
}
