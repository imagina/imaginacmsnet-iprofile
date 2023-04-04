using Core.Interfaces;
using Idata.Data.Entities.Iprofile;

namespace Iprofile.Repositories.Interfaces
{
    public interface IUsersRepository : IRepositoryBase<User>
    {
        public Task<User?> LoginNoAD(string? username, string? password, Guid? guid = null);

        public Task<User?> LoginAD(User? wichUser);
    }
}
