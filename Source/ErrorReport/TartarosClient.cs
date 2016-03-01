using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace pdfforge.PDFCreator.ErrorReport
{
    internal class TartarosClient
    {
        private const string TartarosUri = "https://tartaros.pdfforge.org/api/v1/errors";
        private const string ApiKey = "pe2de9KLvfs6gArNEAdLKGmw";


        public bool SendErrorReport(Report report)
        {
            var request = (HttpWebRequest)WebRequest.Create(TartarosUri);
            request.Method = "POST";
            request.ContentType = "application/json;utf-8";
            request.Accept = "application/json";
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
            request.Headers.Add(HttpRequestHeader.Authorization, ApiKey);

            using (Stream requestStream = request.GetRequestStream())
            using (TextWriter streamWriter = new StreamWriter(requestStream, Encoding.UTF8))
            {
                streamWriter.Write(ReportSerializer.ConvertToJson(report));
            }

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            }
            /*catch (WebException wex)
            {
                var httpResponse = wex.Response as HttpWebResponse;
                httpResponse.StatusCode ...
            }*/
            catch (Exception)
            {
                return false;
            }
        }
    }
}
