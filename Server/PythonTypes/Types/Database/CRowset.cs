using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using PythonTypes.Types.Primitives;

namespace PythonTypes.Types.Database
{
    public class CRowset
    {
        private const string TYPE_NAME = "dbutil.CRowset";
        public DBRowDescriptor Header { get; private set; }
        private PyList Columns { get; set; }
        private PyList Rows { get; set; }

        public CRowset(DBRowDescriptor descriptor)
        {
            this.Header = descriptor;
            this.Rows = new PyList ();
            
            this.PrepareColumnNames();
        }

        public CRowset(DBRowDescriptor descriptor, PyList rows)
        {
            this.Header = descriptor;
            this.Rows = rows;
            
            this.PrepareColumnNames();
        }

        private void PrepareColumnNames()
        {
            this.Columns = new PyList();
            
            foreach (DBRowDescriptor.Column column in this.Header.Columns)
                this.Columns.Add(column.Name);
        }
        
        public virtual PyPackedRow this[int index]
        {
            get { return this.Rows[index] as PyPackedRow; }
            set { this.Rows[index] = value; }
        }

        public void Add(PyPackedRow row)
        {
            this.Rows.Add(row);
        }
        
        public static implicit operator PyDataType(CRowset rowset)
        {
            PyDictionary keywords = new PyDictionary();

            keywords["header"] = rowset.Header;
            keywords["columns"] = rowset.Columns;
            
            return new PyObject(
                new PyObject.ObjectHeader(TYPE_NAME, null, keywords, true),
                rowset.Rows
            );
        }

        public static implicit operator CRowset(PyObject data)
        {
            if (data.Header.Type != TYPE_NAME)
                throw new InvalidDataException($"Expected PyObject of type {data}");

            DBRowDescriptor descriptor = data.Header.Dictionary["header"];

            return new CRowset(descriptor, data.List);
        }

        public static CRowset FromMySqlDataReader(MySqlDataReader reader)
        {
            DBRowDescriptor descriptor = DBRowDescriptor.FromMySqlReader(reader);
            CRowset rowset = new CRowset(descriptor);

            while (reader.Read() == true)
            {
                rowset.Add(PyPackedRow.FromMySqlDataReader(reader, descriptor));
            }

            return rowset;
        }
    }
}