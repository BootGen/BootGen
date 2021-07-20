using System;

namespace Editor.Services
{
    public interface IErrorService
    {
        void LogException(Exception e, string info = null);
        void LogError(AppError e);
    }
}
