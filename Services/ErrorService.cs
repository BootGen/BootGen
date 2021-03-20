using System;

namespace Editor.Services
{
    public class ErrorService : IErrorService
    {
        public void LogException(Exception e)
        {
                using (var db = new DataContext())
                {
                    db.Errors.Add(new Error {
                        Message = e.Message,
                        StackTrace = e.StackTrace
                    });
                    db.SaveChanges();
                }
        }
    }
}
