using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.EventBus.Implementations.AzureEventHub
{
    public class EventHubProcessor
    {
        private readonly EventProcessorClient _processor;
        private readonly ILogger<EventHubProcessor> _logger;
        private readonly Func<EventData, Task> _messageHandler;

        public EventHubProcessor(
            EventHubConfig config,
            ILogger<EventHubProcessor> logger,
            Func<EventData, Task> messageHandler)
        {
            _logger = logger;
            _messageHandler = messageHandler;

            var blobClient = new BlobContainerClient(
                config.BlobStorageConnectionString,
                config.BlobContainerName);

            _processor = new EventProcessorClient(
                blobClient,
                config.ConsumerGroup,
                config.ConnectionString,
                config.EventHubName);

            _processor.ProcessEventAsync += ProcessEventHandler;
            _processor.ProcessErrorAsync += ProcessErrorHandler;
        }

        public async Task StartProcessingAsync()
        {
            try
            {
                await _processor.StartProcessingAsync();
                _logger.LogInformation("Started processing messages from Event Hub");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting Event Hub processor");
                throw;
            }
        }

        public async Task StopProcessingAsync()
        {
            try
            {
                await _processor.StopProcessingAsync();
                _logger.LogInformation("Stopped processing messages from Event Hub");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping Event Hub processor");
                throw;
            }
        }

        private async Task ProcessEventHandler(ProcessEventArgs args)
        {
            try
            {
                await _messageHandler(args.Data);
                await args.UpdateCheckpointAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Event Hub message");
                throw;
            }
        }

        private Task ProcessErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Error in Event Hub processing: {ErrorMessage}", args.Exception.Message);
            return Task.CompletedTask;
        }
    }
}