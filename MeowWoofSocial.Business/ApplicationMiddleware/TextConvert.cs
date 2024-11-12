using System.Text;

namespace MeowWoofSocial.Business.ApplicationMiddleware;

public class TextConvert
{
    public static string ConvertFromUnicodeEscape(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var output = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '\\' && i + 5 < input.Length && input[i + 1] == 'u')
            {
                string unicode = input.Substring(i + 2, 4);
                if (int.TryParse(unicode, System.Globalization.NumberStyles.HexNumber, null, out int code))
                {
                    output.Append((char)code);
                    i += 5;
                    continue;
                }
            }
            output.Append(input[i]);
        }

        return output.ToString();
    }

    public static string ConvertToUnicodeEscape(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var output = new StringBuilder();

        foreach (char c in input)
        {
            if (c >= 0x00C0 && c <= 0x024F || c >= 0x1E00 && c <= 0x1EFF)
            {
                output.Append($"\\u{(int)c:x4}");
            }
            else
            {
                output.Append(c);
            }
        }

        return output.ToString();
    }

    public static string ConvertToUnSign(string s)
    {
        string stFormD = s.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();

        // Loại bỏ các dấu
        for (int ich = 0; ich < stFormD.Length; ich++)
        {
            System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
            if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                sb.Append(stFormD[ich]);
            }
        }

        sb = sb.Replace('Đ', 'D');
        sb = sb.Replace('đ', 'd');

        sb = sb.Replace(' ', '-');

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

}