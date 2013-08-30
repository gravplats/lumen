using System;

namespace Lumen.Security
{
    public class User : IUser
    {
        public Guid? AuthenticationToken { get; private set; }

        public string Username { get; set; }

        public virtual void Revoke()
        {
            AuthenticationToken = null;
        }

        public virtual void SetAuthenticationToken(IAuthenticationToken token)
        {
            Ensure.NotNull(token, "token");
            AuthenticationToken = token.Generate(Username);
        }
    }
}