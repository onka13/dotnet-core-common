using System.ComponentModel.DataAnnotations.Schema;

namespace CoreCommon.Data.EntityFrameworkBase.Models
{
    public class InformationSchemaColumn
    {
        [Column("TABLE_SCHEMA")]
        public string TableSchema { get; set; }
        
        [Column("TABLE_NAME")]
        public string TableName { get; set; }

        [Column("COLUMN_NAME")]
        public string ColumnName { get; set; }

        [Column("IS_NULLABLE")]
        public string IsNullable { get; set; }
        
        [Column("DATA_TYPE")]
        public string DataType { get; set; }

        [Column("CHARACTER_MAXIMUM_LENGTH")]
        public int? CharacterMaximumLength { get; set; }
        
        [Column("NUMERIC_PRECISION")]
        public byte? NumericPrecision { get; set; }
        
        [Column("NUMERIC_SCALE")]
        public int? NumericScale { get; set; }

        [Column("DATETIME_PRECISION")]
        public short? DateTimePrecision { get; set; }
    }
}
