using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Delivery
{
    public class S8580CommentsVM : ViewModelBase
    {

        private string tanNumber, tanCategory, commentType, comment, userComment;

        public string TanNumber { get { return tanNumber; } set { SetProperty(ref tanNumber, value); } }
        public string TanCategory { get { return tanCategory; } set { SetProperty(ref tanCategory, value); } }
        public string CommentType { get { return commentType; } set { SetProperty(ref commentType, value); } }
        public string Comment { get { return comment; } set { SetProperty(ref comment, value); } }
        public string UserComment { get { return userComment; } set { SetProperty(ref userComment, value); } }
    }
}
