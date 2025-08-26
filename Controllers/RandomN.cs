using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

namespace RandomNumberAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomN : ControllerBase
    {
        private static Random _random = new Random();

        [HttpGet("number")]
        public IActionResult GetRandomNumber([FromQuery] int? min, [FromQuery] int? max)
        {
            if (min.HasValue && max.HasValue)
            {
                if (min > max)
                {
                    return BadRequest("El valor de min no puede ser mayor que max.");
                }
                return Ok(_random.Next(min.Value, max.Value + 1));
            }
            return Ok(_random.Next());
        }

        [HttpGet("decimal")]
        public IActionResult GetRandomDecimal()
        {
            return Ok(_random.NextDouble());
        }

        [HttpGet("string")]
        public IActionResult GetRandomString([FromQuery] int length)
        {
            if (length < 1 || length > 1024)
            {
                return BadRequest("La longitud debe estar entre 1 y 1024.");
            }

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringBuilder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[_random.Next(chars.Length)]);
            }
            return Ok(stringBuilder.ToString());
        }

        [HttpPost("custom")]
        public IActionResult GetCustomRandom([FromBody] CustomRandomRequest request)
        {
            if (request.Type == "number")
            {
                if (request.Min > request.Max)
                {
                    return BadRequest("El valor de min no puede ser mayor que max.");
                }
                return Ok(_random.Next(request.Min, request.Max + 1));
            }
            else if (request.Type == "decimal")
            {
                double randomDecimal = Math.Round(_random.NextDouble(), request.Decimals);
                return Ok(randomDecimal);
            }
            else if (request.Type == "string")
            {
                if (request.Length < 1 || request.Length > 1024)
                {
                    return BadRequest("La longitud debe estar entre 1 y 1024.");
                }
                // Llama a GetRandomString y extrae el valor de la respuesta
                var stringResult = GetRandomString(request.Length);
                if (stringResult is OkObjectResult okResult)
                {
                    return Ok(okResult.Value);
                }
                return stringResult; // En caso de que haya un error
            }
            return BadRequest("Tipo no válido.");
        }
    }

    public class CustomRandomRequest
    {
        public required string Type { get; set; } // Marcado como required
        public int Min { get; set; }
        public int Max { get; set; }
        public int Decimals { get; set; } = 2; // Valor por defecto
        public int Length { get; set; } = 8; // Valor por defecto
    }
}
