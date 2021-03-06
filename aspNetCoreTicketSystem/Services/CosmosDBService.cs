﻿namespace aspNetCoreTicketSystem.Services
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

            // get comments 
            List<Comment> comments = await GetCommentsAsync(id);

            // loop through and delete all comments
            foreach(Comment comment in comments)
            {
                await DeleteCommentAsync(comment.Id);
            }
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

        public async Task<List<ProjectTask>> GetTasksAsync(string id)
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.projectID = @id ORDER BY c.dueDate")
                .WithParameter("@id", id);

            var queryResults = this._container.GetItemQueryIterator<ProjectTask>( query );
            List<ProjectTask> results = new List<ProjectTask>();
            while (queryResults.HasMoreResults)
            {
                var response = await queryResults.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async System.Threading.Tasks.Task UpdateTaskAsync(string id, ProjectTask task)
        {
            await this._container.UpsertItemAsync<ProjectTask>(task, new PartitionKey(id));
        }


        // project async services
        public async Task<IEnumerable<Project>> GetProjectsAsync(string userEmail)
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE ARRAY_CONTAINS(c.projectWorkers, @userEmail)")
                    .WithParameter("@userEmail", userEmail);

            var queryResults = this._container.GetItemQueryIterator<Project>( query );
            List<Project> results = new List<Project>();
            while (queryResults.HasMoreResults)
            {
                var response = await queryResults.ReadNextAsync();

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
            // delete project
            await this._container.DeleteItemAsync<Project>(id, new PartitionKey(id));

            // find all tasks that go with project
            List<ProjectTask> tasks = await GetTasksAsync(id);

            // loop through tasks and delete them
            foreach( ProjectTask task in tasks)
            {
                await DeleteTaskAsync(task.Id);
            }
        }


        // comment async services
        public async Task<List<Comment>> GetCommentsAsync(string taskID)
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.TaskID = @taskID ORDER BY c.DateTimeCreated")
            .WithParameter("@taskID", taskID);

            var queryResults = this._container.GetItemQueryIterator<Comment>(query);
            List<Comment> results = new List<Comment>();
            while (queryResults.HasMoreResults)
            {
                var response = await queryResults.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async System.Threading.Tasks.Task AddCommentAsync(Comment comment)
        {
            await this._container.CreateItemAsync<Comment>(comment, new PartitionKey(comment.Id));
        }

        public async System.Threading.Tasks.Task DeleteCommentAsync(string id)
        {
            await this._container.DeleteItemAsync<Comment>(id, new PartitionKey(id));
        }
    }
}
