namespace aspNetCoreTicketSystem.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using aspNetCoreTicketSystem.Models;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Fluent;
    using Microsoft.Extensions.Configuration;

    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async System.Threading.Tasks.Task AddTaskAsync(ProjectTask task)
        {
            await this._container.CreateItemAsync<ProjectTask>(task, new PartitionKey(task.Id));
        }

        public async System.Threading.Tasks.Task DeleteTaskAsync(string id)
        {
            await this._container.DeleteItemAsync<ProjectTask>(id, new PartitionKey(id));
        }

        public async Task<ProjectTask> GetTaskAsync(string id)
        {
            try
            {
                ItemResponse<ProjectTask> response = await this._container.ReadItemAsync<ProjectTask>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<ProjectTask>> GetTasksAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<ProjectTask>(new QueryDefinition(queryString));
            List<ProjectTask> results = new List<ProjectTask>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async System.Threading.Tasks.Task UpdateTaskAsync(string id, ProjectTask task)
        {
            await this._container.UpsertItemAsync<ProjectTask>(task, new PartitionKey(id));
        }
    }
}
