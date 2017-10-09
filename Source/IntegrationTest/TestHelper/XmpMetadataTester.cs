using iTextSharp.text.pdf;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

namespace PDFCreator.TestUtilities
{
    public class XmpMetadataTester
    {
        public static void CheckForXMPMetadataUpdateStrings(Job job)
        {
            foreach (var file in job.OutputFiles)
                CheckForXMPMetadataUpdateStrings(file, job.JobInfo.Metadata, job.Profile.OutputFormat, job.Profile.PdfSettings.Security.Enabled, job.Passwords.PdfOwnerPassword);
        }

        public static void CheckForXMPMetadataUpdateStrings(string file, Metadata metadata, OutputFormat format, bool encryption, string ownerPassword)
        {
            if (encryption)
            {
                file = GetDecryptedFile(file, ownerPassword);
            }

            var content = File.ReadAllText(file);
            var regex = new Regex(@"(<.xpacket begin=[\s\S]*?<.xpacket end=.+>)");
            var matches = regex.Matches(content);

            string xmlString;
            Assert.DoesNotThrow(() => { xmlString = matches[matches.Count - 1].ToString(); },
                "Missing XMP Metadata starting and ending  with \'<?xpacket\'.");
            xmlString = matches[matches.Count - 1].ToString();

            //Check if xmp structure is valid
            var doc = new XmlDocument();
            doc.LoadXml(xmlString); //Throws XmlException
            doc.Schemas = new XmlSchemaSet();

            var reader = new PdfReader(file);

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("pdfaid", "http://www.aiim.org/pdfa/ns/id/");
            nsmgr.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
            nsmgr.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            nsmgr.AddNamespace("xmp", "http://ns.adobe.com/xap/1.0/");
            nsmgr.AddNamespace("xmpMM", "http://ns.adobe.com/xap/1.0/mm/");
            nsmgr.AddNamespace("pdf", "http://ns.adobe.com/pdf/1.3/");

            var node = doc.SelectSingleNode("//pdfaid:part", nsmgr);

            Assert.NotNull(node, "PDF/A id part is not set.");
            var part = format == OutputFormat.PdfA1B ? "1" : "2";
            Assert.AreEqual(part, node.InnerText.Trim(), "PDF/A id part is not set properly.");

            node = doc.SelectSingleNode("//pdfaid:conformance", nsmgr);
            Assert.NotNull(node, "PDF/A id conformance is not set.");
            Assert.AreEqual("B", node.InnerText.Trim(), "PDF/A id conformance is not set properly.");

            node = doc.SelectSingleNode("//xmp:CreateDate", nsmgr);
            Assert.NotNull(node, "Creation date is not set.");
            var expectedDate = PdfDate.Decode(reader.Info["CreationDate"]);
            var creationDate = DateTime.Parse(node.InnerText.Trim());
            Assert.AreEqual(expectedDate, creationDate, "Creation date is wrong.");

            node = doc.SelectSingleNode("//xmp:ModifyDate", nsmgr);
            Assert.NotNull(node, "Modify date is not set.");
            Assert.IsTrue(Regex.IsMatch(node.InnerText.Trim(), @"[1-2][0-9]{3}-[0-1][0-9]-[0-3][0-9]T[0-2][0-9]:[0-5][0-9]:[0-5][0-9]([\+\-Z][0-2][0-9]:[0-5][0-9]){0,1}"), "Modify date in wrong format.");
            //same as creation date???

            node = doc.SelectSingleNode("//dc:title/rdf:Alt/rdf:li", nsmgr);
            Assert.NotNull(node, "Title is not set.");
            Assert.AreEqual(metadata.Title, node.InnerText.Trim(), "Title is not set properly.");

            node = doc.SelectSingleNode("//dc:description/rdf:Alt/rdf:li", nsmgr);
            Assert.NotNull(node, "Description is not set.");
            Assert.AreEqual(metadata.Subject, node.InnerText.Trim(), "Description (Subject) is not set properly.");

            node = doc.SelectSingleNode("//dc:creator/rdf:Seq/rdf:li", nsmgr);
            Assert.NotNull(node, "Creator is not set.");
            Assert.AreEqual(metadata.Author, node.InnerText.Trim(), "Creator (Author) is not set properly.");

            node = doc.SelectSingleNode("//pdf:Producer", nsmgr);
            Assert.NotNull(node, "PDF producer is not set.");
            Assert.IsTrue(Regex.IsMatch(node.InnerText.Trim(), "PDFCreator [0-9].[0-9].[0-9].[0-9].*"), "PDF producer is not PDFCreator X.X.X.X.");

            node = doc.SelectSingleNode("//pdf:Keywords", nsmgr);
            Assert.NotNull(node, "PDF Keywords are not set.");
            Assert.AreEqual(metadata.Keywords, node.InnerText.Trim(), "Keywords are not set properly.");

            /*
            node = doc.SelectSingleNode("//xmpMM:DocumentID", nsmgr);
            Assert.NotNull(node, "Document ID is not set.");
            Assert.IsTrue(Regex.IsMatch(node.InnerText.Trim(), @"uuid:[0-9A-Za-z\-]{32,}"), "Document ID in wrong format.");
            //*/
        }

        private void ValidXSD(string xmlString)
        {
            // Set the validation settings.
            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += ValidationCallBack;

            //settings.Schemas.Add("pdfaid", "http://www.aiim.org/pdfa/ns/id/"); ///////////////////////////////////////////////////////////////////////
            //settings.Schemas.Add("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#"); ///////////////////////////////
            //settings.Schemas.Add("dc", "http://purl.org/dc/elements/1.1/"); ///////////////////////////////
            //settings.Schemas.Add("xmp", "http://ns.adobe.com/xap/1.0/"); ///////////////////////////////
            //settings.Schemas.Add("xmpMM", "http://ns.adobe.com/xap/1.0/mm/"); ///////////////////////////////
            //settings.Schemas.Add("pdf", "http://ns.adobe.com/pdf/1.3/"); ///////////////////////////////

            // Create the XmlReader object.
            //var reader = XmlReader.Create("inlineSchema.xml", settings);
            var xmlStream = new MemoryStream();
            var writer = new StreamWriter(xmlStream);
            writer.Write(xmlString);
            writer.Flush();
            xmlStream.Position = 0;

            var reader = XmlReader.Create(xmlStream, settings);

            // Parse the file.
            while (reader.Read())
            {
            }
        }

        // Display any warnings or errors.
        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Assert.Fail("Warning: Matching schema not found. No validation occurred. (Line " + args.Exception.LineNumber + ")\r\n" + args.Message);
            else
                Assert.Fail("Validation error in line " + args.Exception.LineNumber + ":\r\n" + args.Message);
        }

        private static string GetDecryptedFile(string file, string ownerPassword)
        {
            var decryptedFile = file.Replace(".pdf", "_decrypted.pdf");
            byte[] ownerPasswordBytes = null;
            if (!string.IsNullOrWhiteSpace(ownerPassword))
                ownerPasswordBytes = Encoding.Default.GetBytes(ownerPassword);

            var reader = new PdfReader(file, ownerPasswordBytes);
            var fileStream = new FileStream(decryptedFile, FileMode.Create, FileAccess.Write);
            var stamper = new PdfStamper(reader, fileStream);

            stamper.Close();

            return decryptedFile;
        }
    }
}
