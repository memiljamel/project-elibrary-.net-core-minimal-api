using System.Net;
using System.Text.Json.Serialization;

namespace ELibrary.Models
{
    public class WebResponse<T>
    {
        public HttpStatusCode Code { get; set; }

        public string Status { get; set; }

        public T? Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public MetaResponse Meta { get; set; }
    }
}