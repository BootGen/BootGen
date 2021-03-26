using System;
using System.Diagnostics;

namespace Editor.Services
{
    public class ErrorService : IErrorService
    {
        private readonly ApplicationDbContext dbContext;

        public ErrorService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void LogError(AppError e)
        {
            dbContext.AppErrors.Add(e);
            dbContext.SaveChanges();
        }

        public void LogException(Exception e)
        {
            var frame = new StackTrace(e, true).GetFrame(0);
            dbContext.AppErrors.Add(new AppError
            {
                Kind = "C#",
                Type = e.GetType().Name,
                LineNumber = frame.GetFileLineNumber(),
                ColumnNumber = frame.GetFileColumnNumber(),
                FileName = frame.GetFileName(),
                Message = e.Message,
                StackTrace = e.StackTrace
            });
            dbContext.SaveChanges();
        }
    }
}
