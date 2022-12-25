namespace T4.Models
{
    public class sysdiagrams
    {
          public string name { get; set; } = null!;
          public int principal_id { get; set; }
          public int diagram_id { get; set; }
          public int? version { get; set; }
          public byte[]? definition { get; set; }
    }
}
