using DemoApi.Domain;

namespace DemoApi.Data.InMemory;

public sealed class InMemoryProjectRepository(InMemoryStore store) : IProjectRepository
{
    public Task<IReadOnlyList<Project>> ListAsync(CancellationToken ct)
    {
        lock (store.Sync)
        {
            IReadOnlyList<Project> result = store.Projects.OrderBy(p => p.Id).ToList();
            return Task.FromResult(result);
        }
    }

    public Task<Project?> GetAsync(int id, CancellationToken ct)
    {
        lock (store.Sync)
        {
            return Task.FromResult(store.Projects.FirstOrDefault(p => p.Id == id));
        }
    }

    public Task<bool> NameExistsAsync(string name, CancellationToken ct)
    {
        lock (store.Sync)
        {
            return Task.FromResult(store.Projects.Any(p => p.Name == name));
        }
    }

    public Task<Project> CreateAsync(string name, CancellationToken ct)
    {
        lock (store.Sync)
        {
            var project = new Project(store.NextProjectId++, name, DateTimeOffset.UtcNow);
            store.Projects.Add(project);
            return Task.FromResult(project);
        }
    }

    public Task<Project?> RenameAsync(int id, string name, CancellationToken ct)
    {
        lock (store.Sync)
        {
            var index = store.Projects.FindIndex(p => p.Id == id);
            if (index < 0)
                return Task.FromResult<Project?>(null);

            var project = store.Projects[index] with { Name = name };
            store.Projects[index] = project;
            return Task.FromResult<Project?>(project);
        }
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        lock (store.Sync)
        {
            store.Tasks.RemoveAll(t => t.ProjectId == id);   // как ON DELETE CASCADE в БД
            return Task.FromResult(store.Projects.RemoveAll(p => p.Id == id) > 0);
        }
    }
}
