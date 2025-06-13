using SponsorshipApp.Domain.Models;

namespace SponsorshipApp.Infrastructure.Interfaces
{
    public interface ICommunityProjectRepository
    {
        List<CommunityProject> GetAll();
        CommunityProject GetById(Guid id);
        void Update(CommunityProject project);
    }
}
