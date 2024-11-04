using Fido2NetLib;
using System.Collections.Concurrent;

namespace DevSpaceWeb.Fido2;

public class Fido2Metadata : IMetadataService
{
    protected readonly List<IMetadataRepository> _repositories;
    protected readonly ConcurrentDictionary<Guid, MetadataStatement> _metadataStatements;
    protected readonly ConcurrentDictionary<Guid, MetadataBLOBPayloadEntry> _entries;
    protected bool _initialized;

    public Fido2Metadata(IEnumerable<IMetadataRepository> repositories)
    {
        _repositories = repositories.ToList();
        _metadataStatements = new ConcurrentDictionary<Guid, MetadataStatement>();
        _entries = new ConcurrentDictionary<Guid, MetadataBLOBPayloadEntry>();
    }

    public bool ConformanceTesting()
    {
        return _repositories[0] is ConformanceMetadataRepository;
    }

    protected virtual MetadataBLOBPayloadEntry? GetEntry(Guid aaguid)
    {
        if (!IsInitialized())
            throw new InvalidOperationException("MetadataService must be initialized");

        if (_entries.TryGetValue(aaguid, out MetadataBLOBPayloadEntry? entry))
        {
            if (_metadataStatements.TryGetValue(aaguid, out MetadataStatement? metadataStatement))
            {
                entry.MetadataStatement = metadataStatement;
            }

            return entry;
        }
        else
        {
            return null;
        }
    }

    protected virtual async Task LoadEntryStatementAsync(IMetadataRepository repository, MetadataBLOBPayload blob, MetadataBLOBPayloadEntry entry, CancellationToken cancellationToken)
    {
        if (entry.AaGuid.HasValue)
        {
            MetadataStatement? statement = await repository.GetMetadataStatementAsync(blob, entry, cancellationToken);

            if (statement?.AaGuid is Guid aaGuid)
            {
                _metadataStatements.TryAdd(aaGuid, statement);
            }
        }
    }

    protected virtual async Task InitializeRepositoryAsync(IMetadataRepository repository, CancellationToken cancellationToken)
    {
        MetadataBLOBPayload blob = await repository.GetBLOBAsync(cancellationToken);

        foreach (MetadataBLOBPayloadEntry? entry in blob.Entries)
        {
            if (entry.AaGuid is Guid aaGuid)
            {
                if (_entries.TryAdd(aaGuid, entry))
                {
                    // Load if it doesn't already exist
                    await LoadEntryStatementAsync(repository, blob, entry, cancellationToken);
                }
            }
        }
    }

    public virtual async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        foreach (IMetadataRepository repository in _repositories)
        {
            await InitializeRepositoryAsync(repository, cancellationToken);
        }
        _initialized = true;
    }

    public virtual bool IsInitialized()
    {
        return _initialized;
    }

    public virtual Task<MetadataBLOBPayloadEntry?> GetEntryAsync(Guid aaGuid, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetEntry(aaGuid));
    }
}
