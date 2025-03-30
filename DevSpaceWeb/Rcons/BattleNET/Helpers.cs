/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.3.4 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2018 by it's authors.                    *
 *  Some rights reserved. See license.txt, authors.txt.    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace BattleNET;

internal class Helpers
{
    public static string Hex2Ascii(string hexString)
    {
        int j = 0;
        byte[] tmp = new byte[(hexString.Length) / 2];
        for (int i = 0; i <= hexString.Length - 2; i += 2)
        {
            tmp[j] = (byte)Convert.ToChar(Int32.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber));

            j++;
        }
        return Bytes2String(tmp);
    }

    public static byte[] String2Bytes(string s)
    {
        return Encoding.GetEncoding(1252).GetBytes(s);
    }

    public static string Bytes2String(byte[] bytes)
    {
        return Encoding.GetEncoding(1252).GetString(bytes);
    }

    public static string Bytes2String(byte[] bytes, int index, int count)
    {
        return Encoding.UTF8.GetString(bytes, index, count);
    }

    public static string StringValueOf(Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes =
            (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes.Length > 0)
        {
            return attributes[0].Description;
        }
        else
        {
            return value.ToString();
        }
    }

    public static object EnumValueOf(string value, Type enumType)
    {
        string[] names = Enum.GetNames(enumType);
        foreach (string name in names)
        {
            if (StringValueOf((Enum)Enum.Parse(enumType, name)).Equals(value))
            {
                return Enum.Parse(enumType, name);
            }
        }

        throw new ArgumentException("The string is not a description or value of the specified enum.");
    }
}