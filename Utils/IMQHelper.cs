using System;

namespace NetNote.Utils
{
    public interface IMQHelper
    {
        public void SendMsg<T>(string queName,T msg);
        public void Receive(string queName,Action<string> received);
    }
}
