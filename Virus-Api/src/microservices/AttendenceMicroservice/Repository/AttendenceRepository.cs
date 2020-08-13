using AttendenceMicroservice.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceMicroservice.Repository
{
    public class AttendenceRepository : IAttendenceRepository
    {
        private readonly IMongoCollection<Attendence> attendence;
        public AttendenceRepository(IMongoDatabase database)
        {
            attendence = database.GetCollection<Attendence>(Attendence.DocumentName);
        }
        public async Task ClockIn(Attendence model)
        {
            await attendence.InsertOneAsync(model);
        }
        public async Task ClockOut(Attendence model)
        {
            await attendence.InsertOneAsync(model);
        }
        public async Task<IEnumerable<Attendence>> GetAll()
        {
            return await attendence.Find(x => true).ToListAsync();
        }
        public async Task<Attendence> GetAttendenceByUser(Guid userid)
        {
            //return await attendence.Find(u => u.UserId == userid).FirstOrDefaultAsync();
            var filter = Builders<Attendence>.Filter.Eq("UserId", userid);
            return await attendence.Find(filter).FirstOrDefaultAsync();
        }
    }
}
