namespace Pollenalarm.Core.Models
{
    public interface IPollenTranslation
    {
        string Id { get; set; }
        string PollenId { get; set; }
        string Language { get; set; }
        string Name { get; set; }
        string Description { get; set; }
    }
}
