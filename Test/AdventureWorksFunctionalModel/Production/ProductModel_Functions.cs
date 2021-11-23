using System.Globalization;
using System.Xml.Linq;

namespace AW.Functions;
public static class ProductModel_Functions
{
    [MemberOrder(22)]
    public static ProductDescription LocalCultureDescription(ProductModel pm) =>
        pm.ProductModelProductDescriptionCulture
          .Where(obj => obj.Culture.CultureID.StartsWith(CultureInfo.CurrentCulture.TwoLetterISOLanguageName))
          .Select(obj => obj.ProductDescription)
          .First();

    public static string CatalogDescription(ProductModel pm) =>
        string.IsNullOrEmpty(pm.CatalogDescription) ? "" : XElement.Parse(pm.CatalogDescription).Elements().Select(e => Formatted(e)).Aggregate((i, j) => i + j);

    private static string Formatted(XElement n) =>
        n.Name.ToString().Substring(n.Name.ToString().IndexOf("}") + 1) + ": " + n.Value + "\n";
}