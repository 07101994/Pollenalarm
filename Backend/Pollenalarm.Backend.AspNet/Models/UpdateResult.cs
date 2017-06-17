namespace Pollenalarm.Backend.AspNet.Models
{
    public class UpdateResult<T>
    {
        public bool UpdateNeeded { get; set; }
        public string Message { get; set; }
        public T Update { get; set; }
    }
}