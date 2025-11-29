namespace CatalogService.MessageBroker.Messages
{
    public class ProductCreatedMessage : BasicMessage
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
