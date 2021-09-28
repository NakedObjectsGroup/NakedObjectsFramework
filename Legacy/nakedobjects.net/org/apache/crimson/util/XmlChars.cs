// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.util.XmlChars
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.apache.crimson.util
{
  public class XmlChars
  {
    private XmlChars()
    {
    }

    public static bool isChar(int ucs4char) => ucs4char >= 32 && ucs4char <= 55295 || ucs4char == 10 || ucs4char == 9 || ucs4char == 13 || ucs4char >= 57344 && ucs4char <= 65533 || ucs4char >= 65536 && ucs4char <= 1114111;

    public static bool isNameChar(char c)
    {
      if (XmlChars.isLetter2(c))
        return true;
      switch (c)
      {
        case '-':
        case '.':
        case ':':
        case '_':
          return true;
        case '>':
          return false;
        default:
          if (!XmlChars.isExtender(c))
            return false;
          goto case '-';
      }
    }

    public static bool isNCNameChar(char c) => c != ':' && XmlChars.isNameChar(c);

    public static bool isSpace(char c) => c == ' ' || c == '\t' || c == '\n' || c == '\r';

    public static bool isLetter(char c)
    {
      if (c >= 'a' && c <= 'z')
        return true;
      if (c == '/')
        return false;
      if (c >= 'A' && c <= 'Z')
        return true;
      switch (Character.getType(c))
      {
        case 1:
        case 2:
        case 3:
        case 5:
        case 10:
          return !XmlChars.isCompatibilityChar(c) && (c < '⃝' || c > '⃠');
        default:
          return c >= 'ʻ' && c <= 'ˁ' || c == 'ՙ' || c == 'ۥ' || c == 'ۦ';
      }
    }

    private static bool isCompatibilityChar(char c)
    {
      switch ((int) c >> 8 & (int) byte.MaxValue)
      {
        case 0:
          return c == 'ª' || c == 'µ' || c == 'º';
        case 1:
          return c >= 'Ĳ' && c <= 'ĳ' || c >= 'Ŀ' && c <= 'ŀ' || c == 'ŉ' || c == 'ſ' || c >= 'Ǆ' && c <= 'ǌ' || c >= 'Ǳ' && c <= 'ǳ';
        case 2:
          return c >= 'ʰ' && c <= 'ʸ' || c >= 'ˠ' && c <= 'ˤ';
        case 3:
          return c == 'ͺ';
        case 5:
          return c == 'և';
        case 14:
          return c >= 'ໜ' && c <= 'ໝ';
        case 17:
          return c == 'ᄁ' || c == 'ᄄ' || c == 'ᄈ' || c == 'ᄊ' || c == 'ᄍ' || c >= 'ᄓ' && c <= 'ᄻ' || c == 'ᄽ' || c == 'ᄿ' || c >= 'ᅁ' && c <= 'ᅋ' || c == 'ᅍ' || c == 'ᅏ' || c >= 'ᅑ' && c <= 'ᅓ' || c >= 'ᅖ' && c <= 'ᅘ' || c == 'ᅢ' || c == 'ᅤ' || c == 'ᅦ' || c == 'ᅨ' || c >= 'ᅪ' && c <= 'ᅬ' || c >= 'ᅯ' && c <= 'ᅱ' || c == 'ᅴ' || c >= 'ᅶ' && c <= 'ᆝ' || c >= 'ᆟ' && c <= 'ᆢ' || c >= 'ᆩ' && c <= 'ᆪ' || c >= 'ᆬ' && c <= 'ᆭ' || c >= 'ᆰ' && c <= 'ᆶ' || c == 'ᆹ' || c == 'ᆻ' || c >= 'ᇃ' && c <= 'ᇪ' || c >= 'ᇬ' && c <= 'ᇯ' || c >= 'ᇱ' && c <= 'ᇸ';
        case 32:
          return c == 'ⁿ';
        case 33:
          return c == 'ℂ' || c == 'ℇ' || c >= 'ℊ' && c <= 'ℓ' || c == 'ℕ' || c >= '℘' && c <= 'ℝ' || c == 'ℤ' || c == 'ℨ' || c >= 'ℬ' && c <= 'ℭ' || c >= 'ℯ' && c <= 'ℸ' || c >= 'Ⅰ' && c <= 'ⅿ';
        case 48:
          return c >= '゛' && c <= '゜';
        case 49:
          return c >= 'ㄱ' && c <= 'ㆎ';
        case 249:
        case 250:
        case 251:
        case 252:
        case 253:
        case 254:
        case (int) byte.MaxValue:
          return true;
        default:
          return false;
      }
    }

    private static bool isLetter2(char c)
    {
      if (c >= 'a' && c <= 'z')
        return true;
      if (c == '>')
        return false;
      if (c >= 'A' && c <= 'Z')
        return true;
      switch (Character.getType(c))
      {
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
        case 10:
          return !XmlChars.isCompatibilityChar(c) && (c < '⃝' || c > '⃠');
        default:
          return c == '·';
      }
    }

    private static bool isDigit(char c) => Character.isDigit(c) && (c < '０' || c > '９');

    private static bool isExtender(char c) => c == '·' || c == 'ː' || c == 'ˑ' || c == '·' || c == 'ـ' || c == 'ๆ' || c == 'ໆ' || c == '々' || c >= '〱' && c <= '〵' || c >= 'ゝ' && c <= 'ゞ' || c >= 'ー' && c <= 'ヾ';

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      XmlChars xmlChars = this;
      ObjectImpl.clone((object) xmlChars);
      return ((object) xmlChars).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
