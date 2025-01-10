using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Domain.Entities;

namespace Template.Infrastructure.SQLite
{
    public class UserSQLite : SQLiteBase<UserEntity>
    {
        public UserSQLite() : base(() => new UserEntity(string.Empty, string.Empty))
        {
        }
    }
}
