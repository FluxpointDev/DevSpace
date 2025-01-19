namespace DaRT
{
    public class BanOffline
    {
        public string guid = "";
        public String name = "";
        public String duration = "";
        public String reason = "";

        public BanOffline(string guid, String name, String duration, String reason)
        {
            this.guid = guid;
            this.name = name;
            this.duration = duration;
            this.reason = reason;
        }
    }
}
