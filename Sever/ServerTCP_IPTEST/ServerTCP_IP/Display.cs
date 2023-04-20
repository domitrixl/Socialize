using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialClassCollection;
using System.Threading.Tasks;

namespace ServerTCP_IP
{
    internal class Display : IObserver<Message>
    {
        private string name;
        private List<string> messageInfos = new List<string>();
        private IDisposable cancellation;

        public Display(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("The observer must be assigned a name.");

            this.name = name;
        }

        public virtual void Subscribe(MessageHandler provider)
        {
            cancellation = provider.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            cancellation.Dispose();
            messageInfos.Clear();
        }

        public virtual void OnCompleted()
        {
            messageInfos.Clear();
        }

        // No implementation needed: Method is not called by the BaggageHandler class.
        public virtual void OnError(Exception e)
        {
            // No implementation.
        }

        public virtual void OnNext(Message message)
        {
            bool updated = false;

            if (message.Read)
            {
                var messagesToRemove = new List<string>();

                foreach(var messageToRemove in messagesToRemove)
                {
                    messageInfos.Remove(messageToRemove);
                }

                messagesToRemove.Clear();
            }
            else
            {
                string messageInfo = message.UserId.ToString();

                if (!messageInfos.Contains(messageInfo))
                {
                    messageInfos.Add(messageInfo);             
                    updated = true;
                }
            }

            if (updated)
            {
                messageInfos.Sort();
                Console.WriteLine("Arrivals information from {0}", this.name);
                foreach (var messageInfo in messageInfos)
                    Console.WriteLine(messageInfo);

                Console.WriteLine();
            }
        }
    }
}
