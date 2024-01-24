using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compass.CommomLibrary {
    public class DeckFile {

        public string BasePath {
            get { return System.IO.Path.Combine(BaseFolder, BaseFileName); }
            set {
                BaseFolder = System.IO.Path.GetDirectoryName(value);
                BaseFileName = System.IO.Path.GetFileName(value);
            }
        }
        public string BaseFileName { get; set; }
        public string BaseFolder { get; set; }

        public string Path { get { return System.IO.Path.Combine(Folder, FileName); } }
        public string Folder { get; set; }
        public string FileName { get; set; }

        BaseDocument document = null;
        public BaseDocument Document {
            get {

                if (document == null) {
                    try {
                        document = DocumentFactory.Create(BasePath);
                    } finally { }
                }
                return document;
            }
            set { document = value; }
        }

        public DeckFile(string baseFile) {
            BasePath = baseFile;
            FileName = BaseFileName;
        }

        public void BackUp() {
            System.IO.File.Copy(BasePath, BasePath + DateTime.Now.ToString("_yyyyMMddHHmmss.bak") , true);
        }
    }
}
