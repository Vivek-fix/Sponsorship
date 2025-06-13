using SponsorshipApp.Application.Interfaces;
using SponsorshipApp.Domain.Models;
using SponsorshipApp.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SponsorshipApp.Application.Services
{
    public class CommunityProjectService : ICommunityProjectService
    {
        private readonly ICommunityProjectRepository _repository;

        public CommunityProjectService(ICommunityProjectRepository repository)
        {
            _repository = repository;
        }

        public List<CommunityProject> GetAll() => _repository.GetAll();
    }

}
