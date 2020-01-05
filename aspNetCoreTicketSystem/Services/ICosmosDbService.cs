using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspNetCoreTicketSystem.Models;

namespace aspNetCoreTicketSystem.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<ProjectTask>> GetTasksAsync(string query);
        Task<ProjectTask> GetTaskAsync(string id);
        System.Threading.Tasks.Task AddTaskAsync(ProjectTask task);
        System.Threading.Tasks.Task UpdateTaskAsync(string id, ProjectTask task);
        System.Threading.Tasks.Task DeleteTaskAsync(string id);
    }
}