using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebhostMySQLConnection.SchoologyAPI
{
    public interface ISchoologyCreatable
    {
        String Resource { get; set; }

        String ToCreateJson();
    }
}
