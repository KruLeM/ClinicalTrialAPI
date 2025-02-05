using Microsoft.AspNetCore.Http;

namespace Infrastructure.DTOs
{
    public class UploadJsonFileRequestDTO
    {
        public IFormFile File { get; set; }
    }
}
