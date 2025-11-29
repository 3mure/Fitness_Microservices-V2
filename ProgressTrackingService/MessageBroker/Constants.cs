namespace CatalogService.MessageBroker
{
    public static class Constants
    {
        public const string ProductCreatedExchangeName = "ProgressTracking.events";
        public const string WeightupdatedKey = "weight.updated";
        public const string ProgressTrackingQueue = ".events.q";
    }
}
