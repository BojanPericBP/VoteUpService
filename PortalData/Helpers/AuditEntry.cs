using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using VoteUp.PortalData.Models.Base;

namespace VoteUp.PortalData.Helpers.Audits;
public class AuditEntry(EntityEntry entry)
{
    public EntityEntry Entry { get; } = entry;
    public string TableName { get; set; } = "";
    public Guid? UserId { get; set; }
    public Dictionary<string, object> KeyValues { get; } = [];
    public Dictionary<string, object> OldValues { get; } = [];
    public Dictionary<string, object> NewValues { get; } = [];
    public List<PropertyEntry> TemporaryProperties { get; } = [];

    public bool HasTemporaryProperties => TemporaryProperties.Count != 0;

    public Audit ToAudit()
    {
        var audit = new Audit
        {
            UserId = UserId,
            TableName = TableName,
            Timestamp = DateTime.UtcNow,
            KeyValues = JsonConvert.SerializeObject(KeyValues),
            OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues),
            NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues)
        };
        return audit;
    }
}