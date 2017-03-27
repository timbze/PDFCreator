using System;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.ITextProcessing;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace PDFCreator.TestUtilities
{
    public static class SigningTester
    {
        public static void TestSignature(Job job)
        {
            foreach (var file in job.OutputFiles)
            {
                TestSignature(file, job.Profile, job.Passwords);
            }
        }

        private static PdfReader BuildPdfReader(string testFile, ConversionProfile profile, JobPasswords passwords)
        {
            PdfReader pdfReader;
            if (profile.PdfSettings.Security.Enabled)
            {
                if (profile.PdfSettings.Security.RequireUserPassword)
                    pdfReader = new PdfReader(testFile, Encoding.Default.GetBytes(passwords.PdfUserPassword));
                else
                    pdfReader = new PdfReader(testFile, Encoding.Default.GetBytes(passwords.PdfOwnerPassword));
            }
            else
            {
                pdfReader = new PdfReader(testFile);
            }

            return pdfReader;
        }

        public static void TestSignature(string testFile, ConversionProfile profile, JobPasswords passwords)
        {
            var pdfReader = BuildPdfReader(testFile, profile, passwords);

            var af = pdfReader.AcroFields;
            //Stop here if no Signing was requested 
            if (!profile.PdfSettings.Signature.Enabled)
            {
                Assert.AreEqual(0, af.GetSignatureNames().Count, "SignatureName(s) in unsigned file." + Environment.NewLine + "(" + testFile + ")");
                return;
            }
            //Proceed with checking the number of signatures
            Assert.AreEqual(1, af.GetSignatureNames().Count, "Number of SignatureNames must be 1" + Environment.NewLine + "(" + testFile + ")");

            var signatureName = af.GetSignatureNames()[0];
            var pk = af.VerifySignature(signatureName);

            Assert.IsTrue(pk.Verify(), "(Last) Signature is not valid.");
            Assert.IsTrue(af.SignatureCoversWholeDocument(signatureName), "(Last) signature does not cover whole document.");

            var ts = DateTime.Now.ToUniversalTime() - pk.TimeStampDate;
            Assert.IsTrue(Math.Abs(ts.TotalHours) < 1, "Time stamp has a difference bigger than 1h from now." + Environment.NewLine + "(" + testFile + ")");

            Assert.AreEqual(profile.PdfSettings.Signature.SignLocation, pk.Location ?? "", "Wrong SignLocation." + Environment.NewLine + "(" + testFile + ")");
            Assert.AreEqual(profile.PdfSettings.Signature.SignReason, pk.Reason ?? "", "Wrong SignReason." + Environment.NewLine + "(" + testFile + ")");

            if (profile.PdfSettings.Signature.DisplaySignatureInDocument)
            {
                switch (profile.PdfSettings.Signature.SignaturePage)
                {
                    case SignaturePage.FirstPage:
                        Assert.AreEqual(1, af.GetFieldPositions(signatureName)[0].page, "Signature is not on the first page." + Environment.NewLine + "(" + testFile + ")");
                        break;
                    case SignaturePage.CustomPage:
                        if (profile.PdfSettings.Signature.SignatureCustomPage > pdfReader.NumberOfPages)
                            Assert.AreEqual(pdfReader.NumberOfPages, af.GetFieldPositions(signatureName)[0].page, "Signature is not on the requested page." + Environment.NewLine + "(" + testFile + ")");
                        else
                            Assert.AreEqual(profile.PdfSettings.Signature.SignatureCustomPage, af.GetFieldPositions(signatureName)[0].page, "Signature is not on the requested page." + Environment.NewLine + "(" + testFile + ")");
                        break;
                    case SignaturePage.LastPage:
                        Assert.AreEqual(pdfReader.NumberOfPages, af.GetFieldPositions(signatureName)[0].page, "Signature is not on the last Page." + Environment.NewLine + "(" + testFile + ")");
                        break;
                }
                Assert.AreEqual(profile.PdfSettings.Signature.LeftX, (int) af.GetFieldPositions(signatureName)[0].position.GetLeft(0), "Wrong position for LeftX." + Environment.NewLine + "(" + testFile + ")");
                Assert.AreEqual(profile.PdfSettings.Signature.LeftY, (int) af.GetFieldPositions(signatureName)[0].position.GetBottom(0), "Wrong position for LeftY." + Environment.NewLine + "(" + testFile + ")");
                Assert.AreEqual(profile.PdfSettings.Signature.RightX, (int) af.GetFieldPositions(signatureName)[0].position.GetRight(0), "Wrong position for RightX." + Environment.NewLine + "(" + testFile + ")");
                Assert.AreEqual(profile.PdfSettings.Signature.RightY, (int) af.GetFieldPositions(signatureName)[0].position.GetTop(0), "Wrong position for RightY." + Environment.NewLine + "(" + testFile + ")");
            }
            else
            {
                Assert.AreEqual(0, (int) af.GetFieldPositions(signatureName)[0].position.GetLeft(0), "Wrong position for \"invisible\" signature." + Environment.NewLine + "(" + testFile + ")");
                Assert.AreEqual(0, (int) af.GetFieldPositions(signatureName)[0].position.GetBottom(0), "Wrong position for \"invisible\" signature." + Environment.NewLine + "(" + testFile + ")");
                Assert.AreEqual(0, (int) af.GetFieldPositions(signatureName)[0].position.GetRight(0), "Wrong position for \"invisible\" signature." + Environment.NewLine + "(" + testFile + ")");
                Assert.AreEqual(0, (int) af.GetFieldPositions(signatureName)[0].position.GetTop(0), "Wrong position for \"invisible\" signature." + Environment.NewLine + "(" + testFile + ")");
            }
        }

        public static string TestMultipleSigning(Job job, IPdfProcessor pdfProcessor)
        {
            return TestMultipleSigning(job.OutputFiles[0], job.Profile, job.Passwords, pdfProcessor);
        }

        public static string TestMultipleSigning(string testFile, ConversionProfile profile, JobPasswords passwords, IPdfProcessor pdfProcessor)
        {
            var pdfReader = BuildPdfReader(testFile, profile, passwords);

            var doubleSignedFile = testFile.Replace(".pdf", "_doubleSignedFile.pdf");
            var fileStream = new FileStream(doubleSignedFile, FileMode.Create, FileAccess.Write);
            var tempFile = testFile.Replace(".pdf", "_tempFile.pdf");

            var intendedPdfVersion = pdfProcessor.DeterminePdfVersion(profile);
            var pdfVersion = PdfWriter.VERSION_1_4;
            if (intendedPdfVersion == "1.5")
                pdfVersion = PdfWriter.VERSION_1_5;
            else if (intendedPdfVersion == "1.6")
                pdfVersion = PdfWriter.VERSION_1_6;
            else if (intendedPdfVersion == "1.7")
                pdfVersion = PdfWriter.VERSION_1_7;

            //Create Stamper in append mode
            var stamper = PdfStamper.CreateSignature(pdfReader, fileStream, pdfVersion, tempFile, true);

            profile.PdfSettings.Signature.SignaturePage = SignaturePage.LastPage;
            var signer = new ITextSigner();
            signer.SignPdfFile(stamper, profile, passwords);

            stamper.Close();

            var doubleSignedPdfReader = BuildPdfReader(doubleSignedFile, profile, passwords);
            var af = doubleSignedPdfReader.AcroFields;
            
            Assert.AreEqual(2, af.GetSignatureNames().Count, "Number of SignatureNames must be 2" + Environment.NewLine + "(" + testFile + ")");

            //There is currently no way for testing multisigning.
            //-> af.SignatureCoversWholeDocument(firstSignatureName) is always false, since a singature can't cover future signing
            //-> firstSignature.Verify() returns always true.
            /*
            var firstSignatureName = af.GetSignatureNames()[0];
            var firstSignature = af.VerifySignature(firstSignatureName);

            if (profile.PdfSettings.Signature.AllowMultiSigning)
            {
                Assert.IsTrue(firstSignature.Verify(), "First signature is invalid altough multi signing was enabled");
            }
            else
            {
                Assert.IsFalse(firstSignature.Verify(), "First signature is valid altough multi signing was disabled");
            }
            //*/

            return doubleSignedFile;
        }
    }
}