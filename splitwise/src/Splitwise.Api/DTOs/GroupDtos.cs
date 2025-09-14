namespace Splitwise.Api.DTOs;


public record CreateGroupRequest(string Name, string? Description);
public record GroupResponse(Guid Id, string Name);
public record AddMemberRequest(Guid UserId);
