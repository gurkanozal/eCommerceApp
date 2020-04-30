using System;
using System.Threading;
using System.Threading.Tasks;
using Cart.Domain.Exception;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cart.Application
{
    public class ExceptionLoggerPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<ExceptionLoggerPipelineBehaviour<TRequest, TResponse>> _logger;

        public ExceptionLoggerPipelineBehaviour(ILogger<ExceptionLoggerPipelineBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (RequestValidationException exception)
            {
                _logger.LogWarning(exception, exception.Message);
                throw;
            }
            catch (DomainException exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                throw;
            }
        }
    }
}