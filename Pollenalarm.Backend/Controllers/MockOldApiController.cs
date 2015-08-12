using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace Pollenalarm.Backend.Controllers
{
    [RoutePrefix("api/old")]
    public class MockOldApiController : ApiController
    {
        [Route("pdf")]
        public HttpResponseMessage GetFromHexalPdf(string zip)
        {
            var res = Request.CreateResponse(HttpStatusCode.OK);
            var uri = new Uri("http://www.allergie.hexal.de/pollenflug/vorhersage/pdf_create.php?plz=" + zip);

            try
            {
                var pdfContent = ExtractTextFromPdf(uri);
                var resultXml = ExtractPollenInformationFromPdfContent(pdfContent);
                res.Content = new StringContent(resultXml.ToString(), Encoding.UTF8, "text/xml");
            }
            catch (Exception ex)
            {
                res.Content = new StringContent("Error", Encoding.UTF8, "text/xml");
            }

            return res;
        }

        [Route("GetMapConcentration")]
        public HttpResponseMessage GetMapConcentration()
        {
            var res = Request.CreateResponse(HttpStatusCode.OK);

            var xmlFile = new XDocument();
            xmlFile.Add(new XElement("pollenbelastung"));
            xmlFile.Root.Add(GetMaxConcentration("18055", "Rostock"));
		    xmlFile.Root.Add(GetMaxConcentration("20095", "Hamburg"));
		    xmlFile.Root.Add(GetMaxConcentration("10115", "Berlin"));
		    xmlFile.Root.Add(GetMaxConcentration("30159", "Hannover"));
		    xmlFile.Root.Add(GetMaxConcentration("01067", "Dresden"));
		    xmlFile.Root.Add(GetMaxConcentration("53111", "Bonn"));
		    xmlFile.Root.Add(GetMaxConcentration("60311", "Frankfurt"));
		    xmlFile.Root.Add(GetMaxConcentration("90402", "Nürnberg"));
		    xmlFile.Root.Add(GetMaxConcentration("66111", "Saarbrücken"));
		    xmlFile.Root.Add(GetMaxConcentration("70173", "Stuttgart"));
		    xmlFile.Root.Add(GetMaxConcentration("80331", "München"));

            res.Content = new StringContent(xmlFile.ToString(), Encoding.UTF8, "text/xml");
            return res;
        }

        [Route("GetGeneralInformation")]
        public HttpResponseMessage GetGeneralInformation()
        {
            var res = Request.CreateResponse(HttpStatusCode.OK);

            var xmlFile = new XDocument();
            xmlFile.Add(new XElement("generalInformation"));

            var xInfos = new XElement("information");
            var xDay = new XElement("day");
            var xText = new XElement("text");

            xInfos.Add(xDay);
            xInfos.Add(xText);
            xmlFile.Root.Add(xInfos);

            var uri = new Uri("http://www.allergie.hexal.de/pollenflug/vorhersage/pdf_create.php?plz=40764");
            try
            {
                var pdfContent = ExtractTextFromPdf(uri);
                pdfContent = pdfContent.Substring(pdfContent.IndexOf("Für Deutschland:") + 17, (pdfContent.IndexOf("Für Postleitzahl") - (pdfContent.IndexOf("Für Deutschland:") + 17)));
                xText.Add(pdfContent);
            }
            catch (Exception)
            {
                xText.Add("Derzeit sind keine weiteren Informationen vorhanden. Wir wünschen eine Pollenfreie Zeit!");
            }


            res.Content = new StringContent(xmlFile.ToString(), Encoding.UTF8, "text/xml");
            return res;
        }

        private XDocument ExtractPollenInformationFromPdfContent(string pdfContent)
        {
            XDocument xmlFile = new XDocument();
            xmlFile.Add(new XElement("polleninformation"));

            pdfContent = pdfContent.Substring(pdfContent.IndexOf("Ambrosia"), (pdfContent.IndexOf("Stand") - (pdfContent.IndexOf("Ambrosia") + 0)));
            var pollutionTable = pdfContent.Split('\n');

            for (int i = 0; i < 3; i++)
            {
                try
                {


                    XElement day = new XElement("day");
                    day.Add(new XAttribute("name", DateTime.Now.AddDays(i).ToString("dddd, dd.MM.yyyy")));

                    foreach (var pollutionString in pollutionTable)
                    {

                        XElement xmlPollution = new XElement("pollen");
                        var trimmedPollutionString = pollutionString.Trim();
                        if (trimmedPollutionString.Length <= 1)
                            continue;

                        var data = trimmedPollutionString.Split(' ');

                        xmlPollution.Add(new XElement("name", data[0]));
                        xmlPollution.Add(new XElement("concentration", data[1 + i]));

                        day.Add(xmlPollution);
                    }

                    xmlFile.Root.Add(day);
                }
                catch (Exception)
                {

                }
            }

            return xmlFile;
        }

        private string ExtractTextFromPdf(Uri path)
        {
            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }

                return text.ToString();
            }
        }


        private XElement GetMaxConcentration(string zip, string name)
        {
            var uri = new Uri("http://www.allergie.hexal.de/pollenflug/vorhersage/pdf_create.php?plz=" + zip);

            var xPlace = new XElement("ort");
            var xName = new XElement("name");
            xName.Add(name);
            var xZip = new XElement("plz");
            xZip.Add(zip);
            var xPollution = new XElement("belastung");

            try
            {
                var pdfContent = ExtractTextFromPdf(uri);
                pdfContent = pdfContent.Substring(pdfContent.IndexOf("Ambrosia"),
                    (pdfContent.IndexOf("Stand") - (pdfContent.IndexOf("Ambrosia") + 0)));

                if (pdfContent.Contains("stark"))
                    xPollution.Add(3);
                else if (pdfContent.Contains("mäßig"))
                    xPollution.Add(2);
                else if (pdfContent.Contains("schwach"))
                    xPollution.Add(1);
                else
                    xPollution.Add(0);
            }
            catch (Exception)
            {
                xPollution.Add(0);
            }

            xPlace.Add(xName);
            xPlace.Add(xZip);
            xPlace.Add(xPollution);

            return xPlace;
        }




    }
}
