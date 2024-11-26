using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.UsersRepository
{
    public interface IUserRepository
    {
        public Task<User> GetByUsername(string username);
        public Task Add(User user);
    }
}
