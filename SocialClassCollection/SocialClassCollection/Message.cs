using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialClassCollection
{
    [Serializable]
    public class Message
    {
        public string Id { get; set;
        }
        public string UserId { get; set; }
        public string Text { get; set; }

        public string ToUser { get; set; }

        public bool Read { get; set; }

        public Message(string Text, string Username, string ToUser)
        {
            this.UserId = Username;
            this.Text = Text;
            this.ToUser = ToUser;
        }

        public Message(byte[] Image, string Username, string ToUser)
        {
            this.Image = Image;
            this.Text = Text;
            this.ToUser = ToUser;
        }

        public Message(string Text, byte[] Image, string UserId, string ToUser, bool read)
        {
            this.UserId = UserId;
            this.Image = Image;
            this.Text = Text;
            this.ToUser = ToUser;
            Read = read;
        }

        public Datatype Datatype { get; set; }

        public byte[] Image { get; set; }

        public Message() { }
    }

    public enum Datatype
    {
        Text,
        Image,
        Video
    }
}
