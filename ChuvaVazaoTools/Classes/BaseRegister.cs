using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compass.CommomLibrary
{

    public abstract class BaseDocument : ICloneable
    {

        public string File { get; set; }

        public abstract Dictionary<string, IBlock<BaseLine>> Blocos { get; }

        public string BottonComments { get; set; }

        public virtual string ToText()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var block in Blocos)
            {
                sb.Append(block.Value.ToText());
            }

            if (BottonComments != null)
            {
                sb.Append(BottonComments);
            }

            return sb.ToString();
        }

        public virtual void Load(string fileContent)
        {
            var lines = fileContent.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);


            string comments = null;
            foreach (var line in lines)
            {
                if (IsComment(line))
                {
                    comments = comments == null ? line : comments + Environment.NewLine + line;
                }
                else
                {
                    var cod = (line + "  ").Substring(0, 2);
                    if (line.Trim().StartsWith("UH") && line.Trim().EndsWith("NW"))
                    {
                        cod = "NW";
                    }
                    if (Blocos.Keys.Any(k => k.Split(' ').Contains(cod)))
                    {
                        var block = Blocos.First(k => k.Key.Split(' ').Contains(cod)).Value;
                        var newLine = block.CreateLine(line);

                        newLine.Comment = comments;
                        comments = null;
                        block.Add(newLine);
                    }
                }
            }

            if (comments != null)
            {
                BottonComments = comments;
            }
        }

        public static T Create<T>(string fileContent) where T : BaseDocument
        {

            var doc = Activator.CreateInstance<T>();
            doc.Load(fileContent);

            return doc;
        }

        public object Clone()
        {
            var clone = (BaseDocument)Activator.CreateInstance(this.GetType());

            clone.File = this.File;
            clone.BottonComments = this.BottonComments;


            foreach (var block in Blocos)
            {
                clone.Blocos[block.Key] = (IBlock<BaseLine>)block.Value.Clone();
            }
            return clone;
        }


        //public virtual void SaveToFile() {
        //    var text = ToText();
        //    System.IO.File.WriteAllText(File, text, Encoding.Default);
        //}


        public virtual void SaveToFile(string filePath = null, bool createBackup = false)
        {

            filePath = filePath ?? File;

            if (createBackup && System.IO.File.Exists(filePath))
            {
                var bkp = filePath + DateTime.Now.ToString("_yyyyMMddHHmmss.bak");
                System.IO.File.Copy(filePath, bkp);
            }


            var text = ToText();
            System.IO.File.WriteAllText(filePath, text, Encoding.Default);
            //SaveToFile(File);           
        }

        public virtual void SaveToFileDadgnlbkp(string filePath = null, bool createBackup = false)
        {

            filePath = filePath ?? File;

            if (createBackup && System.IO.File.Exists(filePath))
            {
                var bkp = filePath.Replace("DADGNL.", "OFICIAL_DADGNL.").Replace("dadgnl.", "OFICIAL_DADGNL.");
                System.IO.File.Copy(filePath, bkp, true);
            }


            var text = ToText();
            System.IO.File.WriteAllText(filePath, text, Encoding.Default);
            //SaveToFile(File);           
        }

        //public virtual void SaveToFile(string filePath) {
        //    var text = ToText();
        //    System.IO.File.WriteAllText(filePath, text, Encoding.Default);            
        //}

        public virtual bool IsComment(string line)
        {
            return false;
        }
    }

    public class DummyDocument : BaseDocument
    {
        byte[] fileContent;

        public DummyDocument(string filepath)
            : base()
        {
            fileContent = System.IO.File.ReadAllBytes(filepath);
        }

        public override void SaveToFile(string filePath = null, bool createBackup = false)
        {

            filePath = filePath ?? File;

            if (createBackup && System.IO.File.Exists(filePath))
            {
                var bkp = filePath + DateTime.Now.ToString("_yyyyMMddHHmmss.bak");
                System.IO.File.Copy(filePath, bkp);
            }
            System.IO.File.WriteAllBytes(filePath, fileContent);
        }

        public override Dictionary<string, IBlock<BaseLine>> Blocos
        {
            get { throw new NotImplementedException(); }
        }
    }

    public abstract class BaseBlockDocument<T> : BaseDocument, IList<T> where T : BaseLine
    {

        protected abstract BaseBlock<T> blockDocument { get; }

        //private BaseBlockDocument() { }

        //public BaseBlockDocument(BaseBlock<T> block) {
        //    this.block = block;
        //}

        public T CreateLine(string line)
        {
            return blockDocument.CreateLine(line);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.blockDocument.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.blockDocument.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return this.blockDocument.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.blockDocument.Insert(index, item);
        }

        public void InsertAfter(T prevItem, T itemToAdd)
        {
            Insert(IndexOf(prevItem) + 1, itemToAdd);
        }

        public void RemoveAt(int index)
        {
            this.blockDocument.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return this.blockDocument[index];
            }
            set
            {
                this.blockDocument[index] = value;
            }
        }

        public void Add(T item)
        {
            this.blockDocument.Add(item);
        }

        public void Clear()
        {
            this.blockDocument.Clear();
        }

        public bool Contains(T item)
        {
            return this.blockDocument.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.blockDocument.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.blockDocument.Count(); }
        }

        public bool IsReadOnly
        {
            get { return this.blockDocument.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return this.blockDocument.Remove(item);
        }

    }

    public interface IBlock<out T> : IEnumerable<T>, ICloneable where T : BaseLine
    {

        T CreateLine(string line = null);

        void Add(BaseLine newLine);

        //        void RVX();
        string ToText();


    }

    public abstract class BaseBlock<T> : IBlock<T>, IList<T> where T : BaseLine
    {

        List<T> lines = new List<T>();

        //        public virtual void RVX() { }

        public virtual T CreateLine(string line = null)
        {
            var newLine = BaseLine.Create<T>(line);
            return (T)newLine;
        }

        public virtual string ToText()
        {
            var result = new StringBuilder();

            foreach (var item in this)
            {
                result.AppendLine(item.ToText());
            }


            return result.ToString();
        }

        public string fictLine()
        {
            if (this.Any(x => x.FictLine.Trim().StartsWith("FICT.")))
            {
                return this.Where(x => x.FictLine.Trim().StartsWith("FICT.")).First().FictLine;
            }
            return "";
        }
        public IEnumerator<T> GetEnumerator()
        {
            return lines.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return lines.GetEnumerator();
        }

        public void Add(T item)
        {
            lines.Add((T)item);
        }

        public void Add(BaseLine newLine)
        {
            Add((T)newLine);
        }

        public void Clear()
        {
            lines.Clear();
        }

        public bool Contains(T item)
        {
            return lines.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lines.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return lines.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return lines.Remove(item);
        }

        public object Clone()
        {
            var clone = (BaseBlock<T>)Activator.CreateInstance(this.GetType());
            foreach (var l in this.lines)
            {
                clone.lines.Add((T)l.Clone());
            }

            return clone;
        }

        public int IndexOf(T item)
        {
            return lines.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lines.Insert(index, item);
        }

        public void InsertAfter(T prevItem, T itemToAdd)
        {
            Insert(IndexOf(prevItem) + 1, itemToAdd);
        }

        public void InsertBefore(T prevItem, T itemToAdd)
        {
            Insert(IndexOf(prevItem), itemToAdd);
        }

        public void RemoveAt(int index)
        {
            lines.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return lines[index];
            }
            set
            {
                lines[index] = value;
            }
        }
    }

    public abstract class BaseLine
    {

        public string Comment { get; set; }
        public string FictLine { get; set; }

        public virtual BaseField[] Campos { get; private set; }
        protected Dictionary<BaseField, dynamic> valores = new Dictionary<BaseField, dynamic>();
        public dynamic[] Valores
        {
            get
            {
                return valores.Select(x => x.Value).ToArray();
            }
        }

        public BaseLine()
        {
            if (Campos != null)
            {
                for (int i = 0; i < Campos.Length; i++)
                {
                    var campo = Campos[i];
                    valores.Add(campo, null);
                }
            }
        }

        public virtual void Load(string line)
        {
            if (line != null)
            {
                for (int i = 0; i < Campos.Length; i++)
                {
                    var campo = Campos[i];

                    if (campo.Tamanho < 1 || campo.Inicio < 1) continue;

                    SetValue(i, line.PadRight(campo.Fim).Substring((campo.Inicio - 1), campo.Tamanho));
                }
            }
        }

        internal static T Create<T>(string line = null) where T : BaseLine
        {
            var lineType = typeof(T);

            var result = (BaseLine)Activator.CreateInstance(lineType);

            result.Load(line);

            return (T)result;
        }

        public virtual string ToText()
        {
            var result = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(Comment)) result.AppendLine(Comment);

            int pos = 0;
            for (int i = 0; i < Campos.Length; i++)
            {

                var campo = Campos[i];
                if (campo.Inicio > 0)
                {
                    result.Append(new string(' ', (campo.Inicio - 1) - pos));
                    result.Append(campo.ConvertToString(valores[campo]));
                    pos = (campo.Fim);
                }
            }

            return result.ToString().TrimEnd();
        }

        public void SetValue(int index, dynamic value)
        {

            var field = Campos[index];
            var val = field.TryParse(value);
            valores[field] = val;
        }

        public dynamic this[string fieldName]
        {
            get
            {
                var key = GetField(fieldName);
                if (!valores.ContainsKey(key))
                    valores.Add(key, null);

                return valores[key];
            }
            set
            {
                var key = GetField(fieldName);
                if (!valores.ContainsKey(key))
                    valores.Add(key, value);
                else
                    valores[key] = value;
            }
        }

        public dynamic this[int index]
        {
            get
            {
                var key = GetField(index);
                if (!valores.ContainsKey(key))
                    valores.Add(key, null);

                return valores[GetField(index)];
            }
            set
            {
                var key = GetField(index);
                if (!valores.ContainsKey(key))
                    valores.Add(key, value);
                else
                    valores[key] = value;
            }
        }

        public dynamic this[BaseField field]
        {
            get
            {
                return valores[field];
            }
            set
            {
                valores[field] = value;
            }
        }


        BaseField GetField(string name)
        {
            return Campos.First(x => x.Nome.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        BaseField GetField(int index)
        {
            return Campos[index];
        }

        public BaseLine Clone()
        {
            Type t = this.GetType();
            var cloned = (BaseLine)Activator.CreateInstance(t);

            foreach (var field in this.Campos)
            {
                cloned.valores[field] = this.valores[field];
            }

            cloned.Comment = this.Comment;

            return cloned;
        }

    }

    public struct BaseField
    {
        int inicio;
        public int Inicio { get { return inicio; } }
        int fim;
        public int Fim { get { return fim; } }

        public int Tamanho { get { return fim - inicio + 1; } }

        bool leftAlign;
        public bool LeftAlign { get { return leftAlign; } }
        string formato;
        public string Formato { get { return formato; } set { formato = value; } }

        string nome;
        public string Nome { get { return nome; } }

        public BaseField(int inicio, int fim, string formato, string nome = "")
        {

            this.inicio = inicio;
            this.fim = fim;
            this.formato = formato;
            this.leftAlign = formato.StartsWith("A");
            this.nome = nome;

        }

        public string ConvertToString(dynamic value)
        {
            var Culture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            var style = System.Globalization.NumberStyles.Any;
            string strValue;
            if (value != null)
            {
                switch ((Formato + " ")[0])
                {
                    case 'F':
                    case 'f':

                        double fVal;
                        var fmt = Formato.Substring(1).Split('.');
                        int len, dec;

                        if (double.TryParse((string)value.ToString(), out fVal) &&
                            int.TryParse(fmt[0], out len) &&
                            int.TryParse(fmt[1], out dec)
                            )
                        {
                            if ((Formato + " ")[0] == 'f')
                            {
                                if (dec == 0)
                                {
                                    strValue = fVal.ToString("0.0", System.Globalization.NumberFormatInfo.InvariantInfo);
                                    strValue = strValue.Remove(strValue.Length - 1);
                                }
                                else
                                {
                                    strValue = fVal.ToString("0.0" + new string('#', dec - 1), System.Globalization.NumberFormatInfo.InvariantInfo);
                                }
                            }
                            else
                                if (dec == 0)
                            {
                                strValue = fVal.ToString("0.##", System.Globalization.NumberFormatInfo.InvariantInfo);
                            }
                            else
                            {
                                strValue = fVal.ToString("F" + dec.ToString(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            }
                           
                        }
                        else
                            strValue = value.ToString();
                        break;
                    case 'E':

                        double eVal;
                        var efmt = Formato.Substring(1).Split('.');
                        int elen, edec;

                        if (double.TryParse((string)value.ToString(), out eVal) &&
                            int.TryParse(efmt[0], out elen) &&
                            int.TryParse(efmt[1], out edec)
                            )
                        {
                            if (eVal >= 1 || eVal == 0)
                            {
                                if (edec == 0)
                                {
                                    strValue = eVal.ToString("0.0", System.Globalization.NumberFormatInfo.InvariantInfo);
                                    strValue = strValue.Remove(strValue.Length - 1);
                                }
                                else
                                    strValue = eVal.ToString("0.000" + ((edec > 3) ? new string('#', edec - 3) : ""), System.Globalization.NumberFormatInfo.InvariantInfo);
                            }
                            else
                            {
                                strValue = eVal.ToString("E" + edec.ToString(), System.Globalization.NumberFormatInfo.InvariantInfo)
                                    .Replace("E+0", "E+").Replace("E-0", "E-");
                            }
                        }
                        else
                            strValue = value.ToString();
                        break;
                    case 'Z':
                        int zVal;
                        int zLen;
                        var zFmt = Formato.Substring(1);
                        if (int.TryParse((string)value.ToString(), out zVal) &&
                            int.TryParse(zFmt, out zLen))
                        {
                            strValue = zVal.ToString().PadLeft(zLen, '0');
                        }
                        else
                            strValue = value.ToString();
                        break;
                    case 'I':
                        int iVal;
                        if (int.TryParse(value.ToString(), out iVal))
                        {
                            strValue = iVal.ToString();
                        }
                        else
                            strValue = value.ToString();
                        break;

                    default:
                        strValue = value.ToString();
                        break;
                }
            }
            else
                strValue = "";

            if (strValue.Length > Tamanho) strValue = strValue.Substring(0, Tamanho);
            else if (strValue.Length < Tamanho)
            {
                if (LeftAlign) strValue = strValue.PadRight(Tamanho);
                else strValue = strValue.PadLeft(Tamanho);
            }


            return strValue;
        }

        public dynamic TryParse(object value)
        {

            switch ((Formato.ToUpper() + " ")[0])
            {
                case 'I':
                case 'Z':
                    int i;
                    if (value == null)
                        return (int?)null;
                    else if (int.TryParse((string)value.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out i))
                    {
                        return i;
                    }
                    else
                    {
                        return (int?)null;// value;
                    }
                case 'F':
                case 'E':
                    double f;
                    if (value == null)
                        return (double?)null;
                    else if (double.TryParse((string)value.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out f))
                    {
                        return f;
                    }
                    else
                    {
                        return (double?)null;// value;
                    }
                case 'A':
                    return value != null ? (string)value.ToString() : (string)null;
                default:
                    return value;
            }
        }

        public dynamic TryParse(int value)
        {
            var f = (Formato + " ")[0];
            if (f == 'I' || f == 'Z')
            {
                return value;
            }
            else
            {
                return TryParse((object)value);
            }
        }


        public dynamic TryParse(float value)
        {
            return TryParse((double)value);
        }

        public dynamic TryParse(double value)
        {
            var f = (Formato.ToUpper() + " ")[0];
            if (f == 'I' || f == 'Z')
            {
                return (int)value;
            }
            else if (f == 'F' || f == 'E')
            {
                return value;
            }
            else
            {
                return TryParse((object)value);
            }
        }

        internal dynamic ExtractValue(byte[] regBytes)
        {

            dynamic result;
            switch (Formato[0])
            {
                case 'A':
                    result = Encoding.UTF8.GetString(regBytes, Inicio - 1, Tamanho); ;
                    break;
                case 'I':
                case 'Z':
                    result = BitConverter.ToInt32(regBytes, Inicio - 1);
                    break;
                case 'F':
                case 'f':
                    result = BitConverter.ToSingle(regBytes, Inicio - 1);
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        internal void InsertValue(byte[] destBytes, dynamic value)
        {

            byte[] result;
            if (string.IsNullOrWhiteSpace(Formato) || value == null)
                return;

            switch (Formato[0])
            {
                case 'A':
                    result = Encoding.UTF8.GetBytes((string)value.ToString());
                    for (int i = 0; i < result.Length; i++)
                    {
                        destBytes.SetValue(result[i], Inicio - 1 + i);
                    }
                    break;
                case 'I':
                case 'Z':
                    result = BitConverter.GetBytes((int)value);
                    for (int i = 0; i < result.Length; i++)
                    {
                        destBytes.SetValue(result[i], Inicio - 1 + i);
                    }
                    break;
                case 'F':
                case 'f':
                    result = BitConverter.GetBytes((float)value);
                    for (int i = 0; i < result.Length; i++)
                    {
                        destBytes.SetValue(result[i], Inicio - 1 + i);
                    }
                    break;
                default:
                    result = null;
                    break;
            }
        }

    }
}
