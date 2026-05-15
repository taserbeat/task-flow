using Application.Repositories;
using Domain.Entities.Roles;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Roles
{
    public class GetRolesUseCase
    {
        private readonly ILogger<GetRolesUseCase> _logger;
        private readonly IRoleRepository _roleRepository;

        public GetRolesUseCase(ILogger<GetRolesUseCase> logger, IRoleRepository roleRepository)
        {
            _logger = logger;
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleEm>> Execute()
        {
            return await _roleRepository.GetRolesAsync();
        }
    }
}