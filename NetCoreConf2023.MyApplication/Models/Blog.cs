namespace NetCoreConf2023.MyApplication.Models
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public int? UserId { get; set; }
    }
}