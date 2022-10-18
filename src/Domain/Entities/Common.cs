namespace Domain.Entities
{
    public class DataAndCount<TModel> where TModel : class
    {
        public List<TModel> Data { get; set; }
        public int Count { get; set; }
    }
}
