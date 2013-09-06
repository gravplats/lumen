using System;
using System.IO;
using System.Security.Principal;

namespace Lumen.AspNetMvc.Security.AppHarbor
{
    public class AuthenticationCookie
    {
        private readonly short cookieType;
        private readonly Guid id;
        private readonly bool persistent;
        private DateTime issueDate;
        private readonly string name;
        private readonly byte[] tag;
        private readonly string[] roles;

        private AuthenticationCookie(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                cookieType = binaryReader.ReadInt16();
                id = new Guid(binaryReader.ReadBytes(16));
                persistent = binaryReader.ReadBoolean();
                issueDate = DateTime.FromBinary(binaryReader.ReadInt64());
                name = binaryReader.ReadString();

                short rolesLength = binaryReader.ReadInt16();
                roles = new string[rolesLength];

                for (int i = 0; i < roles.Length; i++)
                {
                    roles[i] = binaryReader.ReadString();
                }

                short tagLength = binaryReader.ReadInt16();
                tag = tagLength == 0 ? null : binaryReader.ReadBytes(tagLength);
            }
        }

        public AuthenticationCookie(short cookieType, Guid id, bool persistent, string name, string[] roles = null, byte[] tag = null)
        {
            this.cookieType = cookieType;
            this.id = id;
            this.persistent = persistent;
            this.name = name;
            this.roles = roles ?? new string[0];
            this.tag = tag;
            issueDate = DateTime.UtcNow;
        }

        public short CookieType
        {
            get { return cookieType; }
        }

        public Guid Id
        {
            get { return id; }
        }

        public DateTime IssueDate
        {
            get { return issueDate; }
        }

        public string Name
        {
            get { return name; }
        }

        public bool Persistent
        {
            get { return persistent; }
        }

        public string[] Roles
        {
            get { return roles; }
        }

        public byte[] Tag
        {
            get { return tag; }
        }

        public IPrincipal GetPrincipal()
        {
            var identity = new CookieIdentity(this);
            return new GenericPrincipal(identity, roles);
        }

        public bool IsExpired(TimeSpan validity)
        {
            return issueDate.Add(validity) <= DateTime.UtcNow;
        }

        public void Renew()
        {
            issueDate = DateTime.UtcNow;
        }

        public byte[] Serialize()
        {
            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(cookieType);
                binaryWriter.Write(id.ToByteArray());
                binaryWriter.Write(persistent);
                binaryWriter.Write(issueDate.ToBinary());
                binaryWriter.Write(name);

                if (roles == null)
                {
                    binaryWriter.Write((short)0);
                }
                else
                {
                    binaryWriter.Write((short)roles.Length);
                    foreach (var role in roles)
                    {
                        binaryWriter.Write(role);
                    }
                }

                if (tag == null)
                {
                    binaryWriter.Write((short)0);
                }
                else
                {
                    binaryWriter.Write((short)tag.Length);
                    binaryWriter.Write(tag);
                }

                return memoryStream.ToArray();
            }
        }

        public static AuthenticationCookie Deserialize(byte[] data)
        {
            return new AuthenticationCookie(data);
        }
    }
}
