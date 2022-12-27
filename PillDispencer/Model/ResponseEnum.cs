using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PillDispencer.Model
{
    public enum ResponseEnum
    {
        success = 200,
        created = 201,
        bad_request = 400,
        unauthorized = 401,
        forbidden = 403,
        not_found = 404,
        session_expired = 440,
        internal_server_error = 500,
        bad_gateway = 502,
        service_unavailable = 503,
    }
}
