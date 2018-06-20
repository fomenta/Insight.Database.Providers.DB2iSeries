namespace Insight.Tests.DB2iSeries.Model
{
    public class Beer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Style { get; set; }

        public static Beer GetSample() => new Beer() { Name = "HopDevil", Style = "Hoppy" };
    }
}
