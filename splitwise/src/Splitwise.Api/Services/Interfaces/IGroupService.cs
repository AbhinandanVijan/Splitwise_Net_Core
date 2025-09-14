using Splitwise.Api.DTOs;

namespace Splitwise.Api.Services.Interfaces;
public interface IGroupService
{
    Task<GroupResponse> CreateGroupAsync(CreateGroupRequest req, Guid creatorId);
    Task AddMemberAsync(Guid groupId, Guid userId);
}
