using DependencySystem.DTOs.Team;
using DependencySystem.Models;

namespace DependencySystem.Services.Teams
{
    public interface ITeamService
    {
        Task<TeamMemberProfile> GetMyProfileAsync(string userId);
        Task<TeamMemberProfile> UpdateMyProfileAsync(string userId, UpdateProfileDto dto);

        Task<ProjectTeamMember> AssignUserToProjectAsync(AssignProjectMemberDto dto);
        Task<List<ProjectTeamMember>> GetProjectTeamAsync(int projectId);
        Task<bool> RemoveProjectMemberAsync(int projectId, string userId);
    }
}
