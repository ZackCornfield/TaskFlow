using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Board; // Adjusted namespace for TaskDto
using TaskFlowApi.Dtos.Comment;
using TaskFlowApi.Dtos.Tag;
using TaskFlowApi.Dtos.Task;
using TaskFlowApi.Models;

namespace TaskFlowApi.Services
{
    public interface ITaskService
    {
        Task<TaskDto> CreateTaskAsync(int columnId, TaskRequestDto request);
        Task<TaskDto> UpdateTaskAsync(int id, TaskRequestDto request);
        Task DeleteTaskAsync(int id);
        Task<TaskDto> MoveTaskAsync(int id, MoveTaskDto request);
    }

    public class TaskService : ITaskService
    {
        private readonly TaskFlowDbContext _dbContext;

        public TaskService(TaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskDto> CreateTaskAsync(int columnId, TaskRequestDto request)
        {
            var column = await _dbContext.Columns.FindAsync(columnId);
            if (column is null)
            {
                throw new KeyNotFoundException($"Column with ID {columnId} not found.");
            }

            if (request.AssignedToId.HasValue)
            {
                var userExists = await _dbContext.Users.AnyAsync(u =>
                    u.Id == request.AssignedToId.Value
                );
                if (!userExists)
                {
                    throw new ArgumentException(
                        $"User with ID {request.AssignedToId.Value} does not exist."
                    );
                }
            }

            var task = new TaskItem
            {
                ColumnId = columnId,
                Title = request.Title,
                Description = request.Description,
                SortOrder = request.SortOrder,
                DueDate = request.DueDate,
                CreatedById = request.CreatedById,
                AssignedToId = request.AssignedToId,
                IsCompleted = request.IsCompleted, // Added IsCompleted property
            };

            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                ColumnId = task.ColumnId,
                Title = task.Title,
                Description = task.Description,
                SortOrder = task.SortOrder,
                DueDate = task.DueDate,
                CreatedById = task.CreatedById,
                AssignedToId = task.AssignedToId,
                IsCompleted = task.IsCompleted, // Added IsCompleted property
                Tags = new List<TagDto>(),
                Comments = new List<CommentDto>(),
            };
        }

        public async Task<TaskDto> UpdateTaskAsync(int id, TaskRequestDto request)
        {
            var task = await _dbContext.Tasks.FindAsync(id);
            if (task is null)
            {
                throw new KeyNotFoundException($"Task with ID {id} not found.");
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.SortOrder = request.SortOrder;
            task.DueDate = request.DueDate;
            task.AssignedToId = request.AssignedToId;
            task.IsCompleted = request.IsCompleted; // Added IsCompleted property

            await _dbContext.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                ColumnId = task.ColumnId,
                Title = task.Title,
                Description = task.Description,
                SortOrder = task.SortOrder,
                DueDate = task.DueDate,
                CreatedById = task.CreatedById,
                AssignedToId = task.AssignedToId,
                IsCompleted = task.IsCompleted, // Added IsCompleted property
                Tags = task.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name }).ToList(),
                Comments = task
                    .Comments.Select(c => new CommentDto { Id = c.Id, Content = c.Content })
                    .ToList(),
            };
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _dbContext.Tasks.FindAsync(id);
            if (task is null)
            {
                throw new KeyNotFoundException($"Task with ID {id} not found.");
            }

            _dbContext.Tasks.Remove(task);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TaskDto> MoveTaskAsync(int id, MoveTaskDto request)
        {
            var task = await _dbContext.Tasks.FindAsync(id);
            if (task is null)
            {
                throw new KeyNotFoundException($"Task with ID {id} not found.");
            }

            var targetColumn = await _dbContext.Columns.FindAsync(request.TargetColumnId);
            if (targetColumn is null)
            {
                throw new KeyNotFoundException(
                    $"Target column with ID {request.TargetColumnId} not found."
                );
            }

            task.ColumnId = request.TargetColumnId;
            task.SortOrder = request.SortOrder;

            await _dbContext.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                ColumnId = task.ColumnId,
                Title = task.Title,
                Description = task.Description,
                SortOrder = task.SortOrder,
                DueDate = task.DueDate,
                CreatedById = task.CreatedById,
                AssignedToId = task.AssignedToId,
                Tags = new List<TagDto>(),
                Comments = new List<CommentDto>(),
            };
        }
    }
}
