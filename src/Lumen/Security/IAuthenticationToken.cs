using System;

namespace Lumen.Security
{
    public interface IAuthenticationToken
    {
        void Clear(IUser user);

        Guid Generate(string identifier);
    }
}