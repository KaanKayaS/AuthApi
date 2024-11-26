using AuthServer.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext dbContext;

        public UnitOfWork(AppDbContext appDbContext)
        {
            dbContext = appDbContext; 
        }

        public void Save()
        {
           dbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
          await dbContext.SaveChangesAsync();
        }
    }
}
