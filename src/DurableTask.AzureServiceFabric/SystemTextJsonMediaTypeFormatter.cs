//  ----------------------------------------------------------------------------------
//  Copyright Microsoft Corporation
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  ----------------------------------------------------------------------------------

namespace DurableTask.AzureServiceFabric
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SystemTextJsonMediaTypeFormatter : MediaTypeFormatter
    {
        private readonly JsonSerializerOptions _options;

        public SystemTextJsonMediaTypeFormatter(JsonSerializerOptions options)
            => _options = options;

        public override bool CanReadType(Type type)
            => true;

        public override bool CanWriteType(Type type)
            => true;

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
            => this; // ReadFromJsonAsync should be able to handle other stream encodings from the HttpContent

        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger, CancellationToken cancellationToken)
        {
            try
            {
                return await content.ReadFromJsonAsync(type, _options, cancellationToken);
            }
            catch (JsonException ex)
            {
                formatterLogger.LogError(ex.Path, ex);
                throw;
            }
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext, CancellationToken cancellationToken)
        {
            // TODO: Write with other encodings?
            return JsonSerializer.SerializeAsync(writeStream, value, type, _options, cancellationToken);
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            headers.ContentType = new MediaTypeHeaderValue("application/json");
            headers.ContentEncoding.Add(Encoding.UTF8.WebName);
        }
    }
}
