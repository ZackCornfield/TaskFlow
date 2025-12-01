using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Board; // Adjusted namespace for ColumnDto
using TaskFlowApi.Dtos.Column;
using TaskFlowApi.Models;

namespace TaskFlowApi.Services
{
    public interface IColumnService
    {
        Task<ColumnDto> CreateColumnAsync(int boardId, ColumnRequestDto request);
        Task<ColumnDto> UpdateColumnAsync(int id, ColumnRequestDto request);
        Task DeleteColumnAsync(int id);
    }

    public class ColumnService : IColumnService
    {
        private readonly TaskFlowDbContext _dbContext;

        public ColumnService(TaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ColumnDto> CreateColumnAsync(int boardId, ColumnRequestDto request)
        {
            var board = await _dbContext.Boards.FindAsync(boardId);
            if (board is null)
            {
                throw new KeyNotFoundException($"Board with ID {boardId} not found.");
            }

            var column = new Column
            {
                BoardId = boardId,
                Title = request.Title,
                SortOrder = (int)request.SortOrder,
            };

            _dbContext.Columns.Add(column);
            await _dbContext.SaveChangesAsync();

            return new ColumnDto
            {
                Id = column.Id,
                BoardId = column.BoardId,
                Title = column.Title,
                SortOrder = column.SortOrder,
                Tasks = new List<TaskDto>(), // Initialize empty tasks list
            };
        }

        public async Task<ColumnDto> UpdateColumnAsync(int id, ColumnRequestDto request)
        {
            var column = await _dbContext.Columns.FindAsync(id);
            if (column is null)
            {
                throw new KeyNotFoundException($"Column with ID {id} not found.");
            }

            column.Title = request.Title;
            column.SortOrder = (int)request.SortOrder; // Adjusted to cast SortOrder to int

            await _dbContext.SaveChangesAsync();

            return new ColumnDto
            {
                Id = column.Id,
                BoardId = column.BoardId,
                Title = column.Title,
                SortOrder = column.SortOrder,
                Tasks = new List<TaskDto>(), // Initialize empty tasks list
            };
        }

        public async Task DeleteColumnAsync(int id)
        {
            var column = await _dbContext.Columns.FindAsync(id);
            if (column is null)
            {
                throw new KeyNotFoundException($"Column with ID {id} not found.");
            }

            _dbContext.Columns.Remove(column);
            await _dbContext.SaveChangesAsync();
        }
    }
}
