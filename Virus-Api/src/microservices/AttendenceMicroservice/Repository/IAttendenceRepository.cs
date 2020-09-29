using AttendenceMicroservice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceMicroservice.Repository
{
    public interface IAttendenceRepository
    {
        IEnumerable<Attendence> GetAll();
        void ClockIn(Attendence attendence);
        void ClockOut(Attendence model);

        //Attendence GetAttendenceByUser(Guid userid);
        //Task<IEnumerable<Attendence>> GetAll();
        //Task<Attendence> GetAttendenceByUser(Guid userid);
        //Task ClockIn(Attendence attendence);
        //Task ClockOut(Attendence attendence);
        //Task<bool> UpdatePrice(Attendence attendence);
        //Task<DeleteResult> RemoveProduct(string name);
        //Task<DeleteResult> RemoveAllProducts();
    }
}
