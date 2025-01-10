using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Domain.Entities
{
    public sealed class UserEntity
    {
        [PrimaryKey]
        public string UserId { get; }
        public string UserName { get; }

        public UserEntity(string userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }
}
