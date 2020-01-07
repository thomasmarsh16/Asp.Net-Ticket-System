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

        // Task async services
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


        // project async services
        public async Task<IEnumerable<Project>> GetProjectsAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<Project>(new QueryDefinition(queryString));
            List<Project> results = new List<Project>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async System.Threading.Tasks.Task AddProjectAsync( Project project )
        {
            await this._container.CreateItemAsync<Project>(project, new PartitionKey(project.ProjectId));
        }

        public async Task<Project> GetProjectAsync(string id)
        {
            try
            {
                ItemResponse<Project> response = await this._container.ReadItemAsync<Project>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async System.Threading.Tasks.Task UpdateProjectAsync(string id, Project project)
        {
            await this._container.UpsertItemAsync<Project>(project, new PartitionKey(id));
        }

        public async System.Threading.Tasks.Task DeleteProjectAsync(string id)
        {
            await this._container.DeleteItemAsync<Project>(id, new PartitionKey(id));
        }
    }
}
