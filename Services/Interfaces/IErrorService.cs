using System;

namespace Editor.Services
{
    public interface IErrorService
    {
        void LogException(Exception e);
        void LogError(AppError e);
    }
}
