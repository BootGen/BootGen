using System;

namespace Editor.Services
{
    public class ErrorService : IErrorService
    {
        private readonly ApplicationDbContext dbContext;

        public ErrorService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void LogException(Exception e)
        {
            dbContext.Errors.Add(new Error
            {
                Message = e.Message,
                StackTrace = e.StackTrace
            });
            dbContext.SaveChanges();
        }
    }
}
