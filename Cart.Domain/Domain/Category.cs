namespace Cart.Domain.Domain
{
    public class Category
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long? ParentCategoryId { get; set; }
        
    }
}