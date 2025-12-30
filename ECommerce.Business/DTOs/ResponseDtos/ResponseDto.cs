using System;
using System.Text.Json.Serialization;

namespace ECommerce.Business.DTOs.ResponseDtos;
// Burada kullandığımız yapı Factory Design Pattern. Yani bu sınıf kendi tipinden nesneler yaratabilme yeteneğine sahip, bir fabrika gibi çalışıyor.
public class ResponseDto<T>
{
    public T? Data { get; set; }
    public string? Error { get; set; }
    public bool IsSucceed { get; set; }

    [JsonIgnore]
    public int StatusCode { get; set; }



    public static ResponseDto<T> Success(T? data, int statusCode)
    {
        return new ResponseDto<T>
        {
            Data = data,
            StatusCode = statusCode,
            IsSucceed = true
        };
    }


    public static ResponseDto<T> Success(int statusCode)
    {
        return new ResponseDto<T>
        {
            Data = default,
            StatusCode = statusCode,
            IsSucceed = true
        };
    }

    public static ResponseDto<T> Fail(string error, int statusCode)
    {
        return new ResponseDto<T>
        {
            Error = error,
            StatusCode = statusCode,
            IsSucceed = false
        };
    }
}

























/*

    new ResponseDto{
        Data=product,
        Error=null,
        IsSucceed=true,
        StatusCode=200
    }

    new ResponseDto{
        Data=null,
        Error="İlgili ürün bulunamadığı için silme işlemi gerçekleştirilemedi",
        IsSucceed=false
    }

*/
