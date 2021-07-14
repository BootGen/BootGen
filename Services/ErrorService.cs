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
            e.TimeStamp = DateTime.Now;
            dbContext.AppErrors.Add(e);
            dbContext.SaveChanges();
        }

        public void LogException(Exception e, string info)
        {
            var appError = ConvertToAppError(e);
            appError.Info = info;
            dbContext.AppErrors.Add(appError);
            if (e.InnerException != null)
                dbContext.AppErrors.Add(ConvertToAppError(e.InnerException));
            dbContext.SaveChanges();
        }

        private static AppError ConvertToAppError(Exception e)
        {
            var frame = new StackTrace(e, true).GetFrame(0);
            return new AppError
            {
                Kind = "C#",
                Type = e.GetType().Name,
                LineNumber = frame.GetFileLineNumber(),
                ColumnNumber = frame.GetFileColumnNumber(),
                FileName = frame.GetFileName(),
                Message = e.Message,
                StackTrace = e.StackTrace,
                TimeStamp = DateTime.Now
            };
        }
    }
}
