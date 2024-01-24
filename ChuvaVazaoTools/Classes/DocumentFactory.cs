using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compass.CommomLibrary
{
    public class DocumentFactory
    {

        public static BaseDocument Create(string filePath)
        {

            var fileName = System.IO.Path.GetFileName(filePath).ToLowerInvariant();
            BaseDocument doc;

            if (fileName.StartsWith("dadvaz"))
            {
                doc = BaseDocument.Create<Dadvaz.Dadvaz>(System.IO.File.ReadAllText(filePath, Encoding.Default));
            }

            //else if (fileName.StartsWith("dadvaz"))
            //{
            //    doc = BaseDocument.Create<Dadvaz.Dadvaz>(System.IO.File.ReadAllText(filePath, Encoding.Default));
            //}
            
            else
            {
                doc = new Compass.CommomLibrary.DummyDocument(filePath);
                //throw new ArgumentException("Tipo de arquivo não preparado para leitura");
            }

            doc.File = filePath;

            return doc;

        }

    }
}
