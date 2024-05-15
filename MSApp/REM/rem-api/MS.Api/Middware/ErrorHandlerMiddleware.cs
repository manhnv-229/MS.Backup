using Microsoft.AspNetCore.Http;
using MS.Core.Exceptions;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using MS.Core.DTOs;

namespace Ms.Api.Middware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try     
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseObject = new ResponseObject()
                {
                    DevMsg = error.Message,
                    UserMsg = MS.Core.Resource.Resource.Exception_Message_General,
                    errors = error.Data
                };
                switch (error)
                {
                    case WebException e:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        //FtpWebResponse newResponse = e.Response as FtpWebResponse;
                        //if (newResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                        //{
                        //    responseObject.UserMsg = "Tệp hoặc thư mục không tồn tại trên server files";
                        //}
                        //else
                        //{
                        //    responseObject.UserMsg = "Không thể thực hiện xóa các tệp, vui lòng thử lại.";
                        //}
                        break;
                    case MSException e:
                        // custom application error
                        response.StatusCode = (int)e.StatusCode;
                        responseObject.UserMsg = error.Message;
                        break;

                    case MSMySqlException e:
                        // custom application error
                        response.StatusCode = (int)e.StatusCode;
                        responseObject.UserMsg = error.Message;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case UnauthorizedException e:
                        // Unauthorized
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
#if DEBUG
                        Console.WriteLine(error.Message);
#endif
                        break;
                }
                
                var result = JsonSerializer      .Serialize(responseObject);
                await response.WriteAsync(result);
            }
        }
    }
}
