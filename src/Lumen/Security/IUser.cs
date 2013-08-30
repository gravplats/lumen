using System;

namespace Lumen.Security
{
    public interface IUser
    {
        Guid? AuthenticationToken { get; }

        string Username { get; }

        void SetAuthenticationToken(IAuthenticationToken token);

        void Revoke();
    }
}