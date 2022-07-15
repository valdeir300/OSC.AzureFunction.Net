using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System.IO;
using System;
using System.Text;

namespace OSC.AzureFunction.v2
{
    public static class ConvertHTMLtoPDF
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string html = await req.Content.ReadAsStringAsync();

            byte[] pdf = BuildPdf(html);

            var res = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(pdf)
            };

            res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            res.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            return res;
        }

        public static byte[] BuildPdf(string html)
        {
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);
            WebKitConverterSettings settings = new WebKitConverterSettings();
            string htmlText = html;
            string baseUrl = string.Empty;
            settings.WebKitPath = "../../../QtBinaries";
            settings.Orientation = PdfPageOrientation.Portrait;
            htmlConverter.ConverterSettings = settings;

            PdfDocument document = htmlConverter.Convert(htmlText, baseUrl);
            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            document.Close(true);

            string base64 = Convert.ToBase64String(stream.ToArray());
            return Convert.FromBase64String(base64);
        }
    }
}
