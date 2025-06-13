using SponsorshipApp.Domain.Models;
using SponsorshipApp.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SponsorshipApp.Infrastructure.Repositories
{
    public class InMemoryCommunityProjectRepository : ICommunityProjectRepository
    {
        private readonly List<CommunityProject> _projects = new()
    {
        new() { Name = "Clean Water Project", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(6), TotalRequired = 50000 },
        new() { Name = "School Supplies Drive", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(3), TotalRequired = 20000 },
    };

        public List<CommunityProject> GetAll() => _projects;

        public CommunityProject GetById(Guid id) => _projects.FirstOrDefault(p => p.Id == id);

        public void Update(CommunityProject project)
        {
            var index = _projects.FindIndex(p => p.Id == project.Id);
            if (index >= 0) _projects[index] = project;
        }
    }

}
