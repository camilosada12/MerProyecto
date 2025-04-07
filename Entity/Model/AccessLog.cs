using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class AccessLog
    {
        public int Id { get; set; }
        public bool Action { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Status { get; set; }
        public string Details { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool isDelete { get; set; }
    }
}
