namespace CatalogService.MessageBroker
{
    public static class Constant
    {
        public const string WeightUpdateExchangeName = "progress.exchange.events";
        public const string WeightupdatedKey = "progress.weight.updated";
        public const string ProgressTrackingQueue = "progress.tracking.weight.queue";
    }
}
