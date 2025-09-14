using System;

namespace Splitwise.Api.Entities;

public class Membership
{
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = default!;

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
