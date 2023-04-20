//using MongoDB.Bson.Serialization.Attributes;
//using MongoDB.Bson;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.ComponentModel;

//namespace Fithub.Classes
//{
//    public class SocialManagement :INotifyPropertyChanged
//    {
//        private string _id;
//        private string _userId;
//        private string _username;
//        private string _profilePic;
//        private List<string> _friends;
//        private List<string> _frRecieved;
//        private List<string> _frSent;

//        [BsonId]
//        [BsonRepresentation(BsonType.ObjectId)]
//        public string Id { get => _id; set
//            {
//                _id = value;
//                RaisePropertyChanged(nameof(Id));
//             } 
//        }


//        [BsonElement("userId")]
//        public string UserId
//        {
//            get => _userId; set
//            {
//                _userId = value;
//                RaisePropertyChanged(nameof(UserId));
//            }
//        }

//        [BsonElement("username")]
//        public string Username
//        {
//            get => _username; set
//            {
//                _username = value;
//                RaisePropertyChanged(nameof(Username));
//            }
//        }

//        [BsonElement("profilePic")]
//        public string ProfilePic
//        {
//            get => _profilePic; set
//            {
//                _profilePic = value;
//                RaisePropertyChanged(nameof(ProfilePic));
//            }
//        }

//        [BsonElement("friends")]
//        public List<string> Friends
//        {
//            get => _friends; 
//            set
//            {
//                _friends = value;
//                RaisePropertyChanged(nameof(Friends));
//            }
//        }

//        [BsonElement("fRequestsSent")]
//        public List<string> FRequestsSent 
//        {
//            get => _frSent; 
//            set
//            {
//                _frSent = value;
//                RaisePropertyChanged(nameof(FRequestsSent));
//             } 
//        }

//        [BsonElement("fRequestsRecieved")]
//        public List<string> FRequestsRecieved { get => _frRecieved; set
//            {
//                _frRecieved = value;
//                RaisePropertyChanged(nameof(FRequestsRecieved));
//             } 
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public void RaisePropertyChanged(string propertyName)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//}
