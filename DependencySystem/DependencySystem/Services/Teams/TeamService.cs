using DependencySystem.DAL;
using DependencySystem.DTOs.Team;
using DependencySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Teams
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;

        public TeamService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================== PROFILE =====================
        public async Task<TeamMemberProfile> GetMyProfileAsync(string userId)
        {
            var profile = await _context.TeamMemberProfiles
                .Include(p => p.Department)
                .ThenInclude(d => d.Company)
                .FirstOrDefaultAsync(p => p.UserID == userId);

            if (profile == null)
            {
                profile = new TeamMemberProfile
                {
                    UserID = userId,
                    Designation = "Developer",
                    ExperienceYears = 0
                };

                _context.TeamMemberProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            return profile;
        }

        public async Task<TeamMemberProfile> UpdateMyProfileAsync(string userId, UpdateProfileDto dto)
        {
            var profile = await _context.TeamMemberProfiles
                .FirstOrDefaultAsync(p => p.UserID == userId);

            if (profile == null)
            {
                profile = new TeamMemberProfile { UserID = userId };
                _context.TeamMemberProfiles.Add(profile);
            }

            if (dto.DepartmentID.HasValue)
            {
                var deptExists = await _context.Departments.AnyAsync(d => d.DepartmentID == dto.DepartmentID.Value);
                if (!deptExists) throw new Exception("Department not found.");
            }

            profile.DepartmentID = dto.DepartmentID;
            profile.Designation = dto.Designation;
            profile.ExperienceYears = dto.ExperienceYears;

            await _context.SaveChangesAsync();
            return profile;
        }

        // ===================== PROJECT TEAM =====================
        public async Task<ProjectTeamMember> AssignUserToProjectAsync(AssignProjectMemberDto dto)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.ProjectID == dto.ProjectID);
            if (!projectExists) throw new Exception("Project not found.");

            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserID);
            if (!userExists) throw new Exception("User not found.");

            var already = await _context.ProjectTeamMembers
                .AnyAsync(x => x.ProjectID == dto.ProjectID && x.UserID == dto.UserID);

            if (already) throw new Exception("User already assigned to this project.");

            var ptm = new ProjectTeamMember
            {
                ProjectID = dto.ProjectID,
                UserID = dto.UserID,
                Role = dto.Role
            };

            _context.ProjectTeamMembers.Add(ptm);
            await _context.SaveChangesAsync();
            return ptm;
        }

        public async Task<List<ProjectTeamMember>> GetProjectTeamAsync(int projectId)
        {
            return await _context.ProjectTeamMembers
                .Where(x => x.ProjectID == projectId)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<bool> RemoveProjectMemberAsync(int projectId, string userId)
        {
            var ptm = await _context.ProjectTeamMembers
                .FirstOrDefaultAsync(x => x.ProjectID == projectId && x.UserID == userId);

            if (ptm == null) return false;

            _context.ProjectTeamMembers.Remove(ptm);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
