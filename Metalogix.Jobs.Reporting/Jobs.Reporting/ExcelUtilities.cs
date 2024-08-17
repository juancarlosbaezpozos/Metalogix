using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Metalogix.Jobs.Reporting
{
	internal static class ExcelUtilities
	{
		internal const int ExcelMaxCellSize = 32767;

	    internal static Cell CreateDateCell(DateTime dateTime, string column, UInt32Value row, UInt32Value styleId)
	    {
	        double num;
	        try
	        {
	            num = dateTime.ToOADate();
	        }
	        catch (OverflowException)
	        {
	            num = 0.0;
	        }
	        return new Cell
	        {
	            CellReference = column + row,
	            StyleIndex = styleId,
	            DataType = CellValues.Number,
	            CellValue = new CellValue(num.ToString(CultureInfo.InvariantCulture))
	        };
	    }

        internal static Cell CreateDateCell(TimeSpan duration, string column, UInt32Value row, UInt32Value styleId)
		{
			DateTime dateTime = new DateTime() + duration;
			return ExcelUtilities.CreateDateCell(dateTime, column, row, styleId);
		}

	    internal static Cell CreateInlineStringCell(string str, string column, UInt32Value row, UInt32Value styleId)
	    {
	        if (string.IsNullOrEmpty(str))
	        {
	            return new Cell
	            {
	                CellReference = column + row,
	                StyleIndex = styleId
	            };
	        }
	        if (str.Length > 32767)
	        {
	            str = str.Remove(32766);
	        }
	        Cell cell = new Cell
	        {
	            CellReference = column + row,
	            StyleIndex = styleId,
	            DataType = CellValues.InlineString
	        };
	        Text newChild = new Text(str);
	        InlineString inlineString = new InlineString();
	        inlineString.AppendChild<Text>(newChild);
	        cell.AppendChild<InlineString>(inlineString);
	        return cell;
	    }

	    internal static Cell CreateNumberCell(long value, string column, UInt32Value row, UInt32Value styleId)
	    {
	        return new Cell
	        {
	            CellReference = column + row,
	            StyleIndex = styleId,
	            DataType = CellValues.Number,
	            CellValue = new CellValue(value.ToString(CultureInfo.InvariantCulture))
	        };
	    }

	    internal static Cell CreateSharedStringCell(SharedStringTablePart sharedStringPart, string str, string column, UInt32Value row, UInt32Value styleId)
	    {
	        if (string.IsNullOrEmpty(str))
	        {
	            return new Cell
	            {
	                CellReference = column + row,
	                StyleIndex = styleId
	            };
	        }
	        if (str.Length > 32767)
	        {
	            str = str.Remove(32766);
	        }
	        return new Cell
	        {
	            CellReference = column + row,
	            StyleIndex = styleId,
	            DataType = CellValues.SharedString,
	            CellValue = new CellValue(ExcelUtilities.InsertSharedStringItem(str, sharedStringPart).ToString(CultureInfo.InvariantCulture))
	        };
	    }

        internal static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
		{
			int num;
			int num1 = 0;
			if (shareStringPart.SharedStringTable == null)
			{
				shareStringPart.SharedStringTable = new SharedStringTable();
			}
			using (IEnumerator<SharedStringItem> enumerator = shareStringPart.SharedStringTable.Elements<SharedStringItem>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.InnerText != text)
					{
						num1++;
					}
					else
					{
						num = num1;
						return num;
					}
				}
				SharedStringTable sharedStringTable = shareStringPart.SharedStringTable;
				OpenXmlElement[] openXmlElementArrays = new OpenXmlElement[] { new Text(text) };
				sharedStringTable.AppendChild<SharedStringItem>(new SharedStringItem(openXmlElementArrays));
				shareStringPart.SharedStringTable.Save();
				return num1;
			}
			return num;
		}
	}
}