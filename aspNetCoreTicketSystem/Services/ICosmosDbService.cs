﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspNetCoreTicketSystem.Models;

namespace aspNetCoreTicketSystem.Services
{
    public interface ICosmosDbService
    {
        // project task data async services
        Task<List<ProjectTask>> GetTasksAsync(string id);
        Task<ProjectTask> GetTaskAsync(string id);
        System.Threading.Tasks.Task AddTaskAsync(ProjectTask task);
        System.Threading.Tasks.Task UpdateTaskAsync(string id, ProjectTask task);
        System.Threading.Tasks.Task DeleteTaskAsync(string id);


        // project data async services
        Task<IEnumerable<Project>> GetProjectsAsync(string query);
        Task<Project> GetProjectAsync(string id);
        System.Threading.Tasks.Task AddProjectAsync(Project project);
        System.Threading.Tasks.Task UpdateProjectAsync(string id, Project project);
        System.Threading.Tasks.Task DeleteProjectAsync(string id);

        // Task Comment async services
        Task<List<Comment>> GetCommentsAsync(string ProjectID);

        System.Threading.Tasks.Task AddCommentAsync(Comment comment);
    }
}