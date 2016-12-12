using System;
using System.Text;
using iTextSharp.text.pdf;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace PDFCreator.TestUtilities
{
    public static class SigningTester
    {
        public static void TestSignature(Job job, int numberOfSignatures = 1, bool allowMultisigning = false)
        {
            foreach (var file in job.OutputFiles)
            {
                TestSignature(file, job.Profile, job.Passwords, numberOfSignatures, allowMultisigning);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="testFile"></param>
        /// <param name="profile"></param>
        /// <param name="passwords"></param>
        /// <param name="numberOfSignatures"></param>
        /// <param name="allowMultisigning"></param>
        public static void TestSignature(string testFile, ConversionProfile profile, JobPasswords passwords, int numberOfSignatures, bool allowMultisigning)
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

            var af = pdfReader.AcroFields;
            //Stop here if no Signing was requested 
            if (!profile.PdfSettings.Signature.Enabled)
            {
                Assert.AreEqual(0, af.GetSignatureNames().Count, "SignatureName(s) in unsigned file." + Environment.NewLine + "(" + testFile + ")");
                return;
            }
            //Proceed with checking the number of signatures
            Assert.AreEqual(numberOfSignatures, af.GetSignatureNames().Count, "Number of SignatureNames must be " + numberOfSignatures + Environment.NewLine + "(" + testFile + ")");

            #region Verify the last or single signature in document, which must be always valid

            var signatureName = af.GetSignatureNames()[numberOfSignatures - 1];
            var pk = af.VerifySignature(signatureName);

            Assert.IsTrue(pk.Verify(), "(Last) Signature in document, is not valid.");
            Assert.IsTrue(af.SignatureCoversWholeDocument(signatureName), "(Last) signature in document, does not cover whole document.");

            var ts = DateTime.Now.ToUniversalTime() - pk.TimeStampDate;
            Assert.IsTrue(Math.Abs(ts.TotalHours) < 1, "Time stamp has a difference bigger than 1h from now." + Environment.NewLine + "(" + testFile + ")");

            Assert.AreEqual(profile.PdfSettings.Signature.SignLocation, pk.Location, "Wrong SignLocation." + Environment.NewLine + "(" + testFile + ")");
            Assert.AreEqual(profile.PdfSettings.Signature.SignReason, pk.Reason, "Wrong SignReason." + Environment.NewLine + "(" + testFile + ")");

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
                Assert.AreEqual(1, af.GetFieldPositions(signatureName)[0].page, "Wrong position for \"invisible\" signature." + Environment.NewLine + "(" + testFile + ")");
                Assert.AreEqual(0, (int) af.GetFieldPositions(signatureName)[0].position.GetLeft(0), "Wrong position for \"invisible\" signature." + Environment.NewLine + "(" + testFile + ")");
                Assert.AreEqual(0, (int) af.GetFieldPositions(signatureName)[0].position.GetBottom(0), "Wrong position for \"invisible\" signature." + Environment.NewLine + "(" + testFile + ")");
                Assert.AreEqual(0, (int) af.GetFieldPositions(signatureName)[0].position.GetRight(0), "Wrong position for \"invisible\" signature." + Environment.NewLine + "(" + testFile + ")");
                Assert.AreEqual(0, (int) af.GetFieldPositions(signatureName)[0].position.GetTop(0), "Wrong position for \"invisible\" signature." + Environment.NewLine + "(" + testFile + ")");
            }

            #endregion

            /*
            var stamper = new PdfStamper(pdfReader, );

            if (profile.PdfSettings.Signature.AllowMultiSigning)
                Assert.AreEqual(0, stamper.SignatureAppearance);
            else
                Assert.AreEqual(1, stamper.SignatureAppearance);

            stamper.Close();
            */

            /*
            //Check validity of previous signatures. They must be valid if multi signing is allowed, otherwise they must be invalid. 
            for (int i = 1; i < numberOfSignatures; i++)
            {
                String previousSignName = af.GetSignatureNames()[i-1];
                //PdfPKCS7 previousPk = af.VerifySignature(previousSignName);

                if (allowMultisigning)
                {
                    var sig = af.VerifySignature(previousSignName);
                    Assert.IsTrue(sig.Verify(), "");
                    Assert.IsTrue(af.SignatureCoversWholeDocument(previousSignName),
                        "Last or single signature in document, does not cover whole document , although multi signing was enabled.");
                }
                else
                {
                    var sig = af.VerifySignature(previousSignName);
                    Assert.IsFalse(sig.Verify(), "");
                    Assert.IsFalse(af.SignatureCoversWholeDocument(previousSignName),
                        "Last or single signature in document, covers whole document , although multi signing was disabled.");
                }
            }
            */
        }
    }
}