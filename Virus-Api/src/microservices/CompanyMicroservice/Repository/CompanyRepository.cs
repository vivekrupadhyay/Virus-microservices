using CompanyMicroservice.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyMicroservice.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IMongoCollection<Company> company;
        public CompanyRepository(IMongoDatabase database)
        {
            company = database.GetCollection<Company>(Company.DocumentName);
        }
    }
}
