using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvHelper
{

    /// <summary>
    /// Class to hold csv data
    /// </summary>
    [Serializable]
    public sealed class CsvFile
    {

        #region Properties

      /// <summary>
      /// 存储标题
      /// </summary>
        public readonly List<string> Headers = new List<string>();

       /// <summary>
       /// 存储记录
       /// </summary>
        public readonly CsvRecords Records = new CsvRecords();

       /// <summary>
       /// 列数
       /// </summary>
        public int HeaderCount
        {
            get
            {
                return Headers.Count;
            }
        }

        /// <summary>
        /// 行数（不包括标题）
        /// </summary>
        public int RecordCount
        {   
            get
            {
                return Records.Count;   
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// 获得指定行数的记录
        /// </summary>
        /// <param name="recordIndex"></param>
        /// <returns></returns>
        public CsvRecord this[int recordIndex]
        {
            get
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                return Records[recordIndex];
            }
        }

        /// <summary>
        /// 获得或设置指定行指定列的字段值
        /// </summary>
        /// <param name="recordIndex"></param>
        /// <param name="fieldIndex"></param>
        /// <returns></returns>
        public string this[int recordIndex, int fieldIndex]
        {
            get
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = Records[recordIndex];
                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0} in record {1}.", fieldIndex, recordIndex));

                return record.Fields[fieldIndex];
            }
            set
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = Records[recordIndex];

                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0}.", fieldIndex));

                record.Fields[fieldIndex] = value;
            }
        }

        /// <summary>
        /// 获得或设置指定行指定字段名的字段值
        /// </summary>
        /// <param name="recordIndex"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string this[int recordIndex, string fieldName]
        {
            get
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = Records[recordIndex];

                int fieldIndex = -1;

                for (int i = 0; i < Headers.Count; i++)
                {
                    if (string.Compare(Headers[i], fieldName) != 0) 
                        continue;

                    fieldIndex = i;
                    break;
                }

                if (fieldIndex == -1)
                    throw new ArgumentException(string.Format("There is no field header with the name '{0}'", fieldName));

                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0} in record {1}.", fieldIndex, recordIndex));

                return record.Fields[fieldIndex];
            }
            set
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = Records[recordIndex];

                int fieldIndex = -1;

                for (int i = 0; i < Headers.Count; i++)
                {
                    if (string.Compare(Headers[i], fieldName) != 0)
                        continue;

                    fieldIndex = i;
                    break;
                }

                if (fieldIndex == -1)
                    throw new ArgumentException(string.Format("There is no field header with the name '{0}'", fieldName));

                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0} in record {1}.", fieldIndex, recordIndex));

                record.Fields[fieldIndex] = value;
            }
        }

        #endregion Indexers

        #region Methods

        public void Populate(string filePath, bool hasHeaderRow)
        {
            Populate(filePath, null, hasHeaderRow, false);
        }


        public void Populate(string filePath, bool hasHeaderRow, bool trimColumns)
        {
            Populate(filePath, null, hasHeaderRow, trimColumns);
        }


        public void Populate(string filePath, Encoding encoding, bool hasHeaderRow, bool trimColumns)
        {
            using (CsvReader reader = new CsvReader(filePath, encoding){HasHeaderRow = hasHeaderRow, TrimColumns = trimColumns})
            {
                PopulateCsvFile(reader);
            }
        }

        public void Populate(Stream stream, bool hasHeaderRow)
        {
            Populate(stream, null, hasHeaderRow, false);
        }


        public void Populate(Stream stream, bool hasHeaderRow, bool trimColumns)
        {
            Populate(stream, null, hasHeaderRow, trimColumns);
        }

        public void Populate(Stream stream, Encoding encoding, bool hasHeaderRow, bool trimColumns)
        {
            using (CsvReader reader = new CsvReader(stream, encoding){HasHeaderRow = hasHeaderRow, TrimColumns = trimColumns})
            {
                PopulateCsvFile(reader);
            }
        }

        public void Populate(bool hasHeaderRow, string csvContent)
        {
            Populate(hasHeaderRow, csvContent, null, false);
        }

        public void Populate(bool hasHeaderRow, string csvContent, bool trimColumns)
        {
            Populate(hasHeaderRow, csvContent, null, trimColumns);
        }

        /// <summary>
        /// 使用Csv字符串填充CSV文件字段
        /// </summary>
        /// <param name="hasHeaderRow">true表示有标题行</param>
        /// <param name="csvContent"></param>
        /// <param name="encoding"></param>
        /// <param name="trimColumns">true自动去除字段值两端的空白符</param>
        public void Populate(bool hasHeaderRow, string csvContent, Encoding encoding, bool trimColumns)
        {
            using (CsvReader reader = new CsvReader(encoding, csvContent){HasHeaderRow = hasHeaderRow, TrimColumns = trimColumns})
            {
                PopulateCsvFile(reader);
            }
        }

        /// <summary>
        /// 使用CsvReader实例填充CSV文件字段
        /// </summary>
        /// <param name="reader"></param>
        private void PopulateCsvFile(CsvReader reader)
        {
            Headers.Clear();
            Records.Clear();

            bool addedHeader = false;

            while (reader.ReadNextRecord())
            {
                if (reader.HasHeaderRow && !addedHeader)
                {
                    reader.Fields.ForEach(field => Headers.Add(field));
                    addedHeader = true;
                    continue;
                }

                CsvRecord record = new CsvRecord();
                reader.Fields.ForEach(field => record.Fields.Add(field));
                Records.Add(record);
            }
        }

        #endregion Methods

    }

    /// <summary>
    /// Class for a collection of CsvRecord objects
    /// </summary>
    [Serializable]
    public sealed class CsvRecords : List<CsvRecord>
    {  
    }

    /// <summary>
    /// Csv record class
    /// </summary>
    [Serializable]
    public sealed class CsvRecord
    {
        #region Properties

        /// <summary>
        /// Gets the Fields in the record
        /// </summary>
        public readonly List<string> Fields = new List<string>();

        /// <summary>
        /// Gets the number of fields in the record
        /// </summary>
        public int FieldCount
        {
            get
            {
                return Fields.Count;
            }
        }

        #endregion Properties
    }
}
