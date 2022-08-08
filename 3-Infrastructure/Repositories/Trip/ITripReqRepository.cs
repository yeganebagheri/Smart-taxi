using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Infrastructure.Repositories.Trip
{
    public interface ITripReqRepository
    {
        public Task InsertTripReq(Core.Entities.Trip_req user);
    }
}
