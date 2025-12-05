using System;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Tag;
using TaskFlowApi.Models;

namespace TaskFlowApi.Services;

public interface ITagService
{
    public Task<List<TagDto>> GetAllTagsAsync();
    public Task<TagDto> CreateTagAsync(TagRequestDto request);
    public Task DeleteTagAsync(int tagId);
    public Task<TagDto> AddTagToTaskAsync(int taskId, int tagId);
    public Task RemoveTagFromTaskAsync(int taskId, int tagId);
    public Task<List<TagDto>> GetTagsForTaskAsync(int taskId);
    public Task<List<TagDto>> AddTagsToTaskAsync(int taskId, List<int> tagIds);
    public Task RemoveTagsFromTaskAsync(int taskId, List<int> tagIds);
}

public class TagService(TaskFlowDbContext dbContext) : ITagService
{
    public async Task<List<TagDto>> GetAllTagsAsync()
    {
        var tags = await dbContext
            .Tags.AsNoTracking()
            .Select(tag => new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Color = tag.Color,
            })
            .ToListAsync();
        return tags;
    }

    public async Task<TagDto> CreateTagAsync(TagRequestDto request)
    {
        var tag = new Tag { Name = request.Name, Color = request.Color };

        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync();

        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Color = tag.Color,
        };
    }

    public async Task DeleteTagAsync(int tagId)
    {
        var tag = await dbContext.Tags.FindAsync(tagId);
        if (tag is null)
        {
            throw new KeyNotFoundException($"Tag with ID {tagId} not found.");
        }

        dbContext.Tags.Remove(tag);
        await dbContext.SaveChangesAsync();

        return;
    }

    public async Task<TagDto> AddTagToTaskAsync(int taskId, int tagId)
    {
        var task = await dbContext
            .Tasks.Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found.");
        }

        var tag = await dbContext.Tags.FindAsync(tagId);
        if (tag is null)
        {
            throw new KeyNotFoundException($"Tag with ID {tagId} not found.");
        }

        task.Tags.Add(tag);
        await dbContext.SaveChangesAsync();

        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Color = tag.Color,
        };
    }

    public async Task RemoveTagFromTaskAsync(int taskId, int tagId)
    {
        var task = await dbContext
            .Tasks.Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found.");
        }

        var tag = await dbContext.Tags.FindAsync(tagId);
        if (tag is null)
        {
            throw new KeyNotFoundException($"Tag with ID {tagId} not found.");
        }

        task.Tags.Remove(tag);
        await dbContext.SaveChangesAsync();

        return;
    }

    public async Task<List<TagDto>> GetTagsForTaskAsync(int taskId)
    {
        var task = await dbContext
            .Tasks.Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found.");
        }

        return task
            .Tags.Select(tag => new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Color = tag.Color,
            })
            .ToList();
    }

    public async Task<List<TagDto>> AddTagsToTaskAsync(int taskId, List<int> tagIds)
    {
        var task = await dbContext
            .Tasks.Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found.");
        }

        var tags = await dbContext.Tags.Where(tag => tagIds.Contains(tag.Id)).ToListAsync();

        if (tags.Count != tagIds.Count)
        {
            var missingTagIds = tagIds.Except(tags.Select(tag => tag.Id)).ToList();
            throw new KeyNotFoundException(
                $"Tags with IDs {string.Join(", ", missingTagIds)} not found."
            );
        }

        foreach (var tag in tags)
        {
            if (!task.Tags.Contains(tag))
            {
                task.Tags.Add(tag);
            }
        }

        await dbContext.SaveChangesAsync();

        return task
            .Tags.Select(tag => new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Color = tag.Color,
            })
            .ToList();
    }

    public async Task RemoveTagsFromTaskAsync(int taskId, List<int> tagIds)
    {
        var task = await dbContext
            .Tasks.Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found.");
        }

        var tagsToRemove = task.Tags.Where(tag => tagIds.Contains(tag.Id)).ToList();
        foreach (var tag in tagsToRemove)
        {
            task.Tags.Remove(tag);
        }

        await dbContext.SaveChangesAsync();

        return;
    }
}
