using Dental.DataAccess.Repo;
using Dental.Model;
using Dental_Surgery.Model;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Dental_Surgery.DataAccess.Repo
{
    public interface IUserLoginRepo : IRepository<UserLogin>
    {
        public void Update(UserLogin userLogin);
    }
}
