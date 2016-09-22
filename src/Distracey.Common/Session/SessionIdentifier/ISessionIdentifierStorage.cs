using System;

namespace Distracey.Common.Session.SessionIdentifier
{
    public interface ISessionIdentifierStorage
    {
        Guid? Current { get; set; }

        void Clear();
    }
}