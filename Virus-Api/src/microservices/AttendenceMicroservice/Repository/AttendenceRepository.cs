using AttendenceMicroservice.Model;
using Microsoft.EntityFrameworkCore;
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
        public void ClockIn(Attendence model)
        {
             attendence.InsertOne(model);
             
        }
        public void ClockOut(Attendence model)
        {
            attendence.InsertOne(model);
           
        }
        public IEnumerable<Attendence> GetAll()
        {
            var query = attendence.AsQueryable<Attendence>().Select(c => c).ToList();
            return query;
            //return await attendence.Find(x => true).ToListAsync();
        }
        //public async Task<Attendence> GetAttendenceByUser(Guid userid)
        //{
        //    //return await attendence.Find(u => u.UserId == userid).FirstOrDefaultAsync();
        //    var filter = Builders<Attendence>.Filter.Eq("UserId", userid);
        //    return await attendence.Find(filter).FirstOrDefaultAsync();
        //}
    }
}
