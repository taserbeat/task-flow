using Application.Services;
using Infrastructure.Extensions.Exceptions;

namespace Infrastructure.Services
{
    public class ExceptionService : IExceptionService
    {
        public bool IsUniqueConstraintViolation(Exception ex)
        {
            return ex.IsUniqueConstraintViolation();
        }

        public bool IsForeignKeyViolation(Exception ex)
        {
            return ex.IsForeignKeyViolation();
        }

        public bool IsNotNullViolation(Exception ex)
        {
            return ex.IsNotNullViolation();
        }
    }
}