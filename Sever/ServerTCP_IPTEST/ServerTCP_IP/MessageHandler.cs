using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialClassCollection;

namespace ServerTCP_IP
{
    internal class MessageHandler : IObservable<Message>
    {
        private List<IObserver<Message>> observers;
        private List<Message> messages;

        public MessageHandler()
        {
            observers = new List<IObserver<Message>>();
            messages = new List<Message>();
        }

        public IDisposable Subscribe(IObserver<Message> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
                
                foreach(var item in messages)
                {
                    observer.OnNext(item);
                }
            }

            return new Unsubscriber<Message>(observers, observer);
        }

        //public void MessageStatus(Message msg)
        //{
        //    MessageStatus(msg);
        //}

        public void MessageStatus(Message msg)
        {

            if (!msg.Read)
            {
                messages.Add(msg);

                foreach(var observer in observers)
                {
                    observer.OnNext(msg);
                }
            }
            else
            {
                var messagesToRemove = new List<Message>();
                foreach(var message in messages)
                {
                    if (message.Read)
                    {
                        messagesToRemove.Add(message);
                        foreach (var observer in observers)
                            observer.OnNext(msg);
                    }
                }
                foreach (var messageToRemove in messagesToRemove)
                {
                    messages.Remove(messageToRemove);
                }
                messagesToRemove.Clear();
            }
        }

        public void LastSent()
        {
            foreach (var observer in observers)
            {
                observer.OnCompleted();
            }

            observers.Clear();
        }
    }

    internal class Unsubscriber<BaggageInfo> : IDisposable
    {
        private List<IObserver<Message>> _observers;
        private IObserver<Message> _observer;

        internal Unsubscriber(List<IObserver<Message>> observers, IObserver<Message> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
