using System;
using System.Collections.Generic;
using pdfforge.PDFCreator.Core.Jobs;

namespace PDFCreator.Core.UnitTest.Mocks
{
    class MockJobInfo : IJobInfo
    {
        private IList<SourceFileInfo> _sourceFiles = new List<SourceFileInfo>();
        public string InfFile { get; set; }

        public IList<SourceFileInfo> SourceFiles
        {
            get { return _sourceFiles; }
            set { _sourceFiles = value; }
        }

        public Metadata Metadata
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public JobType JobType
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void SaveInf()
        {
            throw new NotImplementedException();
        }

        public void Delete(bool includeSourceFiles)
        {
            throw new NotImplementedException();
        }

        public void Merge(IJobInfo jobInfo)
        {
            throw new NotImplementedException();
        }
    }
}
